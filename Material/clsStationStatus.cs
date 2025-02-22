﻿using AGVSystemCommonNet6.PartsModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.Material
{
    [Index(nameof(StationTag))]
    [Index(nameof(StationName))]
    [Index(nameof(MaterialID))]
    public class clsStationStatus
    {
        [Key]
        [MaxLength(150)]  // 或其他適當的長度限制
        public string StationName { get; set; } = "";

        public int StationCol { get; set; } = -1;

        public int StationRow { get; set; } = -1;

        public string StationTag { get; set; } = "-1";

        public string MaterialID { get; set; } = "";

        public MaterialType Type { get; set; } = MaterialType.None;

        public bool IsNGPort { get; set; } = false;

        public bool IsEnable { get; set; } = true;
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
