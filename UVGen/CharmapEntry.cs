using System;
using System.Collections.Generic;
using System.Text;

namespace UVGen
{
    public class CharmapEntry
    {
        public char Character { get; }
        public int Index { get; }
        public int Width { get; }
        public CharmapEntry(char character, int index, int width)
        {
            this.Character = character;
            this.Index = index;
            this.Width = width;
        }
    }
}
