using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace BBossBars;

public class TextDisplay : GlobalBossBar
{
    public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams)
	{
		string text = "";
		var instance = new BBossBars();
		// Get the boss name
		string bossName = instance.BossNamingCheck(npc);
		if (bossName != "")
		{
			if (drawParams.Shield > 0) // Handle any shield bars (Ex: Pillars)
			{
				string shieldName = bossName + " Shield";
				text = ((!(text != "")) ? shieldName : (text + ": " + shieldName));
			}
			else
			{
				text = ((!(text != "")) ? bossName : (text + ": " + bossName));
			}
		}

		// Get HP and max HP
		int bossLife = instance.BossHPCheck(npc);
		int bossMaxLife = instance.BossMaxHPCheck(npc);
		if (drawParams.Shield > 0) // Handle any shield bars (Ex: Pillars)
		{
			bossLife = (int)(drawParams.Shield * 100f);
			bossMaxLife = 100;
		}
		if (bossLife < 0)
        {
			bossLife = 0;
        }
		if (drawParams.Shield > 0) // Displays shield life as a percentage
		{
			text = ((!(text != "")) ? (text + bossLife + "/" + bossMaxLife) : (text + ": " + bossLife + "/" + bossMaxLife + "%"));
		}
        else
        {
			text = ((!(text != "")) ? (text + bossLife + "/" + bossMaxLife) : (text + ": " + bossLife + "/" + bossMaxLife));
		}

		if(drawParams.Shield <= 0 && ModContent.GetInstance<Config>().percentHPDisplay)
        {
			float percentLife = (float)bossLife / (float)bossMaxLife;
			percentLife = (float)Math.Round(100f * percentLife);

			text += $" ({percentLife}%)";
        }

        // Draw the text display, so long as vanilla's numerical HP text display is off and the mod's config is on
        if (ModContent.GetInstance<Config>().betterHPDisplay && !BigProgressBarSystem.ShowText)
        {
			DynamicSpriteFont font = FontAssets.MouseText.Value;
			Vector2 scale = Vector2.One;
			Vector2 pos = drawParams.BarCenter; 
			pos.Y += 2f; // Nudge the draw position down a little
			Vector2 size = font.MeasureString(text); // Also used for positioning, this is more or less the length of the text string displayed
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, pos, Color.White, 0f, size / 2f, scale);
		}
	}
}
