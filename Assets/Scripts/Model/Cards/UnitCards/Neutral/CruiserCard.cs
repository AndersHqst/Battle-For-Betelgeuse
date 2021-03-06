using UnityEngine;
using System.Collections;

public class CruiserCard : UnitCard
{
	public override int Attack {
		get {
			return 3;
		}
	}

	public override int Cost {
		get {
			return 3;
		}
	}

	public override int Health {
		get {
			return 3;
		}
	}

	public override int Movement {
		get {
			return 5;
		}
	}

	public override string Name {
		get {
			return "Star Cruiser";
		}
	}

	public override string PrefabPath {
		get {
			return "Units/star_cruiser";
		}
	}

	public override string Image {
		get {
			return "cruiser";
		}
	}
	
	public CruiserCard() {
		StandardSpecials.Add(new StandardSpecial.Ranged());
		setStandardCardText();
	}

	public override void OnPlay (StateObject s)
	{
		StandardOnPlay(s);
		return;
	}
	
	public override string Projectile {
		get {
			return "missiles";
		}
	}
}

