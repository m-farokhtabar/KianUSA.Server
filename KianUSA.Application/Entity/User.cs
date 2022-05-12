using System;
using System.Collections.Generic;

namespace KianUSA.Application.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// MD5
        /// </summary>
        public string Password { get; set; }
        public string LastName { get; set; }
        public string StoreName { get; set; }
        public string Security { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; }
    }
}
