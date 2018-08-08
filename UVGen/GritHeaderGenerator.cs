using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Unicode;

namespace UVGen
{
    public class GritHeaderGenerator
    {

        public GritHeaderGenerator(FontInfo info, string size)
        {
            this.FontInfo = info;
            this.TextSizeName = size;
            this.File = new StringBuilder();
            this.WriteHeaderDefs();
            for (int i = 0; i < info.UVInfo.AuxTextureCount + 1; i++)
            {
                File.AppendLine($"#include \"{this.TextSizeName.ToLowerInvariant()}_font_{i}.h\"");
            }
            this.WriteBitmapPointers(info.UVInfo.AuxTextureCount + 1);
            this.WritePalettePointers(info.UVInfo.AuxTextureCount + 1);
            this.WriteEof();
        }
        private void WriteHeaderDefs()
        {
            File.AppendLine("/*======================================================================");
            File.AppendLine();
            File.AppendLine($"{this.TextSizeName}_FONT");
            File.AppendLine();
            File.AppendLine("======================================================================*/");
            File.AppendLine();
            File.AppendLine($"#ifndef {this.TextSizeName}_FONT_GFX_H");
            File.AppendLine($"#define {this.TextSizeName}_FONT_GFX_H");
            File.AppendLine($"#pragma once");
          
            File.AppendLine();
        
            File.AppendLine();
        }

        private void WriteBitmapPointers(int count)
        {
            File.AppendLine($"static constexpr unsigned int *{this.TextSizeName.ToLowerInvariant()}_fontBitmaps[] = {{");
            for (int i = 0; i < count; i++)
            {
                File.AppendLine($"    {this.TextSizeName.ToLowerInvariant()}_font_{i}Bitmap,");
            }
            File.AppendLine();
            File.AppendLine("};");
        }

        private void WritePalettePointers(int count)
        {
            File.AppendLine($"static constexpr unsigned int *{this.TextSizeName.ToLowerInvariant()}_fontPal[] = {{");
            for (int i = 0; i < count; i++)
            {
                File.AppendLine($"    {this.TextSizeName.ToLowerInvariant()}_font_{i}Pal,");
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
