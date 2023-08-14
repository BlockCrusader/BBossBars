using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using ReLogic.Graphics;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.GameContent.Creative;
using System;

namespace BBossBars;
public class BlocksBossBarStyle : ModBossBarStyle
{
	private const string _classicTexturePath = "BBossBars/Assets/ClassicBar";
	private const string _expertTexturePath = "BBossBars/Assets/ExpertBar";
	private const string _masterTexturePath = "BBossBars/Assets/MasterBar";
	private const string _worthyTexturePath = "BBossBars/Assets/WorthyBar";
	private const string _legendaryTexturePath = "BBossBars/Assets/LegendaryBar";
	private const string _nullBossHeadTexturePath = "BBossBars/Assets/NullBossHead";

	public override bool PreventDraw => true; 

    public override void Draw(SpriteBatch spriteBatch, IBigProgressBar currentBar, BigProgressBarInfo info) 
	{
		if (Main.npc[info.npcIndexToAimAt] == null || currentBar == null || Main.npc[info.npcIndexToAimAt].active == false)
        {
			return;
        } 

		// From here, go through each case of vanilla boss bar, and attend to each as needed so we can perform a draw method with the accessible data
		
		int currentShieldValue = 0; // Used for shield bars

		// Mechdusa requires a substantial amount of extra logic, part of which is contained here
		bool twinsInGFB = Main.zenithWorld && Main.remixWorld && currentBar is TwinsBigProgressBar && ModContent.GetInstance<Config>().mechDisplay == "Name & HP";
		
		if (currentBar is CommonBossBigProgressBar || twinsInGFB)
		{
			// TODO: Clean up a bit more so it's not a big jumble of if-else statements...

			NPC npcCheck = Main.npc[info.npcIndexToAimAt];

			// Various bosses with mod-exclusive logic fall udner the 'Common' bar category. Here we determine what modded logic to use
			if (npcCheck.type == NPCID.SkeletronHead || npcCheck.type == NPCID.SkeletronPrime || npcCheck.type == NPCID.TheDestroyer || twinsInGFB)
            {
				// Skeletron
                if (npcCheck.type == NPCID.SkeletronHead 
					&& ModContent.GetInstance<Config>().skeletronHands 
					&& ModContent.GetInstance<Config>().optionalPartAccounting)
                {
					PrepSpecialBarSkeletron(spriteBatch, info);
				}
				// Mechs
				else if ((npcCheck.type == NPCID.SkeletronPrime 
					|| npcCheck.type == NPCID.Spazmatism || npcCheck.type == NPCID.Retinazer
					|| npcCheck.type == NPCID.TheDestroyer) && Main.getGoodWorld && Main.remixWorld
					&& ModContent.GetInstance<Config>().mechDisplay == "Name & HP")
				{
					PrepSpecialBarMechdusa(spriteBatch, info);
				}
				else if (npcCheck.type == NPCID.SkeletronPrime 
					&& ModContent.GetInstance<Config>().primeHands 
					&& ModContent.GetInstance<Config>().optionalPartAccounting)
				{
					PrepSpecialBarPrime(spriteBatch, info);
				}
				else
				{
					PrepCommonBar(spriteBatch, info);
				}
			}
            else
            {
				PrepCommonBar(spriteBatch, info);
			}
		}
		// The rest of the bossbar logic is adapted from vanilla. Some have extra logic for toggleable modded additions
		else if (currentBar is EaterOfWorldsProgressBar)
		{
			int segCount = NPC.GetEaterOfWorldsSegmentsCount();
			float cumulativeHP = 0f;
			for (int i = 0; i < 200; i++)
			{
				NPC nPC = Main.npc[i];
				if (nPC.active && nPC.type >= NPCID.EaterofWorldsHead && nPC.type <= NPCID.EaterofWorldsTail)
				{
					cumulativeHP += (float)nPC.life / (float)nPC.lifeMax;
				}
			}
			float percentHP = Utils.Clamp<float>(cumulativeHP / (float)segCount, 0f, 1f);

			int headIndex = NPCID.Sets.BossHeadTextures[NPCID.EaterofWorldsHead];

			DrawBar(spriteBatch, percentHP, headIndex);
		}
		else if (currentBar is BrainOfCthuluBigProgressBar)
		{
			NPC nPC = Main.npc[info.npcIndexToAimAt];

			int creeperCount = NPC.GetBrainOfCthuluCreepersCount();
			NPC dummyCreeper = new NPC();
			dummyCreeper.SetDefaults(267, nPC.GetMatchingSpawnParams());

			int maxCreeperHP = dummyCreeper.lifeMax * creeperCount;
			float creeperLife = 0f;
			for (int i = 0; i < 200; i++)
			{
				NPC nPC2 = Main.npc[i];
				if (nPC2.active && nPC2.type == dummyCreeper.type)
				{
					creeperLife += (float)nPC2.life;
				}
			}

			float currentLife = (float)nPC.life + creeperLife;
			int maxLife = nPC.lifeMax + maxCreeperHP;
			float percentHP = Utils.Clamp<float>(currentLife / (float)maxLife, 0f, 1f);

			int headIndex = NPCID.Sets.BossHeadTextures[NPCID.BrainofCthulhu];

			DrawBar(spriteBatch, percentHP, headIndex);
		}
		else if (currentBar is DeerclopsBigProgressBar) // Same as common. Vanilla just uses this bar to make Deerclops's bar appear from farther away
		{
			PrepCommonBar(spriteBatch, info);
		}
		else if (currentBar is TwinsBigProgressBar)
		{
			NPC nPC = Main.npc[info.npcIndexToAimAt];
			int otherTwinID = ((nPC.type == NPCID.Spazmatism) ? NPCID.Retinazer : NPCID.Spazmatism);
			int maxHP = nPC.lifeMax;
			int currentHP = nPC.life;

			if(ModContent.GetInstance<Config>().twinsDisplay == "Vanilla (Default)"
				|| ModContent.GetInstance<Config>().twinsDisplay == "Cumulative Text"
				|| ModContent.GetInstance<Config>().twinsDisplay == "Singular Text")
            {
				for (int i = 0; i < 200; i++)
				{
					NPC nPC2 = Main.npc[i];
					if (nPC2.active && nPC2.type == otherTwinID)
					{
						maxHP += nPC2.lifeMax;
						currentHP += nPC2.life;
						break;
					}
				}
			}
			else if(ModContent.GetInstance<Config>().twinsDisplay == "Cumulative Text & HP")
            {
				NPC referenceSpaz = new NPC();
				referenceSpaz.SetDefaults(NPCID.Spazmatism, nPC.GetMatchingSpawnParams());
				NPC referenceRet = new NPC();
				referenceRet.SetDefaults(NPCID.Retinazer, nPC.GetMatchingSpawnParams());
				if(otherTwinID == referenceSpaz.type)
                {
					maxHP += referenceSpaz.lifeMax;
				}
				else if (otherTwinID == referenceRet.type)
				{
					maxHP += referenceRet.lifeMax;
				}
				for (int i = 0; i < 200; i++)
				{
					NPC nPC2 = Main.npc[i];
					if (nPC2.active && nPC2.type == otherTwinID)
					{
						currentHP += nPC2.life;
						break;
					}
				}
			}
			
			float percentHP = Utils.Clamp<float>((float)currentHP / (float)maxHP, 0f, 1f);

			int headIndex = nPC.GetBossHeadTextureIndex();

			DrawBar(spriteBatch, percentHP, headIndex);
		}
		else if (currentBar is GolemHeadProgressBar)
		{
			HashSet<int> validIDs = new();
			validIDs.Add(NPCID.GolemHead);
			validIDs.Add(NPCID.Golem);
			if (ModContent.GetInstance<Config>().golemHands
				&& ModContent.GetInstance<Config>().optionalPartAccounting)
			{
				validIDs.Add(NPCID.GolemFistLeft);
				validIDs.Add(NPCID.GolemFistRight);
			}

			NPC nPC = Main.npc[info.npcIndexToAimAt];
			if (!nPC.active)
			{
				bool check = false;
				for (int i = 0; i < 200; i++)
				{
					NPC golemCheck = Main.npc[i];
					if (golemCheck.active 
						&& golemCheck.type != NPCID.GolemFistLeft && golemCheck.type != NPCID.GolemFistRight
						&& validIDs.Contains(golemCheck.type))
					{
						info.npcIndexToAimAt = i;
						check = true;
					}
				}
				if (check == false)
                {
					return;
				}
			}
			int maxHP = 0;
			NPC dummyNPC = new NPC();
			dummyNPC.SetDefaults(NPCID.GolemHead, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax;
			dummyNPC.SetDefaults(NPCID.Golem, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax;
            if (ModContent.GetInstance<Config>().golemHands
				&& ModContent.GetInstance<Config>().optionalPartAccounting)
            {
				dummyNPC.SetDefaults(NPCID.GolemFistLeft, nPC.GetMatchingSpawnParams());
				maxHP += dummyNPC.lifeMax;
				dummyNPC.SetDefaults(NPCID.GolemFistRight, nPC.GetMatchingSpawnParams());
				maxHP += dummyNPC.lifeMax;
			}

			float currentHP = 0f;
			for (int i = 0; i < 200; i++)
			{
				NPC nPC2 = Main.npc[i];
				if (nPC2.active && validIDs.Contains(nPC2.type))
				{
					currentHP += (float)nPC2.life;
				}
			}
			float percentHP = Utils.Clamp<float>(currentHP / (float)maxHP, 0f, 1f);

			int headIndex = NPCID.Sets.BossHeadTextures[NPCID.GolemHead];

			DrawBar(spriteBatch, percentHP, headIndex);
		}
		else if (currentBar is MoonLordProgressBar)
		{
			NPC nPC = Main.npc[info.npcIndexToAimAt];
            HashSet<int> validIDs = new HashSet<int>
            {
                NPCID.MoonLordCore,
				NPCID.MoonLordHand,
				NPCID.MoonLordHead
			};
			NPC dummyNPC = new NPC();

			int maxHP = 0;
			dummyNPC.SetDefaults(NPCID.MoonLordCore, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax;
			dummyNPC.SetDefaults(NPCID.MoonLordHand, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax;
			dummyNPC.SetDefaults(NPCID.MoonLordHead, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax;
			maxHP += dummyNPC.lifeMax;

			float currentHP = 0f;
			var instance = new BBossBars();
			for (int i = 0; i < 200; i++)
			{
				NPC nPC2 = Main.npc[i];
				if (nPC2.active && validIDs.Contains(nPC2.type) && !instance.IsInBadAI(nPC2))
				{
					currentHP += (float)nPC2.life;
				}
			}

			float percentHP = Utils.Clamp<float>(currentHP / (float)maxHP, 0f, 1f);

			int headIndex = NPCID.Sets.BossHeadTextures[NPCID.MoonLordHead];

			DrawBar(spriteBatch, percentHP, headIndex);
		}
		else if (currentBar is PirateShipBigProgressBar)
		{
			NPC nPC = Main.npc[info.npcIndexToAimAt];

			NPC dummyNPC = new NPC();

			int maxHP = 0;
			dummyNPC.SetDefaults(NPCID.PirateShipCannon, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax * 4;
			float currentHP = 0f;
			for (int i = 0; i < 4; i++)
			{
				int num3 = (int)nPC.ai[i];
				if (Main.npc.IndexInRange(num3))
				{
					NPC nPC2 = Main.npc[num3];
					if (nPC2.active && nPC2.type == NPCID.PirateShipCannon)
					{
						currentHP += (float)nPC2.life;
					}
				}
			}
			float percentHP = Utils.Clamp<float>(currentHP / (float)maxHP, 0f, 1f);

			int headIndex = NPCID.Sets.BossHeadTextures[NPCID.PirateShip];

			DrawBar(spriteBatch, percentHP, headIndex);
		}
		else if (currentBar is MartianSaucerBigProgressBar)
		{
			NPC nPC = Main.npc[info.npcIndexToAimAt];
            HashSet<int> validIDs = new HashSet<int>
            {
                NPCID.MartianSaucerCore,
				NPCID.MartianSaucerCannon,
				NPCID.MartianSaucerTurret
			};
            NPC dummyNPC = new NPC();

			int maxHP = 0;
			if (Main.expertMode)
			{
				dummyNPC.SetDefaults(NPCID.MartianSaucerCore, nPC.GetMatchingSpawnParams());
				maxHP += dummyNPC.lifeMax;
			}
			dummyNPC.SetDefaults(NPCID.MartianSaucerCannon, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax * 2;
			dummyNPC.SetDefaults(NPCID.MartianSaucerTurret, nPC.GetMatchingSpawnParams());
			maxHP += dummyNPC.lifeMax * 2;
			float currentHP = 0f;
			for (int i = 0; i < 200; i++)
			{
				NPC nPC2 = Main.npc[i];
				if (nPC2.active && validIDs.Contains(nPC2.type) && (Main.expertMode || nPC2.type != NPCID.MartianSaucerCore))
				{
					currentHP += (float)nPC2.life;
				}
			}
			float percentHP = Utils.Clamp<float>(currentHP / (float)maxHP, 0f, 1f);

			int headIndex = NPCID.Sets.BossHeadTextures[NPCID.MartianSaucerCore];

			DrawBar(spriteBatch, percentHP, headIndex);
		}
		else if (currentBar is LunarPillarBigProgessBar)
		{
			NPC nPC = Main.npc[info.npcIndexToAimAt];

			float lifePercentToShow = Utils.Clamp<float>((float)nPC.life / (float)nPC.lifeMax, 0f, 1f);

			int maxShieldValue = NPC.LunarShieldPowerNormal;
			currentShieldValue = 0;
			if (nPC.type == NPCID.LunarTowerSolar)
			{
				currentShieldValue = NPC.ShieldStrengthTowerSolar;
			}
			if (nPC.type == NPCID.LunarTowerStardust)
			{
				currentShieldValue = NPC.ShieldStrengthTowerStardust;
			}
			if (nPC.type == NPCID.LunarTowerNebula)
			{
				currentShieldValue = NPC.ShieldStrengthTowerNebula;
			}
			if (nPC.type == NPCID.LunarTowerVortex)
			{
				currentShieldValue = NPC.ShieldStrengthTowerVortex;
			}
			float shieldPercentToShow = (float)(int)MathHelper.Clamp(currentShieldValue, 0f, maxShieldValue) / maxShieldValue;
			
			int headIndex = nPC.GetBossHeadTextureIndex();

			DrawBar(spriteBatch, lifePercentToShow, headIndex, shieldPercentToShow);
		}
		else // If the bar type is somehow out of these checks' scope, just let it draw normally.
		{
			currentBar.Draw(ref info, spriteBatch);
		}

		DrawText(currentBar, info, currentShieldValue);
	}

	/// <summary>
	/// Handles drawing BBB's boss bar for most bosses without special logic.
	/// </summary>
	public void PrepCommonBar(SpriteBatch spriteBatch, BigProgressBarInfo info)
    {
		NPC nPC = Main.npc[info.npcIndexToAimAt];
		int bossHeadTextureIndex = nPC.GetBossHeadTextureIndex();
		float percentHP = Utils.Clamp<float>((float)nPC.life / (float)nPC.lifeMax, 0f, 1f);

		int headIndex = bossHeadTextureIndex;

		DrawBar(spriteBatch, percentHP, headIndex);
	}

	/// <summary>
	/// Handles drawing BBB's boss bar for Skeletron. Can account for hands, unlike vanilla.
	/// </summary>
	public void PrepSpecialBarSkeletron(SpriteBatch spriteBatch, BigProgressBarInfo info)
	{
		NPC nPC = Main.npc[info.npcIndexToAimAt];
		int bossHeadTextureIndex = nPC.GetBossHeadTextureIndex();
		int bossHP = 0;
		int bossMaxHP = nPC.lifeMax;
		HashSet<int> validIDs = new HashSet<int>();
		validIDs.Add(NPCID.SkeletronHead);
		validIDs.Add(NPCID.SkeletronHand);
		HashSet<int> ValidIds = validIDs;

		NPC refDummy = new NPC();
		refDummy.SetDefaults(NPCID.SkeletronHand, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.SkeletronHand, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		for (int i = 0; i < 200; i++)
		{
			NPC nPC2 = Main.npc[i];
			if (nPC2.active && ValidIds.Contains(nPC2.type))
			{
				bossHP += nPC2.life;
			}
		}

		float percentHP = Utils.Clamp<float>((float)bossHP / (float)bossMaxHP, 0f, 1f);

		int headIndex = bossHeadTextureIndex;

		DrawBar(spriteBatch, percentHP, headIndex);
	}

	/// <summary>
	/// Handles drawing BBB's exclusive boss bar for Mechdusa.
	/// </summary>
	public void PrepSpecialBarMechdusa(SpriteBatch spriteBatch, BigProgressBarInfo info)
	{
		NPC nPC = Main.npc[info.npcIndexToAimAt];
		int bossHP = 0;
		int bossMaxHP = 0;
		HashSet<int> validIDs = new HashSet<int>();
		validIDs.Add(NPCID.SkeletronPrime);
		validIDs.Add(NPCID.PrimeCannon);
		validIDs.Add(NPCID.PrimeVice);
		validIDs.Add(NPCID.PrimeSaw);
		validIDs.Add(NPCID.PrimeLaser);
		validIDs.Add(NPCID.TheDestroyer);
		validIDs.Add(NPCID.Retinazer);
		validIDs.Add(NPCID.Spazmatism);
		HashSet<int> ValidIds = validIDs;

		NPC refDummy = new NPC();
		refDummy.SetDefaults(NPCID.SkeletronPrime, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.PrimeVice, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.PrimeSaw, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.PrimeLaser, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.PrimeCannon, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.TheDestroyer, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.Retinazer, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.Spazmatism, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;

		for (int i = 0; i < 200; i++)
		{
			NPC nPC2 = Main.npc[i];
			if (nPC2.active && ValidIds.Contains(nPC2.type))
			{
				bossHP += nPC2.life;
			}
		}

		float percentHP = Utils.Clamp<float>((float)bossHP / (float)bossMaxHP, 0f, 1f);

		int headIndex = NPCID.Sets.BossHeadTextures[NPCID.SkeletronPrime];

		DrawBar(spriteBatch, percentHP, headIndex);
	}

	/// <summary>
	/// Handles drawing BBB's boss bar for Skeletron Prime. Can account for hands, unlike vanilla.
	/// </summary>
	public void PrepSpecialBarPrime(SpriteBatch spriteBatch, BigProgressBarInfo info)
	{
		NPC nPC = Main.npc[info.npcIndexToAimAt];
		int bossHeadTextureIndex = nPC.GetBossHeadTextureIndex();
		int bossHP = 0;
		int bossMaxHP = nPC.lifeMax;
		HashSet<int> validIDs = new HashSet<int>();
		validIDs.Add(NPCID.SkeletronPrime);
		validIDs.Add(NPCID.PrimeCannon);
		validIDs.Add(NPCID.PrimeVice);
		validIDs.Add(NPCID.PrimeSaw);
		validIDs.Add(NPCID.PrimeLaser);
		HashSet<int> ValidIds = validIDs;

		NPC refDummy = new NPC();
		refDummy.SetDefaults(NPCID.PrimeCannon, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.PrimeVice, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.PrimeSaw, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;
		refDummy.SetDefaults(NPCID.PrimeLaser, nPC.GetMatchingSpawnParams());
		bossMaxHP += refDummy.lifeMax;

		for (int i = 0; i < 200; i++)
		{
			NPC nPC2 = Main.npc[i];
			if (nPC2.active && ValidIds.Contains(nPC2.type))
			{
				bossHP += nPC2.life;
			}
		}

		float percentHP = Utils.Clamp<float>((float)bossHP / (float)bossMaxHP, 0f, 1f);
		int headIndex = bossHeadTextureIndex;

		DrawBar(spriteBatch, percentHP, headIndex);
	}

	/// <summary>
	/// Handles drawing BBB's text display on its bossbars. This display is very similar to, but seperate from this mod's system for vanilla's bar.
	/// </summary>
	public void DrawText(IBigProgressBar currentBar, BigProgressBarInfo info, int currentShieldValue)
    {
		var instance = new BBossBars();
		if (ModContent.GetInstance<Config>().betterHPDisplay == true && currentBar is NeverValidProgressBar == false)
		{
			NPC npc = Main.npc[info.npcIndexToAimAt];
			string text = "";
			string bossName = instance.BossNamingCheck(npc);
			if (bossName != "")
			{
				if (currentShieldValue > 0) 
				{
					string shieldName = bossName + " Shield";
					text = ((!(text != "")) ? shieldName : (text + ": " + shieldName));
				}
				else
				{
					text = ((!(text != "")) ? bossName : (text + ": " + bossName));
				}
			}

			int bossLife = instance.BossHPCheck(npc);
			int bossMaxLife = instance.BossMaxHPCheck(npc);
			if (currentShieldValue > 0) 
			{
				bossLife = (int)(currentShieldValue * 100f);
				bossMaxLife = 100;
			}
			if (bossLife < 0)
			{
				bossLife = 0;
			}
			if (currentShieldValue > 0)
			{
				text = ((!(text != "")) ? (text + bossLife + "/" + bossMaxLife) : (text + ": " + bossLife + "/" + bossMaxLife + "%"));
			}
			else
			{
				text = ((!(text != "")) ? (text + bossLife + "/" + bossMaxLife) : (text + ": " + bossLife + "/" + bossMaxLife));
			}

			if (currentShieldValue <= 0 && ModContent.GetInstance<Config>().percentHPDisplay)
			{
				float percentLife = (float)bossLife / (float)bossMaxLife;
				percentLife = (float)Math.Round(100f * percentLife);

				text += $" ({percentLife}%)";
			}

			if (ModContent.GetInstance<Config>().betterHPDisplay == true)
			{
				DynamicSpriteFont font = FontAssets.MouseText.Value; 
				Vector2 scale = Vector2.One;
				scale *= 1.35f; 
				// NOTICE: This draw position is hard-coded!
				Point p = new Point(456, 22);
				Rectangle r = Utils.CenteredRectangle(Main.ScreenSize.ToVector2() * new Vector2(0.5f, 1f) + new Vector2(0f, -50f), p.ToVector2());
				Vector2 pos = r.Center();

				pos.Y += 2f; 
				Vector2 size = font.MeasureString(text); 
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, pos, Color.White, 0f, size / 2f, scale);
			}
		}

	}

	/// <summary>
	///  Handles drawing BBB's bossbar. This is adapated from vanilla's drawing code.
	/// </summary>
	public static void DrawBar(SpriteBatch spriteBatch, float lifePercent, int bossHeadIndex, float shieldPercent = 0f)
	{
		// TODO: Optimize drawing and offset code to minimize hard-code

		Texture2D bossIcon = ModContent.Request<Texture2D>(_nullBossHeadTexturePath).Value;
		if (bossHeadIndex > -1)
        {
			bossIcon = TextureAssets.NpcHeadBoss[bossHeadIndex].Value;
		}
		Rectangle barIconFrame = bossIcon.Frame();

		Asset<Texture2D> barSpritesheet = GetBarStyle();
		var texture = barSpritesheet;
		Point pointOffset = new Point(456, 22);
		Point pointOffset2 = new Point(32, 24);

		Rectangle barBGTexture = texture.Frame(1, 6, 0, 3);
		Color bgColor = Color.White * 0.75f;

		int healthBarScalar = (int)((float)pointOffset.X * lifePercent);
		healthBarScalar -= healthBarScalar % 2;
		Rectangle hpFillTexture = texture.Frame(1, 6, 0, 2);
		hpFillTexture.X += pointOffset2.X;
		hpFillTexture.Y += pointOffset2.Y;
		hpFillTexture.Width = 2;
		hpFillTexture.Height = pointOffset.Y;
		
		Rectangle shieldFillTexture = texture.Frame(1, 6, 0, 5);
		shieldFillTexture.X += pointOffset2.X;
		shieldFillTexture.Y += pointOffset2.Y;
		shieldFillTexture.Width = 2;
		shieldFillTexture.Height = pointOffset.Y;

		Rectangle bossbar = Utils.CenteredRectangle(Main.ScreenSize.ToVector2() * new Vector2(0.5f, 1f) + new Vector2(0f, -50f), pointOffset.ToVector2());
		Vector2 barDrawPosOffset = bossbar.TopLeft() - pointOffset2.ToVector2();
		float scale = 1.35f;

		Color hpFill = GetHealthColour(lifePercent, false);
		spriteBatch.Draw(texture.Value, new Vector2(barDrawPosOffset.X - 90, barDrawPosOffset.Y - 14), (Rectangle?)barBGTexture, bgColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		for (int i = 0; i < 2; i++)
		{
			spriteBatch.Draw(texture.Value, new Vector2(bossbar.TopLeft().X - 78, bossbar.TopLeft().Y - 6.25f), (Rectangle?)hpFillTexture, hpFill, 0f, Vector2.Zero, new Vector2((float)(healthBarScalar / hpFillTexture.Width), 1f) * scale, SpriteEffects.None, 0f);
		}

		if(shieldPercent > 0f)
        {
			Color shieldFill = GetHealthColour(shieldPercent, true);
			int shieldBarScalar = (int)((float)pointOffset.X * shieldPercent);
			shieldBarScalar -= shieldBarScalar % 2;
			for (int j = 0; j < 2; j++)
			{
				spriteBatch.Draw(texture.Value, new Vector2(bossbar.TopLeft().X - 78, bossbar.TopLeft().Y - 6.25f), (Rectangle?)shieldFillTexture, shieldFill, 0f, Vector2.Zero, new Vector2((float)(shieldBarScalar / shieldFillTexture.Width), 1f) * scale, SpriteEffects.None, 0f);
			}
		}
		
		Rectangle barFrameTexture = texture.Frame(1, 6);
		spriteBatch.Draw(texture.Value, new Vector2(barDrawPosOffset.X - 90, barDrawPosOffset.Y - 14), (Rectangle?)barFrameTexture, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		
		Vector2 iconOffset = new Vector2(4f, 20f) + barIconFrame.Size() / 2f;
		spriteBatch.Draw(bossIcon, new Vector2(barDrawPosOffset.X - 84, barDrawPosOffset.Y - 2) + iconOffset, (Rectangle?)barIconFrame, Color.White, 0f, barIconFrame.Size() / 2f, scale, SpriteEffects.None, 0f);
	}

	/// <summary>
	/// Selects and returns the sprite used to draw BBB's bossbars, based on config and difficulty.
	/// </summary>
	private static Asset<Texture2D> GetBarStyle()
    {
		// Used for difficulty logic when determining if to use Legendary mode style
		CreativePowers.DifficultySliderPower power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();

        switch (ModContent.GetInstance<Config>().barStyleOverride)
        {
			case "No Override (Default)":
				Asset<Texture2D> texture;
				if (Main.masterMode)
				{
					texture = ModContent.Request<Texture2D>(_masterTexturePath);
				}
				else if (Main.expertMode)
				{
					texture = ModContent.Request<Texture2D>(_expertTexturePath);
				}
				else // Classic or Journey
				{
					texture = ModContent.Request<Texture2D>(_classicTexturePath);
				}

				// FTW, GFB, and Legendary
				if (Main.getGoodWorld && ModContent.GetInstance<Config>().getGoodBars)
				{
					texture = ModContent.Request<Texture2D>(_worthyTexturePath);
				}
				if (Main.getGoodWorld
					&& (Main.GameModeInfo.IsMasterMode || (Main.GameModeInfo.IsJourneyMode && power.StrengthMultiplierToGiveNPCs >= 3f))
					&& ModContent.GetInstance<Config>().getGoodBars)
				{
					if (Main.remixWorld || !ModContent.GetInstance<Config>().getFixedBars)
					{
						texture = ModContent.Request<Texture2D>(_legendaryTexturePath);
					}
				}
				return texture;
			case "Expert":
				return ModContent.Request<Texture2D>(_expertTexturePath);
			case "Master":
				return ModContent.Request<Texture2D>(_masterTexturePath);
			case "Worthy":
				return ModContent.Request<Texture2D>(_worthyTexturePath);
			case "Legendary":
				return ModContent.Request<Texture2D>(_legendaryTexturePath);
			default:
				return ModContent.Request<Texture2D>(_classicTexturePath);
		}
	}

	/// <summary>
	/// Returns a fill color for BBB's bossbars, based on %HP, difficulty, config, and shields.
	/// </summary>
	/// <param name="lifePercent">The %HP remaining on the boss/bossbar.</param>
	/// <param name="shield">If the bar being drawn is a shield bar. Different colors are used when true.</param>
	public static Color GetHealthColour(float lifePercent, bool shield)
	{
		// Default colors
		Color barColourFull = new Color(0f, 1f, 0f);
	    Color barColourMid = new Color(1f, 1f, 0f);
	    Color barColourEmpty = new Color(1f, 0f, 0f);
		if (shield)
        {
			barColourFull = new Color(0f, 1f, 1f);
			barColourMid = new Color(0f, 0f, 1f);
			barColourEmpty = new Color(0.5f, 0f, 1f);
		}

		// Used for difficulty logic when determining if to use Legendary mode style
		CreativePowers.DifficultySliderPower power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();

		if (ModContent.GetInstance<Config>().barStyleOverride != "Classic"
			&& ModContent.GetInstance<Config>().barStyleOverride != "Expert"
			&& ModContent.GetInstance<Config>().barStyleOverride != "Master"
			&& (ModContent.GetInstance<Config>().getGoodBars
			|| ModContent.GetInstance<Config>().barStyleOverride == "Worthy"
			|| ModContent.GetInstance<Config>().barStyleOverride == "Legendary"))
        {
			// Legendary colors
			if ((Main.getGoodWorld 
				&& (Main.GameModeInfo.IsMasterMode || (Main.GameModeInfo.IsJourneyMode && power.StrengthMultiplierToGiveNPCs >= 3f)) 
				&& ModContent.GetInstance<Config>().barStyleOverride != "Worthy") 
				|| ModContent.GetInstance<Config>().barStyleOverride == "Legendary")
			{
				if (Main.remixWorld || !ModContent.GetInstance<Config>().getFixedBars 
					|| ModContent.GetInstance<Config>().barStyleOverride == "Legendary")
				{
                    if (ModContent.GetInstance<Config>().legendaryFilling)
                    {
						barColourFull = new Color(0.15f, 0.4f, 0.15f);
						barColourMid = new Color(0.6f, 1f, 0.75f);
						barColourEmpty = new Color(.9f, 0.9f, 0.9f);
						if (shield)
						{
							barColourFull = new Color(0.6f, 0.1f, 1f);
							barColourMid = new Color(0.45f, 0.2f, 0.6f);
							barColourEmpty = new Color(0f, 0.25f, 0.3f);
						}
					}
				}
			}
			// Worthy Colors
			else if (Main.getGoodWorld 
				|| ModContent.GetInstance<Config>().barStyleOverride == "Worthy")
			{
				if (ModContent.GetInstance<Config>().worthyFilling)
				{
					barColourFull = new Color(1f, 0.5f, 0f);
					barColourMid = new Color(0.75f, 0.25f, 0f);
					barColourEmpty = new Color(0.25f, 0.25f, 0.25f);
					if (shield)
					{
						barColourFull = new Color(1f, 0f, 1f);
						barColourMid = new Color(1f, 0f, 0.5f);
						barColourEmpty = new Color(1f, 0f, 0f);
					}
				}
			}
		}
		if (lifePercent > 0.5f)
		{
			return Color.Lerp(barColourMid, barColourFull, (lifePercent - 0.5f) * 2);
		}
		else
		{
			return Color.Lerp(barColourEmpty, barColourMid, lifePercent * 2);
		}
	}
}

