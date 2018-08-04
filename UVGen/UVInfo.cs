using System;
using System.Collections.Generic;
using System.Text;

namespace UVGen
{
    public class UVInfo
    {
        public int ColumnsPerTexture { get; }
        public int RowsPerTexture { get; }
        public int SpritesPerTexture { get; }
        public int TileHeight { get; }
        public int TileWidth { get; }
        public int TextureCount { get; }

        public UVInfo(int cpt, int rpt, int spt, int th, int tw, int tc)
        {
            this.ColumnsPerTexture = cpt;
            this.RowsPerTexture = rpt;
            this.SpritesPerTexture = spt;
            this.TileHeight = th;
            this.TileWidth = tw;
            this.TextureCount = tc;
        }
    }
}
