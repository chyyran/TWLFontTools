using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Unicode;

namespace UVGen
{
    public class UVCoordinateGenerator
    {

        public UVCoordinateGenerator(FontInfo info, string size, int imageheight, int imagewidth)
        {
            this.FontInfo = info;
            this.TextSizeName = size;
            this.File = new StringBuilder();
            this.WriteHeaderDefs(imageheight, imagewidth);
            for(int i = 0; i < info.UVInfo.AuxTextureCount + 1; i++)
            {
                this.WriteTextCoords(i);
            }
            this.WriteAuxPointers(info.UVInfo.AuxTextureCount + 1);
            this.WriteEof();
        }

        private void WriteAuxPointers(int count)
        {
            File.AppendLine($"static constexpr unsigned int *{this.TextSizeName.ToLowerInvariant()}_font_texcoords[] = {{");
            for (int i = 0; i < count; i++)
            {
                File.AppendLine($"    {this.TextSizeName.ToLowerInvariant()}_font_{i}_texcoords,");
            }
            File.AppendLine();
            File.AppendLine("};");
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
            File.AppendLine($"#define {this.TextSizeName}_FONT_PRM_NUM_IMAGES  0x{this.FontInfo.UVInfo.PrimarySpritesPerTexture.ToString("X2")}");
            File.AppendLine($"#define {this.TextSizeName}_FONT_AUX_NUM_IMAGES  0x{this.FontInfo.UVInfo.AuxSpritesPerTexture.ToString("X2")}");

            File.AppendLine();
            File.AppendLine("// U,V,Width,Height");
            File.AppendLine();
            File.AppendLine($"#define TEXT_{this.TextSizeName.Substring(0,1)}Y {this.FontInfo.UVInfo.TileHeight}");
            File.AppendLine();
        }

        /*
         * //	Format:
		//	U,V,Width,Height
        */
        private void WriteTextCoords(int textureIndex)
        {
            int rpt;
            int cpt;
            int spt;
            int offset;
            if (textureIndex == 0)
            {
                rpt = this.FontInfo.UVInfo.PrimaryRowsPerTexture;
                cpt = this.FontInfo.UVInfo.PrimaryColumnsPerTexture;
                spt = this.FontInfo.UVInfo.PrimarySpritesPerTexture;
                offset = 0;
            } else
            {
                rpt = this.FontInfo.UVInfo.AuxRowsPerTexture;
                cpt = this.FontInfo.UVInfo.AuxColumnsPerTexture;
                spt = this.FontInfo.UVInfo.AuxSpritesPerTexture;
                offset = this.FontInfo.UVInfo.PrimarySpritesPerTexture - this.FontInfo.UVInfo.AuxSpritesPerTexture; 
            }

            File.AppendLine($"static constexpr unsigned int {this.TextSizeName.ToLowerInvariant()}_font_{textureIndex}_texcoords[] = {{");
            for (int i = 0; i < rpt; i++)
            {
                for (int j = 0; j < cpt; j++)
                {
                    int characterIndex = (i * cpt) + j + (spt * textureIndex) + offset;
                    if (characterIndex >= this.FontInfo.CharMap.Count) break;
                    var charEntry = this.FontInfo.CharMap[characterIndex];
                    File.AppendLine($"    {j * this.FontInfo.UVInfo.TileWidth}, " +
                        $"{i * this.FontInfo.UVInfo.TileHeight}, {charEntry.Width}, " +
                        $"TEXT_{this.TextSizeName.Substring(0, 1)}Y, // {UnicodeInfo.GetName(charEntry.Character)}");
                }
            }
            File.AppendLine();
            File.AppendLine("};");
        }

        private void WriteEof()
        {
            File.AppendLine();
            File.AppendLine("#endif");
        }

        public override string ToString()
        {
            return this.File.ToString();
        }

        public FontInfo FontInfo { get; }
        private string TextSizeName { get; }
        private StringBuilder File { get; }
    }
}
