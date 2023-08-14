using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BBossBars
{
	// Since this mod isn't very large, its helper methods have been placed here in the main Mod class
	public class BBossBars : Mod
	{
		/// <summary>
		/// Gets the name of the npc for the purposes of the boss bar's display.
		/// </summary>
		/// <param name="npc">The npc to get the name of.</param>
		public string BossNamingCheck(NPC npc)
		{
            switch (npc.type)
            {
				case NPCID.MoonLordCore:
				case NPCID.MoonLordHand:
				case NPCID.MoonLordHead:
					return "Moon Lord";
				case NPCID.PirateShip:
				case NPCID.PirateShipCannon:
					return "Flying Dutchman";
				case NPCID.MartianSaucerCore:
				case NPCID.MartianSaucerCannon:
				case NPCID.MartianSaucerTurret:
					return "Martian Saucer";
				case NPCID.Golem:
				case NPCID.GolemHead:
					return "Golem";
				default:
					break;
            }
			// Mechdusa (Get Fixed Boi)
			if ((npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer 
				|| npc.type == NPCID.TheDestroyer
				|| npc.type == NPCID.SkeletronPrime)
				&& ModContent.GetInstance<Config>().mechDisplay != "Vanilla" && GetFixedBoiCheck())
			{
				return "Mechdusa";
			}
			else if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
			{
				bool oneTwin = CompanionCheck(NPCID.Retinazer) + CompanionCheck(NPCID.Spazmatism) == 1;
				if (ModContent.GetInstance<Config>().twinsDisplay == "Seperate"
					|| ModContent.GetInstance<Config>().twinsDisplay == "Singular Text"
					|| (ModContent.GetInstance<Config>().twinsDisplay == "Vanilla (Default)" && oneTwin))
				{
					return npc.FullName;
				}
				else
				{
					return "The Twins";
				}
			}
			// Default to the npc's display name
			return npc.FullName;
		}

		/// <summary>
		/// Gets the current HP of the npc for the purposes of the boss bar's display. Many bosses have special logic to account for multiple parts.
		/// </summary>
		/// <param name="npc">The npc to get the HP of.</param>
		public int BossHPCheck(NPC npc)
		{
			List<int> targets;
			switch (npc.type)
			{
				case NPCID.MoonLordCore:
				case NPCID.MoonLordHand:
				case NPCID.MoonLordHead:
                    targets = new List<int>
                    {
                        NPCID.MoonLordCore,
                        NPCID.MoonLordHand,
                        NPCID.MoonLordHead
                    };
                    return CumulativeCurrentHP(targets);

				case NPCID.PirateShip:
				case NPCID.PirateShipCannon:
                    targets = new List<int>
                    {
                        NPCID.PirateShipCannon
                    };
                    return CumulativeCurrentHP(targets);

				case NPCID.MartianSaucerCore:
				case NPCID.MartianSaucerCannon:
				case NPCID.MartianSaucerTurret:
                    targets = new List<int>
                    {
                        NPCID.MartianSaucerCannon,
                        NPCID.MartianSaucerTurret
                    };
                    if (Main.expertMode || Main.masterMode) // Core is only a killable part of the encounter in Expert or higher
					{
						targets.Add(NPCID.MartianSaucerCore);
					}
					return CumulativeCurrentHP(targets);

				case NPCID.Golem:
				case NPCID.GolemHead:
                    targets = new List<int>
                    {
                        NPCID.Golem,
                        NPCID.GolemHead
                    };
                    if (ModContent.GetInstance<Config>().golemHands == true)
					{
						targets.Add(NPCID.GolemFistLeft);
						targets.Add(NPCID.GolemFistRight);
					}
					return CumulativeCurrentHP(targets);

				case NPCID.BrainofCthulhu:
                    targets = new List<int>
                    {
                        NPCID.BrainofCthulhu
                    };
                    if (ModContent.GetInstance<Config>().brainCreepers == true)
					{
						targets.Add(NPCID.Creeper);
					}
					return CumulativeCurrentHP(targets);

				case NPCID.EaterofWorldsHead:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
                    targets = new List<int>
                    {
						NPCID.EaterofWorldsHead,
						NPCID.EaterofWorldsBody,
						NPCID.EaterofWorldsTail
					};
                    return CumulativeCurrentHP(targets);

				case NPCID.SkeletronHead:
                    targets = new List<int>
                    {
                        NPCID.SkeletronHead
                    };
                    if (ModContent.GetInstance<Config>().skeletronHands == true)
					{
						targets.Add(NPCID.SkeletronHand);
					}
					return CumulativeCurrentHP(targets);

				default:
					break;
			}

			// Mechdusa (Get Fixed Boi)
			if ((npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer
				|| npc.type == NPCID.TheDestroyer
				|| npc.type == NPCID.SkeletronPrime)
				&& ModContent.GetInstance<Config>().mechDisplay == "Name & HP" && GetFixedBoiCheck())
			{
                targets = new List<int>
                {
                    NPCID.Spazmatism,
                    NPCID.Retinazer,
                    NPCID.TheDestroyer,
                    NPCID.SkeletronPrime
                };
                if (ModContent.GetInstance<Config>().primeHands == true)
				{
					targets.Add(NPCID.PrimeCannon);
					targets.Add(NPCID.PrimeLaser);
					targets.Add(NPCID.PrimeSaw);
					targets.Add(NPCID.PrimeVice);
				}
				return CumulativeCurrentHP(targets);
			}
			else if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
			{
				bool oneTwin = CompanionCheck(NPCID.Retinazer) + CompanionCheck(NPCID.Spazmatism) == 1;
				if (ModContent.GetInstance<Config>().twinsDisplay == "Seperate"
					|| ModContent.GetInstance<Config>().twinsDisplay == "Singular Text"
					|| (ModContent.GetInstance<Config>().twinsDisplay == "Vanilla (Default)" && oneTwin))
				{
					return npc.life;
				}
				else
				{
                    targets = new List<int>
                    {
                        NPCID.Spazmatism,
                        NPCID.Retinazer
                    };
                    return CumulativeCurrentHP(targets);
				}
			}
			else if (npc.type == NPCID.SkeletronPrime)
			{
                targets = new List<int>
                {
                    NPCID.SkeletronPrime
                };
                if (ModContent.GetInstance<Config>().primeHands == true)
				{
					targets.Add(NPCID.PrimeCannon);
					targets.Add(NPCID.PrimeLaser);
					targets.Add(NPCID.PrimeSaw);
					targets.Add(NPCID.PrimeVice);
				}
				return CumulativeCurrentHP(targets);
			}
			// Default to npc.life
			return npc.life;
		}

		/// <summary>
		/// Gets the max HP of the npc for the purposes of the boss bar's display. Many bosses have special logic to account for multiple parts.
		/// </summary>
		/// <param name="npc">The npc to get the max HP of.</param>
		public int BossMaxHPCheck(NPC npc)
		{
			List<int> targets;
			switch (npc.type)
			{
				case NPCID.MoonLordCore:
				case NPCID.MoonLordHand:
				case NPCID.MoonLordHead:
					targets = new List<int>
					{
						NPCID.MoonLordCore,
						NPCID.MoonLordHand,
						NPCID.MoonLordHand,
						NPCID.MoonLordHead
					};
					return CumulativeMaxHP(targets);

				case NPCID.PirateShip:
				case NPCID.PirateShipCannon:
					targets = new List<int>
					{
						NPCID.PirateShipCannon,
						NPCID.PirateShipCannon,
						NPCID.PirateShipCannon,
						NPCID.PirateShipCannon
					};
					return CumulativeMaxHP(targets);

				case NPCID.MartianSaucerCore:
				case NPCID.MartianSaucerCannon:
				case NPCID.MartianSaucerTurret:
					targets = new List<int>
					{
						NPCID.MartianSaucerCannon,
						NPCID.MartianSaucerCannon,
						NPCID.MartianSaucerTurret,
						NPCID.MartianSaucerTurret
					};
					if (Main.expertMode || Main.masterMode) // Core is only a killable part of the encounter in Expert or higher
					{
						targets.Add(NPCID.MartianSaucerCore);
					}
					return CumulativeMaxHP(targets);

				case NPCID.Golem:
				case NPCID.GolemHead:
					targets = new List<int>
					{
						NPCID.Golem,
						NPCID.GolemHead
					};
					if (ModContent.GetInstance<Config>().golemHands == true)
					{
						targets.Add(NPCID.GolemFistLeft);
						targets.Add(NPCID.GolemFistRight);
					}
					return CumulativeMaxHP(targets);

				case NPCID.BrainofCthulhu:
					targets = new List<int>
					{
						NPCID.BrainofCthulhu
					};
					if (ModContent.GetInstance<Config>().brainCreepers == true)
					{		
						int creeperCount = NPC.GetBrainOfCthuluCreepersCount();
						for (int i = 0; i < creeperCount; i++)
						{
							targets.Add(NPCID.Creeper);
						}
					}
					return CumulativeMaxHP(targets);
				case NPCID.EaterofWorldsHead:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
					targets = new List<int>();
					int wormLength = NPC.GetEaterOfWorldsSegmentsCount();
					for (int i = 0; i < wormLength; i++)
					{
						targets.Add(NPCID.EaterofWorldsBody);
					}
					return CumulativeMaxHP(targets);

				case NPCID.SkeletronHead:
					targets = new List<int>
					{
						NPCID.SkeletronHead
					};
					if (ModContent.GetInstance<Config>().skeletronHands == true)
					{
						targets.Add(NPCID.SkeletronHand);
						targets.Add(NPCID.SkeletronHand);
					}
					return CumulativeMaxHP(targets);

				default:
					break;
			}
			
			// Mechdusa (Get Fixed Boi)
			if ((npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer
				|| npc.type == NPCID.TheDestroyer
				|| npc.type == NPCID.SkeletronPrime)
				&& ModContent.GetInstance<Config>().mechDisplay == "Name & HP" && GetFixedBoiCheck())
			{
				targets = new List<int>
				{
					NPCID.Spazmatism,
					NPCID.Retinazer,
					NPCID.TheDestroyer,
					NPCID.SkeletronPrime
				};
				if (ModContent.GetInstance<Config>().primeHands == true)
				{
					targets.Add(NPCID.PrimeCannon);
					targets.Add(NPCID.PrimeLaser);
					targets.Add(NPCID.PrimeSaw);
					targets.Add(NPCID.PrimeVice);
				}
				return CumulativeMaxHP(targets);
			}
			else if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
			{
				bool oneTwin = CompanionCheck(NPCID.Retinazer) + CompanionCheck(NPCID.Spazmatism) == 1;
				if (ModContent.GetInstance<Config>().twinsDisplay == "Seperate"
					|| ModContent.GetInstance<Config>().twinsDisplay == "Singular Text"
					|| (ModContent.GetInstance<Config>().twinsDisplay == "Vanilla (Default)" && oneTwin))
				{
					return npc.lifeMax;
				}
				else
				{
					targets = new List<int>
					{
						NPCID.Spazmatism,
						NPCID.Retinazer
					};
					return CumulativeMaxHP(targets);
				}
			}
			else if (npc.type == NPCID.SkeletronPrime)
			{
				targets = new List<int>
				{
					NPCID.SkeletronPrime
				};
				if (ModContent.GetInstance<Config>().primeHands == true)
				{
					targets.Add(NPCID.PrimeCannon);
					targets.Add(NPCID.PrimeLaser);
					targets.Add(NPCID.PrimeSaw);
					targets.Add(NPCID.PrimeVice);
				}
				return CumulativeMaxHP(targets);
			}
			// Default to npc.lifeMax
			return npc.lifeMax;
		}

		/// <summary>
		/// Determines if Mechdusa can be spawned in the given world. This requires the Remix and FTW seeds to be active.
		/// </summary>
		public bool GetFixedBoiCheck()
		{
			return Main.remixWorld && Main.getGoodWorld;
		}

		/// <summary>
		/// Finds and counts up other boss parts/segments.
		/// </summary>
		/// <param name="companionID">The ID of the npc type to look for.</param>
		public int CompanionCheck(int companionID)
		{
			int found = 0;
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC target = Main.npc[k];
				if (target.type == companionID && target.active)
				{
					found++;
				}
			}
			return found;
		}

		/// <summary>
		/// Counts up the HP of all NPCs whose type ID matches an entry in the target list.
		/// </summary>
		/// <param name="targets">The IDs of the npc types to count.</param>
		public int CumulativeCurrentHP(List<int> targets)
		{
			int totalHP = 0;
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				NPC target = Main.npc[k];
				if (targets.Contains(target.type))
				{
					int hpToAdd = target.life;
					if (hpToAdd < 0 || ((target.type == NPCID.MoonLordHand || target.type == NPCID.MoonLordHead) && IsInBadAI(target)))
					{
						hpToAdd = 0;
					}
					totalHP += hpToAdd;
				}
			}
			if (totalHP < 0)
			{
				return 0;
			}
			return totalHP;
		}

		/// <summary>
		/// Adapted from vanilla method of the same name. Used to determine if parts of the Moon Lord are in a vulnerable state or not.
		/// </summary>
		public bool IsInBadAI(NPC npc)
		{
			if (npc.type == NPCID.MoonLordCore && (npc.ai[0] == 2f || npc.ai[0] == -1f))
			{
				return true;
			}
			if (npc.ai[0] == -2f || npc.ai[0] == -3f)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Counts up the max HP of all NPCs in the target list.
		/// </summary>
		/// <param name="targets">The IDs of the npc types to count.</param>
		public int CumulativeMaxHP(List<int> targets)
		{
			int totalHP = 0;
			for (int k = 0; k < targets.Count; k++)
			{
				NPC dummyNPC = new NPC();
				dummyNPC.CloneDefaults(targets[k]);
				totalHP += dummyNPC.lifeMax;
			}
			return totalHP;
		}
	}
}