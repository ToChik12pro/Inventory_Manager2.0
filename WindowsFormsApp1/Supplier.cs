using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace WindowsFormsApp1
{
    internal class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Эта аннотация указывает, что идентификатор будет генерироваться автоматически
        public int Id { get; set; }

        public string Name { get; set; }
        public string ContactPerson { get; set; }

    }
           
}
