﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KianUSA.Application.Entity
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public int Order { get; set; }
    }
}
