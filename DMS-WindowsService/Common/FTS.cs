using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DMS_WindowsService.Common
{

    public class Document
    {
        public string Document_Name { get; set; }
        public string Document_Id { get; set; }
        public string Description { get; set; }
    }

    public class NGChangeDocumentPropertyOutput
    {
        public Document Document { get; set; }
    }

    public class RootObject
    {
        public NGChangeDocumentPropertyOutput NGChangeDocumentProperty_Output { get; set; }
    }
    public class DMSdetails
    {
        public string Param1 { get; set; }
        public string DocCategory { get; set; }
        public string DocActive { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string DmsDocId { get; set; }
        public string DepartmentName { get; set; }
        public string DocType { get; set; }
        public string DocName { get; set; }
        public string UniqueNumber { get; set; }
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
    }
    public class DBMethodsCommon
    {
        //string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ConnStringDMS"].ConnectionString;
        public SqlConnection Conn;
        public DBMethodsCommon()
        {
            Conn = new SqlConnection(Utility.constr);
            Conn.Close();
            Conn.Open();
        }

        #region FTS related function
        public DataTable GetDataFromDMS(int Counter)
        {
            DataTable dt = new DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("GetDMSDocumentList", Conn);
                cmd.Parameters.AddWithValue("@Counter", Counter);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                Conn.Close();
            }
            catch (Exception ex)
            {
                new DBMethodsCommon().Insert_tbl_DocumentUpdatedInDMSForFTS(Counter, Counter, ex.Message, "Exception from GetDataFromDMS function");
                dt = null;
            }
            return dt;
        }

        public DataTable GetTotalNumberOfRecord()
        {
            DataTable dt = new DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("GetTotalNumberOfRecordsForFTS", Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                Conn.Close();
            }
            catch (Exception ex)
            {
                new DBMethodsCommon().Insert_tbl_DocumentUpdatedInDMSForFTS(0, 0, ex.Message, "Exception from GetTotalNumberOfRecord function");
                dt = null;
            }
            return dt;
        }

        /// <summary>
        /// Keeping log into the Database which Id's has been updated into DMS.
        /// </summary>
        /// <param name="DMSDocumentID"></param>
        /// <param name="DocumentId"></param>
        /// <param name="OutputXML"></param>
        /// <returns></returns>
        public DataTable Insert_tbl_DocumentUpdatedInDMSForFTS(int DMSDocumentID, int DocumentId, string OutputXML, string InputXML, string Document_Name = null, string Document_Id = null, string Description = null)
        {
            DataTable dt = new DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("Insert_tbl_DocumentUpdatedInDMSForFTS", Conn);
                cmd.Parameters.AddWithValue("@DMSDocumentID", DMSDocumentID);
                cmd.Parameters.AddWithValue("@DocumentId", DocumentId);
                cmd.Parameters.AddWithValue("@OutputXML", OutputXML);
                cmd.Parameters.AddWithValue("@InputXML", InputXML);
                cmd.Parameters.AddWithValue("@Document_Id", Document_Id);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@Document_Name", Document_Name);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                Conn.Close();
            }
            catch (Exception ex)
            {
                dt = null;
            }
            return dt;
        }
        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion
    }




}


