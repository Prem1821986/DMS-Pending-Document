using DMS.Business;
using DMS_WindowsService.BAL;
using DMS_WindowsService.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace DMS_WindowsService
{
    partial class DMSPendingDocumentService : ServiceBase
    {
        ServiceLog ServiceErrorLog = null;
        UploadFileBL uploadBL = null;

        ThickClientService thickClient = null;
        int NumOfThreads = 16;

        public DMSPendingDocumentService()
        {
            InitializeComponent();
            ServiceErrorLog = new ServiceLog();
            thickClient = new ThickClientService(Utility.constr);
            NumOfThreads = 16;
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            ServiceErrorLog.ErrorLog("On Start", "DAILY", "", "", "", "OnStart");
            //args = new string[] { "DemoService", "Art, Culture and Archaeology Department", "Rajasthan State Archive, Bikaner", "Historical Record" };

            string str = "";
            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    str += args[i] + "_ ";
                }
                ServiceErrorLog.ErrorLog("On Start", "Parameters", str, "", "", "OnStart");
                FirstThreadFunc(args[1], args[2], args[3], args[4]);
                ShutDownService(args[0]);
            }

            ServiceErrorLog.ErrorLog("On Start Executed", "DAILY", "", "", "", "OnStart");
        }

        private void FirstThreadFunc(string deptID, string subDeptID, string docTypeID, string userID = "", string frmDate = "", string toDate = "")
        {
            int departmentid = Convert.ToInt32(deptID);
            int SubDepartmentid = Convert.ToInt32(subDeptID);
            int DocTypeID = Convert.ToInt32(docTypeID);
            string fromdate = string.Empty;
            DataTable dt = thickClient.GetPendingDocumentDtls(departmentid, SubDepartmentid, DocTypeID, fromdate, toDate);

            ServiceErrorLog.ErrorLog("Document Count", Convert.ToString(dt.Rows.Count), "", "", userID, "UploadDocumentToDMS");
            this.UploadDocumentToDMS(dt, departmentid, SubDepartmentid, DocTypeID, userID);
        }

        protected void UploadDocumentToDMS(DataTable dtDoc, int deptID, int subDeptID, int docTypeID, string userID)
        {
            string rootPath = string.Empty;

            foreach (DataRow drrow in dtDoc.Rows)
            {

                #region variable Initialization
                var DocName = drrow["DocumentName"].ToString();
                var DocumentId = drrow["DocumentId"].ToString();
                var Department = drrow["Department"].ToString();
                var DocTypeName = drrow["DocTypeName"].ToString();
                #endregion

                ServiceErrorLog.ErrorLog("Document Name", DocName, "", userID, "", "UploadDocumentToDMS");
                //if(DocTypeName == "")

                string extension = Path.GetExtension(DocName);
                switch (extension.ToLower())
                {
                    case ".pdf":
                        //rootPath = "~/ESignUpload/ALL_PDF/";
                        rootPath = System.Configuration.ConfigurationManager.AppSettings["ReadPdfDocumentFilePath"].ToString();
                        if (!System.IO.File.Exists(rootPath + DocName))
                        {
                            //rootPath = "~/UploadedDocument/";
                            rootPath = System.Configuration.ConfigurationManager.AppSettings["ReadFilePathWithoutPdf"].ToString();
                        }
                        break;
                    default:
                        //rootPath = "~/UploadedDocument/";
                        rootPath = System.Configuration.ConfigurationManager.AppSettings["ReadFilePathWithoutPdf"].ToString();
                        break;
                }
                string argSourcePath = rootPath;
                string path = argSourcePath + DocName;
                ServiceErrorLog.ErrorLog("Document Path", path, "", userID, "", "UploadDocumentToDMS");
                if (System.IO.File.Exists(path))
                {
                    ServiceErrorLog.ErrorLog("Document Path Exist", DocName, "", "", "", "UploadDocumentToDMS");
                    DataTable dtDMS = thickClient.GetPendingDocumentFromDMS(DocumentId, DocName);
                    if (dtDMS.Rows.Count > 0)
                    {
                        var DMSDocumentId = UploadFileToDMS(dtDMS, argSourcePath, DocTypeName, DocumentTypeForEmitra.DocumentUpload); // 1
                        if (DMSDocumentId > 0)
                        {
                            thickClient.InsertDMSPendingDocsPushDtl(Convert.ToInt32(DocumentId), Convert.ToInt32(DMSDocumentId), Convert.ToInt32(userID));
                            if (Convert.ToInt64(dtDMS.Rows[0]["DocStatus"]) != 4)
                            {
                                string result = thickClient.InsertDocStatus(Convert.ToInt32(DocumentId), 4, 0, ReviewStatus.Published.ToString(), ReviewStatus.Published.ToString());
                            }

                        }
                    }
                    else
                    {
                        //UploadFileToDMS(DocumentId, DocName, Department, DocTypeName, DMSDocumentType.DocumentUploadMain, DocumentTypeForEmitra.DocumentUpload, path, DocumentType, argUserID);
                    }
                }
                else
                {
                    ServiceErrorLog.ErrorLog("Document Path Not Exist", DocName, "", "ocument not found.", "", "UploadDocumentToDMS");
                }

                //end if(DocTypeName == "")
            }
        }


        public Int64 UploadFileToDMS(DataTable dt, string argSourcePath, string DocType, DocumentTypeForEmitra argDocumentTypeForEmitra)
        {

            Int64 DMSDocId = 0; string DMSDocPath = string.Empty;
            try
            {
                var uploadFile = new UploadFile();
                int insertedId = 0;

                if (dt.IsValid())
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        uploadFile.CreatedBy = Convert.ToInt64(dt.Rows[i]["CreatedBy"]);
                        uploadFile.UpdatedBy = Convert.ToInt64(dt.Rows[i]["UpdatedBy"]);
                        uploadFile.CreationDate = Convert.ToDateTime(dt.Rows[i]["CreationDate"]);
                        uploadFile.UpdatedOn = Convert.ToDateTime(dt.Rows[i]["UpdatedOn"]);
                        uploadFile.DepartmentName = Convert.ToString(dt.Rows[i]["DepartmentName"]);
                        uploadFile.DocName = Convert.ToString(dt.Rows[i]["DocName"]);
                        uploadFile.DocType = Convert.ToString(dt.Rows[i]["DocType"]);
                        uploadFile.Param1 = Convert.ToString(dt.Rows[i]["Param1"]);
                        uploadFile.UniqueNumber = new Guid(Convert.ToString(dt.Rows[i]["UniqueNumber"]));
                        uploadFile.UploadedDocumentId = Convert.ToInt64(dt.Rows[i]["UploadedDocumentId"]);

                        uploadFile.Param5 = Convert.ToString(dt.Rows[i]["Param5"]);
                        uploadFile.Param6 = Convert.ToString(dt.Rows[i]["Param6"]);
                        uploadFile.DocActive = Convert.ToString(dt.Rows[i]["DocActive"]);
                        uploadFile.Param4 = Convert.ToString(dt.Rows[i]["Param4"]);
                        uploadFile.DocCategory = Convert.ToString(dt.Rows[i]["DocCategory"]);
                        insertedId = Convert.ToInt32(dt.Rows[i]["Id"]);

                        var jsonrequest = JsonConvert.SerializeObject(uploadFile);
                        ServiceErrorLog.ErrorLog("ReAMS API", jsonrequest, "", "", "", "UploadFileToDMS");

                        string File = argSourcePath + uploadFile.DocName;

                        if (System.IO.File.Exists(File))
                        {
                            //try
                            //{
                            var ObjRISL = new WebReference.DMSWebService_Raj();


                            string ImpersonationUserName = string.Empty;
                            string ImpersonationIP = string.Empty;
                            string ImpersonationPassword = string.Empty;
                            string DMSFilePath = string.Empty; // ConfigurationManager.AppSettings["DMSFilePath"].ToString() + DocName;

                            DMSConfiguration dtDMS = Common.Common.GetDMSConfiguration();

                            DMSFilePath = dtDMS.DMSFilePath + uploadFile.DocName;

                            // Check this path is its working then fine otherwise remove this path. Its used in the service to upload the Documents on DMS Server.
                            ObjRISL.Url = dtDMS.DMSWebService;
                            DMSDocPath = dtDMS.DMSFilePath;
                            try
                            {
                                using (new Impersonator(dtDMS.ImpersonationUserName, dtDMS.ImpersonationIP, dtDMS.ImpersonationPassword))
                                {
                                    // The following code is executed under the impersonated user.
                                    //System.IO.File.Copy(fileToUpload, DMSFilePath);
                                    System.IO.File.Copy(File, DMSFilePath, true);
                                    ServiceErrorLog.ErrorLog("File Copy Success", "", "", "", "", "UploadDocumentToDMS");
                                }
                            }
                            catch (Exception ex)
                            {
                                ServiceErrorLog.ErrorLog("File Copy Exception", ex.Message, ex.StackTrace, "File is not Copy at DMS Server", "", "UploadFileToDMS");
                            }

                            string sInputXML; string sOutputXml; string base64xml; string folderName = DocType; string folderHierarchy = uploadFile.DepartmentName;
                            string userName = dtDMS.DMSUser;
                            string password = dtDMS.DMSPass;
                            string dataClass = dtDMS.DMSDataClass;

                            //string actualxml;

                            sInputXML = @"<NGOAddDocument_Input>
                            <Documents>
                            <Document>
                            <DocumentName>" + uploadFile.DocName.ToString() + @"</DocumentName>
                            <DataDefinition>
                            <Fields>
                            <Field><IndexName>DepartmentName</IndexName><IndexValue>" + uploadFile.DepartmentName + "</IndexValue></Field>"
                                            + "<Field><IndexName>DocType</IndexName><IndexValue>" + uploadFile.DocType + "</IndexValue></Field>"
                                            + "<Field><IndexName>DocName</IndexName><IndexValue>" + uploadFile.DocName + "</IndexValue></Field>"
                                            + "<Field><IndexName>UniqueNumber</IndexName><IndexValue>" + uploadFile.UniqueNumber + "</IndexValue></Field>"
                                            + "<Field><IndexName>DocumentID</IndexName><IndexValue>" + insertedId + "</IndexValue></Field>"
                                            + "<Field><IndexName>Param1</IndexName><IndexValue>" + uploadFile.Param1 + "</IndexValue></Field>"
                                            + "<Field><IndexName>DocCategory</IndexName><IndexValue>" + uploadFile.DocCategory + "</IndexValue></Field>"
                                            + "<Field><IndexName>DocActive</IndexName><IndexValue>" + uploadFile.DocActive + "</IndexValue></Field>"
                                            + "<Field><IndexName>Param4</IndexName><IndexValue>" + uploadFile.Param4 + "</IndexValue></Field>"
                                            + "<Field><IndexName>Param5</IndexName><IndexValue>" + uploadFile.Param5 + "</IndexValue></Field>"
                                            + "<Field><IndexName>Param6</IndexName><IndexValue>" + uploadFile.Param6 + "</IndexValue></Field>"
                                            + "</Fields>"
                                            + "</DataDefinition>"
                                            + "</Document>"
                                            + "</Documents>"
                                       + "</NGOAddDocument_Input>";


                            ServiceErrorLog.ErrorLog("ReAMS API", sInputXML, "", "", "", "UploadFileToDMS");
                            byte[] byt = System.Text.Encoding.UTF8.GetBytes(sInputXML);
                            //convert the byte array to a Base64 string
                            base64xml = Convert.ToBase64String(byt);

                            //byte[] b = Convert.FromBase64String(base64xml);
                            //actualxml = System.Text.Encoding.UTF8.GetString(b);

                            sOutputXml = ObjRISL.addNGDocumentExt(base64xml.ToString(), userName, password, DMSDocPath, folderName, folderHierarchy, dataClass);

                            ServiceErrorLog.ErrorLog("ReAMS External API", sOutputXml, "", "", "", "UploadFileToDMS");

                            XDocument xmlDoc = XDocument.Parse(sOutputXml);

                            var Status = xmlDoc.Descendants("NGAddDocumentExt_Output")
                                                .Descendants("Document")
                                                .Descendants("Description")
                                                .FirstOrDefault().Value;

                            ServiceErrorLog.ErrorLog("ReAMS External API Status", Status, "", "", "", "UploadFileToDMS");
                            if (Status == "Success")
                            {
                                var DMSDocID = xmlDoc.Descendants("NGAddDocumentExt_Output")
                                            .Descendants("Document")
                                            .Descendants("Document_Id")
                                            .FirstOrDefault().Value;
                                var PDfFlag = xmlDoc.Descendants("NGAddDocumentExt_Output")
                                            .Descendants("Document")
                                            .Descendants("DocumentType")
                                            .FirstOrDefault().Value;

                                uploadFile.DMSDocumentId = Convert.ToInt64(DMSDocID);
                                uploadFile.DMSDocType = PDfFlag;

                                ServiceErrorLog.ErrorLog("", "DMSDocumentId: " + DMSDocID + " :: DMSDocType: " + PDfFlag, "", "", "", "UploadFileToDMS");

                                DMSDocId = Convert.ToInt64(DMSDocID);
                                DataTable dtdms = thickClient.AddUpdateDMSDocumentDetails(uploadFile);

                                if (argDocumentTypeForEmitra != DocumentTypeForEmitra.Citizen)
                                {
                                    thickClient.UpdateDMSDocumentID(Convert.ToInt32(dt.Rows[i]["UploadedDocumentId"]), int.Parse(DMSDocID)); // Divya - can skip
                                }
                            }

                            try
                            {
                                if (DMSDocId != 0)
                                {
                                    System.IO.File.Delete(File);
                                }
                            }
                            catch (Exception ex)
                            {
                                ServiceErrorLog.ErrorLog("Exception", ex.Message.ToString(), ex.StackTrace, "", "", "UploadFileToDMS");
                            }

                            ObjRISL.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceErrorLog.ErrorLog("Exception Occured", ex.Message, ex.StackTrace, "DMS Server is Down.", "", "UploadFileToDMS");
            }
            return DMSDocId;
        }

        public void ShutDownService(string serviceName, string deptName = "Pending Document")
        {
            try
            {
                string response = "";

                //string uri = "http://103.203.139.230/WindowServices/StopService?nameOfService=" + serviceName + "&deptName=" + deptName;
                string uri = "https://reams.rajasthan.gov.in/WindowServices/StopService?nameOfService=" + serviceName + "&deptName=" + deptName;

                WebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                HttpWebResponse res = null;
                res = (HttpWebResponse)request.GetResponse();
                using (Stream stm = res.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stm);
                    response = sr.ReadToEnd();
                    sr.Close();
                }

                ServiceErrorLog.ErrorLog("Information inside ReAMS API", "Success Message", response, "", "", "ShutDownService");
            }
            catch (Exception ex)
            {
                ServiceErrorLog.ErrorLog("Information inside ReAMS API", ex.Message, ex.StackTrace, ex.ToString(), "", "ShutDownService");
                throw ex;
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
