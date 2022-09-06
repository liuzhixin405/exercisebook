## 单机版 Redis的安装配置及使用

### 1、安装Redis

#### 安装依赖

yum install -y tcl gcc-c++

官网下载redis-4.0.12版本

解压: tar xf redis-4.0.12-tar.gz -C /usr/local 进入redis解压根目录

执行命令: make MALLOC=libc

执行安装: make PREFIX=/redis安装目录 install

### 2、redis的生产环境启动方案

要把redis作为一个系统的daemon进程去运行的，每次系统启动，redis进程一起启动

（1）redis utils目录下，有个redis_init_script脚本
（2）将redis_init_script脚本拷贝到linux的/etc/init.d目录中，将redis_init_script重命名为redis_6379，6379是我们希望这个redis实例监听的端口号
（3）修改redis_6379脚本的第6行的REDISPORT，设置为相同的端口号（默认就是6379）
（4）创建两个目录：/etc/redis（存放redis的配置文件），/var/redis/6379（存放redis的持久化文件）
（5）修改redis配置文件（默认在根目录下，redis.conf），拷贝到/etc/redis目录中，修改名称为6379.conf

（6）修改redis.conf中的部分配置为生产环境

daemonize	yes							让redis以daemon进程运行
pidfile		/var/run/redis_6379.pid 	设置redis的pid文件位置
port		6379						设置redis的监听端口号
dir 		/var/redis/6379				设置持久化文件的存储位置

（7）启动redis，执行cd /etc/init.d, chmod 777 redis_6379，./redis_6379 start

（8）确认redis进程是否启动，ps -ef | grep redis

（9）让redis跟随系统启动自动启动

在redis_6379脚本中，最上面，加入两行注释

```
#!/bin/sh
# chkconfig:   2345 90 10
# Simple Redis init.d script conceived to work on Linux systems
# as it does use of the /proc filesystem.
```

添加文件执行权限
chmod +x /etc/rc.d/init.d/redis
把redis这个脚本添加到开机启动项里面
chkconfig --add redis
查看是否添加成功
chkconfig --list

打开服务

service redis start

关闭服务

service redis stop
