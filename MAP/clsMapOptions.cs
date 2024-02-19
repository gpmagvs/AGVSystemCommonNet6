using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemCommonNet6.MAP
{
    public class clsMapOptions
    {
        public string pathColor { get; set; } = "rgb(45,42,46)";
        public int fontSizeOfDisplayName { get; set; } = 12;
        public int fontSizeOfAsCandicates { get; set; } = 16;
        public List<string> EQIcons { get; set; } = new List<string>();
        /// <summary>
        /// 格線尺寸(單位:公尺)
        /// </summary>
        public double gridSize { get; set; } = 1;
        /// <summary>
        /// 預設地圖背景是否顯示
        /// </summary>
        public bool defaultShowBackgroudImage { get; set; } = false;

    }
}
