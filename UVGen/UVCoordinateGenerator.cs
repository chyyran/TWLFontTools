using System;
using System.Collections.Generic;
using System.Text;
using System.Unicode;

namespace UVGen
{
    public class UVCoordinateGenerator
    {

        public static readonly int CharactersPerLine = 0x20;

        public UVCoordinateGenerator(IList<CharmapEntry> chars, int uvheight, int uvwidth, string size, int imageheight, int imagewidth)
        {
            this.Chars = chars;
            this.TextHeight = uvheight;
            this.TextWidth = uvwidth;
            this.TextSizeName = size;
            this.File = new StringBuilder();
            this.WriteHeaderDefs(imageheight, imagewidth);
            this.WriteTextCoords();
            this.WriteEof();
            
        }

        private void WriteHeaderDefs(int texHeight, int texWidth)
        {
            File.AppendLine("/*======================================================================");
            File.AppendLine();
            File.AppendLine($"{this.TextSizeName}_FONT texture coordinates");
            File.AppendLine();
            File.AppendLine("======================================================================*/");
            File.AppendLine();
            File.AppendLine($"#ifndef {this.TextSizeName}_FONT__H");
            File.AppendLine($"#define {this.TextSizeName}_FONT__H");
            File.AppendLine($"#define {this.TextSizeName}_FONT_BITMAP_WIDTH  {texWidth}");
            File.AppendLine($"#define {this.TextSizeName}_FONT_BITMAP_HEIGHT  {texHeight}");
            File.AppendLine($"#define {this.TextSizeName}_FONT_NUM_IMAGES  0x{this.Chars.Count.ToString("X2")}");
            File.AppendLine();
            File.AppendLine($"#define TEXT_LY {this.TextHeight}");
            File.AppendLine();
            File.AppendLine($"extern const unsigned int {this.TextSizeName.ToLowerInvariant()}_font_texcoords[] = {{");
        }

        /*
         * //	Format:
		//	U,V,Width,Height
        */
        private void WriteTextCoords()
        {
            int rows = Convert.ToInt32(Math.Ceiling(this.Chars.Count / 32M)); //0x20 = 32
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < CharactersPerLine; j++)
                {
                    int characterIndex = (i * CharactersPerLine) + j;
                    if (characterIndex >= this.Chars.Count) break;
                    var charEntry = this.Chars[characterIndex];
                    File.AppendLine($"    {j * TextWidth}, {(i + 1) * TextHeight}, {charEntry.Width}, TEXT_LY, // {UnicodeInfo.GetName(charEntry.Character)}");
                }
            } 
        }

        private void WriteEof()
        {
            File.AppendLine();
            File.AppendLine("};");
            File.AppendLine();
            File.AppendLine("#endif");
        }

        public override string ToString()
        {
            return this.File.ToString();
        }

        private IList<CharmapEntry> Chars { get; }
        private int TextHeight { get; }
        private int TextWidth { get; }
        private string TextSizeName { get; }
        private StringBuilder File { get; }
    }
}
