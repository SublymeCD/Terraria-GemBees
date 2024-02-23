using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.ID;
using System.Threading.Tasks;
using Terraria.WorldBuilding;
using ReLogic.Utilities;
using Terraria.Utilities;
using System;

namespace GemBees.Common.Systems.WorldGeneration
{
    internal class DebugUtils : ModSystem
    {
		public static bool JustPressed(Keys key)
		{
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}

		public override void PostUpdateWorld()
		{
			if (JustPressed(Keys.D1))
            {
				TestMethod2(new Point((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16));
			}
		}

		private void TestMethod2(Point point)
        {
			WorldUtils.Gen(point, new Shapes.Circle(15), Actions.Chain(new Modifiers.RadialDither(0,10), new Actions.SetTile(TileID.Hive)));

		}

		private void TestMethod(Point point)
		{
			int x = point.X;
			int y = point.Y;
			Point origin = point;

			Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, Color.YellowGreen, null);

			//test clearing initial space?
			WorldUtils.Gen(origin, new Shapes.Circle(15), new Actions.ClearTile());


			//generating the walls/replacing the blocks
			int num = 0;
			int[] array = new int[1000];
			int[] array2 = new int[1000];
			Vector2D val = point.ToVector2D();

			for (int i = 0; i < 5; i++)
			{
				Vector2D val2 = val;
				int num3 = WorldGen.genRand.Next(2, 5);
				for (int j = 0; j < num3; j++)
				{
					val2 = CreateHiveTunnel((int)val.X, (int)val.Y, WorldGen.genRand);
				}
				val = val2;
				array[num] = (int)val.X;
				array2[num] = (int)val.Y;
				num++;
			}

			FrameOutAllHiveContents(point, 50);

			for (int k = 0; k < num; k++)
			{
				int num4 = array[k];
				int l = array2[k];
				int num5 = 1;
				if (WorldGen.genRand.NextBool(2))
				{
					num5 = -1;
				}
				bool flag = false;
				while (WorldGen.InWorld(num4, l, 10))
				{
					num4 += num5;
					if (Math.Abs(num4 - array[k]) > 50)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num4 += num5;
					CreateBlockedHoneyCube(num4, l);
					CreateDentForHoneyFall(num4, l, num5);
					
				}
			}

			//inner tunnel? the missing link?


			if (WorldGen.SolidTile(origin.X, origin.Y))
			{
				return;
			}
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(80), new Conditions.IsSolid()), out var result))
			{
				return;
			}
			result.Y += 2;
			Ref<int> @ref = new Ref<int>(0);
			WorldUtils.Gen(result, new Shapes.Circle(8), Actions.Chain(new Modifiers.IsSolid(), new Actions.Scanner(@ref)));
			if (@ref.Value < 20)
			{
				return;
			}
			//if (!structures.CanPlace(new Rectangle(result.X - 8, result.Y - 8, 16, 16)))
			//{
			//	return;
			//}
			WorldUtils.Gen(result, new Shapes.Circle(8), Actions.Chain(new Modifiers.RadialDither(0.0, 10.0), new Modifiers.IsSolid(), new Actions.SetTile(229, setSelfFrames: true)));
			ShapeData data = new ShapeData();
			WorldUtils.Gen(result, new Shapes.Circle(4, 3), Actions.Chain(new Modifiers.Blotches(), new Modifiers.IsSolid(), new Actions.ClearTile(frameNeighbors: true), new Modifiers.RectangleMask(-6, 6, 0, 3).Output(data), new Actions.SetLiquid(2)));
			WorldUtils.Gen(new Point(result.X, result.Y + 1), new ModShapes.InnerOutline(data), Actions.Chain(new Modifiers.IsEmpty(), new Modifiers.RectangleMask(-6, 6, 1, 3), new Actions.SetTile(59, setSelfFrames: true)));
			//structures.AddProtectedStructure(new Rectangle(result.X - 8, result.Y - 8, 16, 16));
			return;

		}
		private static Vector2D CreateHiveTunnel(int i, int j, UnifiedRandom random)
		{
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			double num = random.Next(12, 21);
			double num2 = random.Next(10, 21);
			double num4 = num;
			Vector2D val = default(Vector2D);
			val.X = i;
			val.Y = j;
			Vector2D val2 = default(Vector2D);
			val2.X = (double)random.Next(-10, 11) * 0.2;
			val2.Y = (double)random.Next(-10, 11) * 0.2;
			while (num > 0.0 && num2 > 0.0)
			{
				if (val.Y > (double)(Main.maxTilesY - 250))
				{
					num2 = 0.0;
				}
				num = num4 * (1.0 + (double)random.Next(-20, 20) * 0.01);
				num2 -= 1.0;
				int num5 = (int)(val.X - num);
				int num6 = (int)(val.X + num);
				int num7 = (int)(val.Y - num);
				int num8 = (int)(val.Y + num);
				if (num5 < 1)
				{
					num5 = 1;
				}
				if (num6 > Main.maxTilesX - 1)
				{
					num6 = Main.maxTilesX - 1;
				}
				if (num7 < 1)
				{
					num7 = 1;
				}
				if (num8 > Main.maxTilesY - 1)
				{
					num8 = Main.maxTilesY - 1;
				}
				for (int k = num5; k < num6; k++)
				{
					for (int l = num7; l < num8; l++)
					{
						if (!WorldGen.InWorld(k, l, 50))
						{
							num2 = 0.0;
						}
						else
						{
							if (Main.tile[k - 10, l].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
							if (Main.tile[k + 10, l].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
							if (Main.tile[k, l - 10].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
							if (Main.tile[k, l + 10].WallType == WallID.LihzahrdBrickUnsafe)
							{
								num2 = 0.0;
							}
						}
						if ((double)l < Main.worldSurface && Main.tile[k, l - 5].WallType == WallID.None)
						{
							num2 = 0.0;
						}
						double num9 = Math.Abs((double)k - val.X);
						double num10 = Math.Abs((double)l - val.Y);
						double num11 = Math.Sqrt(num9 * num9 + num10 * num10);
						if (num11 < num4 * 0.4 * (1.0 + (double)random.Next(-10, 11) * 0.005))
						{
							Main.tile[k, l].WallType = WallID.HiveUnsafe;
						}
						else if (num11 < num4 * 0.75 * (1.0 + (double)random.Next(-10, 11) * 0.005))
						{
							if (Main.tile[k, l].WallType != WallID.HiveUnsafe)
							{
								Main.tile[k, l].TileType = TileID.Hive;
							}
						}
						if (num11 < num4 * 0.6 * (1.0 + (double)random.Next(-10, 11) * 0.005))
						{
							Main.tile[k, l].WallType = WallID.HiveUnsafe;
						}
					}
				}
				val += val2;
				num2 -= 1.0;
				val2.Y += (double)random.Next(-10, 11) * 0.05;
				val2.X += (double)random.Next(-10, 11) * 0.05;
			}
			return val;
		}

		private static void FrameOutAllHiveContents(Point origin, int squareHalfWidth)
		{
			int num = Math.Max(10, origin.X - squareHalfWidth);
			int num2 = Math.Min(Main.maxTilesX - 10, origin.X + squareHalfWidth);
			int num3 = Math.Max(10, origin.Y - squareHalfWidth);
			int num4 = Math.Min(Main.maxTilesY - 10, origin.Y + squareHalfWidth);
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile.TileType == TileID.Hive)
					{
						WorldGen.SquareTileFrame(i, j);
					}
					if (tile.WallType == WallID.HiveUnsafe)
					{
						WorldGen.SquareWallFrame(i, j);
					}
				}
			}
		}

		private static void CreateDentForHoneyFall(int x, int y, int dir)
		{
			dir *= -1;
			y++;
			int num = 0;
			while ((num < 4 || WorldGen.SolidTile(x, y)) && x > 10 && x < Main.maxTilesX - 10)
			{
				num++;
				x += dir;
				if (WorldGen.SolidTile(x, y))
				{
					WorldGen.PoundTile(x, y);
					Main.tile[x, y + 1].TileType = TileID.Hive;
				}
			}
		}

		private static void CreateBlockedHoneyCube(int x, int y)
		{
			for (int i = x - 1; i <= x + 2; i++)
			{
				for (int j = y - 1; j <= y + 2; j++)
				{
					if (i >= x && i <= x + 1 && j >= y && j <= y + 1)
					{
					}
					else
					{
						Main.tile[i, j].TileType = TileID.Hive;
					}
				}
			}
		}
	}
}
