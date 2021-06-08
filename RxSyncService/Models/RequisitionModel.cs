
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxSyncService.Models
{
    class RequisitionModel
    {
        public class RequisitionItem
        {
            public RequisitionItem(
                int id,
                int order_qty,
                string product_code,
                string product_name,
                int requisition_id
            )
            {
                this.id = id;
                this.order_qty = order_qty;
                this.product_code = product_code;
                this.product_name = product_name;
                this.requisition_id = requisition_id;
            }

            public int id { get; }
            public int order_qty { get; }
            public string product_code { get; }
            public string product_name { get; }
            public int requisition_id { get; }
        }

        public class Root
        {
            public Root(
                int demander_id,
                string demander_str,
                int facility_code,
                string ordered_by_str,
                string ordered_dat,
                List<RequisitionItem> requisitionItems,
                string requisition_dat,
                int requisition_id,
                string requisition_str,
                string retail_pharmacy
            )
            {
                this.demander_id = demander_id;
                this.demander_str = demander_str;
                this.facility_code = facility_code;
                this.ordered_by_str = ordered_by_str;
                this.ordered_dat = ordered_dat;
                this.requisitionItems = requisitionItems;
                this.requisition_dat = requisition_dat;
                this.requisition_id = requisition_id;
                this.requisition_str = requisition_str;
                this.retail_pharmacy = retail_pharmacy;
            }

            public int demander_id { get; }
            public string demander_str { get; }
            public int facility_code { get; }
            public string ordered_by_str { get; }
            public string ordered_dat { get; }
            public IReadOnlyList<RequisitionItem> requisitionItems { get; }
            public string requisition_dat { get; }
            public int requisition_id { get; }
            public string requisition_str { get; }
            public string retail_pharmacy { get; }
        }
    }
}
