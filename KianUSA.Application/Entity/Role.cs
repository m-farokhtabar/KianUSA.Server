using System;

namespace KianUSA.Application.Entity
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        ///  ایندکس قیمت های قابل نمایش یرای محصولات
        ///  [0],[1],[2],[3] => Excel Format
        ///  [0,1,2,3] json => Db Format
        /// </summary>
        public string Prices { get; set; }
        /// <summary>
        /// PageName[R,W,E,D],PageName[R,W,E,D]
        /// </summary>
        public string Pages { get; set; }
    }
}
