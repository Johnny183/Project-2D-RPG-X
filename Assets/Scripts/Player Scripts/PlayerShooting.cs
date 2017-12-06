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
	
	[SerializeField]
	private int playerDamage;

	private Vector3 position;

	void Start () {
		playerController = GetComponent<PlayerController>();
		//EquipmentManager.instance.onEquipmentChanged += FindWeapon;

		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		int statsDamage = 0;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				statsDamage += currentEquipment[i].damageModifier;
			}
		}
		playerDamage = GameManager.instance.playerStartingDamage + statsDamage;
		//FindWeapon();
	}

	private void UpdatePlayer(){
		playerController.ChangeDirection();
		playerController.StartPlayerShootingCooldown();
	}

	void Update () {
		if(weapon == null){ return; }

		// todo: Add
		if(specialBlur != null){
			if(specialBlur.fillAmount != 0){
				specialBlur.fillAmount -= 1 / specialCooldown * Time.deltaTime;
			}
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

	/*public void FindWeapon(){
		Equipment[] currentEquipment = EquipmentManager.instance.currentEquipment;
		for(int i = 0; i < currentEquipment.Length; i++){
			if(currentEquipment[i] != null){
				if(currentEquipment[i].equipSlot == EquipmentSlot.Weapon && currentEquipment[i].refObject != null){
					weapon = currentEquipment[i].refObject;
					specialCooldownUI = GameObject.FindGameObjectWithTag("SpecialCooldown");
					specialCooldownUI.SetActive(true);
					StartSpecialCooldown(weapon.GetComponent<WeaponBaseInterface>().GetSpecialImage(), weapon.GetComponent<WeaponBaseInterface>().GetSpecialCooldownTime());
				}
			}
		}

		if(specialCooldownUI != null){
			specialCooldownUI.SetActive(false);	
		}

		specialCooldownUI = null;
		weapon = null;
	}*/

	public void NewWeapon(Equipment item){
		if(item.refObject != null){
			Debug.Log("New weapon equiped");
			weapon = item.refObject;
			specialCooldownUI = GameObject.FindGameObjectWithTag("SpecialCooldown");
			specialCooldownUI.SetActive(true);
			StartSpecialCooldown(weapon.GetComponent<WeaponBaseInterface>().GetSpecialImage(), weapon.GetComponent<WeaponBaseInterface>().GetSpecialCooldownTime());
		}
	}

	public void LoseWeapon(){
		Debug.Log("Weapon removed");
		StopAllCoroutines();

		if(specialCooldownUI != null){
			specialCooldownUI.SetActive(false);	
		}

		specialCooldownUI = null;
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
			int randDamage = Random.Range(playerDamage-3, playerDamage+3);
			weapon.GetComponent<WeaponBaseInterface>().FireSpecial(gameObject, playerController.facingDirection, position, randDamage);
			StartSpecialCooldown(weapon.GetComponent<WeaponBaseInterface>().GetSpecialImage(), weapon.GetComponent<WeaponBaseInterface>().GetSpecialCooldownTime());
		}
	}

	// todo: Setup special cooldown stuff here like cooldown time and special image provided by specific weapon
	public void StartSpecialCooldown(Sprite image, float cooldownTime){
		specialActive = false;
		specialBlur.fillAmount = 1;
		specialImage.sprite = image;
		specialTime.text = "" + cooldownTime;
		specialCooldown = cooldownTime;

		StartCoroutine(SpecialCooldown(cooldownTime));
	}

	// todo: Handle cooldown here
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
	float GetSpecialCooldownTime();
}