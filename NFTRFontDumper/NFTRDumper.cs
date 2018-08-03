using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace NFTRFontDumper
{
    class NFTRDumper
    {
        sNFTR font;
        Dictionary<int, int> charTile;
        Rgba32[] palette;


        public NFTRDumper(sNFTR font, Rgba32 background)
        {
            this.font = font;
            this.palette = NFTR.CalculatePalette(font.plgc.depth, false, background);
            Fill_CharTile();
        }

        public IEnumerable<Image<Rgba32>> GetTextureMapping()
        {
            return NFTR.ToTileset(font, palette);
        }

        public XDocument GetXmlInfo()
        {
            return NFTR.ExportInfo(charTile, font);
        }

        private void Fill_CharTile()
        {
            charTile = new Dictionary<int, int>();
            for (int p = 0; p < font.pamc.Count; p++)
            {
                if (font.pamc[p].info is sNFTR.PAMC.Type0)
                {
                    sNFTR.PAMC.Type0 type0 = (sNFTR.PAMC.Type0)font.pamc[p].info;
                    int interval = font.pamc[p].last_char - font.pamc[p].first_char;

                    for (int j = 0; j <= interval; j++)
                        try { charTile.Add(font.pamc[p].first_char + j, type0.fist_char_code + j); }
                        catch { }
                }
                else if (font.pamc[p].info is sNFTR.PAMC.Type1)
                {
                    sNFTR.PAMC.Type1 type1 = (sNFTR.PAMC.Type1)font.pamc[p].info;

                    for (int j = 0; j < type1.char_code.Length; j++)
                    {
                        if (type1.char_code[j] == 0xFFFF)
                            continue;

                        try { charTile.Add(font.pamc[p].first_char + j, type1.char_code[j]); }
                        catch { }
                    }
                }
                else if (font.pamc[p].info is sNFTR.PAMC.Type2)
                {
                    sNFTR.PAMC.Type2 type2 = (sNFTR.PAMC.Type2)font.pamc[p].info;

                    for (int j = 0; j < type2.num_chars; j++)
                    {
                        if (type2.charInfo[j].chars == 0xFFFF)
                            continue;
                        try { charTile.Add(type2.charInfo[j].chars_code, type2.charInfo[j].chars); }
                        catch { }
                    }
                }
            }
        }
    }
}
