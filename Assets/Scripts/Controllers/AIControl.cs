using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AIControl : MonoBehaviour
{
	Player player;
	GameControl gameControl;
	Dictionary<Unit, List<Hex>> unitMoves = new Dictionary<Unit, List<Hex>>();
	Dictionary<Unit, List<Hex>> unitBlacklist = new Dictionary<Unit, List<Hex>>();	
	AIState aistate = AIState.ENEMYTURN;
	List<List<Card>> cardOptions = new List<List<Card>>();
	List<Card> chosenHand = new List<Card>();
	
	public void DoTurn() {
	}
	
	/// <summary>
	/// Calculates the hex value.
	/// </summary>
	/// <returns>
	/// The hex value.
	/// </returns>
	/// <param name='hex'>
	/// Hex.
	/// </param>
	int CalculateHexValue(Hex hex) {
		if(hex.Unit != null) {
			if(hex.Unit.Team == player.Team || hex.Unit.Team == 0) {
				return int.MinValue;
			} else {
				return int.MaxValue - hex.Unit.Attack;
			}
		} else {
			return 10000 - Mathf.FloorToInt(hex.Distance(gameControl.thisPlayer.Base.Hex));
		}
	}
	
	/// <summary>
	/// Sets AI.
	/// </summary>
	/// <returns>
	/// The AIControl for the player.
	/// </returns>
	/// <param name='player'>
	/// Player.
	/// </param>
	/// <param name='gameControl'>
	/// Game control.
	/// </param>
	public AIControl SetAI(Player player, GameControl gameControl) {
		player.Ai = true;
		this.player = player;
		this.gameControl = gameControl;
		return this;
	}
	
	
	bool MyTurn() {
		return gameControl.state == State.ENEMYTURN;
	}
	
	bool NoMovesInProgress() {
		return iTween.tweens.Count < 1;
	}
	
	List<Unit> MyUnits() {
		return gameControl.units.FindAll(u => u.Team == player.Team);
	}
	
	void getMoves(Hex hex, int i, List<Hex> acc) {
		if(i < 1) {
			return;
		} else {
			List<Hex> adj = hex.Adjacent(gameControl.gridControl.Map).FindAll(h => !acc.Contains(h) && (h.Unit == null || (h.Unit.Team != 0 && h.Unit.Team != player.Team)));
			acc.AddRange(adj);
			adj.ForEach(h => getMoves(h, --i, acc));
		}
	}
	
	List<List<Card>> CardPlayCombinations() {		
		List<List<Card>> result = new List<List<Card>>();
		foreach(List<Card> l in PathFinder.GetPowerSet<Card>(player.Hand).ToList().ConvertAll<List<Card>>(ie => ie.ToList())) {
			int i = 0;
			l.ForEach(c => i += c.Cost);
			if(i <= player.ManaLeft()) {
				result.Add(l);
			}
		}
		return result;		
	}
	
	bool PlayCard(Card card) {
		// TODO When adding different card types, improve this.
		// Choose a card
		if(card == null) { return false; }
		if(typeof(SpellCard).IsAssignableFrom(card.GetType())) { return PlaySpellCard((SpellCard) card); }
		
		// Choose a tile
		Hex targetHex = null;
		
		player.SetTargetsForCard(card);
		if(player.targets.Count > 1) {
			targetHex = player.targets[Random.Range(0, player.targets.Count-1)];
		} else if(player.targets.Count == 1) {
			targetHex = player.targets[0];
		} else {
			return false;
		}
		// Play card
		gameControl.EnemyCardPlayed(card);
		Unit unit = gameControl.PlayCardOnHex(card, targetHex, System.Guid.NewGuid().ToString());
		player.SpendMana(card.Cost);
		player.Hand.Remove(card);
		unitMoves[unit] = StandardList();
		unitBlacklist[unit] = new List<Hex>();
		return true;
	}
	
	bool PlaySpellCard(SpellCard card) {
		player.SetTargetsForCard(card);
		Hex targetHex = null;
		foreach(Hex hex in player.targets) {
			if(targetHex == null || SpellOnHexValue(card, hex) > SpellOnHexValue(card, targetHex)) {
				targetHex = hex;
			}
		}
		if(targetHex == null) { return false; }
		gameControl.EnemyCardPlayed(card);
		gameControl.PlayCardOnHex(card, targetHex, System.Guid.NewGuid().ToString());
		player.SpendMana(card.Cost);
		player.Hand.Remove(card);
		return true;
	}
		
	bool MoveUnits() {
		i = 100;
		// Find a unit
        List<Unit> unitsWithMoves = MyUnits().FindAll(u => u.MovementLeft() > 0);
        Unit unit = null;
        if(unitsWithMoves.Count > 1) {
                unit = unitsWithMoves[Random.Range(0, unitsWithMoves.Count-1)];
        } else if(unitsWithMoves.Count == 1) {
                unit = unitsWithMoves[0];
        } else {
                return false;
        }
        // Move unit
        Hex targetHex = null;
        unitMoves[unit] = SortHexList(unitMoves[unit]);
		if(unitMoves[unit].Count > 0) {
			int j = 0;
			try {
				while(PathFinder.DepthFirstSearch(unit.Hex, unitMoves[unit][j], gameControl.gridControl.Map, unit.MovementLeft()).Count < 1) {
					j++;
				}
			} catch {
				return false;
			}
			targetHex = unitMoves[unit][j];
		} else {
			return false;
		}
		unitBlacklist[unit] = new List<Hex>();
		unitMoves[unit] = StandardList();
        unit.PrepareMove(targetHex);
        return true;
	}
	
	
	public void EndTurn() {
		aistate = AIState.ENEMYTURN;
		gameControl.EnemeyEndTurn();
	}
	
	List<Hex> SortHexList(List<Hex> hexs) {
		IEnumerable<Hex> k = from h in hexs
				orderby CalculateHexValue(h) descending
				select h;
		return new List<Hex>(k);
	}
	
	List<Hex> StandardList() {
		List<Hex> returnList = new List<Hex>();
		foreach(List<Hex> row in gameControl.gridControl.Map) {
			foreach(Hex h in row) {
				if(h != null) {
					returnList.Add(h);
				}
			}
		}
		return SortHexList(returnList);
	}
	
	int UnitCardValue(EntityCard card) {
		return card.Attack + card.Health/2 + card.Movement/2 + card.StandardSpecials.Count*2;
	}
	
	int SpellOnHexValue(SpellCard sCard, Hex hex) {
		if(hex.Unit != null) {
			MockUnit mo = new MockUnit(hex.Unit);
			int originValue = mo.Value();
			mo = sCard.MockOnPlay(mo);
			int newValue = mo.Value();
			if(mo.Team == player.Team) {
				return ((originValue - newValue) * -1);
			} else {
				return ((originValue - newValue));
			}
		}
		return -1;
	}
	
	int ValuateCard(Card card) {
		int v = 0;
		if(typeof(UnitCard).IsAssignableFrom(card.GetType())) {
			v = UnitCardValue((UnitCard) card);
		} else if(typeof(BuildingCard).IsAssignableFrom(card.GetType())) {
			// TODO Find a better way of calculating buildingvalues
			v = UnitCardValue((BuildingCard) card);
		} else if(typeof(SpellCard).IsAssignableFrom(card.GetType())) {
			SpellCard sCard = (SpellCard) card;
			player.SetTargetsForCard(card);
			foreach(Hex hex in player.targets) {
				int k = SpellOnHexValue(sCard, hex);
				v = k > v ? k : v;
			}
			player.targets = new List<Hex>();
		} 
		return v;
	}
	
	int ValulateCardCombOption(List<Card> cards) {
		int v = 0;
		cards.ForEach(c => v += c.Cost);
		return v;
	}
	
	int i = 0;
	
	void DoMove() {
		switch(aistate) {
		case AIState.ENEMYTURN: if(NoMovesInProgress()) { aistate++; } break;
		case AIState.BUILDINGHAND: cardOptions = CardPlayCombinations(); i = cardOptions.Count; aistate++; break;
		case AIState.ANALYZINGCARDOPTIONS: 
			if(i < 1) { 
				aistate++; 
			} else { 
				if(ValulateCardCombOption(cardOptions[i-1]) > ValulateCardCombOption(chosenHand)) {
					chosenHand = cardOptions[i-1];
				}
				i--; 
			} 
			break;
		case AIState.PLAYINGCARDS: 
			
			if(chosenHand.Count < 1) {
				aistate++; 
			} else {
				if(i == 0) {
					PlayCard (chosenHand.First());	
					chosenHand.Remove(chosenHand.First());
					i = 100;
				} else {
					i--;
				}
			}
			break;
		case AIState.ANALYZINGBOARD: aistate++; break;
		case AIState.MOVINGUNITS: if(NoMovesInProgress() && !MoveUnits()) { aistate++; } else {  } break;
		case AIState.DONE: EndTurn(); break;
		default: break;
		}
	}
	
	int antiStuck = 0;
	void Update() {
		if(iTween.tweens.Count == 1 && MyTurn()) {
			antiStuck++;
			if(antiStuck > 1000) {
				iTween.Stop();
				EndTurn();
				antiStuck = 0;
			}
		} else {
			antiStuck = 0;
		}
		if(MyTurn() && NoMovesInProgress()) {
			DoMove();
		}
	}
	
	enum AIState { ENEMYTURN, BUILDINGHAND, ANALYZINGCARDOPTIONS, PLAYINGCARDS, ANALYZINGBOARD, MOVINGUNITS, DONE }
}



