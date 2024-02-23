using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace GemBees.Common.Systems.WorldGeneration
{
    public class GeodePass : GenPass
    {
        public GeodePass(string name, double loadWeight) : base(name, loadWeight) { }
        public const int GeodeSize = 100;
        public const int WallSize = 5;
        public const int InnerSize = GeodeSize - WallSize;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Forming Geode Hives";

            for(int i = 0; i < 6; i++)
            {
                Point point = new Point(WorldGen.genRand.Next(GeodeSize, Main.maxTilesX - GeodeSize), WorldGen.genRand.Next((int)Main.rockLayer + GeodeSize, Main.UnderworldLayer - GeodeSize));
                CreateGeode(point, i);
            }
        }

        public static void CreateGeode(Point centerPoint, int gemType)
        {
            WorldUtils.Gen(centerPoint, new Shapes.Circle(GeodeSize, GeodeSize), Actions.Chain(new GenAction[]
            {
                    new Actions.SetTile(TileID.Stone),
                //new Actions.Custom((i, j, args) => {Dust.QuickDust(new Point(i, j), Color.Purple); return true; }),
            }));

            WorldUtils.Gen(centerPoint, new Shapes.Circle(InnerSize, InnerSize), Actions.Chain(new GenAction[]
            {
                    new Actions.PlaceWall(WallID.Stone),
                    new Actions.ClearTile(),
                //new Actions.Custom((i, j, args) => {Dust.QuickDust(new Point(i, j), Color.Red); return true; }),
            })); ;

            WorldUtils.Gen(centerPoint, new Shapes.Circle(InnerSize, InnerSize), Actions.Chain(new GenAction[]
            {
                    new Modifiers.Dither(.2),
                    new Actions.PlaceTile(TileID.ExposedGems, gemType)
            }));
        }
    }
}
