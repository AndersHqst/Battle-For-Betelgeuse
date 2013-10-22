using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hex : MonoBehaviour
{
	public Vector2 GridPosition;
	public Unit Unit { get; set; }
	public bool Movable { get; set; }
	public bool IsSelected { get; set; }
	
	void Start ()
	{
	}
		
	void Update ()
	{
		if(IsSelected) {
			renderer.material.color = Color.blue;
		} else if(Unit != null) {
			if(Unit.Team == 1) {
				renderer.material.color = Color.green;
			} else {
				renderer.material.color = Color.red;
			}
		}
	}
	
	public List<Hex> Adjacent(List<List<Hex>> map) {
		List<Hex> resultList = new List<Hex>();
		int x = Mathf.FloorToInt(GridPosition.x);
		int y = Mathf.FloorToInt(GridPosition.y);
		AddHex(x+1,y, resultList, map);
		AddHex(x-1,y, resultList, map);
		x = (y % 2 == 0) ? x : x + 1;
		AddHex(x-1, y-1, resultList, map);
		AddHex(x-1,y+1, resultList, map);
		AddHex(x,y-1, resultList, map);
		AddHex(x,y+1, resultList, map);
		return resultList;
	}
	
	List<Hex> AddHex(int x, int y, List<Hex> list, List<List<Hex>> map) {
		try {
			Hex hex = map[x][y];
			if(hex != null) {
				list.Add(hex);
			} 
		} catch {
		}
		return list;
	}
	
}

