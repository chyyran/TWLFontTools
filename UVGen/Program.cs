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

            string size = args[1].ToUpperInvariant();

            XDocument doc = XDocument.Load(File.OpenRead(args[0]));
            var charmap = new FontInfo(doc);

            var coords = new UVCoordinateGenerator(charmap, size, 512, 256);
            File.WriteAllText($"uvcoord_{size.ToLowerInvariant()}_font.h", coords.ToString());
            var grith = new GritHeaderGenerator(charmap, size);
            File.WriteAllText($"{size.ToLowerInvariant()}_font.h", grith.ToString());
        }
    }
}
