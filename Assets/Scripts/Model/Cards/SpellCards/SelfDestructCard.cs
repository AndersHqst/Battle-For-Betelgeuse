using UnityEngine;
using System.Collections;

public class SelfDestructCard : SpellCard
{
	public override int Cost {
		get {
			return 4;
		}
	}

	public override int id {
		get {
			return 17;
		}
	}

	public override string Name {
		get {
			return "Self-Destruct";
		}
	}

	public override System.Collections.Generic.List<Hex> Targets (StateObject s)
	{
		return s.Units.FindAll(u => u.Team == s.Caster.Team).ConvertAll<Hex>(u => u.Hex);
	}
	
	public SelfDestructCard() {
		CardText += "Blows up the unit dealing damage equal to its remaining health to all adjacent units.";
	}

	public override void SpellEffect (StateObject s)
	{
		int damage = s.TargetUnit.CurrentHealth();
		foreach(Hex h in s.TargetUnit.Hex.Adjacent(GameControl.gameControl.gridControl.Map)) {
			if(h.Unit != null) {
				h.Unit.Damage(damage);
			}
		}
		
		s.TargetUnit.Damage(int.MaxValue);	
	}
}

