using System;
using System.Collections.Generic;
using System.Linq;

namespace KianUSA.Application.Services.Helper
{
    public static class Tools
    {
        public static string HashData(string Data)
        {
            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Data);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes);
        }
        public static List<string> SecurityToList(string Security) => string.IsNullOrWhiteSpace(Security) ? null : Security.Split(",").ToList();
    }
}
