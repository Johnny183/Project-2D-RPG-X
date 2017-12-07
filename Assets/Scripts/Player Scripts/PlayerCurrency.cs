using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerCurrency : MonoBehaviour {
	public RawImage currencyImage;
	public Text currencyText;

	private int playerCoins;

	void Start () {
		playerCoins = GameManager.instance.playerCoins;
		currencyText.color = Color.yellow;
		UpdateCoins();
	}
	
	void Update () {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.71f, transform.position.z));
		currencyImage.transform.position = screenPos;
	}

	public void AddCoins(int amount){
		playerCoins += amount;
		UpdateCoins();
	}

	public bool TakeCoins(int amount){
		int holdAmount = playerCoins - amount;
		if(holdAmount < 0) return false;

		holdAmount = playerCoins;
		UpdateCoins();
		return true;
	}

	private void UpdateCoins(){
		currencyText.text = "" + playerCoins;
		GameManager.instance.playerCoins = playerCoins;
	}
}
