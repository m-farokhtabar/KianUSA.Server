﻿using System;

namespace KianUSA.Domain.Entity
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
        /// <summary>
        /// [Group],[Group],[Group]
        /// ["Group","Group","Group"] json => Db Format
        /// </summary>
        public string Groups { get; set; }
    }
}
