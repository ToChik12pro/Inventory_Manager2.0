using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class PurchaseOrderItem 
    {
        public int Id { get; set; }
        [ForeignKey("PurchaseOrder")]
        public int PurchaseOrderId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Навигационные свойства
        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}

