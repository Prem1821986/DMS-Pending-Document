using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Business
{
    public class ThickClientService
    {

        ServiceErrorLog errorLog = null;
        DAL dal = null; // Divya

        public ThickClientService(string Conn)
        {
            dal = new DAL(Conn);
            errorLog = new ServiceErrorLog(Conn);
        }

        /// <summary>
        /// Login with userName and Password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Login(string userName, string password)
        {
            int result = 0;
            try
            {
                SqlParameter[] parameter = {
                                              new SqlParameter() { ParameterName = "@UserName",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = userName},
                                              new SqlParameter() { ParameterName = "@Password",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = password},
                                              new SqlParameter() { ParameterName = "@Valid",SqlDbType =  SqlDbType.Int,Direction =  ParameterDirection.Output },
                                           };


                dal.ExecuteNonQuery(SPNames.Login, parameter);

                parameter[2].Direction = ParameterDirection.Output;

                result = Convert.ToInt32(parameter[2].Value);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("Login", "Check Login vaildity", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "Login");
            }

            return result;
        }


        /// <summary>
        /// Get departments list
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public DataTable GetDepartments(ulong departmentId)
        {
            DataTable dt = new DataTable("getDepartment");
            try
            {
                SqlParameter[] parameter = {
                                              new SqlParameter() { ParameterName = "@deptid",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = departmentId},
                                           };


                dal.Fill(dt, SPNames.DepartmentList, parameter);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetDepartments", "Get Department details", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetDepartments");
            }
            return dt;
        }

        /// <summary>
        /// Get sub-department list
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="subDepartmentId"></param>
        /// <returns></returns>
        public DataTable GetSubDepartments(long departmentId, long subDepartmentId)
        {
            DataTable dt = new DataTable("getSubDepartment");
            try
            {

                SqlParameter[] parameter = {
                                                new SqlParameter() { ParameterName = "@deptid",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = departmentId},
                                                new SqlParameter() { ParameterName = "@SubDepartmentID",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = subDepartmentId},

                                             };


                dal.Fill(dt, SPNames.SubDepartmentList, parameter);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetSubDepartments", "Get SubDepartment details", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetSubDepartments");
            }

            return dt;

        }


        /// <summary>
        /// Get document type
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="docTypeId"></param>
        /// <returns></returns>
        public DataTable GetDocumentType(long subDepartmentID, long docTypeId)
        {
            DataTable dt = new DataTable("getDocumentType");
            try
            {

                SqlParameter[] parameter = {
                                                new SqlParameter() { ParameterName = "@subdeptid",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = subDepartmentID},
                                                new SqlParameter() { ParameterName = "@doctypeid",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = docTypeId},
                                           };


                dal.Fill(dt, SPNames.DocumentType, parameter);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetDocumentType", "Get Document Type by SubDepartmentID", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetDocumentType");
            }

            return dt;
        }

        /// <summary>
        /// GET DOCUMENT META DATA DETAILS
        /// </summary>
        /// <param name="docTypeId"></param>
        /// <param name="departMentId"></param>
        /// <param name="subDepartMentId"></param>
        /// <returns></returns>
        public DataSet GetDocumentDetails(long docTypeId, long departMentId, long subDepartMentId)
        {
            DataSet ds = new DataSet("getDocumentDetails");
            try
            {

                SqlParameter[] parameter = {
                                                new SqlParameter() { ParameterName = "@DOCTYPEID",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = docTypeId},
                                                new SqlParameter() { ParameterName = "@DEPTID",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = departMentId},
                                                new SqlParameter() { ParameterName = "@SUBDEPARTMENTID",SqlDbType =  SqlDbType.BigInt,Direction =  ParameterDirection.Input, Value = subDepartMentId},
                                           };

                dal.Fill(ds, SPNames.DocumentDetails, parameter);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetDocumentDetails", "Get Document Details by Dept,SubDept,DocType", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetDocumentDetails");
            }

            return ds;
        }



        public DataTable GetColumns(string departmentName, string subDocumentName, string docType)
        {
            DataTable dt = new DataTable("getColumn");
            try
            {
                SqlParameter[] parameter = {
                                              new SqlParameter() { ParameterName = "@documentName",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = departmentName},
                                              new SqlParameter() { ParameterName = "@subDocumentName",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = subDocumentName},
                                              new SqlParameter() { ParameterName = "@docType",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = docType},
                                           };


                dal.Fill(dt, SPNames.GetColumns, parameter);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetColumns", "Get MetaData Column Names by Dept,SubDept, DocType", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetColumns");
            }
            return dt;
        }

        public DataTable GetMandatoryColumns(string departmentName, string subDocumentName, string docType)
        {
            DataTable dt = new DataTable("getMandatoryColumn");
            try
            {
                SqlParameter[] parameter = {
                                              new SqlParameter() { ParameterName = "@departMentName",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = departmentName},
                                              new SqlParameter() { ParameterName = "@subDocumentName",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = subDocumentName},
                                              new SqlParameter() { ParameterName = "@docType",SqlDbType =  SqlDbType.VarChar,Direction =  ParameterDirection.Input, Value = docType},
                                           };


                dal.Fill(dt, SPNames.MandatoryColumns, parameter);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetMandatoryColumns", "Get mandatory cloumns", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetMandatoryColumns");
            }
            return dt;
        }

        public void UpdateDMSDocumentID(int docID, int DMSDocID, int PreviewDMSDOCID, string PreviewDMSDOCName)
        {
            try
            {
                string result = string.Empty;
                SqlCommand cmd = new SqlCommand(SPNames.UpdateDMSDocument, dal.Conn);
                cmd.Parameters.AddWithValue("@DocumentIndexId", DMSDocID);
                cmd.Parameters.AddWithValue("@PreviewDocumentIndexId", PreviewDMSDOCID);
                cmd.Parameters.AddWithValue("@PreviewDocumentName", PreviewDMSDOCName);
                cmd.Parameters.AddWithValue("@DocumentID", docID);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("UpdateDMSDocumentID", "Update DMS DocumentID", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "UpdateDMSDocumentID");
            }

        }

        public long GetDocTtypeId(string departmentName, string subDocumentName, string docType)
        {
            long docTtypeId = 0;
            try
            {
                string result = string.Empty;
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand(SPNames.GetDocTypeID, dal.Conn);
                cmd.Parameters.AddWithValue("@Department", departmentName);
                cmd.Parameters.AddWithValue("@SubDepartment", subDocumentName);
                cmd.Parameters.AddWithValue("@DocType", docType);
                cmd.CommandType = CommandType.StoredProcedure;
                object value = cmd.ExecuteScalar();
                docTtypeId = long.Parse(value.ToString());
                return docTtypeId;
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetDocTtypeId", "Get Doucment Type ID", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetDocTtypeId");
            }

            return docTtypeId;
        }

        public DataTable GetDMSDocToUpload()
        {
            DataTable dt = new DataTable("getDMSDoc");
            try
            {
                SqlCommand cmd = new SqlCommand(SPNames.GetDMSDocuments, dal.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetDMSDocToUpload", "Get doucments to upload on DMS", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetDMSDocToUpload");
            }
            return dt;
        }

        public DataTable GetDMSDocToUpload(string deptName, string subDeptName, string docType)
        {
            DataTable dt = new DataTable("getDMSDoc");
            try
            {
                SqlCommand cmd = new SqlCommand("sp_Get_DMS_DocToUpload_By_DepartmentWise", dal.Conn);
                cmd.Parameters.AddWithValue("@DeptName", deptName);
                cmd.Parameters.AddWithValue("@SubDeptName", subDeptName);
                cmd.Parameters.AddWithValue("@DocTypeName", docType);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetDMSDocToUpload", "Get doucments to upload on DMS", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetDMSDocToUpload");
            }
            return dt;
        }

        public DataTable GetBulkUtilityUplodingType(string DepartmentName,string Doctype)
		{
			DataTable dt = new DataTable();
			try
			{
				SqlCommand cmd = new SqlCommand(SPNames.spGetSetBulkUtilityUplodingType, dal.Conn);
				cmd.Parameters.AddWithValue("@DeptName", DepartmentName);
				cmd.Parameters.AddWithValue("@DocTypeName", Doctype);
				cmd.Parameters.AddWithValue("@ScriptType", 2);
				cmd.CommandType = CommandType.StoredProcedure;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				da.Fill(dt);
			}
			catch (Exception ex)
			{
				errorLog.ErrorLog("GetBulkUtilityUplodingType", "Get Bulk Utility Uploding Type", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetBulkUtilityUplodingType");
			}
			return dt;
		}

        public DataTable GetDMSDocumentDetails(int uploadedDocumentID, string DocTypeDMS)
        {
            DataTable dt = new DataTable("GetDMSDocumentDetail");
            try
            {
                SqlCommand cmd = new SqlCommand(SPNames.GetDMSDocumentDetails, dal.Conn);
                cmd.Parameters.AddWithValue("@docId", uploadedDocumentID);
                cmd.Parameters.AddWithValue("@docTypeDMS", DocTypeDMS);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetDMSDocumentDetails", "Get DMS Document details by DocID and DocType of DMS(Preview/Main)", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetDMSDocumentDetails");
            }
            return dt;
        }

        public DataTable Master_Config(String Key, bool GetAllConfig)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand(SPNames.GetConfig, dal.Conn);
                cmd.Parameters.AddWithValue("@Key", Key);
                cmd.Parameters.AddWithValue("@GetAllConfig", GetAllConfig);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("Master_Config", "Get Master Config details", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "Master_Config");
            }
            return dt;
        }


        public DataSet GetRegNStampsDDLData(string DistrictCode, string SROCode, string MainTypeDocumentID, string SubTypeDocumentID)
        {
            DataSet ds = new DataSet("GetRegNStampsDDLData");
            try
            {
                SqlCommand cmd = new SqlCommand(SPNames.SPRegNStampsDDLData, dal.Conn);
                cmd.Parameters.AddWithValue("@DistrictCode", DistrictCode);
                cmd.Parameters.AddWithValue("@SROCode", SROCode);
                cmd.Parameters.AddWithValue("@MainTypeDocumentID", MainTypeDocumentID);
                cmd.Parameters.AddWithValue("@SubTypeDocumentID", SubTypeDocumentID);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                errorLog.ErrorLog("GetRegNStampsDDLData", "Get Registration and Stamps District", ex.Message.ToString(), ex.StackTrace.ToString(), Convert.ToString(ex.InnerException), "GetRegNStampsDDLData");
            }
            return ds;
        }

        #region 15-02-2025

        public void UpdateDMSDocumentID(int docID, int DMSDocID)
        {
            try
            {
                string result = string.Empty;
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("sp_Update_PendingDMSDOCID", dal.Conn);
                cmd.Parameters.AddWithValue("@docID", docID);
                cmd.Parameters.AddWithValue("@dmsDocID", DMSDocID);
                cmd.CommandType = CommandType.StoredProcedure;
                dal.Conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dal.Conn.Close();
            }
        }

        public void InsertDMSPendingDocsPushDtl(int DocumentId, int DMSDocumentId, int UserID)
        {
            try
            {
                string result = string.Empty;
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("SP_insertDMSPendingDocsPushDtl", dal.Conn);
                cmd.Parameters.AddWithValue("@DocumentID", DocumentId);
                cmd.Parameters.AddWithValue("@DMSDocid", DMSDocumentId);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.CommandType = CommandType.StoredProcedure;
                dal.Conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dal.Conn.Close();
            }
        }

        public DataTable GetPendingDocumentDtls(int departmentid, int SubDepartmentid, int DocTypeID, string fromdate, string todate)
        {
            DataTable dt1 = new DataTable();
            try
            {
                SqlCommand sqlCmd = new SqlCommand("SP_GetPendingDocumenttoUploadOnDMS", dal.Conn);
                sqlCmd.Parameters.AddWithValue("@departmentid", departmentid);
                sqlCmd.Parameters.AddWithValue("@SubDepartmentid", SubDepartmentid);
                sqlCmd.Parameters.AddWithValue("@DocTypeID", DocTypeID);
                sqlCmd.Parameters.AddWithValue("@fromdate", fromdate);
                sqlCmd.Parameters.AddWithValue("@todate", todate);
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dt1);
                return dt1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dal.Conn.Close();
            }
        }

        public DataTable AddUpdateDMSDocumentDetails(UploadFile model)
        {
            DataTable dt = new DataTable("addDMSDocumentData");
            try
            {
                //DAL obj = new DAL();

                SqlCommand cmd = new SqlCommand("sp_addUpdate_DMSDocumentDetails", dal.Conn);
                cmd.Parameters.AddWithValue("@UploadedDocumentId", model.UploadedDocumentId);
                cmd.Parameters.AddWithValue("@DMSDocumentId", model.DMSDocumentId);
                cmd.Parameters.AddWithValue("@DepartmentName", model.DepartmentName);
                cmd.Parameters.AddWithValue("@DocType", model.DocType);
                cmd.Parameters.AddWithValue("@DocName", model.DocName);
                cmd.Parameters.AddWithValue("@UniqueNumber", model.UniqueNumber);
                cmd.Parameters.AddWithValue("@Param1", model.Param1);

                cmd.Parameters.AddWithValue("@DocCategory", model.DocCategory);
                cmd.Parameters.AddWithValue("@DocActive", model.DocActive);
                cmd.Parameters.AddWithValue("@Param4", model.Param4);
                cmd.Parameters.AddWithValue("@Param5", model.Param5);
                cmd.Parameters.AddWithValue("@Param6", model.Param6);
                cmd.Parameters.AddWithValue("@DMSDocType", model.DMSDocType);

                cmd.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                cmd.Parameters.AddWithValue("@CitizenAmendmentID", model.CitizenAmendmentID);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);


            }
            catch (Exception ex)
            {
                //errorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "AddDMSDocumentData", Convert.ToString(ex.InnerException), "AddDMSDocumentData");
            }
            finally
            {
                dal.Conn.Close();
            }

            return dt;
        }

        public string InsertDocStatus(int docID, int levelId, int reasonID, string status, string Comments, int argUserID = 0)
        {
            try
            {
                string result = string.Empty;
                DataTable dt = new DataTable();
                SqlCommand cmd = new SqlCommand("Set_DocumentStatus_Backup", dal.Conn);
                cmd.Parameters.AddWithValue("@DocumentID", docID);
                cmd.Parameters.AddWithValue("@LevelID", levelId);
                cmd.Parameters.AddWithValue("@RejectReasonID", reasonID);
                cmd.Parameters.AddWithValue("@LevelStatus", status);
                cmd.Parameters.AddWithValue("@Comments", Comments);
                cmd.Parameters.AddWithValue("@UserID", argUserID);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    result = Convert.ToString(dt.Rows[0]["Column1"]);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dal.Conn.Close();
            }
        }

        public DataTable GetPendingDocumentFromDMS(string DocumentId, string DocName)
        {
            DataTable dtDMS = new DataTable();
            try
            {
                SqlCommand sqlCmd = new SqlCommand("Sp_GetPendingDocFromDMS", dal.Conn);
                sqlCmd.Parameters.AddWithValue("@DocumentID", DocumentId);
                sqlCmd.Parameters.AddWithValue("@DocName", DocName);
                SqlDataAdapter sqlDa = new SqlDataAdapter(sqlCmd);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlDa.Fill(dtDMS);
                return dtDMS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dal.Conn.Close();
            }
        }
        #endregion
    }
}






