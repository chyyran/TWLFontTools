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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TWLFontTableDumper
{
    public class TWLFontTableCompressedResourceWriter
    {
        public TWLFontTableCompressedResourceWriter(Stream twlDatStream, TWLFontTableCompressedResource resource)
        {
            this.Resource = resource;
            this.FontTableStream = twlDatStream;
        }

        private TWLFontTableCompressedResource Resource { get; }
        private Stream FontTableStream { get; }

        private byte[] GetCompressedResourceBytes()
        {
            this.FontTableStream.Seek(this.Resource.CompressedResourceStart, SeekOrigin.Begin);
            byte[] buffer = new byte[this.Resource.CompressedResourceSize];
            this.FontTableStream.Read(buffer, 0, this.Resource.CompressedResourceSize);
            return buffer;
        }

        /// <summary>
        /// Implementation of LZrev decompression algorithm
        /// http://problemkaputt.de/gbatek.htm#lzdecompressionfunctions
        /// </summary>
        /// <returns></returns>
        private byte[] GetDecompressedResourceBytes()
        {
            byte[] compressed = this.GetCompressedResourceBytes();
            this.VerifyCompressedResource(compressed);

            byte[] buffer = new byte[this.Resource.DecompressedResourceSize]; //dst
            compressed.CopyTo(buffer, 0); // copy the compressed contents to the buffer
            int sourcePointer = compressed.Length; // Begin decompression from the end of the buffer.

            // Destination is the source + extra length specified in the footer.
            int destPointer = sourcePointer + BitConverter.ToInt32(buffer, sourcePointer - 4) - 1;
            int finishedPointer = sourcePointer - (BitConverter.ToInt32(buffer, sourcePointer - 8) & 0x00FFFFFF); // Should always work out to 0x15
            sourcePointer = sourcePointer - (buffer[sourcePointer - 5]) - 1;

            // thanks to 3DSExplorer for the base implementation (LZSS.cs)
            while (sourcePointer > finishedPointer)
            {
                var flag = buffer[sourcePointer]; // Compression flag
                sourcePointer--;
                for (var i = 7; i >= 0; i--)
                {
                    if ((flag & (1 << i)) == 0) // Data is not compressed
                    {
                        buffer[destPointer] = buffer[sourcePointer];
                        sourcePointer--;
                        destPointer--;
                    }
                    else // Data is compressed
                    {
                        var length = (buffer[sourcePointer] >> 4) + 3;
                        var distance = (((buffer[sourcePointer] & 0xF) << 8) | buffer[sourcePointer - 1]) + 3;
                        sourcePointer -= 2;

                        // Copy the data
                        for (var j = 1; j <= length; j++)
                        {
                            buffer[destPointer] = buffer[destPointer + distance];
                            destPointer -= 1;
                        }
                       
                    }
                    // Check for out of range
                    if (sourcePointer <= finishedPointer)
                    {
                        break;
                    }
                }
            }
            return buffer;
        }


        public void WriteCompressedResource()
        {
            var resourceWrite = File.Create(this.Resource.ResourceName + ".lzrev");
            byte[] buffer = this.GetCompressedResourceBytes();
            this.VerifyCompressedResource(buffer);
            resourceWrite.Write(buffer, 0, this.Resource.CompressedResourceSize);
            resourceWrite.Close();
        }

        public void WriteResource()
        {
            var resourceWrite = File.Create(this.Resource.ResourceName);
            resourceWrite.Write(this.GetDecompressedResourceBytes(), 0, this.Resource.DecompressedResourceSize);
            resourceWrite.Close();
        }

        private void VerifyCompressedResource(byte[] buffer)
        {
            using (var sha1 = SHA1.Create())
            {
                byte[] bufferSha1 = sha1.ComputeHash(buffer);
                bool sha1Matches = this.Resource.CompressedResourceSHA1.SequenceEqual(bufferSha1);
                if (!sha1Matches)
                {
                    throw new IOException($"SHA1 Mismatch for resource {this.Resource.ResourceName}! " +
                      $"Should have SHA1 {BitConverter.ToString(this.Resource.CompressedResourceSHA1).Replace("-", "")}, but has " +
                      $"actual SHA1 {BitConverter.ToString(bufferSha1).Replace("-", "")}");
                }
            }
        }
    }
}
