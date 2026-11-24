#!/usr/bin/env bash
# 限时冒烟：启动全部进程 → 逐项检查 → 全部结束。不会留下常驻进程。
# 需要：本机 Valkey/Redis 监听 127.0.0.1:6379；先 `dotnet build ColdHub.slnx`。
set -u
cd "$(dirname "$0")"
PORTS="5310 5321 5322 5330 5410 5340"
KEY=demo-client-key
rm -rf .run history src/Admin/admin-data.json && mkdir -p .run

stop() {
  for port in $PORTS; do
    pid=$(netstat -ano 2>/dev/null | grep ":$port " | grep LISTEN | awk '{print $5}' | head -1)
    [ -n "$pid" ] && { taskkill //F //PID "$pid" >/dev/null 2>&1 || kill "$pid" 2>/dev/null; }
  done
}
trap stop EXIT

start() { # 目录 程序集 日志名 环境变量...
  local dir=$1 dll=$2 tag=$3; shift 3
  (cd "src/$dir" && exec env "$@" dotnet "bin/Debug/net10.0/$dll.dll" >"../../.run/$tag.log" 2>&1) &
}
check() { # 描述 期望 实际
  if [ "$2" = "$3" ]; then echo "  ok   $1 = $3"; else echo "  FAIL $1: expected $2, got $3"; FAILED=1; fi
}
FAILED=0
code() { curl -s -o /dev/null -w '%{http_code}' "$@"; }
dl() { local out=$1; shift; curl -s -o "$out" -w '%{http_code}' "$@"; }

start Admin Admin admin ASPNETCORE_URLS=http://localhost:5310
sleep 3
start Source Source src-a ASPNETCORE_URLS=http://localhost:5321 Source__Name=warehouse-a Source__Connector=sine
start Source Source src-b ASPNETCORE_URLS=http://localhost:5322 Source__Name=warehouse-b Source__Connector=random
start Recorder Recorder rec ASPNETCORE_URLS=http://localhost:5330
start Hub Hub hub ASPNETCORE_URLS=http://localhost:5410
start Console Console console ASPNETCORE_URLS=http://localhost:5340
sleep 10

echo "[1] 就绪检查"
for port in $PORTS; do check "ready :$port" 200 "$(code localhost:$port/health/ready)"; done

echo "[2] 实时订阅（Client 4 秒）"
OUT=$(dotnet src/Client/bin/Debug/net10.0/Client.dll ws://localhost:5410/ws "$KEY" Reading:warehouse-a:freezer-1,Reading:warehouse-b:dock-1 4)
echo "$OUT" | head -4 | sed 's/^/       /'
check "收到读数" yes "$(echo "$OUT" | grep -q 'warehouse-a/freezer-1' && echo yes || echo no)"
check "错误 key 的 /ws" 401 "$(code 'localhost:5410/ws?key=bad')"

echo "[2b] 慢客户端（服务端每发一条停 1 秒，队列容量 10）"
dotnet src/Client/bin/Debug/net10.0/Client.dll "ws://localhost:5410/ws?delayMs=1000" "$KEY" Reading:warehouse-a:freezer-1,Reading:warehouse-a:freezer-2,Reading:warehouse-a:chiller-1 6 >/dev/null
DROPPED=$(curl -s localhost:5410/metrics | grep '^coldhub_hub_messages_dropped' | awk '{print $2}')
check "慢客户端丢了数据(>0)" yes "$([ "${DROPPED:-0}" -gt 0 ] && echo yes || echo no)"
echo "       dropped=$DROPPED"

echo "[3] 历史下载"
sleep 2
D=$(date -u +%F); H="Authorization: ApiKey $KEY"
check "全天文件" 200 "$(dl /tmp/ch-all.gz -H "$H" "localhost:5410/history/warehouse-a/$D")"
echo "       行数(含表头): $(zcat /tmp/ch-all.gz | wc -l)，设备: $(zcat /tmp/ch-all.gz | tail -n +2 | cut -d, -f2 | sort -u | tr -d '"' | tr '\n' ' ')"
check "按设备过滤" 200 "$(dl /tmp/ch-f.gz -H "$H" "localhost:5410/history/warehouse-a/$D?devices=freezer-1&format=csv.gz")"
check "过滤后只剩 freezer-1" '"freezer-1"' "$(zcat /tmp/ch-f.gz | tail -n +2 | cut -d, -f2 | sort -u)"
check "不带 key" 401 "$(code localhost:5410/history/warehouse-a/$D)"
check "Bearer 前缀" 401 "$(code -H "Authorization: Bearer $KEY" localhost:5410/history/warehouse-a/$D)"
check "路径穿越" 400 "$(code -H "$H" 'localhost:5410/history/..%2F..%2Fx/2026-01-01')"
check "日期格式错" 400 "$(code -H "$H" localhost:5410/history/warehouse-a/2026-1-1)"
check "没有数据的日期" 404 "$(code -H "$H" localhost:5410/history/warehouse-a/2020-01-01)"

echo "[4] Admin 权限与 key 生命周期"
check "service key 不能建 key" 403 "$(code -X POST -H 'Authorization: ApiKey demo-service-key' -H 'Content-Type: application/json' -d '{"owner":"x"}' localhost:5310/keys)"
NEW=$(curl -s -X POST -H 'Authorization: ApiKey demo-admin-key' -H 'Content-Type: application/json' -d '{"owner":"smoke"}' localhost:5310/keys)
ID=$(echo "$NEW" | sed 's/.*"id":"\([^"]*\)".*/\1/'); NK=$(echo "$NEW" | sed 's/.*"key":"\([^"]*\)".*/\1/')
sleep 6
check "新 key 在 Hub 生效" 200 "$(code -H "Authorization: ApiKey $NK" -o /dev/null "localhost:5410/history/warehouse-a/$D")"
code -X DELETE -H 'Authorization: ApiKey demo-admin-key' localhost:5310/keys/$ID >/dev/null
sleep 6
check "吊销后失效" 401 "$(code -H "Authorization: ApiKey $NK" "localhost:5410/history/warehouse-a/$D")"
check "已注册的名字不能复用" 409 "$(code -X POST -H 'Authorization: ApiKey demo-admin-key' -H 'Content-Type: application/json' -d '{"name":"warehouse-a","connector":"sine"}' localhost:5310/sources)"

echo "[5] 配置热更新（把 warehouse-a 的间隔改成 1500ms，Source 不重启）"
check "PUT config" 200 "$(code -X PUT -H 'Authorization: ApiKey demo-admin-key' -H 'Content-Type: application/json' -d '{"devices":["freezer-1","freezer-2","chiller-1","chiller-2"],"intervalMs":1500,"maxPerSecond":0}' localhost:5310/sources/warehouse-a/config)"
sleep 4
check "Source 收到新配置" yes "$(grep -q '4 devices every 1500 ms' .run/src-a.log && echo yes || echo no)"

echo "[6] 指标"
curl -s localhost:5322/metrics | sed 's/^/       /'
check "丢弃按原因计数(限流必然出现)" yes "$(curl -s localhost:5322/metrics | grep -q 'coldhub_drop_reason="throttled"' && echo yes || echo no)"
echo "       --- hub"; curl -s localhost:5410/metrics | sed 's/^/       /'
echo "       --- recorder"; curl -s localhost:5330/metrics | sed 's/^/       /'

echo "[7] 日志里的 Warning/Error（应当每种故障只有一条）"
grep -hE "^(warn|fail|crit)" -A1 .run/src-b.log .run/hub.log .run/rec.log | grep -v '^--' | sed 's/^/       /' | head -12

echo "[8] Console（后台管理）"
C=localhost:5340; J='Content-Type: application/json'
check "首页" 200 "$(code $C/)"
check "页面带 CSP" yes "$(curl -sI $C/ | grep -qi "content-security-policy: default-src 'self'" && echo yes || echo no)"
check "未登录读 /api/sources" 401 "$(code $C/api/sources)"
check "错误密码" 401 "$(code -X POST -H "$J" -d '{"name":"alice","password":"nope"}' $C/api/login)"
check "不存在的用户(与错密码同一回答)" 401 "$(code -X POST -H "$J" -d '{"name":"mallory","password":"x"}' $C/api/login)"
check "bob(viewer)登录" 200 "$(code -c .run/bob.jar -X POST -H "$J" -d '{"name":"bob","password":"bob-demo-pass"}' $C/api/login)"
check "viewer 读源" 200 "$(code -b .run/bob.jar $C/api/sources)"
check "viewer 注册源" 403 "$(code -b .run/bob.jar -X POST -H "$J" -d '{"name":"x1","connector":"sine"}' $C/api/sources)"
check "viewer 看 key 列表" 403 "$(code -b .run/bob.jar $C/api/keys)"
check "alice(admin)登录" 200 "$(code -c .run/alice.jar -X POST -H "$J" -d '{"name":"alice","password":"alice-demo-pass"}' $C/api/login)"
check "admin 注册新源" 201 "$(code -b .run/alice.jar -X POST -H "$J" -d '{"name":"warehouse-c","connector":"sine"}' $C/api/sources)"
check "非 JSON 的写请求" 415 "$(code -b .run/alice.jar -X POST -H 'Content-Type: text/plain' -d 'x' $C/api/sources)"
check "admin 改配置" 200 "$(code -b .run/alice.jar -X PUT -H "$J" -d '{"devices":["c-1"],"intervalMs":1000,"maxPerSecond":0}' $C/api/sources/warehouse-c/config)"
check "admin 创建 key" 200 "$(code -b .run/alice.jar -X POST -H "$J" -d '{"owner":"console-test"}' $C/api/keys)"
check "列表里没有 key 明文和哈希" no "$(curl -s -b .run/alice.jar $C/api/keys | grep -qiE '"(key|hash)"' && echo yes || echo no)"
check "响应里不含 Admin key" no "$(curl -s -b .run/alice.jar $C/api/sources | grep -q 'demo-admin-key' && echo yes || echo no)"
check "健康页:5 个进程全部就绪" 5 "$(curl -s -b .run/alice.jar $C/api/health | grep -o '"ready":true' | wc -l)"
check "Console 的 cookie 到不了 Admin" 401 "$(code -b .run/alice.jar localhost:5310/sources)"
check "Admin key 到不了 Console 的 API" 401 "$(code -H 'Authorization: ApiKey demo-admin-key' $C/api/sources)"

[ $FAILED = 0 ] && echo "ALL OK" || echo "SOME CHECKS FAILED"
exit $FAILED
