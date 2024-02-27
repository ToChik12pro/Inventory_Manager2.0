using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    internal class User 
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        // Другие свойства пользователя

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}

