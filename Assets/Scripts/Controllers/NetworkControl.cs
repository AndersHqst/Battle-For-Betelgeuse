using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameControl))]
public class NetworkControl : Photon.MonoBehaviour {
	
	GameControl gameControl;
	PhotonView NetworkPhoton;
	
	// Use this for initialization
	void Start () {
		if(GameControl.IsMulti) {
			PhotonNetwork.ConnectUsingSettings(Settings.version);
			gameControl = GetComponent<GameControl>();
			NetworkPhoton = GetComponent<PhotonView>();
		}
	}
	
	public void QuitGame() {
		if(PhotonNetwork.connected) {
			PhotonNetwork.Disconnect();
		}
	}
	
	void OnJoinedLobby() {
		PhotonNetwork.JoinRandomRoom();
	}
	
	void OnPhotonRandomJoinFailed() {
		PhotonNetwork.CreateRoom(null);
	}
	
	void OnJoinedRoom() {
		if(PhotonNetwork.isNonMasterClientInRoom) {
			NetworkPhoton.RPC ("Joined", PhotonTargets.MasterClient, null);
		}
	}
	
	void OnCreatedRoom() {	
		gameControl.GuiControl.ShowSplashText("Waiting for an opponent...");
		PhotonNetwork.room.open = true;
	}
	
	[RPC]
	void Joined() {
		gameControl.GuiControl.ShowSplashText("Opponent found!");
		PhotonNetwork.room.open = false;
	}
	
	public void DeckChosen() {
		if(PhotonNetwork.isNonMasterClientInRoom) {
			NetworkPhoton.RPC ("OpponentDeckChosen", PhotonTargets.MasterClient, null);
			gameControl.SetUpClientGame();
		}
	}
	
	[RPC]
	void OpponentDeckChosen() {
		gameControl.SetUpMasterGame();
	}

	
	public void MoveNetworkUnit(Unit unit, Hex hex) {
		System.Object[] args = new System.Object[3];
		args[0] = unit.Id.ToString();
		args[1] = Mathf.FloorToInt(hex.GridPosition.x);
		args[2] = Mathf.FloorToInt(hex.GridPosition.y);
		NetworkPhoton.RPC("ReceiveNetworkUnitMove", PhotonTargets.All, args);	
	}
	
	[RPC]
	public void ReceiveNetworkUnitMove(string id, int x, int y) {
		Unit unit = gameControl.Units.Find(u => u.Id.ToString() == id);
		Hex hex = gameControl.GridControl.Map[x][y];
		unit.PrepareMove(hex);
	}
	
	public void PlayNetworkCardOn(Card card, Hex hex) {
		System.Object[] args = new System.Object[4];
		args[0] = card.Name;
		args[1] = Mathf.FloorToInt(hex.GridPosition.x);
		args[2] = Mathf.FloorToInt(hex.GridPosition.y);
		args[3] = System.Guid.NewGuid().ToString();
		NetworkPhoton.RPC("ReceiveNetworkCard", PhotonTargets.All, args);
		NetworkPhoton.RPC("ReceiveOpponentPlayedCard", PhotonTargets.Others, args);
	}
	
	[RPC]
	public void ReceiveNetworkCard(string name, int x, int y, string guid) {
		Card card = (Card) Card.cardTable[name];
		Hex hex = gameControl.GridControl.Map[x][y];
		gameControl.PlayCardOnHex(card, hex, guid);
	}
	
	[RPC]
	public void ReceiveOpponentPlayedCard(string name, int x, int y, string guid) {
		Card card = (Card) Card.cardTable[name];
		gameControl.EnemyCardPlayed(card);
	}
	
	public void StartNetworkGame() {
		if(PhotonNetwork.isMasterClient) {
			NetworkPhoton.RPC("ReceiveStartGame", PhotonTargets.All, null);
		}
	}
	
	[RPC]
	public void ReceiveStartGame() {
		gameControl.StartGame();
	}
	
	public void EndNetworkTurn() {
		NetworkPhoton.RPC("ReceiverEndNetworkTurn", PhotonTargets.Others, null);
	}
	
	[RPC]
	public void ReceiverEndNetworkTurn() {
		gameControl.EnemyEndTurn();
	}
}
