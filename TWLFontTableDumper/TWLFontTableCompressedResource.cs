/*
 * Copyright (C) 2018 RonnChyran
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
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TWLFontTableDumper
{
    public class TWLFontTableCompressedResource
    {
        private Stream FontTableStream { get; }
        public string ResourceName { get; }
        public int CompressedResourceSize { get; }
        public int DecompressedResourceSize { get; }
        public byte[] CompressedResourceSHA1 { get; }
        public int CompressedResourceStart { get; }
        private int ResourceOffsetSize { get; }


        private readonly int ResourceLength = 0x40;
        private readonly int ResourceNameOffset = 0x00A0;
        private readonly int CompressedResourceSizeOffset = 0x00C0;
        private readonly int CompressedResourceStartOffset = 0x00C4;
        private readonly int DecompressedResourceSizeOffset = 0x00C8;
        private readonly int CompressedResourceSHA1Offset = 0x00CC;


        public TWLFontTableCompressedResource(Stream twlFontTableStream, int resourceNumber)
        {
            this.FontTableStream = twlFontTableStream;
            this.ResourceOffsetSize = resourceNumber * ResourceLength;
            this.ResourceName = this.GetResourceName();
            this.CompressedResourceSize = this.GetCompressedResourceSize();
            this.CompressedResourceStart = this.GetCompressedResourceStart();
            this.DecompressedResourceSize = this.GetDecompressedResourceSize();
            this.CompressedResourceSHA1 = this.ReadBytesFromOffset(CompressedResourceSHA1Offset, 0x14);
        }

        public TWLFontTableCompressedResourceWriter GetWriter()
        {
            return new TWLFontTableCompressedResourceWriter(this.FontTableStream, this);
        }

        private byte[] ReadBytesFromOffset(int offset, int count)
        {
            this.FontTableStream.Seek(offset + ResourceOffsetSize, SeekOrigin.Begin);
            byte[] buffer = new byte[count];
            this.FontTableStream.Read(buffer, 0, count);
            return buffer;
        }

        private string GetResourceName()
        {
            byte[] resourceNameBytes = this.ReadBytesFromOffset(ResourceNameOffset, 0x20);
            return Encoding.ASCII.GetString(resourceNameBytes).TrimEnd('\0');
        }

        private int GetCompressedResourceSize()
        {
            byte[] resourceSizeBytes = this.ReadBytesFromOffset(CompressedResourceSizeOffset, 4);
            return BitConverter.ToInt32(resourceSizeBytes, 0);
        }

        private int GetCompressedResourceStart()
        {
            byte[] resourceSizeBytes = this.ReadBytesFromOffset(CompressedResourceStartOffset, 4);
            return BitConverter.ToInt32(resourceSizeBytes, 0);
        }

        private int GetDecompressedResourceSize()
        {
            byte[] resourceSizeBytes = this.ReadBytesFromOffset(DecompressedResourceSizeOffset, 4);
            return BitConverter.ToInt32(resourceSizeBytes, 0);
        }

    }
}
