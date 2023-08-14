using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BBossBars;

public class Config : ModConfig
{
	[Header("HeaderGeneral")]

	[BackgroundColor(65, 200, 200)]
	[DefaultValue(true)]
	public bool betterHPDisplay;

	[BackgroundColor(65, 200, 200)]
	[DefaultValue(false)]
	public bool percentHPDisplay;

	[Header("HeaderSpecials")]

	[BackgroundColor(230, 60, 110)]
	[DefaultValue(true)]
	public bool brainCreepers;

	[BackgroundColor(230, 60, 110)]
	[DefaultValue(false)]
	public bool skeletronHands;

	[BackgroundColor(230, 60, 110)]
	[DefaultValue(false)]
	public bool primeHands;

	[BackgroundColor(230, 60, 110)]
	[DefaultValue(false)]
	public bool golemHands;

	[BackgroundColor(230, 60, 110)]
	[DefaultValue(true)]
	public bool optionalPartAccounting;

	[BackgroundColor(230, 60, 110)]
	[DrawTicks]
	[OptionStrings(new string[] { "Cumulative Text", "Singular Text", "Cumulative Text & HP", "Seperate", "Vanilla (Default)" })]
	[DefaultValue("Vanilla (Default)")]
	public string twinsDisplay;

	[BackgroundColor(230, 60, 110)]
	[DrawTicks]
	[OptionStrings(new string[] { "Naming Only (Default)", "Name & HP", "Vanilla" })]
	[DefaultValue("Naming Only (Default)")]
	public string mechDisplay;

	[Header("HeaderAppearance")]

	[BackgroundColor(30, 200, 80)]
	[DrawTicks]
	[OptionStrings(new string[] { "No Override (Default)", "Classic", "Expert", "Master", "Worthy", "Legendary" })]
	[DefaultValue("No Override (Default)")]
	public string barStyleOverride;

	[BackgroundColor(30, 200, 80)]
	[DefaultValue(true)]
	public bool getGoodBars;

	[BackgroundColor(30, 200, 80)]
	[DefaultValue(false)]
	public bool getFixedBars;

	[BackgroundColor(30, 200, 80)]
	[DefaultValue(true)]
	public bool worthyFilling;

	[BackgroundColor(30, 200, 80)]
	[DefaultValue(true)]
	public bool legendaryFilling;

	public override ConfigScope Mode => ConfigScope.ClientSide;
}
