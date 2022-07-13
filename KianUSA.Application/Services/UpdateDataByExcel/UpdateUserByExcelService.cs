using KianUSA.Application.Data;
using KianUSA.Application.Entity;
using KianUSA.Application.Services.Helper;
using KianUSA.Application.Services.UpdateDataByExcel.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.Application.Services.UpdateDataByExcel
{
    public class UpdateUserByExcelService
    {
        public async Task Update(Stream stream)
        {
            if (stream is null)
                throw new Exception("Please upload correct excel file.");
            List<User> Users = new();
            try
            {                
                var Tables = UpdateByExcelHelper.ReadExcel(stream);
                if (Tables?.Count > 0 && Tables[0].Rows?.Count > 0)
                {
                    for (int i = 0; i < Tables[0].Rows.Count; i++)
                    {
                        var Row = Tables[0].Rows[i];
                        if (Users.Find(x => x.Email.Equals(Row["Email"].ToString().Trim(), StringComparison.OrdinalIgnoreCase)) is null)
                        {
                            Guid Id = Guid.NewGuid();
                            User NewUser = new()
                            {
                                Id = Id,
                                Email = Row["Email"].ToString().Trim(),
                                Name = Row["Name"].ToString().Trim(),
                                LastName = Row["Last Name"].ToString().Trim(),
                                StoreName = Row["Store Name"].ToString().Trim(),
                                Password = Password(Row["Password"]),
                                Security = Row["Security"].ToString().Trim(),
                                Roles = await GetRolesByName(Id, Row["Roles"]?.ToString()),
                            };
                            Users.Add(NewUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("there are some errors during reading data from excel.[" + ex.Message + "]");
            }

            await UpdateDatabase(Users);
        }
        private static async Task UpdateDatabase(List<User> Users)
        {
            try
            {
                using var Db = new Context();
                using var Trans = await Db.Database.BeginTransactionAsync();
                try
                {
                    Db.Database.ExecuteSqlRaw("DELETE FROM \"User\"");
                    Db.Users.AddRange(Users);
                    await Db.SaveChangesAsync();
                    Trans.Commit();
                }
                catch
                {
                    Trans.Rollback();
                    throw new Exception("Cannot update database");
                }
            }
            catch
            {
                throw new Exception("Cannot connect To the database");
            }
        }

        private async Task<List<UserRole>> GetRolesByName(Guid UserId, string RoleNames)
        {

            if (!string.IsNullOrWhiteSpace(RoleNames))
            {

                try
                {
                    using var Db = new Context();
                    var Roles = await Db.Roles.Select(x => new { x.Id, x.Name }).ToListAsync().ConfigureAwait(false);

                    List<UserRole> UsersRoles = new();
                    string[] RolNames = RoleNames.ToString().Split(",");
                    foreach (var RolName in RolNames)
                    {
                        var RoleId = Roles.Find(x => x.Name.Equals(RolName, StringComparison.OrdinalIgnoreCase));
                        if (RoleId is not null)
                            UsersRoles.Add(new UserRole() { RoleId = RoleId.Id, UserId = UserId });
                    }
                    return UsersRoles;
                }
                catch
                {
                    throw new Exception("Cannot connect To the database");
                }

            }
            return null;
        }
        private string Password(object Pass)
        {
            string Passoword = Pass.ToString();
            if (!string.IsNullOrWhiteSpace(Passoword))
                return Tools.HashData(Passoword);
            else
                return Tools.HashData("123456");
        }
    }
}
