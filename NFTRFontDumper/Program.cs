using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace NFTRFontDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: NFTRFontDump [nftr file]");
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"{args[0]} not found.");
                Environment.Exit(1);
            }
            Rgba32 color = Rgba32.FromHex("f5f8f4");
            try
            {
                color = Rgba32.FromHex(args[1]);
            }
            catch
            {

            }

            Stream nftrData = File.OpenRead(args[0]);
            var font = NFTR.Read(nftrData);
            var dumper = new NFTRDumper(font, color);
            using (var file = File.OpenWrite(Path.GetFileNameWithoutExtension(args[0]) + ".xml"))
            {
                dumper.GetXmlInfo().Save(file);
            }
            using (var file = File.OpenWrite(Path.GetFileNameWithoutExtension(args[0]) + ".png"))
            {
                dumper.GetTextureMapping().Save(file, new PngEncoder());
            }
        }

    }
}
