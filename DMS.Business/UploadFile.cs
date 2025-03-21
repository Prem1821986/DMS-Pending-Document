using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Business
{
	public class UploadFile
	{
        #region properties
        public Int64 Id { get; set; }
        public Nullable<Int64> UploadedDocumentId { get; set; }
        public Int64 DMSDocumentId { get; set; }
        public Int64? CitizenAmendmentID { get; set; }
        public string DepartmentName { get; set; }
        public string DocType { get; set; }
        public string DocName { get; set; }
        public Guid UniqueNumber { get; set; }
        public string Param1 { get; set; }
        public DateTime CreationDate { get; set; }
        public Int64 CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Int64 UpdatedBy { get; set; }
        public String FileType { get; set; }
        public string DocCategory { get; set; }
        public string DocActive { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string Param6 { get; set; }
        public string DMSDocType { get; set; }
        public int DmsDocId { get; set; }
        public int DocumentId { get; set; }

        #endregion
    }

	public class DMSResponse
	{
		public string DMSID { get; set; }
		public string DMSDocuemntType { get; set; }
	}

}
