using KianUSA.Application.Data;
using KianUSA.Application.Services.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.Account
{
    public class AccountService
    {
        public async Task<AccountDto> Login(string Email, string Password)
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                throw new Exception("Username(email) or password cannot be empty.");
            using var Db = new Context();
            var User = await Db.Users.Include(x=>x.Roles).FirstOrDefaultAsync(x => x.Email.ToLower() == Email.ToLower());
            
            if (User is null)
                throw new Exception("user is not found.");
            if (User.Password != Tools.HashData(Password))
                throw new Exception("password is wrong");
            var RolesId = User.Roles.Select(x => x.RoleId).ToList();
            var Roles = await Db.Roles.Where(x => RolesId.Contains(x.Id)).ToListAsync();            
            if (Roles?.Count == 0)
                throw new Exception("user is not valid");

            return new AccountDto()
            {
                Id = User.Id,
                Email = User.Email,
                Name = User.Name,
                LastName = User.LastName,
                Roles = Roles.Select(x=>x.Name).ToList(),
                Securities = Tools.SecurityToList(User.Security),
                Security = User.Security
            };
        }
    }
}
