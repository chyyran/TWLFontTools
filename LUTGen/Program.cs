using System;
using System.Collections.Generic;
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
            if (!File.Exists("charset.csv"))
            {
                var charmap = FontInfoReader.Read(doc).ToList();
                File.WriteAllText($"unicode_font_lut.h", new LUTGenerator(charmap).ToString());
            }
            else
            {
                var charset = new List<char>();
                var charmap = FontInfoReader.Read(doc).ToList();
                var x = File.ReadAllText("charset.csv").Split(",");
                   
                foreach (string c in x)
                {
                    try
                    {
                        charset.Add((char)Int32.Parse(c));
                    } catch
                    {

                    }
                }

                File.WriteAllText($"unicode_font_lut.h", new CharsetLutGenerator(charmap, charset).ToString());

            }
        }
    }
}
