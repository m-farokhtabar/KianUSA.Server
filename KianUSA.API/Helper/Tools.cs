using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KianUSA.API.Helper
{
    public static class Tools
    {
        public static string NullStringToEmpty(string Value) => Value is null ? string.Empty : Value;
        public static List<string> GetRoles(ServerCallContext context)
        {
            List<string> Roles = new();
            var RoleClaimValue = context.GetHttpContext()?.User?.Claims?.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
            if (RoleClaimValue is not null)
            {
                Roles = RoleClaimValue;
            }
            return Roles;
        }
    }
}
