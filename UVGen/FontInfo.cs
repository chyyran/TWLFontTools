using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace UVGen
{
    public class FontInfo
    {
        public FontInfo(XDocument charmapDocument)
        {
            this.CharMap = FontInfo.ReadCharmap(charmapDocument).ToList();
            this.UVInfo = FontInfo.ReadUVInfo(charmapDocument);
        }

        public IList<CharmapEntry> CharMap { get; }
        public UVInfo UVInfo { get; }

        private static UVInfo ReadUVInfo(XDocument charmapDocument)
        {
            var uvgen = charmapDocument.Root.Element("UVGen");
            int cpt = XmlConvert.ToInt32(uvgen.Element("PrimaryColumns").Value);
            int rpt = XmlConvert.ToInt32(uvgen.Element("PrimaryRows").Value);
            int spt = XmlConvert.ToInt32(uvgen.Element("PrimarySprites").Value);

            int acpt = XmlConvert.ToInt32(uvgen.Element("AuxColumns").Value);
            int arpt = XmlConvert.ToInt32(uvgen.Element("AuxRows").Value);
            int aspt = XmlConvert.ToInt32(uvgen.Element("AuxSprites").Value);

            int th = XmlConvert.ToInt32(uvgen.Element("TileHeight").Value);
            int tw = XmlConvert.ToInt32(uvgen.Element("TileWidth").Value);
            int tc = XmlConvert.ToInt32(uvgen.Element("AuxTextureCount").Value);
            return new UVInfo(cpt, rpt, spt, th, tw, tc, acpt, arpt, aspt);
        }

        private static IEnumerable<CharmapEntry> ReadCharmap(XDocument charmapDocument)
        {
            foreach (var charmapEntry in charmapDocument.Root.Elements("CharInfo"))
            {
                int character = XmlConvert.ToInt32(charmapEntry.Attribute("Code").Value);
                int index = XmlConvert.ToInt32(charmapEntry.Attribute("Index").Value);
                int width = XmlConvert.ToInt32(charmapEntry.Attribute("Width").Value);
                yield return new CharmapEntry(Convert.ToChar(character), index, width);
            }
        }


    }
}
