using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DMS_WindowsService.Models
{
    public class UploadForms
    {
        #region properties
        
        public string displayText { get; set; }
         
        public string Type { get; set; }
         
        public string value { get; set; }
        
        public bool isMandatory { get; set; }
         
        public Int32 ID { get; set; }
        
        #endregion
    }
}