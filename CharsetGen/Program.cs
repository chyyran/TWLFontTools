using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CharsetGen
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


            XDocument doc = XDocument.Load(File.OpenRead(args[0]));
            var charmap = new FontInfo(doc);

            StringBuilder sb = new StringBuilder();
            foreach(var f in charmap.CharMap)
            {
                sb.Append($"{(long)f.Character},");
            }
            File.WriteAllText("charset.csv",sb.ToString());
        }
    }
}
