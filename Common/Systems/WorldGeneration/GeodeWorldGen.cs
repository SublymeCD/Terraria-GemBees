using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace GemBees.Common.Systems.WorldGeneration
{
    public class GeodeWorldGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int graniteIndex = tasks.FindIndex(t => t.Name == "Granite");
            tasks.Insert(graniteIndex+1, new GeodePass("Bee Geode Pass name", 100f));
        }
    }
}
