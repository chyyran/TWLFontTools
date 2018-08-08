using System;
using System.Collections.Generic;
using System.Text;

namespace UVGen
{
    public class UVInfo
    {
        public int PrimaryColumnsPerTexture { get; }
        public int PrimaryRowsPerTexture { get; }
        public int PrimarySpritesPerTexture { get; }
        public int PrimaryTextureCount { get; } = 1;

        public int AuxColumnsPerTexture { get; }
        public int AuxRowsPerTexture { get; }
        public int AuxSpritesPerTexture { get; }
        public int AuxTextureCount { get; }

        public int TileHeight { get; }
        public int TileWidth { get; }
        public UVInfo(int cpt, int rpt, int spt, int th, int tw, int tc, int acpt, int arpt, int aspt)
        {
            this.PrimaryColumnsPerTexture = cpt;
            this.PrimaryRowsPerTexture = rpt;
            this.PrimarySpritesPerTexture = spt;

            this.AuxColumnsPerTexture = acpt;
            this.AuxRowsPerTexture = arpt;
            this.AuxSpritesPerTexture = aspt;
            this.AuxTextureCount = tc;
            this.TileHeight = th;
            this.TileWidth = tw;
        }
    }
}
