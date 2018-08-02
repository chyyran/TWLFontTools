using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LUTGen
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists("fontinfo.xml"))
            {
                Console.WriteLine($"fontinfo.xml not found.");
                Environment.Exit(1);
            }

            XDocument doc = XDocument.Load(File.OpenRead("fontinfo.xml"));
            var charmap = FontInfoReader.Read(doc).ToList();
            File.WriteAllText($"unicode_font_lut.h", new LUTGenerator(charmap).ToString());
        }
    }
}
