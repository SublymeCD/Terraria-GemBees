﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace GemBees.Content.Tiles
{
    internal class WackyTile : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;


			AddMapEntry(new Color(200, 200, 200));
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}
