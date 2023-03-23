dotnet创建项目步骤，is4admin为例
1、输入 dotnet new identity --search 命令查看可用模板
2、安装模板 dotnet new -i IdentityServer4.Templates
3、 dotnet new  is4aspid -n Identity4Demo         安装is4aspid取名Identity4Demo
4、 dotnet new --list 查看已安装模板 

最新模板is6aspid,改数据库存信息

代码实例只有一个授权中心，
其他都都授权中心和服务搭配使用。
client是关联授权中心和服务

mvc直接调用home/index会跳转到授权登录中心account/login，登陆后自动跳转到home/index,
退出登录account/logout