using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService.Models
{
    class IssueItems
    {
        public int id { get; set; }
        public int requisition_id { get; set; }
        public string product_code { get; set; }
        public int qtyIssued_int { get; set; }
        public string batchNumber_str { get; set; }
        public DateTime expiry_Dat { get; set; }
        public DateTime created_at { get; set; }
        public DateTime modified_at { get; set; }
        public int issue_ref_id { get; set; }
    }
}
