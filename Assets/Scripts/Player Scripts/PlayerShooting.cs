using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {
	private PlayerController playerController;
	private GameObject weapon;
	
	[SerializeField]
	private int playerDamage;

	private Vector3 position;

	void Start () {
		playerController = GetComponent<PlayerController>();

		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		int statsDamage = 0;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				statsDamage += currentEquipment[i].damageModifier;
			}
		}
		playerDamage = GameManager.instance.playerStartingDamage + statsDamage;
	}

	void Update () {
		if(!FindWeapon()){ return;}

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

	public bool FindWeapon(){
		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				if(currentEquipment[i].equipSlot == EquipmentSlot.Weapon && currentEquipment[i].refObject != null){
					weapon = currentEquipment[i].refObject;
					GameObject.FindGameObjectWithTag("SpecialCooldown").SetActive(true);
					return true;
				}
			}
		}
		return false;
	}

	private void UpdatePlayer(){
		playerController.ChangeDirection();
		playerController.StartPlayerShootingCooldown();
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
		//weapon.GetComponent<CustomWeaponInterface>().FirePrimary(position, rotation);
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
		//weapon.GetComponent<CustomWeaponInterface>().FireSpecial(position, rotation);
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
	void FirePrimary(Vector3 position, int damage);
	void FireSpecial(Vector3 position, int damage);
}