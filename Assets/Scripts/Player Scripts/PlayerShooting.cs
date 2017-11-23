using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {
	private PlayerController playerController;
	private CustomWeaponInterface weaponScript;
	private GameObject weapon;
	private string weaponScriptName;

	private Vector3 position;
	private Vector3 rotation;

	void Start () {
		weapon = GameObject.FindGameObjectWithTag("Weapon");
		weaponScript = weapon.GetComponent<CustomWeaponInterface>();
		playerController = GetComponent<PlayerController>();
	}
	
	void Update () {
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
		weaponScript.FirePrimary(position, rotation);
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
		weaponScript.FireSpecial(position, rotation);
	}

	private void FacingRight(){
		position = new Vector3(transform.position.x + 0.65f, transform.position.y, transform.position.z);
		rotation = new Vector3(0, 0, -90);
	}

	private void FacingLeft(){
		position = new Vector3(transform.position.x - 0.65f, transform.position.y, transform.position.z);
		rotation = new Vector3(0, 0, 90);
	}

	private void FacingUp(){
		position = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
		rotation = new Vector3(0, 0, 0);
	}
	
	private void FacingDown(){
		position = new Vector3(transform.position.x, transform.position.y - 0.8f, transform.position.z);
		rotation = new Vector3(0, 0, 180);
	}
}
