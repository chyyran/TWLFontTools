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
    public class TWLFontTable
    {
        public TWLFontTableRegions Region { get; }
        public int NFTRResourceCount { get; }
        private Stream TWLFontTableDat { get; }

        private static readonly int NFTRResourceCountOffset = 0x0084;
        
        // We should check this, but since there is not much information available on
        // CHN and KOR regions, can not be sure that the RSA Signature matches up with
        // font files on such systems.
        private static readonly uint RSASignature = 0x238BF908;

        public static readonly String TWLFontTableDatFileName = "TWLFontTable.dat";
        public TWLFontTable(Stream twlFontTableDat)
        {
            this.TWLFontTableDat = twlFontTableDat;
            this.NFTRResourceCount = this.GetNFTRResourceCount();
            this.Region = this.GetTWLFontTableRegions();
        }

        /// <summary>
        /// Enumerates over all contained compressed resources.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TWLFontTableCompressedResource> GetCompressedResources()
        {
            for(int i = 0; i < this.NFTRResourceCount; i++)
            {
                yield return new TWLFontTableCompressedResource(this.TWLFontTableDat, i);
            }
        } 

        /// <summary>
        /// Reads the resource count from 0x0084
        /// </summary>
        /// <returns></returns>
        private int GetNFTRResourceCount()
        {
            this.TWLFontTableDat.Seek(NFTRResourceCountOffset, SeekOrigin.Begin);
            return this.TWLFontTableDat.ReadByte();
        }

        /// <summary>
        /// Determines the region of the TWLFontTableRegion
        /// by number of resources.
        /// 3 - Normal (USA/JAP/EUR/AUS)
        /// 9 - KOR
        /// 5 - CHN (?)
        /// </summary>
        /// <returns></returns>
        private TWLFontTableRegions GetTWLFontTableRegions()
        {
            switch (this.NFTRResourceCount)
            {
                case 3:
                    return TWLFontTableRegions.Normal;
                case 9:
                    return TWLFontTableRegions.Korea;
                default:
                    return TWLFontTableRegions.Unknown;
            }
        }
    }
}
