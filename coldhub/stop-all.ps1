# 停止 coldhub 的全部进程：按端口找到监听进程并结束（只动这 6 个端口，不会碰别的 dotnet 进程）。
$ports = 5310, 5321, 5322, 5330, 5340, 5410
foreach ($port in $ports) {
    $connection = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($connection) {
        Stop-Process -Id $connection.OwningProcess -Force
        Write-Host "已停止 :$port (pid $($connection.OwningProcess))"
    }
}
# 进程结束后 cmd 窗口会停在 "dotnet 已退出" 的提示符上，顺手关掉由本脚本启动的窗口
Get-Process cmd -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowTitle -like 'ColdHub *' } | Stop-Process -Force
