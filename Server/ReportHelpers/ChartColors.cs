using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.ReportHelpers
{
    public class ChartColors
    {
        public int ColorId { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public static ArrayList GetChartColorsArrayList()
        {
            ArrayList al = new ArrayList();
            al.Add(new ChartColors { ColorId = 1, Red = 128, Blue = 128, Green = 0 }); //Olive
            al.Add(new ChartColors { ColorId = 2, Red = 128, Blue = 0, Green = 0 }); //Maroon
            al.Add(new ChartColors { ColorId = 3, Red = 107, Blue = 142, Green = 45 }); //OliveDrab 
            al.Add(new ChartColors { ColorId = 4, Red = 255, Blue = 165, Green = 0 }); //Orange
            al.Add(new ChartColors { ColorId = 5, Red = 205, Blue = 133, Green = 63 }); //Peru
            al.Add(new ChartColors { ColorId = 6, Red = 128, Blue = 0, Green = 128}); //Purple
            al.Add(new ChartColors { ColorId = 7, Red = 255, Blue = 0, Green = 0 }); //Red
            al.Add(new ChartColors { ColorId = 8, Red = 65, Blue = 105, Green = 225 }); //RoyalBlue
            al.Add(new ChartColors { ColorId = 9, Red = 0, Blue = 128, Green = 0 }); //Green
            al.Add(new ChartColors { ColorId = 10, Red = 178, Blue = 34, Green = 34 }); //Firebrick            
            al.Add(new ChartColors { ColorId = 11, Red = 139, Blue = 69, Green = 19 }); //SaddleBrown
            al.Add(new ChartColors { ColorId = 12, Red = 70, Blue = 130, Green = 180 }); //SteelBlue
            al.Add(new ChartColors { ColorId = 13, Red = 0, Blue = 128, Green = 128 }); //Teal
            al.Add(new ChartColors { ColorId = 14, Red = 253, Blue = 99, Green = 71 }); //Tomato
            al.Add(new ChartColors { ColorId = 15, Red = 255, Blue = 255, Green = 0 }); //Yellow
            al.Add(new ChartColors { ColorId = 16, Red = 119, Blue = 136, Green = 153 }); //LightSlateGray
            al.Add(new ChartColors { ColorId = 17, Red = 160, Blue = 82, Green = 45 }); //Sienna
            al.Add(new ChartColors { ColorId = 18, Red = 192, Blue = 192, Green = 192 }); //Silver            
            al.Add(new ChartColors { ColorId = 19, Red = 240, Blue = 128, Green = 128 }); //LightCoral
            al.Add(new ChartColors { ColorId = 20, Red = 46, Blue = 139, Green = 87 }); //SeaGreen

            return (al);
        }

        public static ChartColors[] GetChartColorsArray()
        {
            return ((ChartColors[])GetChartColorsArrayList().ToArray(typeof(ChartColors)));
        }

    }      
}
