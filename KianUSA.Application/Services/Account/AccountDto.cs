using System;
using System.Collections.Generic;

namespace KianUSA.Application.Services.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public List<string> Securities { get; set; }
        public string Security { get; set; }
        public List<string> Roles { get; set; }
    }    
}
