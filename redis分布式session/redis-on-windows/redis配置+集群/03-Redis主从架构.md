问题？ 为什么我操作redis的并发并没有你说这么高呢？

1、linux 是天生服务器。

2、Redis在什么情况下会出现性能急速下降？(用户到缓存设计缓存QPS 大20W+)

1、value过大

2、逻辑处理比较复杂，lpush

3、redis-benchmark 压力测试(QPS) 5W~8W

​                                                              list   8k ~2W 

单机redis有性能瓶颈(服务性能有很大关系)

[4核4G]

[2核2G]

如何设计Redis缓存架构能够处理20W+ QPS



### Redis主从架构

####一、主从架构设计原理

##### 1、主从架构的核心原理

当启动一个slave node的时候，它会发送一个PSYNC命令给master node

如果这是slave node重新连接master node，那么master node仅仅会复制给slave部分缺少的数据; 否则如果是slave node第一次连接master node，那么会触发一次full resynchronization

开始full resynchronization的时候，master会启动一个后台线程，开始生成一份RDB快照文件，同时还会将从客户端收到的所有写命令缓存在内存中。RDB文件生成完毕之后，master会将这个RDB发送给slave，slave会先写入本地磁盘，然后再从本地磁盘加载到内存中。然后master会将内存中缓存的写命令发送给slave，slave也会同步这些数据。

slave node如果跟master node有网络故障，断开了连接，会自动重连。master如果发现有多个slave node都来重新连接，仅仅会启动一个rdb save操作，用一份数据服务所有slave node。

##### 2、主从复制的断点续传

从redis 3.8开始，就支持主从复制的断点续传，如果主从复制过程中，网络连接断掉了，那么可以接着上次复制的地方，继续复制下去，而不是从头开始复制一份（增量复制）

master node会在内存中常见一个backlog，master和slave都会保存一个replica offset还有一个master id，offset就是保存在backlog中的。如果master和slave网络连接断掉了，slave会让master从上次的replica offset开始继续复制

但是如果没有找到对应的offset，那么就会执行一次resynchronization

##### 3、无磁盘化复制

master在内存中直接创建rdb，然后发送给slave，不会在自己本地落地磁盘了

repl-diskless-sync
repl-diskless-sync-delay，等待一定时长再开始复制，因为要等更多slave重新连接过来

按照老师经验: 无计可施了情况下需要再次优化性能，可以配置上面参数了。

##### 4、过期key处理

**slave不会过期key**，只会等待master过期key。如果master过期了一个key，或者通过LRU淘汰了一个key，那么会模拟一条del命令发送给slave。

#### 二、主从工作原理深入剖析

##### 1、复制的完整流程

（1）slave node启动，仅仅保存master node的信息，包括master node的host和ip，但是复制流程没开始

master host和ip是从哪儿来的，redis.conf里面的slaveof配置的

（2）slave node内部有个定时任务，每秒检查是否有新的master node要连接和复制，如果发现，就跟master node建立socket网络连接
（3）slave node发送ping命令给master node
（4）口令认证，如果master设置了requirepass，那么salve node必须发送masterauth的口令过去进行认证
（5）master node第一次执行全量复制，将所有数据发给slave node
（6）master node后续持续将写命令，异步复制给slave node

##### 2、数据同步相关的核心机制

指的就是第一次slave连接msater的时候，执行的全量复制，那个过程里面你的一些细节的机制

（1）master和slave都会维护一个offset

master会在自身不断累加offset，slave也会在自身不断累加offset
slave每秒都会上报自己的offset给master，同时master也会保存每个slave的offset

主要是master和slave都要知道各自的数据的offset，才能知道互相之间的数据不一致的情况

（2）backlog

master node有一个backlog，默认是1MB大小
master node给slave node复制数据时，也会将数据在backlog中同步写一份
backlog主要是用来做全量复制中断候的增量复制的

（3）master run id

info server，可以看到master run id
如果根据host+ip定位master node，是不靠谱的，如果master node重启或者数据出现了变化，那么slave node应该根据不同的run id区分，run id不同就做全量复制
如果需要不更改run id重启redis，可以使用redis-cli debug reload命令

（4）psync

从节点使用psync从master node进行复制，psync runid offset
master node会根据自身的情况返回响应信息，可能是FULLRESYNC runid offset触发全量复制，可能是CONTINUE触发增量复制

##### 3、全量复制

（1）master执行bgsave，在本地生成一份rdb快照文件
（2）master node将rdb快照文件发送给salve node，如果rdb复制时间超过60秒（repl-timeout），那么slave node就会认为复制失败，可以适当调节大这个参数
（3）对于千兆网卡的机器，一般每秒传输100MB，6G文件，很可能超过60s
（4）master node在生成rdb时，会将所有新的写命令缓存在内存中，在salve node保存了rdb之后，再将新的写命令复制给salve node
（5）client-output-buffer-limit slave 256MB 64MB 60，如果在复制期间，内存缓冲区持续消耗超过64MB，或者一次性超过256MB，那么停止复制，复制失败
（6）slave node接收到rdb之后，清空自己的旧数据，然后重新加载rdb到自己的内存中，同时基于旧的数据版本对外提供服务
（7）如果slave node开启了AOF，那么会立即执行BGREWRITEAOF，重写AOF

rdb生成、rdb通过网络拷贝、slave旧数据的清理、slave aof rewrite，很耗费时间

如果复制的数据量在4G~6G之间，那么很可能全量复制时间消耗到1分半到2分钟

##### 4、增量复制

（1）如果全量复制过程中，master-slave网络连接断掉，那么salve重新连接master时，会触发增量复制
（2）master直接从自己的backlog中获取部分丢失的数据，发送给slave node，默认backlog就是1MB
（3）msater就是根据slave发送的psync中的offset来从backlog中获取数据的

##### 5、heartbeat

主从节点互相都会发送heartbeat信息

master默认每隔10秒发送一次heartbeat，salve node每隔1秒发送一个heartbeat

##### 6、异步复制

master每次接收到写命令之后，先在内部写入数据，然后异步发送给slave node

#### 三、主从的配置(4.0.12)

```
master conf文件配置的项
1、将 69行 bind 127.0.0.1 注释
2、设置 500行 requirepass master密码
3、设置后端启动 daemonize=yes
4、设置158行 pid ( pidfile /var/run/redis_6379.pid )

slave conf文件配置项
1、将 69行 bind 127.0.0.1 注释
2、开启281行配置 slaveof <masterip> <masterport> eg: slaveof 192.168.3.201 6379
3、配置slave访问master的口令认证 修改 288 行 masterauth <master-password> eg: masterauth master的requirepass

验证主从结构
在master和slave分别执行info命令验证是否配置成功

现在开发中基本都是用redis集群来做。(不是主要用于解决主备切换的，用来处理大数据量的)
replication   小数据量的处理的（哨兵+主从+读写分离）
redis cluster 来处理大数量的  （集群处理更为方便快捷）
```

####四、Redis的集群架构

##### 1、构建redis集群环境准备

```
三台服务器：先设置hosts
10.0.0.231  node1
10.0.0.232  node2
10.0.0.233  node3
```

```
端口分配：
node1:7000 通信端口  10000+7000 = 17000
node1:7001 同上
node2:7002  
node2:7003  
node3:7004  
node3:7005
```

##### 1、下载redis-4.0.12并解压

```
wget http://download.redis.io/releases/redis-4.0.1.tar.gz
tar xzf redis-4.0.1.tar.gz
cd redis-4.0.1
make
```

##### 2、编译安装

```
指定安装目录到:/usr/local/redis

make && make install PREFIX=/usr/local/redis
```

##### 3、创建配置节点

```
node1服务器:
mkdir -p /usr/loca/redis/redis_cluster/7000
mkdir -p /usr/loca/redis/redis_cluster/7001

cp /usr/local/redis/redis.conf /usr/loca/redis/redis_cluster/7000
cp /usr/local/redis/redis.conf /usr/loca/redis/redis_cluster/7001

node2服务器:
mkdir -p /usr/loca/redis/redis_cluster/7002
mkdir -p /usr/loca/redis/redis_cluster/7003

cp /usr/local/redis/redis.conf /usr/loca/redis/redis_cluster/7002
cp /usr/local/redis/redis.conf /usr/loca/redis/redis_cluster/7003

node3服务器:
mkdir -p /usr/loca/redis/redis_cluster/7004
mkdir -p /usr/loca/redis/redis_cluster/7005

cp /usr/local/redis/redis.conf /usr/loca/redis/redis_cluster/7004
cp /usr/local/redis/redis.conf /usr/loca/redis/redis_cluster/7005
```

##### 4、修改redis.conf

```
port  7000                                //端口根据对应的文件夹去配制端口 7000,7001,7002,7003,7004,7005      
requirepass 设置密码必须完全一样
daemonize    yes                          //redis后台运行
pidfile  /var/run/redis_7000.pid          //pidfile文件对应7000,7001,7002,7003,7004,7005
cluster-enabled  yes                      //开启集群  把注释#去掉
cluster-config-file  nodes_7000.conf      //集群的配置  配置文件首次启动自动生成 7000,7001,7002,7003,7004,7005
cluster-node-timeout  15000               //请求超时  默认15秒，可自行设置
appendonly  yes                           //aof日志开启  有需要就开启，它会每次写操作都记录一条日志
```

##### 5、复制src目录中的redis-trib.rb 到/usr/local/redis/bin目录

```
cp  /usr/local/redis/redis-trib.rb /usr/local/redis/bin
```

##### 6、安装ruby环境

```
yum install -y ruby
yum install -y rubygems
```

##### 7、安装ruby的包

```
gem install redis-4.0.0.rc1.gem

如果不成功需要去下载后安装
下载地址
https://rubygems.org/gems/redis/versions/4.0.0.rc1
https://rubygems.org/downloads/redis-4.0.0.rc1.gem

安装命令：gem install -l ./redis-4.0.0.rc1.gem
```

##### 8、启动各个节点

```
node1服务器:
/usr/local/redis/bin/redis-server /usr/loca/redis/redis_cluster/7000/redis.conf
/usr/local/redis/bin/redis-server /usr/loca/redis/redis_cluster/7001/redis.conf

node2服务器:
/usr/local/redis/bin/redis-server /usr/loca/redis/redis_cluster/7002/redis.conf
/usr/local/redis/bin/redis-server /usr/loca/redis/redis_cluster/7003/redis.conf

node3服务器:
/usr/local/redis/bin/redis-server /usr/loca/redis/redis_cluster/7004/redis.conf
/usr/local/redis/bin/redis-server /usr/loca/redis/redis_cluster/7005/redis.conf
```

##### 9、检查各节点是否启动

```
查看进程
ps -ef | grep redis

查看端口
netstat -tnlp | grep redis
```

##### 10、创建集群

```
进入node1的bin目录下,执行以下脚本,解压源文件src目录下面
./redis-trib.rb  create  --replicas  1  192.168.3.220:7001 192.168.3.220:7002 192.168.3.221:7001 192.168.3.221:7002 192.168.3.222:7001 192.168.3.222:7

```

**★★★★注意有坑: 设置ruby脚本执行密码**
**vim /usr/local/rvm/gems/ruby-2.5.1/gems/redis-4.0.1/lib/redis**
**密码必须和redis的requirepass设置密码的一致**

说明: 各个redis集群节点的通信端口为应用端口前面加10000，eg:7001 => 17001(实践出来的)

如何找不到client.rb文件路径可以使用

查找文件名中包含某字符（如"gems"）的文件

find /home/gerry/ -name '*gems*'

find /home/gerry/ -name 'gems*'

find /home/gerry/ -name '*gems'

##### 11、验证集群

```
进入每台服务器的redis/src目录,记住参数  -c连接到集群的不可以少
redis-cli -h ip -p 端口 -a 密码 -c
redis-cli -h node2 -p 7002 -c
redis-cli -h node3 -p 7004 -c
```

#### 五、分布式数据存储的核心算法，数据分布的算法

##### 1、redis cluster

（1）自动将数据进行分片，每个master上放一部分数据
（2）提供内置的高可用支持，部分master不可用时，还是可以继续工作的

在redis cluster架构下，每个redis要放开两个端口号，比如一个是6379，另外一个就是加10000的端口号，比如16379

16379端口号是用来进行节点间通信的，也就是cluster bus的东西，集群总线。cluster bus的通信，用来进行故障检测，配置更新，故障转移授权

cluster bus用了另外一种二进制的协议，主要用于节点间进行高效的数据交换，占用更少的网络带宽和处理时间

##### 2、最老土的hash算法和弊端（大量缓存重建）

##### 3、一致性hash算法（自动缓存迁移）+虚拟节点（自动负载均衡）

##### 4、redis cluster的hash slot算法

redis cluster有固定的16384个hash slot，对每个key计算CRC16值，然后对16384取模，可以获取key对应的hash slot

redis cluster中每个master都会持有部分slot，比如有3个master，那么可能每个master持有5000多个hash slot

hash slot让node的增加和移除很简单，增加一个master，就将其他master的hash slot移动部分过去，减少一个master，就将它的hash slot移动到其他master上去

移动hash slot的成本是非常低的

