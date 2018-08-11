/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace NFTRFontTexGen
{

    // Credits to CUE and Lyan53 in romxhack, thanks:
    // http://romxhack.esforos.com/fuentes-nftr-de-nds-t67
    public static class NFTR
    {
        public const int MAX_WIDTH = 512;
        public const int MAX_HEIGHT = 16;
        public static sNFTR Read(Stream file)
        {
            sNFTR font = new sNFTR();
            BinaryReader br = new BinaryReader(file);

            // Read the standard header
            font.header.type = br.ReadChars(4);
            font.header.endianess = br.ReadUInt16();
            font.header.unknown = br.ReadUInt16();
            font.header.file_size = br.ReadUInt32();
            font.header.block_size = br.ReadUInt16();
            font.header.num_blocks = br.ReadUInt16();

            // Font INFo section
            font.fnif.type = br.ReadChars(4);
            font.fnif.block_size = br.ReadUInt32();

            font.fnif.unknown1 = br.ReadByte();
            font.fnif.height = br.ReadByte();
            font.fnif.nullCharIndex = br.ReadUInt16();
            font.fnif.unknown4 = br.ReadByte();
            font.fnif.width = br.ReadByte();
            font.fnif.width_bis = br.ReadByte();
            font.fnif.encoding = br.ReadByte();

            font.fnif.offset_plgc = br.ReadUInt32();
            font.fnif.offset_hdwc = br.ReadUInt32();
            font.fnif.offset_pamc = br.ReadUInt32();

            if (font.fnif.block_size == 0x20)
            {
                font.fnif.height_font = br.ReadByte();
                font.fnif.widht_font = br.ReadByte();
                font.fnif.bearing_y = br.ReadByte();
                font.fnif.bearing_x = br.ReadByte();
            }

            // Character Graphics LP
            br.BaseStream.Position = font.fnif.offset_plgc - 0x08;
            font.plgc.type = br.ReadChars(4);
            font.plgc.block_size = br.ReadUInt32();
            font.plgc.tile_width = br.ReadByte();
            font.plgc.tile_height = br.ReadByte();
            font.plgc.tile_length = br.ReadUInt16();
            font.plgc.unknown = br.ReadUInt16();
            font.plgc.depth = br.ReadByte();
            font.plgc.rotateMode = br.ReadByte();

            font.plgc.tiles = new Byte[(font.plgc.block_size - 0x10) / font.plgc.tile_length][];
            for (int i = 0; i < font.plgc.tiles.Length; i++)
            {
                font.plgc.tiles[i] = BytesToBits(br.ReadBytes(font.plgc.tile_length));
                if (font.plgc.rotateMode == 2)
                    font.plgc.tiles[i] = Rotate270(font.plgc.tiles[i], font.plgc.tile_width, font.plgc.tile_height, font.plgc.depth);
                else if (font.plgc.rotateMode == 1)
                    font.plgc.tiles[i] = Rotate90(font.plgc.tiles[i], font.plgc.tile_width, font.plgc.tile_height, font.plgc.depth);
                else if (font.plgc.rotateMode == 3)
                    font.plgc.tiles[i] = Rotate180(font.plgc.tiles[i], font.plgc.tile_width, font.plgc.tile_height, font.plgc.depth);

            }


            // Character Width DH
            br.BaseStream.Position = font.fnif.offset_hdwc - 0x08;
            font.hdwc.type = br.ReadChars(4);
            font.hdwc.block_size = br.ReadUInt32();
            font.hdwc.fist_code = br.ReadUInt16();
            font.hdwc.last_code = br.ReadUInt16();
            font.hdwc.unknown1 = br.ReadUInt32();

            font.hdwc.info = new List<sNFTR.HDWC.Info>();
            for (int i = 0; i < font.plgc.tiles.Length; i++)
            {
                sNFTR.HDWC.Info info = new sNFTR.HDWC.Info();
                info.pixel_start = br.ReadSByte();
                info.pixel_width = br.ReadByte();
                info.pixel_length = br.ReadByte();
                font.hdwc.info.Add(info);
            }

            // Character MAP
            br.BaseStream.Position = font.fnif.offset_pamc - 0x08;
            font.pamc = new List<sNFTR.PAMC>();
            uint nextOffset = 0x00;
            do
            {
                sNFTR.PAMC pamc = new sNFTR.PAMC();
                pamc.type = br.ReadChars(4);
                pamc.block_size = br.ReadUInt32();
                pamc.first_char = br.ReadUInt16();
                pamc.last_char = br.ReadUInt16();
                pamc.type_section = br.ReadUInt32();
                nextOffset = br.ReadUInt32();
                pamc.next_section = nextOffset;

                switch (pamc.type_section)
                {
                    case 0:
                        sNFTR.PAMC.Type0 type0 = new sNFTR.PAMC.Type0();
                        type0.fist_char_code = br.ReadUInt16();
                        pamc.info = type0;
                        break;
                    case 1:
                        sNFTR.PAMC.Type1 type1 = new sNFTR.PAMC.Type1();
                        type1.char_code = new ushort[pamc.last_char - pamc.first_char + 1];
                        for (int i = 0; i < type1.char_code.Length; i++)
                            type1.char_code[i] = br.ReadUInt16();

                        pamc.info = type1;
                        break;
                    case 2:
                        sNFTR.PAMC.Type2 type2 = new sNFTR.PAMC.Type2();
                        type2.num_chars = br.ReadUInt16();
                        type2.charInfo = new sNFTR.PAMC.Type2.CharInfo[type2.num_chars];

                        for (int i = 0; i < type2.num_chars; i++)
                        {
                            type2.charInfo[i].chars_code = br.ReadUInt16();
                            type2.charInfo[i].chars = br.ReadUInt16();
                        }
                        pamc.info = type2;
                        break;
                }

                font.pamc.Add(pamc);
                br.BaseStream.Position = nextOffset - 0x08;
            } while (nextOffset != 0x00 && (nextOffset - 0x08) < br.BaseStream.Length);

            font.plgc.rotateMode = 0;

            br.Close();
            return font;
        }

 
        public static Rgba32[] CalculatePalette(int depth, bool inverse, Rgba32 background)
        {
            Rgba32[] palette = new Rgba32[(1 << depth) + 1];
            palette[0] = background;
            for (int i = 1; i < palette.Length - 1; i++)
            {
                int colorIndexRed = i * (0xFF / (palette.Length - 1));
                int colorIndexGreen = i * (0xFF / (palette.Length - 1));
                int colorIndexBlue = i * (0xFF / (palette.Length - 1));


                if (!inverse)
                {
                    colorIndexRed = 0xFF - colorIndexRed;
                    colorIndexGreen = 0xFF - colorIndexGreen;
                    colorIndexBlue = 0xFF - colorIndexBlue;
                }

                var color = new Rgba32((byte)colorIndexRed, (byte)colorIndexGreen, (byte)colorIndexBlue, 0xFF);
                palette[i] = color;
            }

            palette[palette.Length - 1] = background;
            return palette;
        }

        public static Image<Rgba32> Get_Char(sNFTR font, int id, Rgba32[] palette, int zoom = 1)
        {
            return Get_Char(font.plgc.tiles[id], font.plgc.depth, font.plgc.tile_width, font.plgc.tile_height, font.plgc.rotateMode, palette, zoom);
        }

        public static Image<Rgba32> Get_Char(byte[] tiles, int depth, int width, int height, int rotateMode,
            Rgba32[] palette, int zoom = 1)
        {
            var image = new Image<Rgba32>(width * zoom + 1, height * zoom + 1);
            List<Byte> tileData = new List<byte>();

            for (int i = 0; i <= tiles.Length - depth; i += depth)
            {
                Byte byteFromBits = 0;
                for (int b = depth - 1, j = 0; b >= 0; b--, j++)
                {
                    byteFromBits += (byte)(tiles[i + j] << b);
                }
                tileData.Add(byteFromBits);
            }


            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    for (int hzoom = 0; hzoom < zoom; hzoom++)
                        for (int wzoom = 0; wzoom < zoom; wzoom++)
                            image[
                                w * zoom + wzoom,
                                h * zoom + hzoom] = palette[tileData[w + h * width]];
                }
            }

            return image;
        }
 
        public static Byte[] Rotate270(Byte[] bytes, int width, int height, int depth)
        {
            Byte[] rotated = new Byte[bytes.Length];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    byte[] original = new byte[depth];
                    Array.Copy(bytes, (w + h * width) * depth, original, 0, depth);

                    for (int i = 0; i < depth; i++)
                        rotated[(h + width * (height - 1) - w * width) * depth + i] = original[i];
                }
            }

            return rotated;
        }
        public static Byte[] Rotate90(Byte[] bytes, int width, int height, int depth)
        {
            Byte[] rotated = new Byte[bytes.Length];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    byte[] original = new byte[depth];
                    Array.Copy(bytes, (w + h * width) * depth, original, 0, depth);

                    for (int i = 0; i < depth; i++)
                        rotated[((width - 1 - h) + w * width) * depth + i] = original[i];
                }
            }

            return rotated;
        }
        public static Byte[] Rotate180(Byte[] bytes, int width, int height, int depth)
        {
            Byte[] rotated = new Byte[bytes.Length];

            for (int h = 0; h < height; h++)
            {
                for (int w = 0; w < width; w++)
                {
                    byte[] original = new byte[depth];
                    Array.Copy(bytes, (w + h * width) * depth, original, 0, depth);

                    for (int i = 0; i < depth; i++)
                        rotated[((width - 1) + (width - 1) * height - w - h * width) * depth + i] = original[i];
                }
            }

            return rotated;
        }


        public static Byte[] BytesToBits(Byte[] bytes)
        {
            List<Byte> bits = new List<byte>();

            for (int i = 0; i < bytes.Length; i++)
                for (int j = 7; j >= 0; j--)
                    bits.Add((byte)((bytes[i] >> j) & 1));

            return bits.ToArray();
        }
        public static Byte[] BitsToBytes(Byte[] bits)
        {
            List<Byte> bytes = new List<byte>();

            for (int i = 0; i < bits.Length; i += 8)
            {
                Byte newByte = 0;
                int b = 0;
                for (int j = 7; j >= 0; j--, b++)
                {
                    newByte += (byte)(bits[i + b] << j);
                }
                bytes.Add(newByte);
            }

            return bytes.ToArray();
        }

        public static XDocument ExportInfo(Dictionary<int, int> charTable, sNFTR font, int numChars)
        {
            var encoding = Encoding.UTF8;
            switch (font.fnif.encoding)
            {
                case 2:
                    encoding = Encoding.GetEncoding(932); //Shift-JIS
                    break;
                case 0:
                    encoding = Encoding.UTF8;
                    break;
                case 3:
                    encoding = Encoding.GetEncoding(1252);
                    break;
                case 1:
                    encoding = Encoding.Unicode;
                    break;
            }

            XDocument doc = new XDocument();
            doc.Declaration = new XDeclaration("1.0", encoding.BodyName, null);

            

            XElement charMap = new XElement("CharMap");
            XElement uvGen = new XElement("UVGen");
            int numColumns = MAX_WIDTH / font.plgc.tile_width;
            int numRows = (int)Math.Floor((double)MAX_HEIGHT / font.plgc.tile_height);
            int charsPerTexture = (int)Math.Floor((double)numChars / (numColumns * numRows));
            uvGen.Add(new XElement("ColumnsPerTexture", numColumns));
            uvGen.Add(new XElement("RowsPerTexture", numRows));
            uvGen.Add(new XElement("SpritesPerTexture", numColumns * numRows));
            uvGen.Add(new XElement("TileHeight", font.fnif.height));
            uvGen.Add(new XElement("TileWidth", font.fnif.width));
            charMap.Add(uvGen);

            var charCodeTuples = charTable.OrderBy(kvp => kvp.Value).ToList();
            foreach (var kvp in charCodeTuples)
            {
                var c = kvp.Key;

                string ch = "";

                byte[] codes = BitConverter.GetBytes(c).ToArray();
                ch = new String(Encoding.Unicode.GetChars(codes)).Replace("\0", "");

                int tileCode = kvp.Value;
                if (tileCode >= font.hdwc.info.Count)
                    continue;
                sNFTR.HDWC.Info info = font.hdwc.info[tileCode];

                XElement chx = new XElement("CharInfo");
                chx.SetAttributeValue("Char", ch);
                chx.SetAttributeValue("Code", c);
                chx.SetAttributeValue("Hex", c.ToString("x2"));
                chx.SetAttributeValue("Index", tileCode.ToString());
                chx.SetAttributeValue("Width", info.pixel_length.ToString());

                charMap.Add(chx);
            }
            // Export general info
            // FNIF data
            XElement xmlFnif = new XElement("FNIF");
            xmlFnif.Add(new XElement("Unknown1", font.fnif.unknown1));
            xmlFnif.Add(new XElement("Height", font.fnif.height));
            xmlFnif.Add(new XElement("NullCharIndex", font.fnif.nullCharIndex));
            xmlFnif.Add(new XElement("Unknown2", font.fnif.unknown4));
            xmlFnif.Add(new XElement("Width", font.fnif.width));
            xmlFnif.Add(new XElement("WidthBis", font.fnif.width_bis));
            xmlFnif.Add(new XElement("Encoding", encoding.BodyName));
            if (font.fnif.block_size == 0x20)
            {
                xmlFnif.Add(new XElement("GlyphHeight", font.fnif.height_font));
                xmlFnif.Add(new XElement("GlyphWidth", font.fnif.widht_font));
                xmlFnif.Add(new XElement("BearingY", font.fnif.bearing_y));
                xmlFnif.Add(new XElement("BearingX", font.fnif.bearing_x));
            }
            charMap.Add(xmlFnif);
            doc.Add(charMap);
            return doc;
        }

        private static int MakeEven(int n)
        {
            return n + (n % 2);
        }


        public static IEnumerable<Image<Rgba32>> ToTileset(sNFTR font, Rgba32[] palette, IList<CharmapEntry> charmap, IList<char> charset)
        {
            int numChars = charset.Count;

            // Get the image size
            int charWidth = font.plgc.tile_width;
            int charHeight = font.plgc.tile_height;

            int numColumns = (int)Math.Floor((double)MAX_WIDTH / charWidth);
            int numRows = (int)Math.Floor((double)MAX_HEIGHT / charHeight);

            int charsPerTexture = (int)Math.Floor((double)numChars / (numColumns * numRows));

            for (int processedChars = 0; processedChars < numChars;)
            {
                var image = new Image<Rgba32>(MAX_WIDTH, MAX_HEIGHT);
                image.Mutate(im => im.Fill(palette[palette.Length - 1]));

                // add charsPerTexture images here and return the image.
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numColumns; j++)
                    {
                        int x = j * charWidth;
                        // First row is empty.
                        int y = i * charHeight;
                        if (processedChars >= numChars)
                        {
                            break;
                        }
                        var charToDraw = charset[processedChars];
                        processedChars++;
                        int index = charmap.FirstOrDefault(c => c.Character == charToDraw)?.Index ?? 0;
                        image.Mutate(im => im.DrawImage(GraphicsOptions.Default,
                            Get_Char(font, index, palette, 1), new Point(x, y)));
                    }
                }
                yield return image;
            }
        }


        private static IEnumerable<Image<Rgba32>> ToTileset(sNFTR font, Rgba32[] palette, int charWidth, int charHeight,
                                      int rowsPerTexture, int colsPerTexture, int zoom)
        {
            int numChars = font.plgc.tiles.Length;

            for (int processedChars = 0; processedChars < numChars;)
            {
                var image = new Image<Rgba32>(MAX_WIDTH, MAX_HEIGHT);
                image.Mutate(im => im.Fill(palette[palette.Length - 1]));

                // add charsPerTexture images here and return the image.
                for (int i = 0; i < rowsPerTexture; i++)
                {
                    for (int j = 0; j < colsPerTexture; j++)
                    {
                        int index = processedChars++;
                        if (index >= numChars)
                            break;

                        int x = j * charWidth;
                        // First row is empty.
                        int y = i * charHeight;
                        image.Mutate(im => im.DrawImage(GraphicsOptions.Default,
                            Get_Char(font, index, palette, zoom), new Point(x, y)));
                    }
                }
                yield return image;
            }
        }

        public static IEnumerable<Image<Rgba32>> ToTileset(sNFTR font, Rgba32[] palette)
        {
            int numChars = font.plgc.tiles.Length;

            // Get the image size
            int charWidth = font.plgc.tile_width;
            int charHeight = font.plgc.tile_height;

            int numColumns = (int)Math.Floor((double)MAX_WIDTH / charWidth);
            int numRows = (int)Math.Floor((double)MAX_HEIGHT / charHeight);

            int charsPerTexture = (int)Math.Floor((double)numChars / (numColumns * numRows));
            return ToTileset(font, palette, charWidth, charHeight, numRows, numColumns, 1);
        }
    }

    public struct sNFTR // Nitro FonT Resource
    {
        public StandardHeader header;
        public FNIF fnif;
        public PLGC plgc;
        public HDWC hdwc;
        public List<PAMC> pamc;

        public struct StandardHeader
        {
            public char[] type;
            public ushort unknown;
            public ushort endianess;
            public uint file_size;
            public ushort block_size;
            public ushort num_blocks;
        }
        public struct FNIF // Font INFo
        {
            public char[] type;
            public uint block_size;

            public byte unknown1;       // Usually 0x00
            public byte height;
            public ushort nullCharIndex;
            public byte unknown4;       // Usually 0x00
            public byte width;
            public byte width_bis;
            public byte encoding;       // Could be 0(utf-8), 1(utf-16), 2(s-jis) or 3(cp1252)

            public uint offset_plgc;
            public uint offset_hdwc;
            public uint offset_pamc;

            public byte height_font;
            public byte widht_font;
            public byte bearing_y;
            public byte bearing_x;       // Usually 0x00
        }
        public struct PLGC // Character Graphics LP
        {
            public char[] type;
            public uint block_size;
            public byte tile_width;
            public byte tile_height;
            public ushort tile_length;
            public ushort unknown;
            public byte depth;
            public byte rotateMode;
            public byte[][] tiles; // tiles of each char
        }
        public struct HDWC // Character Width DH
        {
            public char[] type;
            public uint block_size;
            public ushort fist_code;
            public ushort last_code;
            public uint unknown1; // always 0x00
            public List<Info> info;

            public struct Info
            {
                public sbyte pixel_start;
                public byte pixel_width;
                public byte pixel_length;
            }
        }
        public struct PAMC // Character MAP
        {
            public char[] type;
            public uint block_size;
            public ushort first_char;
            public ushort last_char;
            public uint type_section;
            public uint next_section;
            public Object info;

            public struct Type0
            {
                public ushort fist_char_code;
            }
            public struct Type1
            {
                public ushort[] char_code;
            }
            public struct Type2
            {
                public ushort num_chars;
                public CharInfo[] charInfo;

                public struct CharInfo
                {
                    public ushort chars_code;
                    public ushort chars;
                }
            }
        }
    }
}
