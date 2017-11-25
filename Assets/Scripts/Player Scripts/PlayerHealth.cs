using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
	public Slider healthSlider;
	public Image damageImage;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
	
	private SpriteRenderer spriteRenderer;
	private int playerHealth;
	private int playerMaxHealth;
	private bool damaged = false;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		playerHealth = GameManager.instance.playerStartingHealth;
		playerMaxHealth = playerHealth + (GameManager.instance.playerLevel * 10);
		playerHealth = playerMaxHealth;
		UpdatePlayerHealth();
	}

	void Update() {
		if(damaged){
			damageImage.color = flashColour;
			spriteRenderer.color = Color.red;
		} else {
			damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, flashSpeed * Time.deltaTime);
		}
		damaged = false;

		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.x));
		healthSlider.transform.position = screenPos;
	}

	public void TakeDamage(int amount){
		playerHealth -= amount;
		if(playerHealth <= 0){
			PlayerDeath();
			return;
		}
		damaged = true;
		UpdatePlayerHealth();
	}

	public void AddHealth(int amount){
		playerHealth += amount;
		if(playerHealth > playerMaxHealth){
			playerHealth = playerMaxHealth;
		}
		UpdatePlayerHealth();
	}
	
	private void UpdatePlayerHealth(){
		healthSlider.maxValue = playerMaxHealth;
		healthSlider.value = playerHealth;
	}

	private void PlayerDeath(){
		GameManager.instance.OnPlayerDeath(gameObject);
		GameManager.instance.LoadGameScene("Level1");
	}
}
