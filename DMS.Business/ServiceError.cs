using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DMS.Business
{
    public class ServiceErrorLog 
    {
        public string connection = null;
        public string constr = ConfigurationManager.ConnectionStrings["ConnStringDMS"].ConnectionString.ToString();

        public ServiceErrorLog(string conn)
        {
            connection = conn;
        }
        /// <summary>
        /// Error Log with all details with function name
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionType"></param>
        /// <param name="stackTrace"></param>
        /// <param name="url"></param>
        /// <param name="innerException"></param>
        /// <param name="functionName"></param>
        public void ErrorLog(string message, string exceptionType, string stackTrace, string url, string innerException, string functionName)
        {
            try
            {
                //using (SqlConnection con = new SqlConnection(connection))
                //{
                //    using (SqlCommand cmd = new SqlCommand("DMS_ADD_ERRORLOG"))
                //    {
                        SqlConnection Conn = new SqlConnection(constr);
                        SqlCommand cmd = new SqlCommand("DMS_ADD_ERRORLOG", Conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = Conn;
                        cmd.Parameters.AddWithValue("@ExceptionMsg", message);
                        cmd.Parameters.AddWithValue("@ExceptionType", exceptionType);
                        cmd.Parameters.AddWithValue("@ExceptionSource", stackTrace);
                        cmd.Parameters.AddWithValue("@ExceptionURL", url);
                        cmd.Parameters.AddWithValue("@InnerException", innerException);
                        cmd.Parameters.AddWithValue("@FunctionName", functionName);

                        Conn.Open();
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                //    }
                //}
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// Error Log with exception classs and function name
        /// </summary>
        /// <param name="exdb"></param>
        /// <param name="functionName"></param>
        public void ErrorLog(Exception exdb, string functionName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connection))
                {
                    using (SqlCommand cmd = new SqlCommand("DMS_ADD_ERRORLOG"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ExceptionMsg", Convert.ToString(exdb.Message));
                        cmd.Parameters.AddWithValue("@ExceptionType", Convert.ToString(exdb.GetType().Name));
                        cmd.Parameters.AddWithValue("@ExceptionSource", Convert.ToString(exdb.StackTrace));
                        cmd.Parameters.AddWithValue("@ExceptionURL", string.Empty);
                        cmd.Parameters.AddWithValue("@InnerException", Convert.ToString(exdb.InnerException));
                        cmd.Parameters.AddWithValue("@FunctionName", functionName);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            { }

        }
    }
}
