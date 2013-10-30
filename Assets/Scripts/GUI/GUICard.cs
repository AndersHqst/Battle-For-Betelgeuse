using UnityEngine;
using System.Collections;

public class GUICard : MonoBehaviour
{
	public GUISkin skin = null;
	
	public Card card;
	Rect position;
	public Player Owner;
	
	public int x = Screen.width;
	public int y = Screen.height;
	public float Rotation = 0;
	float r = 0;
	int i = 0;
	
	public bool selected = false;
	void Start() {
	}
	
	public void SetCard(Card c) {
		card = c;
	}
	
	public void Played() {
		x = Screen.width/3;
		y = Screen.height/4;
		Rotation = 0;
		i = 300;
	}
	
	public bool IsMouseOver = false;
	
	public void ForcePlaceCard(int x, int y) {
		position = new Rect(x, y, 186,300);
	}
	
	public void OnGUI() {
		IsMouseOver = position.Contains(Event.current.mousePosition);
		if(card != null) {
			GUI.skin = skin;
			
			if(position.Contains(Event.current.mousePosition)) {
				position = iTween.RectUpdate(position, new Rect (x,y-50,186,300), 4);
				r = iTween.FloatUpdate(r,Rotation,1);
				GUIUtility.RotateAroundPivot(r, position.center);
				GameControl.gameControl.guiControl.setCardGui(card);
			} else {
				position = iTween.RectUpdate(position, new Rect (x,y,186,300), 4);	
				r = iTween.FloatUpdate(r,Rotation,1);	
				GUIUtility.RotateAroundPivot(r, position.center);
			}
			if(GUI.Button(position, card.Name)){
				if(!selected) {
					
					Owner.SelectCard(this);
				} else {
					Owner.DeselectCard();
					selected = false;
					y += 50;
				}
			}
		}
		if(i > 0) {
			if(i == 100) {
				x = -300;
				Rotation = 90;
			}
			if(i == 1) {
				Destroy(this);
			}
			i--;
			
		}
	}
}

