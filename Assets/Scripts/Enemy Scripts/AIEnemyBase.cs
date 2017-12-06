using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// Needs optimization but it's playable and functional
public class AIEnemyBase : AttackBase {
	[HideInInspector] public string facingDirection = "LEFT";
	[HideInInspector] public bool damaged = false;
	[HideInInspector] public List<GameObject> activeEnemies;

	[Header("AI Options")]
	public string aiName = "Unknown";
	public int aiHealth = 100;
	public int aiMaxHealth = 100;
	public int xpGiveAmount = 50;
	public float visualRange = 4f;
	public float moveSpeed = 3f;
	public int minMeleeDamage = 10;
	public int maxMeleeDamage = 20;
	public float attackDistance = 1f;
	public float attackDelay = 2f;
	public float distanceToTarget = 1f;
	public SpriteRenderer spriteRenderer;
	public LayerMask blockingLayer;

	protected bool canAttack;
	protected float attackDelayCount;
	protected bool targetInRange = false;
	protected bool targetSpotted = false;
	protected GameObject friendlyTarget = null;

	[Header("AI UI")]
	public float yOffaxisHP = 0.6f;
	public float yOffaxisName = 0.8f;
	public Canvas aiCanvas;
	public Text aiNameText;
	public Slider aiHealthSlider;

	protected GameObject target;
	protected float flashSpeed = 5f;

	void Awake(){
		LoadAIEnemyBase();
	}

	public void LoadAIEnemyBase(){
		if(GameObject.FindGameObjectWithTag("Player") != null){
			target = GameObject.FindGameObjectWithTag("Player");
			Debug.Log("Found Player");
		}
	}

	protected void AddEnemyToList(GameObject caller){
		activeEnemies.Add(caller);
	}

	protected void RemoveEnemyToList(GameObject caller){
		activeEnemies.Remove(caller);
	}

	public virtual void TakeDamage(GameObject caller, int damage){
		AIEnemyInterface callerScript = caller.GetComponent<AIEnemyInterface>();
		int curHealth = callerScript.GetAIHealth();
		curHealth -= damage;
		callerScript.SetAIHealth(curHealth);
		if(curHealth <= 0){
			callerScript.EnemyDeath();
		}
		callerScript.SetDamagedTrue();
		callerScript.SetTargetSpotted(true);
		UpdateUIValues(caller);
	}

	public virtual void AddHealth(GameObject caller, int amount, int curHealth, int maxHealth){
		curHealth += amount;
		if(curHealth > maxHealth){
			curHealth = maxHealth;
		}
		caller.GetComponent<AIEnemyInterface>().SetAIHealth(curHealth);
	}

	protected virtual void EnemyDeath(GameObject caller){
		PlayerExperience playerExperience = target.GetComponent<PlayerExperience>();
		playerExperience.AddExp(caller.GetComponent<AIEnemyInterface>().GetAIXPGiveAmount());
		Destroy(caller);
	}

	protected virtual bool IsTargetInVisualRange(GameObject caller, float visualRange, bool targetSpotted){
		BoxCollider2D bc2d = caller.GetComponent<BoxCollider2D>();
		if(!targetSpotted){
			RaycastHit2D checkDirection;
			bc2d.enabled = false;
			checkDirection = Physics2D.Linecast(caller.transform.position, target.transform.position, blockingLayer);
			bc2d.enabled = true;

			if(checkDirection.transform.CompareTag(target.transform.tag)){
				if(checkDirection.distance < visualRange){
					return true;
				}
			}
		} else {
			float checkDirection = Vector3.Distance(caller.transform.position, target.transform.position);
			if(checkDirection <= visualRange){
				return true;
			}
		}
		return false;
	}

	protected virtual void AlertEnemiesInProximity(GameObject caller, float alertRange){
		for(int i = 0; i < activeEnemies.Count; i++){
			if(!activeEnemies[i].GetComponent<AIEnemyInterface>().GetTargetSpotted()) {
				float distance = Vector3.Distance(caller.transform.position, activeEnemies[i].transform.position);
				if(distance < 8){
					activeEnemies[i].GetComponent<AIEnemyInterface>().SetFriendlyTarget(caller);
				}
			}
		}
		AlertEnd(4);
	}

	private IEnumerator AlertEnd(float delayTime){
		yield return new WaitForSeconds(delayTime);
		for(int i = 0; i < activeEnemies.Count; i++){
			activeEnemies[i].GetComponent<AIEnemyInterface>().SetFriendlyTarget(null);
		}
	}

	protected virtual void MoveToFriendly(GameObject caller, Vector3 destination, float moveSpeed, string facingDirection, float distanceToCheck, LayerMask blockingLayer){
		// Called for the AlertEnemiesInProximity method to move the other enemies to the enemy which alerted
	}

	protected virtual void Move(GameObject caller, Vector3 destination, float moveSpeed, string facingDirection, float distanceToCheck, LayerMask blockingLayer){
		AIEnemyInterface callerScript = caller.GetComponent<AIEnemyInterface>();
		bool checkPath = IsPathClear(caller, destination, distanceToCheck, blockingLayer);
		Vector3 curPos = caller.transform.position;
		if(!checkPath){
			// If we can't go towards our target do some side step checking here
			return;
		}
		Vector3 targetPos = Vector3.MoveTowards(caller.transform.position, destination, moveSpeed);
		caller.transform.position = targetPos;

		if(curPos.x > caller.transform.position.x){
			callerScript.SetFacingDirection("LEFT");
		} else if(curPos.x < caller.transform.position.x){
			callerScript.SetFacingDirection("RIGHT");
		} else if(curPos.y > caller.transform.position.y) {
			callerScript.SetFacingDirection("DOWN");
		} else if(curPos.y < caller.transform.position.y) {
			callerScript.SetFacingDirection("UP");
		}
	}

	protected virtual void ChangeDirection(GameObject caller, string facingDirection){
		Debug.Log(facingDirection);
		switch(facingDirection){
			case "RIGHT":
				caller.transform.eulerAngles = new Vector3(caller.transform.rotation.x, 180, caller.transform.rotation.z);
				break;
			case "LEFT":
				caller.transform.eulerAngles = new Vector3(caller.transform.rotation.x, 0, caller.transform.rotation.z);
				break;
			case "UP":
				break;
			case "DOWN":
				break;
		}
	}

	protected virtual bool IsPathClear(GameObject caller, Vector3 destination, float distanceToCheck, LayerMask blockingLayer){
		RaycastHit2D checkDirection;
		BoxCollider2D bc2d = caller.GetComponent<BoxCollider2D>();

		bc2d.enabled = false;
		checkDirection = Physics2D.Linecast(caller.transform.position, destination, blockingLayer);
		bc2d.enabled = true;

		if(checkDirection.distance < distanceToCheck){
			return false;
		} else {
			return true;
		}
	}

	protected void UpdateUIValues(GameObject caller){
		AIEnemyInterface callerScript = caller.GetComponent<AIEnemyInterface>();
		callerScript.SetUIHealthSliderMaxValue(caller.GetComponent<AIEnemyInterface>().GetAIMaxHealth());
		callerScript.SetUIHealthSliderValue(caller.GetComponent<AIEnemyInterface>().GetAIHealth());
		callerScript.SetUINameText(caller.GetComponent<AIEnemyInterface>().GetAINameText());
	}

	protected void UpdateUIPositions(GameObject caller, float yOffaxisHP, float yOffaxisName){
		AIEnemyInterface callerScript = caller.GetComponent<AIEnemyInterface>();

		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + yOffaxisHP, transform.position.x));
		callerScript.SetUIHealthPosition(screenPos);

		screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + yOffaxisName, transform.position.x));
		callerScript.SetUINameTextPosition(screenPos);
	}
}

public class AttackBase : MonoBehaviour {
	protected virtual bool MeleeAttackPlayer(GameObject caller, GameObject target, string facingDirection, float attackDistance, LayerMask blockingLayer, int minDamage, int maxDamage){
		float distance = Vector3.Distance(caller.transform.position, target.transform.position);
		if(distance <= attackDistance) {
			if(target.transform.CompareTag("Player")){
				PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
				int rand = Random.Range(minDamage, maxDamage);
				playerHealth.TakeDamage(rand);
			}
			return true;
		}
		return false;
	}

	private Vector3 positionRangeAttack;
	private Vector3 rotationRangeAttack;
	protected virtual bool RangeAttackPlayer(GameObject caller, GameObject target, GameObject projectile, string facingDirection, float attackDistance){
		float distance = Vector3.Distance(caller.transform.position, target.transform.position);
		if(distance <= attackDistance){
			switch(facingDirection){
				case "RIGHT":
					FacingRight(caller);
					break;
				case "LEFT":
					FacingLeft(caller);
					break;
				case "UP":
					FacingUp(caller);
					break;
				default:
					FacingDown(caller);
					break;
			}
			GameObject firedPrefab = Instantiate(projectile, positionRangeAttack, Quaternion.identity);
			firedPrefab.transform.eulerAngles = rotationRangeAttack;
			return true;
		}
		return false;
	}

	// Projectile reference at rotation and position relative to callers position
	private void FacingRight(GameObject caller){
		positionRangeAttack = new Vector3(caller.transform.position.x + 0.8f, caller.transform.position.y, caller.transform.position.z);
		rotationRangeAttack = new Vector3(0, 0, -90);
	}

	private void FacingLeft(GameObject caller){
		positionRangeAttack = new Vector3(caller.transform.position.x - 0.8f, caller.transform.position.y, caller.transform.position.z);
		rotationRangeAttack = new Vector3(0, 0, 90);
	}

	private void FacingUp(GameObject caller){
		positionRangeAttack = new Vector3(caller.transform.position.x, caller.transform.position.y + 1f, caller.transform.position.z);
		rotationRangeAttack = new Vector3(0, 0, 0);
	}
	
	private void FacingDown(GameObject caller){
		positionRangeAttack = new Vector3(caller.transform.position.x, caller.transform.position.y - 1f, caller.transform.position.z);
		rotationRangeAttack = new Vector3(0, 0, 180);
	}
}

public interface AIEnemyInterface {
	void EnemyDeath();
	void SetDamagedTrue();

	void SetFriendlyTarget(GameObject target);
	void SetTargetSpotted(bool value);
	void SetFacingDirection(string direction);
	void SetAIHealth(int health);
	void SetUIHealthPosition(Vector3 position);
	void SetUINameTextPosition(Vector3 position);
	void SetUIHealthSliderMaxValue(int value);
	void SetUIHealthSliderValue(int value);
	void SetUINameText(string name);

	bool GetTargetSpotted();
	int GetAIXPGiveAmount();
	int GetAIMaxHealth();
	int GetAIHealth();
	string GetAINameText();
}