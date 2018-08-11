using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CharsetGen
{
    public class FontInfo
    {
        public FontInfo(XDocument charmapDocument)
        {
            this.CharMap = FontInfo.ReadCharmap(charmapDocument).ToList();
        }

        public IList<CharmapEntry> CharMap { get; }

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
