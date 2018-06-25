using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
	public bool debug;
	public static Game instance;
	public GameObject endTurnButton;
	public Structure mainBasePrefab;
	public Entity test;
	public int numOfPlayers;
	public GameObject playerPrefab;
	public GameObject aiPrefab;
	public Player[] players;
	private int currentPlayer;
	private OutlinePulsator pulsator;
	private bool delayed;

	[System.Serializable]
	public struct PlayerData {
		public Color teamColor;
		public Territory spawnPosition;
		public bool ai;
	}
	[Header("Game Configurations")]
	 public PlayerData[] playerData;

	void Start(){
		instance = this;
		players = new Player[numOfPlayers];
		for(int i=0;i<numOfPlayers;i++){

			GameObject prefab = playerPrefab;

			if(playerData[i].ai) {
				prefab = aiPrefab;
			}

			GameObject o = Instantiate(prefab);
			players[i] = o.GetComponent<Player>();
			players[i].color = playerData[i].teamColor;


			Structure b = Instantiate(mainBasePrefab.gameObject).GetComponent<Structure>();
			Territory t = playerData[i].spawnPosition;
			b.transform.position = new Vector3(t.transform.position.x, b.transform.position.y, t.transform.position.z);
			players[i].setBase(b);
			Debug.Log(t.transform.position.x + ", " + t.transform.position.z);
			Map.instance.findTerritory((int)t.transform.position.x, (int)t.transform.position.z).put(b, players[i]);
			//b.transform.position = new Vector3(t.transform.position.x, b.transform.position.y, t.transform.position.z);
		}
		players[currentPlayer].startTurn();
		//Map.instance.findTerritory(0,1).put(test, players[currentPlayer]);
		Camera.main.GetComponent<CameraPan>().panTowards(players[currentPlayer].getBase().gameObject);
		pulsator = endTurnButton.GetComponent<OutlinePulsator>();
	}

	public void endTurnButtonPress(){
		if(getCurrentPlayer().GetType() != typeof(AI)){
			endTurn();
		}
	}

	public void endTurn(){
		if(!delayed)
			players[currentPlayer].endTurn();
		
		if(delayed){
			StartCoroutine(endTurnDelay());
			return;
		}

		forceEnd();
	}

	private void forceEnd(){
		players[currentPlayer].myTurn = false;
		currentPlayer++;
		if(currentPlayer >= players.Length)
			currentPlayer = 0;
		players[currentPlayer].startTurn();
		pulsator.stopPulsating();
		delayed = false;
	}

	public void delay(){
		delayed = true;
	}

	public IEnumerator endTurnDelay(){
		while(players[currentPlayer].getUnitQueue().Count > 0){
			yield return null;
		}
		forceEnd();
	}

	////////////////////////////////////////////////////////////TODO: Green pulsating END TURN button/////////////////////////////////////////////////////////////////////////////

	/*public int getActionsLeft(){
		int actions = 0;
		foreach(Territory t in getCurrentPlayer().capturedTerritories){
			Entity e = t.getEntity();
			if(e != null){
				if(e.GetType() == typeof(Unit)){
					Unit u = (Unit)e;
					if(u.uses > 0){
						actions += u.uses;
					}
				}
			}
		}
		return actions;
	}

	public void checkActionsLeft(){
		if(getActionsLeft() <= 0){
			pulsator.pulsate(5);
		} else {
			pulsator.stopPulsating();
		}
	}*/

	public Player getCurrentPlayer(){
		return players[currentPlayer];
	}

	public Player[] getPlayers(){
		return players;
	}
}
