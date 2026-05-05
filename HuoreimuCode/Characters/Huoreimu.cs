using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using AyaMod.Huoreimu.Cards;
using AyaMod.Huoreimu.Extensions;
using System;
using AyaMod.Huoreimu.Relic;

namespace AyaMod.Huoreimu.Character;

public class Huoreimu : PlaceholderCharacterModel
{
	public const string CharacterId = "Huoreimu";

	public override string CustomCharacterSelectBg => "res://Huoreimu/Scenes/Huoreimu_bg.tscn";

	public override string PlaceholderID => "necrobinder";

	public static readonly Color Color = new Color("666666");

	public override Color NameColor => Color;
	public override CharacterGender Gender => CharacterGender.Feminine;
	public override int StartingHp => 66;

	public override IEnumerable<CardModel> StartingDeck => [
		ModelDb.Card<HuoreimuAttack>(),
		ModelDb.Card<HuoreimuAttack>(),
		ModelDb.Card<HuoreimuAttack>(),
		ModelDb.Card<HuoreimuAttack>(),
		ModelDb.Card<HuoreimuBlock>(),
		ModelDb.Card<HuoreimuBlock>(),
		ModelDb.Card<HuoreimuBlock>(),
		ModelDb.Card<HuoreimuBlock>(),
		ModelDb.Card<HeadbuttAttack>(),
		ModelDb.Card<SkywardKick>(),
	];

	public override IReadOnlyList<RelicModel> StartingRelics => new List<RelicModel> { ModelDb.Relic<StarOfDeath>() }.AsReadOnly();

	public override CardPoolModel CardPool => ModelDb.CardPool<HuoreimuCardPool>();
	public override RelicPoolModel RelicPool => ModelDb.RelicPool<HuoreimuRelicPool>();
	public override PotionPoolModel PotionPool => ModelDb.PotionPool<SharedPotionPool>();

	/*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
		override all the other methods that define those assets.
		These are just some of the simplest assets, given some placeholders to differentiate your character with.
		You don't have to, but you're suggested to rename these images. */
	public override string CustomVisualPath => "res://Huoreimu/Scenes/HuoreimuVisual.tscn";
	public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
	public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
	public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
	public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
