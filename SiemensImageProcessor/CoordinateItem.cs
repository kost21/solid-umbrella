using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiemensImageProcessor
{
    public class CoordinateItem
    {
        public CoordinateItem()
        {
            this.JpgFilePath = string.Empty;
        }

        public string JpgFilePath { get; set; } = string.Empty;

        public int RowNumberInExcel { get; set; } = 0;

        public string Coordinate { get; set; } = string.Empty;

        public bool IsForFastTestBase { get; set; } = false;

        public bool Success { get; set; } = false;
    }
}
