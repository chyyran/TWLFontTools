using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace UVGen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"{args[0]} not found.");
                Environment.Exit(1);
            }

            int imageHeight;
            int imageWidth;

            using (Image<Rgba32> i = Image.Load(args[1]))
            {
                imageHeight = i.Height;
                imageWidth = i.Width;
            }

            int width = Convert.ToInt32(args[2]);
            int height = Convert.ToInt32(args[3]);
            string size = args[4].ToUpperInvariant();

            XDocument doc = XDocument.Load(File.OpenRead(args[0]));
            var charmap = FontInfoReader.Read(doc).ToList();

            var coords = new UVCoordinateGenerator(charmap, height, width, size, imageHeight, imageWidth);
            File.WriteAllText($"uvcoords_{size.ToLowerInvariant()}_font.h", coords.ToString());



        }
    }
}
