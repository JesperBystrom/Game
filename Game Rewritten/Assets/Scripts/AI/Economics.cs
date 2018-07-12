using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EconomicStrategies {
	SAVE_UP
}

public class Economics {

	private ShopItemData item;
	private List<int> goldPerRound = new List<int>();

	public Economics(){
		goldPerRound.Add(0);
	}

	public void saveUp(ShopItemData item){
		this.item = item;
	}

	public void onRoundEnd(Player player){
		goldPerRound.Add(player.capturedTerritories.Count);
	}

	public int getAveregeGoldPerRound(){
		int total = 0;
		for(int i=0;i<goldPerRound.Count;i++){
			total += goldPerRound[i];
		}
		Debug.Log("total: " + total + ", " + goldPerRound.Count);
		return total / goldPerRound.Count;
	}
}

public static class EconomicsHelper {
	public static int calculateRoundsUntilPurchaseOf(ShopItemData item, Player player){
		return Mathf.RoundToInt(item.cost / (player.gold + player.economics.getAveregeGoldPerRound()));
	}
}
