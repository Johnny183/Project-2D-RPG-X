using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {
	[SerializeField]
	private int playerHealth;

	public Slider healthSlider;
	public Image damageImage;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
	
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private PlayerController playerController;

	private int playerMaxHealth;
	private bool damaged = false;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		playerController = GetComponent<PlayerController>();

		EquipmentManager.instance.onEquipmentChanged += UpdateStatsHealth;
		UpdateStatsHealth();
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

	public void UpdateStatsHealth(){
		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		int statsHealth = 0;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				statsHealth += currentEquipment[i].armorModifier;
			}
		}
		playerMaxHealth = GameManager.instance.playerStartingHealth + statsHealth;
		
		if(playerMaxHealth <= playerHealth){
			playerHealth = playerMaxHealth;
		}

		UpdatePlayerHealth();
	}


	public void TakeDamage(int amount){
		if(playerHealth != 0){
			playerHealth -= amount;

			if(playerHealth <= 0){
				playerHealth = 0;
				// Player Death animation
				PlayerDeath();
			} else {
				animator.SetTrigger("PlayerHit");
			}

			damaged = true;
			UpdatePlayerHealth();
		}
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
		playerController.isPlayerDead = true;
		UpdatePlayerHealth();
		GameManager.instance.LoadGameScene("Level1");
	}
}
