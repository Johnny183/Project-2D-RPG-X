using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {
	private PlayerController playerController;
	private GameObject weapon;

	private GameObject specialCooldown;
	
	[SerializeField]
	private int playerDamage;

	private Vector3 position;

	void Start () {
		playerController = GetComponent<PlayerController>();
		EquipmentManager.instance.onEquipmentChanged += FindWeapon;

		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		int statsDamage = 0;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				statsDamage += currentEquipment[i].damageModifier;
			}
		}
		playerDamage = GameManager.instance.playerStartingDamage + statsDamage;
		FindWeapon();
	}

	private void UpdatePlayer(){
		playerController.ChangeDirection();
		playerController.StartPlayerShootingCooldown();
	}

	void Update () {
		if(weapon == null){ return; }

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

	// Could be optimized as this is getting called every update frame
	public void FindWeapon(){
		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				if(currentEquipment[i].equipSlot == EquipmentSlot.Weapon && currentEquipment[i].refObject != null){
					weapon = currentEquipment[i].refObject;
					specialCooldown = GameObject.FindGameObjectWithTag("SpecialCooldown");
					specialCooldown.SetActive(true);
				}
			}
		}

		if(specialCooldown != null){
			specialCooldown.SetActive(false);	
		}

		specialCooldown = null;
		weapon = null;
	}

	public void FirePrimary(){
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
		int randDamage = Random.Range(playerDamage-3, playerDamage+3);
		weapon.GetComponent<WeaponBaseInterface>().FirePrimary(gameObject, playerController.facingDirection, position, randDamage);
	}

	public void FireSpecial(){
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
		int randDamage = Random.Range(playerDamage-3, playerDamage+3);
		weapon.GetComponent<WeaponBaseInterface>().FireSpecial(gameObject, playerController.facingDirection, position, randDamage);
	}

	// Setup special cooldown stuff here like cooldown time and special image provided by specific weapon
	public void StartSpecialCooldown(Sprite specialImage, float cooldownTime){

	}

	// Handle cooldown here
	private IEnumerator SpecialCooldown(float cooldownTime){
		while(cooldownTime > 0){
			yield return new WaitForSeconds(1);

			// Handle delay time code here

			cooldownTime--;
		}

		// Handle code once timer has ended
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
}