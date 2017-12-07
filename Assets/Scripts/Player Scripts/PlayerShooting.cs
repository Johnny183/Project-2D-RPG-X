using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

	public GameObject specialCooldownUI;
	public Text specialTime;
	public Image specialImage;
	public Image specialBlur;

	private PlayerController playerController;
	private GameObject weapon;
	private float specialCooldown;
	private bool specialActive = false;
	private float primaryFireDelay = 0.25f;
	private float primaryFireTimer = 0f;
	
	[SerializeField]
	private int playerDamage;

	private Vector3 position;

	void Start () {
		playerController = GetComponent<PlayerController>();
		EquipmentManager.instance.onEquipmentChanged += UpdatePlayerDamage;

		UpdatePlayerDamage();
	}

	private void UpdatePlayerDamage(){
		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		int statsDamage = 0;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				statsDamage += currentEquipment[i].damageModifier;
			}
		}
		playerDamage = GameManager.instance.playerStartingDamage + statsDamage;
	}

	private void UpdatePlayer(){
		playerController.ChangeDirection();
		playerController.StartPlayerShootingCooldown();
	}

	void Update () {
		if(weapon == null){ return; }

		if(specialBlur != null){
			if(specialBlur.fillAmount != 0){
				specialBlur.fillAmount -= 1 / specialCooldown * Time.deltaTime;
			}
		}

		if(primaryFireTimer < primaryFireDelay){
			primaryFireTimer += Time.deltaTime;
		}

		if(Input.GetKeyDown(KeyCode.UpArrow)){
			playerController.facingDirection = "UP";
			UpdatePlayer();
			FirePrimary();
		}
		if(Input.GetKeyDown(KeyCode.DownArrow)){
			playerController.facingDirection = "DOWN";
			UpdatePlayer();
			FirePrimary();
		}
		if(Input.GetKeyDown(KeyCode.LeftArrow)){
			playerController.facingDirection = "LEFT";
			UpdatePlayer();
			FirePrimary();
		}
		if(Input.GetKeyDown(KeyCode.RightArrow)){
			playerController.facingDirection = "RIGHT";
			UpdatePlayer();
			FirePrimary();
		}
		if(Input.GetKeyDown(KeyCode.F)){
			playerController.StartPlayerShootingCooldown();
			FireSpecial();
		}
	}

	// Applies a new weapon reference
	public void NewWeapon(Equipment item){
		if(item.refObject != null){
			weapon = EquipmentManager.instance.currentWeapon;
			specialCooldownUI.SetActive(true);
			specialImage.transform.eulerAngles = new Vector3(0, 0, weapon.GetComponent<WeaponBaseInterface>().GetSpecialUIRotationZ());
			StartSpecialCooldown(weapon.GetComponent<WeaponBaseInterface>().GetSpecialImage(), weapon.GetComponent<WeaponBaseInterface>().GetSpecialCooldownTime());
		}
	}

	// Drops current weapon from reference
	public void LoseWeapon(){
		StopAllCoroutines();

		if(specialCooldownUI != null){
			specialCooldownUI.SetActive(false);	
		}

		weapon = null;
	}

	public void FirePrimary(){
		if(primaryFireTimer >= primaryFireDelay){
			switch(playerController.facingDirection){
				case "RIGHT":
					FacingRight();
					break;
				case "LEFT":
					FacingLeft();
					break;
				case "UP":
					FacingUp();
					break;
				default:
					FacingDown();
					break;
			}
			int randDamage = Random.Range(playerDamage-3, playerDamage);
			weapon.GetComponent<WeaponBaseInterface>().FirePrimary(gameObject, playerController.facingDirection, position, randDamage);
			primaryFireTimer = 0f;
		}
	}

	public void FireSpecial(){
		if(specialActive){
			switch(playerController.facingDirection){
				case "RIGHT":
					FacingRight();
					break;
				case "LEFT":
					FacingLeft();
					break;
				case "UP":
					FacingUp();
					break;
				default:
					FacingDown();
					break;
			}
			int randDamage = Random.Range(playerDamage-3, playerDamage);
			weapon.GetComponent<WeaponBaseInterface>().FireSpecial(gameObject, playerController.facingDirection, position, randDamage);
			StartSpecialCooldown(weapon.GetComponent<WeaponBaseInterface>().GetSpecialImage(), weapon.GetComponent<WeaponBaseInterface>().GetSpecialCooldownTime());
		}
	}

	// Setup special cooldown
	public void StartSpecialCooldown(Sprite image, float cooldownTime){
		specialActive = false;
		specialBlur.fillAmount = 1;
		specialImage.sprite = image;
		specialTime.text = "" + cooldownTime;
		specialCooldown = cooldownTime;

		StartCoroutine(SpecialCooldown(cooldownTime));
	}

	// Handles Special Cooldown
	private IEnumerator SpecialCooldown(float cooldownTime){
		while(cooldownTime > 0){
			yield return new WaitForSeconds(1);
			cooldownTime--;
			specialTime.text = "" + cooldownTime;
		}

		specialTime.text = "";
		specialBlur.fillAmount = 0;
		specialActive = true;
	}

	private void FacingRight(){
		position = new Vector3(transform.position.x + 0.65f, transform.position.y, transform.position.z);
	}

	private void FacingLeft(){
		position = new Vector3(transform.position.x - 0.65f, transform.position.y, transform.position.z);
	}

	private void FacingUp(){
		position = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
	}
	
	private void FacingDown(){
		position = new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z);
	}
}

interface WeaponBaseInterface {
	void FirePrimary(GameObject caller, string facingDirection, Vector3 position, int damage);
	void FireSpecial(GameObject caller, string facingDirection, Vector3 position, int damage);
	Sprite GetSpecialImage();
	int GetSpecialUIRotationZ();
	float GetSpecialCooldownTime();
}