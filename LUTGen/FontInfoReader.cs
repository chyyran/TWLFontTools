using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LUTGen
{
    public class FontInfoReader
    {
        public static IEnumerable<CharmapEntry> Read(XDocument charmapDocument)
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
