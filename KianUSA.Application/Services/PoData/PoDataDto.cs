using System.Collections.Generic;

namespace KianUSA.Application.Services.PoData
{
    public class PoDataDto
    {
        public List<PoExcelDbDataDto> Data { get; set; }
        public List<ColAccess> ColumnsHavePermission { get; set; }
    }
    public class ColAccess
    {
        public ColAccess()
        {

        }

        public ColAccess(string colName, bool writable)
        {
            ColName = colName;
            Writable = writable;
        }

        public string ColName { get; set; }
        public bool Writable { get; set; }
    }
}
