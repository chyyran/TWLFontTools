using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml.Linq;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Quantization;

namespace NFTRFontDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: NFTRFontDump [nftr file] [size]");
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine($"{args[0]} not found.");
                Environment.Exit(1);
            }
            Rgba32 color = Rgba32.FromHex("ff00ff");
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
            
            int counter = 0;
            var encoder = new PngEncoder();
            using (var file = File.OpenWrite($"{args[1]}_font_{counter}.png"))
            {
                dumper.GetTextureMapping().Save(file, encoder);
            }
        }

    }
}
