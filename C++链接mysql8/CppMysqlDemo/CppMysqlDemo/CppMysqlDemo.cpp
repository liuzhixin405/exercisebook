#include <WinSock.h>  //一定要包含这个，或者winsock2.h
#include "mysql.h"    //引入mysql头文件(一种方式是在vc目录里面设置，一种是文件夹拷到工程目录，然后这样包含)
#include <Windows.h>
#include <iostream>
#include<iomanip>
using namespace std;
//包含附加依赖项，也可以在工程--属性里面设置
#pragma comment(lib,"wsock32.lib")
#pragma comment(lib,"libmysql.lib")
MYSQL mysql; // mysql连接
MYSQL_FIELD* fd; // 字段列数组
char field[32][32]; // 存字段名二维数组
MYSQL_RES* res; // 这个结构代表返回行的一个查询结果集
MYSQL_ROW column; // 一个行数据的类型安全(type-safe)的表示，表示数据行的列
char sql[150]; // sql语句
bool ConnectDatabase(); // 连接数据库
void FreeConnect(); // 释放资源
bool QueryDatabase1(); // 查询1
bool QueryDatabase2(); // 查询2
bool InsertData(); // 插入数据
bool ModifyData(); // 修改数据
bool DeleteData(); // 删除数据
int main(int argc, char** argv)
{
	ConnectDatabase();
	QueryDatabase1();
	InsertData();
	QueryDatabase2();
	ModifyData();
	QueryDatabase2();
	DeleteData();
	QueryDatabase2();
	FreeConnect();
	system("pause");
	return 0;
}
bool ConnectDatabase() // 连接数据库
{
	mysql_init(&mysql);  //连接mysql数据库，初始化
	if (!(mysql_real_connect(&mysql, "localhost", "root", "118Root", "test", 3306, NULL, 0))) //中间分别是主机，用户名，密码，数据库名，端口号（可以写默认0或者3306等），可以先写成参数再传进去
	{
		cout << "连接数据库失败:" << mysql_error(&mysql) << endl;
		return false; // 连接失败
	}
	else
	{
		cout << "连接成功..." << endl;
		return true; // 连接成功
	}
}
void FreeConnect() // 释放资源
{
	cout << "释放数据库资源..." << endl;
	mysql_free_result(res);
	mysql_close(&mysql);
}
bool QueryDatabase1()
{
	sprintf_s(sql, "select * from user");
	mysql_query(&mysql, "set names gbk"); // 设置编码格式
	if (mysql_query(&mysql, sql)) // 执行SQL语句
	{
		cout << "查询失败：" << mysql_error(&mysql) << endl;
		return false; // 查询失败
	}
	else
	{
		cout << "查询成功..." << endl; // 查询成功
	}
	if (!(res = mysql_store_result(&mysql))) // 获取结果集
	{
		cout << "查询对象失败：" << mysql_error(&mysql) << endl;
		return false; // 查询对象失败
	}
	cout << "数据行数：" << mysql_affected_rows(&mysql) << endl; // 打印数据行数
	char* str_field[32]; // 定义一个字符串数组存储字段信息
	for (int i = 0; i < 1; i++) // 在已知字段数量的情况下获取字段名
	{
		str_field[i] = mysql_fetch_field(res)->name;
	}
	for (int i = 0; i < 1; i++) // 输出查询信息
	{
		cout << setw(10) << str_field[i] << " ";
	}
	cout << endl;
	while (column = mysql_fetch_row(res)) // 在已知字段数量情况下，获取并打印下一行
	{
		cout << setw(10) << column[0] << " ";
		cout << setw(10) << column[1] << " ";
		cout << setw(10) << column[2] << " ";
		cout << setw(10) << column[3] << endl;
	}
	return true; // 查询成功
}
bool QueryDatabase2()
{
	mysql_query(&mysql, "set names gbk");
	if (mysql_query(&mysql, "select * from user")) // 执行SQL语句
	{
		cout << "查询失败：" << mysql_error(&mysql) << endl;
		return false; // 查询失败
	}
	else
	{
		cout << "查询成功..." << endl;
	}
	res = mysql_store_result(&mysql);
	cout << "数据行数：" << mysql_affected_rows(&mysql) << endl; // 打印数据行数
	for (int i = 0; fd = mysql_fetch_field(res); i++)
	{
		strcpy_s(field[i], fd->name); // 获取字段名
	}
	int j = mysql_num_fields(res); // 获取列数
	for (int i = 0; i < j; i++) // 打印字段
	{
		cout << setw(10) << field[i] << " ";
	}
	cout << endl;
	while (column = mysql_fetch_row(res))
	{
		for (int i = 0; i < j; i++)
		{
			cout << setw(10) << column[i] << " ";
		}
		cout << endl;
	}
	return true; // 查询成功
}
bool InsertData()
{
	sprintf_s(sql, "insert into user values (6, 'lily', 20,'lily@sina.cn');"); // 也可以控制台手动输入sql语句
	if (mysql_query(&mysql, sql)) // 执行SQL语句
	{
		cout << "插入失败：" << mysql_error(&mysql) << endl;
		return false;
	}
	else
	{
		cout << "插入成功..." << endl;
		return true;
	}
}
bool ModifyData()
{
	sprintf_s(sql, "update user set Description='lesk@163.com' where name='test001'"); // 也可以控制台手动输入sql语句
	if (mysql_query(&mysql, sql)) // 执行SQL语句
	{
		cout << "修改失败：" << mysql_error(&mysql) << endl;
		return false;
	}
	else
	{
		cout << "修改成功..." << endl;
		return true;
	}
}
bool DeleteData()
{
	sprintf_s(sql, "delete from user where id=6;");
	if (mysql_query(&mysql, sql)) // 执行SQL语句
	{
		cout << "删除失败：" << mysql_error(&mysql) << endl;
		return false;
	}
	else
	{
		cout << "删除成功..." << endl;
		return true;
	}
}


#pragma region 链接设置
				//下面的代码是一个实现C++连接MYSQL数据库的很好的例子
//
//#include <winsock.h>
//#include <iostream>
//#include <string>
//#include <mysql.h>
//using namespace std;
//
//#pragma comment(lib, "ws2_32.lib")
//#pragma comment(lib, "libmysql.lib")
////单步执行，不想单步执行就注释掉
//#define STEPBYSTEP
//
//int main() {
//    cout << "****************************************" << endl;
//
//#ifdef STEPBYSTEP
//    system("pause");
//#endif
//
//    //必备的一个数据结构
//    MYSQL mydata;
//
//    //初始化数据库
//    if (0 == mysql_library_init(0, NULL, NULL)) {
//        cout << "mysql_library_init() succeed" << endl;
//    }
//    else {
//        cout << "mysql_library_init() failed" << endl;
//        return -1;
//    }
//
//#ifdef STEPBYSTEP
//    system("pause");
//#endif
//
//    //初始化数据结构
//    if (NULL != mysql_init(&mydata)) {
//        cout << "mysql_init() succeed" << endl;
//    }
//    else {
//        cout << "mysql_init() failed" << endl;
//        return -1;
//    }
//
//
//
//#ifdef STEPBYSTEP
//    system("pause");
//#endif
//
//    //在连接数据库之前，设置额外的连接选项
//    //可以设置的选项很多，这里设置字符集，否则无法处理中文
//    if (0 == mysql_options(&mydata, MYSQL_SET_CHARSET_NAME, "gbk")) {
//        cout << "mysql_options() succeed" << endl;
//    }
//    else {
//        cout << "mysql_options() failed" << endl;
//        return -1;
//    }
//
//#ifdef STEPBYSTEP
//    system("pause");
//#endif
//
//    //连接数据库
//    if (NULL != mysql_real_connect(&mydata, "localhost", "root", "123456", "mysql", 3306, NULL, 0))   //这里的地址，用户名，密码，端口可以根据自己本地的情况更改
//    {
//        cout << "mysql_real_connect() succeed" << endl;
//    }
//    else {
//        cout << "mysql_real_connect() failed" << endl;
//        return -1;
//    }
//
//    return 0;
//}  
#pragma endregion
