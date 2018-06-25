using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Entity {
	
	public void onGoldGeneration(int amount){
		Game.instance.getCurrentPlayer().giveGold(amount);
	}

	public override void setTerritory(Territory territory){
		base.setTerritory(territory);
		Player p = territory.getPlayer();
		if(p != null)
			territory.captureNeighbours(p);
	}
}
