using KianUSA.Application.Data;
using KianUSA.Application.Services.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            var User = await Db.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Email.ToLower() == Email.ToLower());

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
                UserName = User.UserName,
                Email = User.Email,
                Name = User.Name,
                LastName = User.LastName,
                Roles = Roles.Select(x => x.Name).ToList(),
                Pages = Roles.Select(x => x.Pages).ToList(),
                Prices = Roles.Select(x => x.Prices).ToList()
            };
        }

        public async Task<CustomerDto> GetEmail(string UserName)
        {
            using var Db = new Context();
            return await Db.Users.Where(x => x.UserName == UserName).Select(x => new CustomerDto { Email = x.Email, FullName = x.Name + " " + x.LastName }).FirstOrDefaultAsync();
        }

        public async Task<List<CustomersOfRepDto>> GetCustomersOfRep(string RepUserName)
        {
            using var Db = new Context();
            var Customers = await Db.Users.Where(x => x.Rep.Contains(RepUserName)).ToListAsync();
            if (Customers?.Count > 0)
            {
                return Customers.Select(x =>
                new CustomersOfRepDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Family = x.LastName,
                    UserName = x.UserName
                }).ToList();
            }
            else
                return null;
        }
    }
}
