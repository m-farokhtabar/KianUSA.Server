using System;
using System.Collections.Generic;
using System.Linq;

namespace KianUSA.Application.Services.Account
{
    public class AuthorizationService
    {
        private readonly List<string> userRoles;
        public AuthorizationService(List<string> userRoles)
        {
            this.userRoles = userRoles;
        }
        public bool HasUserPermissionToUseData(string ListOfAcceptedRolesByData)
        {
            if (userRoles.Any(x => string.Equals(x, "admin", StringComparison.OrdinalIgnoreCase)))
                return true;

            if (string.IsNullOrWhiteSpace(ListOfAcceptedRolesByData))
                return true;
            else
            {
                string SecuritytoLower = ListOfAcceptedRolesByData.ToLower();
                foreach (var userRole in userRoles)
                {
                    if (SecuritytoLower.IndexOf("\"" + userRole.ToLower() + "\"") > 0)
                        return true;
                }
            }
            return false;
        }
        public bool IsWritableColumn(string WritableAccess, string ListOfAcceptedRolesByData)
        {
            if (userRoles.Any(x => string.Equals(x, "admin", StringComparison.OrdinalIgnoreCase)))
                return true;

            if (string.IsNullOrWhiteSpace(ListOfAcceptedRolesByData))
                return false;
            if (string.IsNullOrWhiteSpace(WritableAccess))
                return false;

            string[] writableAccessArray = WritableAccess.Replace("[", "").Replace("]", "").Split(",");
            if (writableAccessArray?.Length == 0)
                return false;
            string[] listOfAcceptedRolesByDataArray = ListOfAcceptedRolesByData.ToLower().Replace("[", "").Replace("]", "").Split(",");
            if (listOfAcceptedRolesByDataArray?.Length == 0)
                return false;

            foreach (var userRole in userRoles)
            {
                int index = Array.IndexOf(listOfAcceptedRolesByDataArray, "\"" + userRole.ToLower() + "\"");
                if (index > -1)
                {
                    if (writableAccessArray.Length > index && string.Equals(writableAccessArray[index], "w", StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
            }
            return false;
        }
    }
}
