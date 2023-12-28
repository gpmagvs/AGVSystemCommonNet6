﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class clsPointRegistInfo
    {
        public bool IsRegisted { get; set; } = false;
        public DateTime RegistTime { get; } = DateTime.MinValue;
        public string RegisterAGVName { get; set; } = "";
        public clsPointRegistInfo(string Name)
        {
            IsRegisted = true;
            RegisterAGVName = Name;
            RegistTime = DateTime.Now;
        }
        public clsPointRegistInfo()
        {
            IsRegisted = false;
            RegisterAGVName = "";
            RegistTime = DateTime.MinValue;
        }
    }
}