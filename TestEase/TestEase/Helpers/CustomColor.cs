using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiGraphicsColor = Microsoft.Maui.Graphics.Color;

namespace TestEase.Helpers
{

    //custom color helper for the on/off function of the modbus broker
    public class CustomColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public CustomColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public MauiGraphicsColor ToMauiColor()
        {
            return MauiGraphicsColor.FromRgb(R, G, B);
        }
    }

}
