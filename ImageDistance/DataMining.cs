using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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

        public IEnumerable<Bitmap> NextImage()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://www.bing.com/images/search?q=" + query);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            Stream resStream = res.GetResponseStream();
            
            yield return null;
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

        public static double PixelDistance(this Bitmap origin, Bitmap target)
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

        public static double ColorDistance(this Bitmap origin, Bitmap target)
        {
            Trace.Assert(origin.Width == target.Width && origin.Height == target.Height, "Images different sizes.");
            return Enumerable.Range(0, origin.Width * origin.Height)
                .Select(x =>
                {
                    return 0;
                })
                .Sum();
        }

        public static bool Similar(this Bitmap origin, Bitmap target, double threshold, bool color)
        {
            return color ? origin.ColorDistance(target) < origin.ColorDistance(origin.Negative()) * threshold : origin.PixelDistance(target) < origin.PixelDistance(origin.Negative()) * threshold;
        }
    }
}
