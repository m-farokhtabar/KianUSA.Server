using System;

namespace KianUSA.Application.Entity
{
    public class Filter
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        /// <summary>
        /// [Tag][Tag][Tag]... Excel Format
        /// ["Tag","Tag","Tag"] json => Db Format
        /// </summary>
        public string Tags { get; set; }
    }
}
