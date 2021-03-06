using UnityEngine;
using System.Collections;

public class CarrierCard : UnitCard
{
	public override int Attack {
		get {
			return 5;
		}
	}

	public override int Cost {
		get {
			return 10;
		}
	}

	public override int Health {
		get {
			return 13;
		}
	}

	public override int Movement {
		get {
			return 5;
		}
	}

	public override string Name {
		get {
			return "Carrier";
		}
	}
	
	public override Faction Faction {
		get {
			return Faction.EVIL;
		}
	}

	public override string PrefabPath {
		get {
			return "Units/carrier";
		}
	}

	public override string Image {
		get {
			return "carrier";
		}
	}
	
	public CarrierCard() {
		StandardSpecials.Add(new StandardSpecial.Boost(5));
		setStandardCardText();
	}

	public override void OnPlay (StateObject s)
	{
		StandardOnPlay(s);
	}


	public override string Projectile {
		get {
			return "suiciders";
		}
	}
}

