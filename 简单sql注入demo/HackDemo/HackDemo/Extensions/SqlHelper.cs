using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackDemo.Extensions
{
    public class SqlHelper
    {
        public bool CheckUser(string connStr,string name ,string password)
        {
            object result = 0;
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = $"select count(1) from TUser where name='{name}' and password='{password}' ;";
                command.Connection = conn;
                 result = command.ExecuteScalar();
            }
            return ((int)(result)) == 1;   
        }

        public bool CheckUserByPars(string connStr, string name, string password)
        {
            object result = 0;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = $"select count(1) from TUser where name='@name' and password='@password' ;";
                command.Parameters.Add("@name",System.Data.SqlDbType.NVarChar);
                command.Parameters["@name"].Value = name;
                command.Parameters.AddWithValue("@password", password);
                command.Connection = conn;
                result = command.ExecuteScalar();
            }
            return ((int)(result)) == 1;
        }
    }
}
