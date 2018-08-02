using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Unicode;

namespace LUTGen
{
    public class LUTGenerator
    {
        public LUTGenerator(IList<CharmapEntry> chars)
        {
            this.Chars = chars;
            this.File = new StringBuilder();
            this.WriteHeaderDefs();
            this.WriteLUTEntries();
            this.WriteEof();
            
        }

        private void WriteHeaderDefs()
        {
            File.AppendLine("/*======================================================================");
            File.AppendLine();
            File.AppendLine($"UTF16LE to UVCoord Mapping");
            File.AppendLine();
            File.AppendLine("======================================================================*/");
            File.AppendLine();
            File.AppendLine();
            File.AppendLine($"extern const unsigned int utf16_lookup_table[] = {{");
        }


        private void WriteLUTEntries() {
            char latestChar = this.Chars.OrderByDescending(c => c.Character).First().Character;
            int lineCounter = 0;
            for (int i = 0; i <= latestChar; i++)
            {
                int index = this.Chars.FirstOrDefault(c => c.Character == i)?.Index ?? 0;
                File.Append($"0x{index.ToString("X2")},");
                if(lineCounter == 0x20)
                {
                    lineCounter = 0;
                    File.AppendLine();
                }
                else
                {
                   lineCounter++;
                }
            }
        }

        private void WriteEof()
        {
            File.AppendLine();
            File.AppendLine("};");
            File.AppendLine();
        }

        public override string ToString()
        {
            return this.File.ToString();
        }

        private IList<CharmapEntry> Chars { get; }

        private StringBuilder File { get; }
    }
}