using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GemBees.Content.Entities.Bees
{
    internal class GemBee : ModNPC
    {
        private int ClonedNPCID = NPCID.Firefly;
		private static int numberSpawned = 0;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = Main.npcFrameCount[ClonedNPCID]; // Copy animation frames
			Main.npcCatchable[Type] = true; // This is for certain release situations

			// These three are typical critter values
			NPCID.Sets.CountsAsCritter[Type] = NPCID.Sets.CountsAsCritter[ClonedNPCID];
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[ClonedNPCID];
			NPCID.Sets.TownCritter[Type] = NPCID.Sets.TownCritter[ClonedNPCID];

			// This is so it appears between the frog and the gold frog
			NPCID.Sets.NormalGoldCritterBestiaryPriority.Insert(NPCID.Sets.NormalGoldCritterBestiaryPriority.IndexOf(ClonedNPCID) + 1, Type);
		}

		public override void SetDefaults()
		{
			NPC.CloneDefaults(ClonedNPCID);
			NPC.rarity = 2;
			NPC.catchItem = ModContent.ItemType<Items.Bees.GemBeeItem>();
			NPC.lavaImmune = true;
			AIType = ClonedNPCID;
			AnimationType = ClonedNPCID;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.AddTags(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("The most adorable goodest spicy child. Do not dare be mean to him!"));
		}

        public override void AI()
        {
			base.AI();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			float spawnChance = 0f;
			if (Utils.TileUtils.CheckNearbyTiles((tile) => TileID.Sets.CountsAsGemTree[tile.TileType], spawnInfo.SpawnTileX, spawnInfo.SpawnTileY, 5))
            {
				spawnChance = 1f;
            }
			return spawnChance;
		}

        public override void OnSpawn(IEntitySource source)
        {
			numberSpawned++;
			Talk($"Total bees spawned so far: {numberSpawned}");
			base.OnSpawn(source);
        }

        private void Talk(string message)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(message, 150, 250, 150);
			}
			else
			{
				//NetworkText text = NetworkText.FromKey("Mods.ExampleMod.NPCTalk", Lang.GetNPCNameValue(npc.type), message);
				NetworkText text = NetworkText.FromLiteral(message);
				NetMessage.SendData(MessageID.ChatText, text: text);
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				for (int i = 0; i < 6; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Worm, 2 * hit.HitDirection, -2f);
					if (Main.rand.NextBool(2))
					{
						dust.noGravity = true;
						dust.scale = 1.2f * NPC.scale;
					}
					else
					{
						dust.scale = 0.7f * NPC.scale;
					}
				}
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}_Gore_Head").Type, NPC.scale);
				Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}_Gore_Leg").Type, NPC.scale);
			}
		}

		public override Color? GetAlpha(Color drawColor)
		{
			// GetAlpha gives our Lava Frog a red glow.
			return drawColor with
			{
				R = 255,
				// Both these do the same in this situation, using these methods is useful.
				G = Terraria.Utils.Clamp<byte>(drawColor.G, 175, 255),
				B = Math.Min(drawColor.B, (byte)75),
				A = 255
			};
		}

		public override bool PreAI()
		{
			// Kills the NPC if it hits water, honey or shimmer
			if (NPC.wet && !Collision.LavaCollision(NPC.position, NPC.width, NPC.height))
			{ // NPC.lavawet not 100% accurate for the frog
			  // These 3 lines instantly kill the npc without showing damage numbers, dropping loot, or playing DeathSound. Use this for instant deaths
				NPC.life = 0;
				NPC.HitEffect();
				NPC.active = false;
				SoundEngine.PlaySound(SoundID.NPCDeath16, NPC.position); // plays a fizzle sound
			}

			return true;
		}

		public override void OnCaughtBy(Player player, Item item, bool failed)
		{
			if (failed)
			{
				return;
			}

			Point npcTile = NPC.Center.ToTileCoordinates();

			if (!WorldGen.SolidTile(npcTile.X, npcTile.Y))
			{ // Check if the tile the npc resides the most in is non solid
				Tile tile = Main.tile[npcTile];
				tile.LiquidAmount = tile.LiquidType == LiquidID.Lava ? // Check if the tile has lava in it
					Math.Max((byte)Main.rand.Next(50, 150), tile.LiquidAmount) // If it does, then top up the amount
					: (byte)Main.rand.Next(50, 150); // If it doesn't, then overwrite the amount. Technically this distinction should never be needed bc it will burn but to be safe it's here
				tile.LiquidType = LiquidID.Lava; // Set the liquid type to lava
				WorldGen.SquareTileFrame(npcTile.X, npcTile.Y, true); // Update the surrounding area in the tilemap
			}
		}
	}
}
