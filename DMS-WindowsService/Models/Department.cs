using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DMS_WindowsService.Models
{
    public class Department
    {
        public Int64 DepartmentId { get; set; }
        
        public string DepartmentName { get; set; }
    }

    public enum DMSDocumentType
    {
        CitizenIdProof = 1,
        CitizenAffiDevit = 2,
        DocumentUploadMain = 3,
        DocumentUploadPreview = 4
    }
}