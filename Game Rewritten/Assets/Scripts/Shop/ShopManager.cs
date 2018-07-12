using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

	public Shop shopType;
	public UI panel;
	public GameObject shopItemReference;
	public Text floatingText;
	public ShopItemData[] items;
	private Entity purchasedEntity;
	private ShopItem shopItem;

	private ShopItemData cheapestItem;

	void Start () {
		RectTransform shopItemRect = shopItemReference.GetComponent<RectTransform>();
		float shopItemWidth = shopItemRect.sizeDelta.x;
		float shopItemHeight = shopItemRect.sizeDelta.y;
		float xOffset = 15;
		float yOffset = 25;
		short itemsPerColumn = 3;

		for(int i=0;i<items.Length;i++){
			Vector3 pos = new Vector3((i%itemsPerColumn)*(shopItemWidth+xOffset), (i/itemsPerColumn)*-(shopItemHeight+yOffset), 0);
			GameObject o = UI.createInstance3D(shopItemReference, pos);
			ShopItem item = o.GetComponent<ShopItem>();
			item.setData(items[i]);
			item.getData().createModel(o);
			o.SetActive(true);

			/*GameObject o = Instantiate(shopItemReference);
			o.transform.SetParent(shopItemReference.transform.parent);
			o.GetComponent<RectTransform>().anchoredPosition3D = new Vector3((i%itemsPerColumn)*(shopItemWidth+xOffset), (i/itemsPerColumn)*-(shopItemHeight+yOffset), 0);
			o.GetComponent<RectTransform>().rotation = shopItemReference.GetComponent<RectTransform>().rotation;
			o.GetComponent<RectTransform>().localScale = Vector3.one;
			o.GetComponent<ShopItem>().setData(items[i]);
			o.SetActive(true);*/
		}
		cheapestItem = findCheapestItem();
	}

	protected virtual void Update(){
		if(purchasedEntity == null) return;

		if(Input.GetMouseButtonDown(Mouse.LEFT_CLICK)){

			if(Mouse.mouseInAir()){
				Destroy(purchasedEntity.gameObject);
				purchasedEntity = null;
				return;
			}

			Territory t = GridMover.instance.getTerritory();

			if(!Game.instance.debug)
				if(t.getPlayer() != Game.instance.getCurrentPlayer()) return;

			if(!t.hasEntity()){
				t.put(purchasedEntity, Game.instance.getCurrentPlayer());
				GridMover.instance.detachObject();
				purchasedEntity = null;
				Game.instance.getCurrentPlayer().removeGold(shopItem.getData().cost);
			}
		}
	}

	public Entity purchaseItem(ShopItemData data){
		Debug.Log("IS IT NULL: " + data);
		if(Game.instance.getCurrentPlayer().gold >= data.cost)
			return data.createItem();
		else
			return null;
	}

	public void onShopItemClick(ShopItem shopItem){
		this.purchasedEntity = purchaseItem(shopItem.getData());
		this.shopItem = shopItem;
		GridMover.instance.attachObject(purchasedEntity.gameObject);
		this.panel.toggle();
	}

	public ShopItemData getItem(Item item){
		foreach(ShopItemData i in items){
			if(i.item == item){
				return i;
			}
		}
		return null;
	}

	public ShopItemData findCheapestItem(){
		ShopItemData itemToBuy = items[0];
		foreach(ShopItemData item in items){
			if(item.cost <= itemToBuy.cost){
				itemToBuy = item;
			}
		}
		return itemToBuy;
	}

	public ShopItemData getItemInPriceRange(int gold){
		ShopItemData itemToBuy = null;

		foreach(ShopItemData item in items){

			int diff = Mathf.Abs(item.cost - gold);

			if(diff <= 0){
				itemToBuy = item;
			} 
		}
		if(itemToBuy == null)
			itemToBuy = cheapestItem;
		
		return itemToBuy;
	}

	public ShopItemData getStrongestItem(){
		ShopItemData itemToBuy = items[0];

		foreach(ShopItemData item in items){

			Ability a = item.getItemAbility(Ability.isDamageAbility);

			int finalValue = (int)(a.damage + a.getRange()/2);

			if(finalValue >= itemToBuy.getValue()){
				itemToBuy = item;
				itemToBuy.setValue(finalValue);
			}
		}
		return itemToBuy;
	}

	public ShopItemData getFlexibleItem(){
		ShopItemData flexible = items[0];

		foreach(ShopItemData item in items){

			Ability a = item.getItemAbility(Ability.isMoveAbility);

			if(a == null) continue;

			if(a.getRange() >= flexible.getValue()){
				flexible = item;
				flexible.setValue((int)a.getRange());
			}
		}
		return flexible;
	}
}

public static class AIBuyStrategy {
	public static Item getMostOptimalPurchase(AI ai, Strategy strategy, ShopManager shop){

		//Vad behöver AIen?
		//I början av spelet behöver den expandera, så den behöver något billigt med hög movement. En unit
		ShopItemData itemToBuy = null;

		Debug.Log("Strategy: " + strategy);

		switch(strategy){
		case Strategy.NEUTRAL:
			itemToBuy = shop.getFlexibleItem();
			break;

		case Strategy.HIT_AND_RUN:
			itemToBuy = shop.getItemInPriceRange(ai.gold);
			break;

		case Strategy.OFFENSIVE:
			itemToBuy = shop.getStrongestItem();
			break;

		case Strategy.DEFEND:
			itemToBuy = shop.getStrongestItem();
			break;
		}

		int percentageOfPlayersStronger = MathUtility.percentage(ai.getAmountOfPlayersStrongerThanMe(), Game.instance.players.Length);
		if(ai.gold < itemToBuy.cost) {
			//Save up?
			if(EconomicsHelper.calculateRoundsUntilPurchaseOf(itemToBuy, ai) <= 2 && percentageOfPlayersStronger < 0.5f) {
				ai.economics.saveUp(itemToBuy);
			} else {
				itemToBuy = shop.getItemInPriceRange(ai.gold);
			}
		}

		Debug.Log("ITEMTOBUY: " + itemToBuy.item);

		return itemToBuy.item;
	}

	public static Item getMostValueablePurchase(Player player){
		return Item.CHICKEN_ASSASIN;
	}
}
