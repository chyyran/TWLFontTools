using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Unicode;

namespace LUTGen
{
    public class CharsetLutGenerator
    {
        public CharsetLutGenerator(IList<CharmapEntry> chars, IList<char> charset)
        {
            this.Chars = chars;
            this.Charset = charset;
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
            for (char i = (char)0; i <= latestChar; i++)
            {
                int index = this.Charset.IndexOf(i);
                index = index == -1 ? 0 : index; //map all unknowns to space.
                //int index = this.Chars.FirstOrDefault(c => c.Character == i)?.Index ?? 0;
                File.Append($"{index},");
                if (lineCounter == 0x20)
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
        public IList<char> Charset { get; }
        private StringBuilder File { get; }
    }
}