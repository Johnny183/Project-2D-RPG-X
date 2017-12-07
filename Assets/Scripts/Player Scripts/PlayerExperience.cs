using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerExperience : MonoBehaviour {
	public Slider experienceSlider;
	public Text levelText;
	public Text levelUpText;

	private PlayerHealth playerHealth;
	private PlayerController playerController;
	private int playerLevel;
	private int playerExp;
	private int playerExpRequired;
	[HideInInspector] public bool playerLevelingUp = false;

	// Use this for initialization
	void Start () {
		playerHealth = GetComponent<PlayerHealth>();
		playerController = GetComponent<PlayerController>();

		playerLevel = GameManager.instance.playerLevel;
		playerExp = GameManager.instance.playerExp;
		playerExpRequired = playerLevel * 150;

		experienceSlider.value = playerExp;
		experienceSlider.maxValue = playerExpRequired;
		levelText.text = "" + playerLevel;
	}
	
	// Update is called once per frame
	void Update () {
		if(experienceSlider.value < experienceSlider.maxValue / 25){
			experienceSlider.fillRect.gameObject.SetActive(false);
		} else {
			experienceSlider.fillRect.gameObject.SetActive(true);
		}
		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.80f, transform.position.x));
		experienceSlider.transform.position = screenPos;

		if(playerController.isPlayerSpeaking){
			screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1.2f, transform.position.x));
		} else {
			screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.x));
		}
		levelUpText.transform.position = screenPos;
	}

	public void AddExp(int amount){
		playerExp += amount;
		if(playerExp >= playerExpRequired){
			LevelUp();
			return;
		}
		UpdatePlayerExperience();
	}

	private void LevelUp(){
		playerExp = playerExp - (playerLevel * 150);
		if(playerExp < 0) playerExp = 0;
		playerLevel++;
		playerExpRequired = playerLevel * 150;
		playerHealth.AddHealth(35);

		UpdatePlayerExperience();
		StartCoroutine(PlayerLevelUpText());
		playerLevelingUp = true;
		Invoke("StopLevelUpText", 6);
	}

	private void UpdatePlayerExperience(){
		experienceSlider.value = playerExp;
		experienceSlider.maxValue = playerExpRequired;
		levelText.text = "" + playerLevel;

		GameManager.instance.playerLevel = playerLevel;
		GameManager.instance.playerExp = playerExp;
	}

	private IEnumerator PlayerLevelUpText(){
		while(true){
			levelUpText.gameObject.SetActive(true);
			Color randomColor = new Color(Random.value, Random.value, Random.value);
			levelUpText.color = randomColor;
			yield return new WaitForSeconds(0.4f);
		}
	}

	private void StopLevelUpText(){
		StopAllCoroutines();
		playerLevelingUp = false;
		levelUpText.gameObject.SetActive(false);
	}
}
