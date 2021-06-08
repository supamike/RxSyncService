using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService.Models
{
    class Issues
    {
        public int issue_id { get; set; }
        public int requisition_id { get; set; }
        public string issued_by_str { get; set; }
        public DateTime issue_date { get; set; }
        public string receivedBy_str { get; set; }
        public DateTime lastUpdate_dat { get; set; }
        //public IssueItems[] issueItems { get; set; }
        public List<IssueItems> issueItems { get; set; }
    }
}
