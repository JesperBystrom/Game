using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public Color color;
	public bool myTurn;
	public int gold;

	private Structure baseStructure;
	public List<Territory> capturedTerritories = new List<Territory>();

	private List<Unit> unitQueue = new List<Unit>();
	private int currentUnit;
	private Ability ability;
	private int goldEarnedThisRound = 0;


	public Economics economics = new Economics();

	void Start () {
		GoldTextSingleton.instance.text.text = gold.ToString();
	}

	public virtual void startTurn(){
		goldEarnedThisRound = 0;
		myTurn = true;
		//Camera.main.GetComponent<CameraPan>().panTowards(baseStructure.gameObject);
		updateGoldText(gold);
	}

	public void endTurn(){
		for(int i=0;i<Map.instance.territories.Length;i++){
			Territory t = Map.instance.territories[i];

			if(t.selected())
				t.unselect(true);

			if(t.getPlayer() != this) continue;

			if(t.hasEntity()){
				Entity e = t.getEntity();
				if(e != null){
					if(e.GetType() == typeof(Unit)){
						Unit u = (Unit)e;
						if(u.castSpellAtEndOfTurn){
							unitQueue.Add(u);
							Game.instance.delay();
							Debug.Log("Game delayed");
						}
						u.reset();
						Debug.Log("Unit reset...");
					}
					if(e.GetType() == typeof(GoldGenerator)){
						GoldGenerator g = (GoldGenerator)e;
						g.onGoldGeneration();
					}
				}
			}
		}
		Map.instance.updateMapDensity(this);
		giveGold(capturedTerritories.Count);
		economics.onRoundEnd(this);
	}

	protected virtual void Update () {
		if(unitQueue.Count > 0){
			if(ability != null){
				if(ability.finished){
					unitQueue[0].reset();
					unitQueue.Remove(unitQueue[0]);
					ability = null;
					return;
				}
			}
			if(unitQueue[0].getPreparedAbility() == null){
				foreach(Ability a in unitQueue[0].abilities){
					unitQueue[0].prepareAbility(a, true);
					ability = a;
				}
			}
		}

		if(!myTurn) return;

		//Unit selection
		if(Input.GetMouseButtonDown(Mouse.LEFT_CLICK)){
			GameObject o = Mouse.getGameObject("Territory");
			if(o != null){
				Territory t = o.GetComponent<Territory>();
				if(t.hasPlayer())
					if(!t.getPlayer().Equals(this)) return;
					
				if(t.hasEntity()){
					if(t.selected()){
						t.unselect(true);
					} else {
						foreach(Territory tt in Map.instance.territories){
							if(tt.selected()){
								tt.unselect(false);
							}
						}
						t.select();
					}
				}
			}
		}
	}

	public void setBase(Structure structure){
		this.baseStructure = structure;
	}

	public Structure getBase(){
		return this.baseStructure;
	}

	public void giveGold(int amount){
		gold += amount;
		updateGoldText(gold);
		goldEarnedThisRound += amount;
	}

	public void removeGold(int amount){
		gold -= amount;
		GoldTextSingleton.instance.onGoldLose(amount);
		updateGoldText(gold);
	}

	public void updateGoldText(int gold){
		GoldTextSingleton.instance.text.text = gold.ToString();
	}

	public List<Unit> getUnitQueue(){
		return unitQueue;
	}

	public Territory getRecentCaptured(){
		return capturedTerritories[capturedTerritories.Count-1];
	}

	public float getStrength(){
		float strength = 0;

		strength += gold / 10;
		strength += capturedTerritories.Count;
	
		capturedTerritories.ForEach(t => {
			if(t.hasEntity()){
				strength += 2;
			}
		});

		return strength / 10;
	}

	public int getAmountOfPlayersStrongerThanMe(int handicap = 2){
		int num = 0;
		foreach(Player p in Game.instance.players){
			if(p == this) continue;

			if(getStrength() > (p.getStrength()*handicap)){
				num++;
			}
		}
		return num;
	}
}
