using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class StockItem 
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateAdded { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
