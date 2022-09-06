using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Diagnostics;

namespace SqlserverConnect
{
    class Program
    {
        private static readonly string cnstr = "server=47.94.198.11,1433;database=BulkTestDB;uid=sa;pwd=Root1230;";
        static void Main(string[] args)
        {
            //var res = new Class1().gettable("select * from student");
            #region 测试一 循环insert
            /* Stopwatch sw = new Stopwatch();
             SqlConnection sqlConn = GetConnection(cnstr);

             SqlCommand sqlComm = new SqlCommand();
             sqlComm.CommandText = string.Format("insert into BulkTestTable (Id,UserName,Pwd) values (@p0,@p1,@p2)");
             sqlComm.Parameters.Add("@p0", SqlDbType.Int);
             sqlComm.Parameters.Add("@p1", SqlDbType.NVarChar);
             sqlComm.Parameters.Add("@p2", SqlDbType.VarChar);
             sqlComm.CommandType = CommandType.Text;
             sqlComm.Connection = sqlConn;
             sqlConn.Open();

             try
             {
                 for (int multiply = 0; multiply < 10; multiply++)
                 {
                     for (int count = multiply * 100000; count < (multiply+1)*100000; count++)
                     {
                         sqlComm.Parameters["@p0"].Value = count;
                         sqlComm.Parameters["@p1"].Value = $"User-{count*multiply}";
                         sqlComm.Parameters["@p2"].Value = $"Pwd-{count * multiply}";

                         sw.Start();
                         sqlComm.ExecuteNonQuery();
                         sw.Stop();
                     }
                     Console.WriteLine($"Elapsed Time is {sw.ElapsedMilliseconds} Milliseconds");
                 }
             }
             catch(Exception ex)
             {
                 throw ex;
             }
             finally
             {
                 sqlConn.Close();
             }
            */
            #endregion

            #region 测试二 bulkcopy
            /*
            Stopwatch sw = new Stopwatch();
            for(int multiply = 0; multiply < 10; multiply++)
            {
                DataTable dt = GetTableSchema();

                for (int count = multiply * 100000; count < (multiply + 1)*100000; count++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = count;
                    dr[1] = $"User-{count * multiply}";
                    dr[2] = $"Pwd-{count * multiply}";
                    dt.Rows.Add(dr);
                }
                sw.Start();
                BulkToDB(dt);
                sw.Stop();
                Console.WriteLine($"Elapsed Time is {sw.ElapsedMilliseconds} Milliseconds");
            }
            */
            #endregion

            #region 测试三 TVPs
            
            Stopwatch sw = new Stopwatch();
            for (int multiply = 0; multiply < 10; multiply++)
            {
                DataTable dt = GetTableSchema();

                for (int count = multiply * 100000; count < (multiply + 1) * 100000; count++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = count;
                    dr[1] = $"User-{count * multiply}";
                    dr[2] = $"Pwd-{count * multiply}";
                    dt.Rows.Add(dr);
                }
                sw.Start();
                TableValueToDB(dt);
                sw.Stop();
                Console.WriteLine($"Elapsed Time is {sw.ElapsedMilliseconds} Milliseconds");
            }
            
            #endregion
            Console.ReadLine();
        }

        static SqlConnection GetConnection(string cnStr)
        {
            return new SqlConnection(cnStr);
        }

        public static void BulkToDB(DataTable dt)
        {
            SqlConnection sqlConnection = new SqlConnection(cnstr);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection);
            bulkCopy.DestinationTableName = "BulkTestTable";
            bulkCopy.BatchSize = dt.Rows.Count;
            try
            {
                sqlConnection.Open();
                if (dt != null && dt.Rows.Count != 0)
                {
                    bulkCopy.WriteToServer(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConnection.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }

        }

        public static DataTable GetTableSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("Id", typeof(int)), new DataColumn("UserName", typeof(string)), new DataColumn("Pwd", typeof(string)) });
            return dt;
        }


        public static void TableValueToDB(DataTable dt)
        {
            SqlConnection sqlConn = new SqlConnection(cnstr);
            const string TSqlStatement = "insert into BulkTestTable (Id,UserName,Pwd) SELECT nc.Id,nc.UserName,nc.Pwd FROM @NewBulkTestTvp As nc";
            SqlCommand cmd = new SqlCommand(TSqlStatement, sqlConn);
            SqlParameter catParam = cmd.Parameters.AddWithValue("@NewBulkTestTvp", dt);
            catParam.SqlDbType = SqlDbType.Structured;
            catParam.TypeName = "dbo.BulkUdt";

            try
            {
                sqlConn.Open();
                if (dt != null && dt.Rows.Count != 0)
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConn.Close();
            }
        }
    }

    public class Class1
    {
        private string cnstr = "server=47.94.198.11,1433;database=demo;uid=sa;pwd=Root1230;";
        public void noquery(string sql)
        {
            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();
            SqlCommand cmd = new SqlCommand(sql, cn);
            cmd.ExecuteNonQuery();
            cn.Close();
            cmd.Clone();
        }
        public DataTable gettable(string sql)
        {
            SqlConnection cn = new SqlConnection(cnstr);
            cn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter(sql, cn);
            sda.Fill(ds, "student");
            cn.Close();
            return ds.Tables["student"];
        }
    }
}
