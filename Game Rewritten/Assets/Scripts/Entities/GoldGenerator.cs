using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldGenerator : Structure {
	
	public int goldPerTurn;

	public void onGoldGeneration(){
		Game.instance.getCurrentPlayer().giveGold(goldPerTurn);
		TextFactory.getInstance().createMovingText(damageTextPrefab, transform.position, goldPerTurn, TextType.GOLD, true);
	}
}
