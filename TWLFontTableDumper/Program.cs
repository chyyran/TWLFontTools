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
using System.IO;

namespace TWLFontTableDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!File.Exists(TWLFontTable.TWLFontTableDatFileName))
            {
                Console.WriteLine("TWLFontTable.dat not found.");
                Environment.Exit(1);
            }

            Stream fontTableStream = new FileStream(TWLFontTable.TWLFontTableDatFileName, FileMode.Open, FileAccess.Read);
            var fontTable = new TWLFontTable(fontTableStream);
            Console.WriteLine($"Found TWLFontTable.dat with {fontTable.NFTRResourceCount} resources for region {fontTable.Region}");
            Console.WriteLine($"Dumping resources...");
            foreach (var resource in fontTable.GetCompressedResources())
            {
                Console.WriteLine($"Found resource {resource.ResourceName} (Size: {resource.CompressedResourceSize}," +
                    $" SHA1: {BitConverter.ToString(resource.CompressedResourceSHA1).Replace("-", "")})");
                try
                {
                    var writer = resource.GetWriter();
                    writer.WriteResource();
                    Console.WriteLine($"Successfully decompressed resource {resource.ResourceName}");
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
