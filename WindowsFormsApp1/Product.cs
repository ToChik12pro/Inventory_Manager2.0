using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть положительным числом.")]
        public decimal Price { get; set; }
        public ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
    }
}
