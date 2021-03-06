using UnityEngine;
using System.Collections.Generic;

public class IntergalacticPoliticsCard : SpellCard {

	int amount = 75;

	public override int Cost {
		get {
			return 9;
		}
	}
	
	public override string Name {
		get {
			return "Intergalactic Diplomacy";
		}
	}
	
	public override Faction Faction {
		get {
			return Faction.GOOD;
		}
	}
	
	public override List<Hex> Targets (StateObject s)
	{
		return new List<Hex>();
	}
	
	public IntergalacticPoliticsCard() {
		CardText += "Grants all players " + amount + " victory points.";
	}
	
	public override bool IsTargetless {
		get {
			return true;
		}
	}
	
	public override string Image {
		get {
			return "diplomacy";
		}
	}
	
	public override void SpellEffect (StateObject s)
	{
		s.Caster.Points += amount;
		s.Opponent.Points += amount;
	}
}


