using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Pathfinding;

// Needs optimization but it's playable and functional
public class AIEnemyBase : AttackBase {
	[HideInInspector] public string facingDirection = "LEFT";
	[HideInInspector] public bool damaged = false;
	[HideInInspector] public List<GameObject> activeEnemies;

	[Header("AI Options")]
	public string aiName = "Unknown";
	public bool displayUIName = false;
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

	public virtual void TakeDamage(int damage){
		int curHealth = aiHealth;
		curHealth -= damage;
		aiHealth = curHealth;
		if(curHealth <= 0){
			EnemyDeath();
		}
		damaged = true;
		GetComponent<AIEnemyInterface>().SetTargetSpotted(true);
		UpdateUIValues(displayUIName);
	}

	public virtual void AddHealth(int amount, int curHealth, int maxHealth){
		curHealth += amount;
		if(curHealth > maxHealth){
			curHealth = maxHealth;
		}
		aiHealth = curHealth;
	}

	protected virtual void EnemyDeath(){
		PlayerExperience playerExperience = target.GetComponent<PlayerExperience>();
		playerExperience.AddExp(xpGiveAmount);
		Destroy(gameObject);
	}

	protected virtual bool IsTargetInVisualRange(float visualRange, bool targetSpotted){
		BoxCollider2D bc2d = GetComponent<BoxCollider2D>();
		if(!targetSpotted){
			// Check if we can see the target and if the target is within range
			RaycastHit2D checkDirection;
			bc2d.enabled = false;
			checkDirection = Physics2D.Linecast(transform.position, target.transform.position);
			bc2d.enabled = true;

			if(checkDirection.transform.CompareTag(target.transform.tag)){
				if(checkDirection.distance < visualRange){
					return true;
				}
			}
		} else {
			// Checks the distance between the target and the enemy disregarding if we can see it or not
			float checkDirection = Vector3.Distance(transform.position, target.transform.position);
			if(checkDirection <= visualRange){
				return true;
			}
		}
		return false;
	}

	protected virtual void AlertEnemiesInProximity(float alertRange){
		for(int i = 0; i < activeEnemies.Count; i++){
			if(!activeEnemies[i].GetComponent<AIEnemyInterface>().GetTargetSpotted()) {
				float distance = Vector3.Distance(transform.position, activeEnemies[i].transform.position);
				if(distance < 8){
					activeEnemies[i].GetComponent<AIEnemyInterface>().SetFriendlyTarget(gameObject);
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

	protected virtual void MoveToFriendly(Vector3 destination, float moveSpeed, string facingDirection, float distanceToCheck, LayerMask blockingLayer){
		// Called for the AlertEnemiesInProximity method to move the other enemies to the enemy which alerted
	}

	protected virtual void Move(Vector2 dir){
		Vector3 newPos = Vector3.Lerp(transform.position, new Vector2(transform.position.x, transform.position.y) + dir, (moveSpeed * Time.fixedDeltaTime));
		transform.position = newPos;

		if(transform.position.x > target.transform.position.x){
			facingDirection = "LEFT";
		} else if(transform.position.x < target.transform.position.x){
			facingDirection = "RIGHT";
		} else if(transform.position.y > target.transform.position.y) {
			facingDirection = "DOWN";
		} else if(transform.position.y < target.transform.position.y) {
			facingDirection = "UP";
		}

		ChangeDirection(facingDirection);
	}

	protected virtual void ChangeDirection(string facingDirection){
		switch(facingDirection){
			case "RIGHT":
				transform.eulerAngles = new Vector3(transform.rotation.x, 180, transform.rotation.z);
				break;
			case "LEFT":
				transform.eulerAngles = new Vector3(transform.rotation.x, 0, transform.rotation.z);
				break;
			case "UP":
				break;
			case "DOWN":
				break;
		}
	}

	protected virtual bool IsPathClear(Vector3 destination, float distanceToCheck, LayerMask blockingLayer){
		RaycastHit2D checkDirection;
		BoxCollider2D bc2d = GetComponent<BoxCollider2D>();

		bc2d.enabled = false;
		checkDirection = Physics2D.Linecast(transform.position, destination, blockingLayer);
		bc2d.enabled = true;

		if(checkDirection.distance < distanceToCheck){
			return false;
		} else {
			return true;
		}
	}

	protected virtual void AttemptAttack(){
		bool attemptAttack = MeleeAttackPlayer(gameObject, target, facingDirection, attackDistance, blockingLayer, minMeleeDamage, maxMeleeDamage);
		if(attemptAttack){
			GetComponent<Animator>().SetTrigger("EnemyAttack");
			canAttack = false;
			attackDelayCount = 0f;
		}
	}

	protected void AIDamaged(){
		if(damaged){
			spriteRenderer.color = Color.red;
		} else {
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, flashSpeed * Time.deltaTime);
		}
		damaged = false;
	}

	protected void SetUINameColor(Color color){
		aiNameText.color = color;
	}

	protected void UpdateUIValues(bool displayName){
		aiHealthSlider.maxValue = aiMaxHealth;
		aiHealthSlider.value = aiHealth;

		if(displayName){
			aiNameText.text = aiName;
		} else {
			aiNameText.text = "";
		}
	}

	protected virtual void UpdateUIPositions(float yOffaxisHP, float yOffaxisName){
		Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + yOffaxisHP, transform.position.x));
		aiHealthSlider.transform.position = screenPos;

		screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + yOffaxisName, transform.position.x));
		aiNameText.transform.position = screenPos;
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
	void SetFriendlyTarget(GameObject target);
	void SetTargetSpotted(bool value);

	bool GetTargetSpotted();
}