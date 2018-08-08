﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain
{
    public class User
    {
        public string Name { get; private set; }

        public User(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
