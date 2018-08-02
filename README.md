# TWLFontTools

Tools to extract Nitro FonT Resource (NFTR) from TWLFontTable.dat and prepare its usage in an EasyGL2D compatible format.

Thanks to http://problemkaputt.de/gbatek.htm#dsisdmmcfirmwarefontfile for documentation on the file format.

## TWLFontTableDumper

Dumps NFTR fonts from TWLFontTable.dat
Usage: `dotnet twlfonttabledumper.dll`, while TWLFontTable.dat is in the same directory.

## UVGen
Generates UV coordinates (for 32 characters per line) for a font texture

Usage: `dotnet uvgen.dll [fontinfo.xml] [texture.png] [width] [height] [size]` where fontinfo.xml is a Tinke* character map for the NFTR.

_\* The modified font plugin is required to generate a compatible fontinfo.xml and texture dump. See https://github.com/RonnChyran/tinke_

## LUTGen
Generates a lookup table from UTF16 codepoints indices to the UV coordinate array index.

Usage: `dotnet lutgen.dll [fontinfo.xml]`
