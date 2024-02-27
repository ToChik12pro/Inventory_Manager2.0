using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class PurchaseOrder 
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int SupplierId { get; set; }
      

        // Навигационные свойства
       
        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; }
        public ICollection<PurchaseOrderItem> OrderItems { get; set; } = new List<PurchaseOrderItem>();
        


    }
}
