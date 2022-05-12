using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KianUSA.API.Helper
{
    public static class Tools
    {
        public static string NullStringToEmpty(string Value) => Value is null ? string.Empty : Value;
    }
}
