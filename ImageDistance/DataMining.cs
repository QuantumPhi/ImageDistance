using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDistance
{
    class DataMiner
    {
        public double Threshold { get; private set; }
        private string query;

        public DataMiner(double threshold, string query)
        {
            Trace.Assert(threshold > 0 && threshold <= 1, "Improper threshold.");
            this.Threshold = threshold;
            this.query = query;
        }
    }

    public static partial class ImageExtensions
    {
        public static Bitmap Negative(this Bitmap original)
        {
            Color temp;
            Bitmap negative = new Bitmap(original.Width, original.Height);

            for (int i = 0; i < negative.Width; i++)
            {
                for (int j = 0; j < negative.Height; j++)
                {
                    temp = original.GetPixel(i, j);
                    negative.SetPixel(i, j, Color.FromArgb(255 - temp.R, 255 - temp.G, 255 - temp.B));
                }
            }

            return negative;
        }

        public static double Distance(this Bitmap origin, Bitmap target)
        {
            Trace.Assert(origin.Width == target.Width && origin.Height == target.Height, "Images different sizes.");
            return Enumerable.Range(0, origin.Width * origin.Height)
                .Select(x =>
                {
                    var i = x % origin.Width;
                    var j = x / origin.Width;

                    var pixel_o = origin.GetPixel(i, j);
                    var pixel_t = origin.GetPixel(i, j);

                    return Math.Pow(pixel_o.R - pixel_t.R, 2) + Math.Pow(pixel_o.G - pixel_t.G, 2) + Math.Pow(pixel_o.B - pixel_t.B, 2);
                })
                .Sum();
        }

        public static bool Similar(this Bitmap origin, Bitmap target, double threshold)
        {
            return origin.Distance(target) < origin.Distance(origin.Negative()) * threshold;
        }
    }
}
