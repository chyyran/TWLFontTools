﻿using System;
using System.Collections.Generic;
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
            var charset = new List<char>();
            var x = File.ReadAllText("charset.csv").Split(",");

            foreach (string c in x)
            {
                try
                {
                    charset.Add((char)Int32.Parse(c));
                }
                catch
                {

                }
            }
            var coords = new UVCoordinateGenerator(charmap, charset, size, 1024, 256);
            File.WriteAllText($"uvcoord_{size.ToLowerInvariant()}_font.h", coords.ToString());
            //var grith = new GritHeaderGenerator(charmap, size);
            //File.WriteAllText($"{size.ToLowerInvariant()}_font.h", grith.ToString());
        }
    }
}
