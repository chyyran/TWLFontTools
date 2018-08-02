# TWLFontTools

Tools to extract Nitro FonT Resource (NFTR) from TWLFontTable.dat and prepare its usage in an EasyGL2D compatible format.

Thanks to http://problemkaputt.de/gbatek.htm#dsisdmmcfirmwarefontfile for documentation on the file format.

To build and run, you will need .NET Core, otherwise all efforts have been made to ensure cross platform compatiblility.

## TWLFontTableDumper

Dumps NFTR fonts from TWLFontTable.dat
Usage: `dotnet twlfonttabledumper.dll`, while TWLFontTable.dat is in the same directory.

## NFTRFontDumper

Generates a texture map and charmap XML from an NFTR font.

Usage: `dotnet nftrfontdumper.dll [font.nftr] (hex color = f5f8f4)`

## UVGen
Generates UV coordinates (for 32 characters per line) for a font texture

Usage: `dotnet uvgen.dll [fontinfo.xml] [texture.png] [width] [height] [size]`.


## LUTGen
Generates a lookup table from UTF16 codepoints indices to the UV coordinate array index.

Usage: `dotnet lutgen.dll [fontinfo.xml]`
