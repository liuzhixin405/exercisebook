dotnet创建项目步骤，is4admin为例
1、输入 dotnet new identity --search 命令查看可用模板
2、安装模板 dotnet new -i IdentityServer4.Templates
3、 dotnet new  is4aspid -n Identity4Demo         安装is4aspid取名Identity4Demo
4、 dotnet new --list 查看已安装模板 



代码实例两 一个是客户端获取id4服务token再调用api，一个是访问mvc直接跳转到id4服务登录后再跳转回来（即授权），使用内存数据库，没有用到sqlserver。