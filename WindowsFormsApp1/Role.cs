using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class Role 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Другие свойства роли

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
