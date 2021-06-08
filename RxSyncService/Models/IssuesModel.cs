using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService.Models
{
    class IssuesModel
    {
        public class Root
        {
            public int count { get; set; }
            public int per_page { get; set; }
            public int current_page { get; set; }
            //public Issue[] items { get; set; }
            public List<Issue> items { get; set; }
        }

        public class Issue
        {
            public int issue_id { get; set; }
            public int requisition_id { get; set; }
            public string issued_by_str { get; set; }
            public DateTime issue_date { get; set; }
            public DateTime received_date { get; set; }
            public string receivedBy_str { get; set; }
            public DateTime lastUpdate_dat { get; set; }
            //public Issueitems[] issueItems { get; set; }
            public List<Issueitems> issueItems { get; set; }
        }

        public class Issueitems
        {
            public int id { get; set; }
            public int requisition_id { get; set; }
            public string product_code { get; set; }
            public int qtyIssued_int { get; set; }
            public string batchNumber_str { get; set; }
            public DateTime expiry_Dat { get; set; }
            public DateTime created_at { get; set; }
            public DateTime modified_at { get; set; }
        }
    }
}
