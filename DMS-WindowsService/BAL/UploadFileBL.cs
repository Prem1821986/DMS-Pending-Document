using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMS_WindowsService.Common;
using System.Reflection;
//using DMS_WindowsService.Models;
using System.Data.SqlClient;
using DMS.Business;
//using E_Archival_DMS_ThickClient_Service;

namespace DMS_WindowsService.BAL
{
    class UploadFileBL
    {
        //E_Archival_DMS_ThickClient_Service.ThickClientServiceClient thickClient = new E_Archival_DMS_ThickClient_Service.ThickClientServiceClient();
        //E_Archival_DMS_ThickClient_Service.ThickClientService thickClient = new E_Archival_DMS_ThickClient_Service.ThickClientService();
        ThickClientService thickClient = null;

        ServiceLog ServiceErrorLog = null;

        public UploadFileBL()
        {
            ServiceErrorLog = new ServiceLog();
            thickClient = new ThickClientService(Utility.constr);
        }

        /// <summary>
        /// GET ALL COLUMNS
        /// </summary>
        /// <param name="departmentName"></param>
        /// <param name="subDocumentName"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        public List<string> GetColumns(string departmentName, string subDocumentName, string docType)
        {
            List<string> stringList =null;
            try
            {
                DataTable dt = thickClient.GetColumns(departmentName, subDocumentName, docType);
                stringList = dt.AsEnumerable()
                             .Select(r => r.Field<string>("Document Name"))
                             .ToList();
            }
            catch (Exception ex)
            {
                //errorLogClient.ErrorLogWithDescrtion(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetColumns");
                ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetColumns");
            }

            return stringList;
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }  

        /// <summary>
        /// Get Mandatory fields
        /// </summary>
        /// <param name="departmentName"></param>
        /// <param name="subDocumentName"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        public List<string> GetMandatoryColumns(string departmentName, string subDocumentName, string docType)
        {
            List<string> stringMandatoryList = new List<string>();
           
            try
            {
                DataTable dt = thickClient.GetMandatoryColumns(departmentName, subDocumentName, docType);
                stringMandatoryList = ConvertDataTable<string>(dt);
                //stringMandatoryList = thickClient.GetMandatoryColumns(departmentName, subDocumentName, docType).ToList();
            
            }
            catch (Exception ex)
            {
                //errorLogClient.ErrorLogWithDescrtion(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetMandatoryColumns");               
                ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetMandatoryColumns");
            }

            return stringMandatoryList;
        }

        public long GetDocType(string departmentName, string subDocumentName, string docType)
        {
            long docTtypeId = 0;
            try
            {
                docTtypeId = thickClient.GetDocTtypeId(departmentName, subDocumentName, docType);
               
            }
            catch (Exception ex)
            {
                //errorLogClient.ErrorLogWithDescrtion(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetDocType");               
                ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetDocType");
            }

            return docTtypeId;
        }

        public bool CheckDocumentType(DataTable dts)
        {
            bool isValid;

            try
            {
                if (dts != null && dts.Rows.Count > 0)
                {
                    #region Check Departmens

                    //Check departments 
                    var departments = (from a in dts.AsEnumerable()
                                       select a.Field<string>("DepartmentName")).ToList().Distinct();

                    var department1 = departments.Where(i=>i!=null).Select(a => a).ToList();

                    if (department1 != null && department1.Count() == 1)
                    {
                        #region Sub-Documents
                        //Check sub- departments
                        var subdepartments = (from a in dts.AsEnumerable()
                                              select a.Field<string>("SubDepartmentName")).ToList().Distinct();

                        var subdepartments1 = subdepartments.Where(i => i != null).Select(a => a).ToList();

                        if (subdepartments1 != null && subdepartments1.Count() == 1)
                        {

                            #region DocumentType

                            //Check document Type
                            var documentTyps = (from a in dts.AsEnumerable()
                                                select a.Field<string>("DocType")).ToList().Distinct();

                            var documentTyps1 = documentTyps.Where(i => i != null).Select(a => a).ToList();

                            if (documentTyps1 != null && documentTyps1.Count() == 1)
                            {
                                string departmentName = department1.Select(a => a).FirstOrDefault();
                                string subDepartMent = subdepartments1.Select(b => b).FirstOrDefault();
                                string documentTyp = documentTyps1.Select(c => c).FirstOrDefault();

                                //Get all columns from database
                                List<string> dtColumns = GetColumns(departmentName, subDepartMent, documentTyp);
                                dtColumns.Remove("DocumentPath"); // Remove this column from validation part becoz not required - Parul

                                if (CompareTable(dts, dtColumns))
                                {
                                    //Check Mandatory fields
                                    //List<string> dtMandatoryColumns = GetMandatoryColumns(departmentName, subDepartMent, documentTyp);
                                    //string strMandatoryFields = string.Join<string>(",", dtMandatoryColumns);
                                    isValid = true;                            
                                }
                                else
                                {
                                    //MessageBox.Show("Mismatch between metadata fields values in Database and excel uploaded");
                                    isValid = false;
                                }
                               
                            }
                            else
                            {
                                //MessageBox.Show("All document type should be same.");
                                isValid = false;
                                
                            }

                            #endregion
                        }
                        else
                        {
                            //MessageBox.Show("All sub-department should be same.");
                            isValid = false;
                            
                        }
                        #endregion
                    }
                    else
                    {
                        //MessageBox.Show("All department should be same.");
                        isValid = false;
                       
                    }

                    #endregion
                }
                else
                {
                    //MessageBox.Show("Sheet name does not exist or No data to import");
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                //errorLogClient.ErrorLogWithDescrtion(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "CheckDocumentType");               
                ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "CheckDocumentType");
                isValid = false;
            }

            return isValid;
        }

        public bool CompareTable(DataTable dtBaseTable, List<string> dtDBTable)
        {
            bool isContains = true;
            try
            {
               
                DataColumnCollection dc = dtBaseTable.Columns;

                //foreach (string item in dtDBTable)
                //{
                //    if (!dc.Contains(item))
                //    {
                //        isContains = false;
                //        return isContains;
                //    }
                //}
                foreach (string item in dtDBTable)
                {

                    if (!dc.Contains(item.Trim()))
                    {
                        isContains = false;
                        return isContains;
                    }
                }

            }
            catch (Exception ex)
            {
                //errorLogClient.ErrorLogWithDescrtion(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "CompareTable");               
                ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "CompareTable");
            }

            return isContains;
        }

        public bool CompareMandatoryFields(DataTable dtBaseTable, List<string> dtDBTable)
        {
            bool isContains = true;
            try
            {
               // DataColumnCollection dc = dtBaseTable.Columns;

                foreach (string item in dtDBTable)
                {
                    // check Mandatory fields
                    var CheckMandatryFields = (from a in dtBaseTable.AsEnumerable()
                                               where a.Field<string>(item) != null || a.Field<string>(item).Equals(string.Empty)
                                               select a.Field<string>(item)).ToList();

                    if (CheckMandatryFields != null && CheckMandatryFields.Count() > 0)
                    {
                        isContains = false;
                        break;
                    }
                    return isContains;                       
                }
            }
            catch (Exception ex)
            {
                //errorLogClient.ErrorLogWithDescrtion(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "CompareMandatoryFields");               
                ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "CompareMandatoryFields");
            }

            return isContains;
        }

        public List<UploadForms> GetMetaDataList(List<string> dtColumns,DataRow row, string departmentName)
        {
            List<UploadForms> lst = new List<UploadForms>();
            string[] strExcludeItems = { "Document Name", "DocumentPath","DepartmentName","SubDepartmentName","DocType" };
            
            try
            {
                if (dtColumns !=null && dtColumns.Count() > 0 )
                {
                   var result =  dtColumns.Except(strExcludeItems);

                   if (departmentName.ToUpper().Equals(Utility.REGISTRATION_AND_STAMPS_DEPT_NAME.ToUpper()))
                   {
                       ProcessRegistrationAndStampsMetaData(row, lst, result);
                   }
                   else
                   {
                       foreach (string str in result)
                       {
                           UploadForms metaData = new UploadForms();

                           //if (str == Convert.ToString(row[str]))
                           // {
                           metaData.displayText = Convert.ToString(str).Trim();
                           metaData.value = Convert.ToString(Convert.ToString(row[str.Trim()]));
                           lst.Add(metaData);
                           //}
                       }
                   }
                }
            }
            catch (Exception ex)
            {
                //errorLogClient.ErrorLogWithDescrtion(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetMetaDataList");               
                ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  UploadFileBL", Convert.ToString(ex.InnerException), "GetMetaDataList");
            }

            return lst;
        }

        private void ProcessRegistrationAndStampsMetaData(DataRow row, List<UploadForms> lst, IEnumerable<string> result)
        {

            string DistrictCode = "0", SROCode = "0", MainTypeDocumentID = "0", SubTypeDocumentID = "0";

            DistrictCode = Convert.ToString(row[RegistrationNStamps.DistrictCode]);
            SROCode = Convert.ToString(row[RegistrationNStamps.SROCode]);
            MainTypeDocumentID = Convert.ToString(row[RegistrationNStamps.MainTypeofDocument]);
            SubTypeDocumentID = Convert.ToString(row[RegistrationNStamps.SubTypeofDocument]);

            RegNStamps obRegNStamps = GetRegNStamps_DDlData(DistrictCode, SROCode, MainTypeDocumentID, SubTypeDocumentID);

            foreach (string str in result)
            {
                UploadForms metaData = new UploadForms();
                switch (Convert.ToString(str).Trim())
                {
                    case RegistrationNStamps.DistrictCode:
                        metaData.displayText = RegistrationNStamps.DistrictCode;
                        metaData.value = obRegNStamps.District_Code.ToString();
                        lst.Add(metaData);
                        break;
                    case RegistrationNStamps.SROCode:
                        metaData.displayText = RegistrationNStamps.SROCode;
                        metaData.value = obRegNStamps.SRO_Code.ToString();
                        lst.Add(metaData);
                        break;
                    case RegistrationNStamps.MainTypeofDocument:
                        metaData.displayText = RegistrationNStamps.MainTypeofDocument;
                        metaData.value = obRegNStamps.MainType_Document.ToString();
                        lst.Add(metaData);
                        break;
                    case RegistrationNStamps.SubTypeofDocument:
                        metaData.displayText = RegistrationNStamps.SubTypeofDocument;
                        metaData.value = obRegNStamps.SubType_Document.ToString();
                        lst.Add(metaData);
                        break;
                    default:
                        metaData.displayText = Convert.ToString(str).Trim();
                        metaData.value = Convert.ToString(Convert.ToString(row[str.Trim()]));
                        lst.Add(metaData);
                        break;
                }
            }

        }

        public RegNStamps GetRegNStamps_DDlData(string argDistrictCode, string argSROCode, string argMainTypeDocumentID, string argSubTypeDocumentID)
        {
            RegNStamps obRegNStamps = new RegNStamps();

            try
            {
                DataSet ds = thickClient.GetRegNStampsDDLData(argDistrictCode, argSROCode, argMainTypeDocumentID, argSubTypeDocumentID);
               
                if (ds.Tables.Count > 0)
                {
                    if(ds.Tables[0].Rows.Count > 0)
                    {
                        obRegNStamps.District_Code = ds.Tables[0].Rows[0]["District_Desc"].ToString();
                        obRegNStamps.SRO_Code = ds.Tables[0].Rows[0]["SRO_Desc"].ToString();
                    }
                    else
                    {
                        obRegNStamps.District_Code = argDistrictCode;
                        obRegNStamps.SRO_Code = argSROCode;
                    }

                    if(ds.Tables[1].Rows.Count>0)
                    {
                        obRegNStamps.MainType_Document = ds.Tables[1].Rows[0]["MainType_Document"].ToString();
                        obRegNStamps.SubType_Document = ds.Tables[1].Rows[0]["SubType_Document"].ToString();
                    }
                    else
                    {
                        obRegNStamps.MainType_Document = argMainTypeDocumentID;
                        obRegNStamps.SubType_Document = argSubTypeDocumentID;
                    }
                }

                return obRegNStamps;
            }
            catch (Exception ex)
            { 
                //ServiceErrorLog.ErrorLog(ex.Message, Convert.ToString(ex.GetType().Name), ex.StackTrace, "E-Archival DMS ThickClient -  GetMetaDataList", Convert.ToString(ex.InnerException), "GetRegNStamps_DDlData");
                obRegNStamps.District_Code = argDistrictCode;
                obRegNStamps.MainType_Document = argMainTypeDocumentID;
                obRegNStamps.SRO_Code = argSROCode;
                obRegNStamps.SubType_Document = argSubTypeDocumentID;
            }

            return obRegNStamps;
        }
    }
}
