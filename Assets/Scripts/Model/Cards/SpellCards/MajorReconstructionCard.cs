﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MajorReconstructionCard : SpellCard {
	

	public override int Cost {
		get {
			return 4;
		}
	}


	public override int id {
		get {
			return 8;
		}
	}


	public override string Name {
		get {
			return "Major Reconstruction";
		}
	}

	public override MockUnit MockOnPlay (MockUnit mo)
	{
		if(mo.CurrentHealth < mo.MaxHealth) {
			if(mo.MaxHealth - mo.CurrentHealth < 5) {
				mo.CurrentHealth = mo.MaxHealth;
			} else {
				mo.CurrentHealth += 5;
			}
		}
		return mo;
	}
	
	public override List<Hex> Targets (StateObject s)
	{
		List<Hex> result = s.Units.FindAll(u => u.Team == s.Caster.Team).ConvertAll<Hex>(u => u.Hex);
		return result;
	}
	
	public MajorReconstructionCard() {
		CardText += "Heals target unit or building for five.";
	}

	public override void SpellEffect (StateObject s)
	{
		
		if(s.TargetUnit.CurrentHealth() < s.TargetUnit.MaxHealth) {
			if(s.TargetUnit.MaxHealth - s.TargetUnit.CurrentHealth() < 5) {
				s.TargetUnit.Damage(-(s.TargetUnit.MaxHealth - s.TargetUnit.CurrentHealth()));
			} else {
				s.TargetUnit.Damage(-5);
			}
		}
	}
}
