using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Pathfinding;

public class ZombieEnemy : AIEnemyBase, AIEnemyInterface {

	Seeker seeker;
	Path path;
	private int currentWaypoint;
	private bool pathing = false;
	private float repathRate = 0.3f;
	private Rigidbody2D rb2D;
	private Animator animator;

	void Start(){
		aiCanvas.gameObject.SetActive(true);
		seeker = GetComponent<Seeker>();
		rb2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		StartCoroutine(RepeatGetPath());
	}

	IEnumerator RepeatGetPath(){
		while (true)  {
			yield return new WaitForSeconds(repathRate);
			StartPath();
		}
	}

	public void StartPath() {
		if(targetInRange){
			targetSpotted = true;
			pathing = true;
			seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
		} else {
			pathing = false;
			targetSpotted = false;
		}
	}

	public void OnPathComplete(Path p){
		if(!p.error){
			path = p;
			currentWaypoint = 0;
		}
	}

	void Update(){
		if(damaged){
			spriteRenderer.color = Color.red;
		} else {
			spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, flashSpeed * Time.deltaTime);
		}
		damaged = false;

		if(canAttack){
			AttemptAttack();
		} else {
			attackDelayCount += Time.deltaTime;
			if(attackDelayCount >= attackDelay){
				canAttack = true;
			}
		}
	}

	void FixedUpdate(){
		UpdateUIPositions(gameObject, yOffaxisHP, yOffaxisName);
		UpdateUIValues(gameObject);

		if(targetSpotted){
			// Visual Range amplifier if we are chasing the target
			targetInRange = IsTargetInVisualRange(gameObject, visualRange * 1.4f, targetSpotted);
		} else {
			// If we have yet to spot the target, normal visual range.
			targetInRange = IsTargetInVisualRange(gameObject, visualRange, targetSpotted);
		}

		if(pathing){
			if(path == null){
				return;
			}

			if(currentWaypoint >= path.vectorPath.Count){
				return;
			}

			Vector2 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
			if(Vector2.Distance(transform.position, target.transform.position) >= distanceToTarget){
				Vector3 curPos = transform.position;
				rb2D.MovePosition(new Vector2(transform.position.x, transform.position.y) + dir * (moveSpeed * Time.fixedDeltaTime));

				Debug.Log("Old position: " + curPos + "\nNew Position: " + transform.position);
				if(curPos.x > transform.position.x){
					facingDirection = "LEFT";
				} else if(curPos.x < transform.position.x){
					facingDirection = "RIGHT";
				} else if(curPos.y > transform.position.y) {
					facingDirection = "DOWN";
				} else if(curPos.y < transform.position.y) {
					facingDirection = "UP";
				}
				ChangeDirection(gameObject, facingDirection);
			}

			if(Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < 1f){
				currentWaypoint++;
			}
		}
	}

	public void AttemptAttack(){
		bool attemptAttack = base.MeleeAttackPlayer(gameObject, target, facingDirection, attackDistance, blockingLayer, minMeleeDamage, maxMeleeDamage);
		if(attemptAttack){
			animator.SetTrigger("EnemyAttack");
			canAttack = false;
			attackDelayCount = 0f;
		}
	}

	public void EnemyDeath(){
		base.EnemyDeath(gameObject);
	}
	
	public void SetFriendlyTarget(GameObject value){
		friendlyTarget = value;
	}

	public void SetTargetSpotted(bool value){
		targetSpotted = value;
	}

	public void SetFacingDirection(string direction){
		facingDirection = direction;
	}

	public void SetDamagedTrue(){
		damaged = true;
	}

	public void SetAIHealth(int health){
		aiHealth = health;
	}

	public void SetUIHealthPosition(Vector3 position){
		aiHealthSlider.transform.position = position;
	}

	public void SetUINameTextPosition(Vector3 position){
		aiNameText.transform.position = position;
	}

	public void SetUIHealthSliderMaxValue(int value){
		aiHealthSlider.maxValue = value;
	}

	public void SetUIHealthSliderValue(int value){
		aiHealthSlider.value = value;
	}

	public void SetUINameText(string name){
		aiNameText.text = name;
	}
	
	public bool GetTargetSpotted(){
		return targetSpotted;
	}

	public int GetAIXPGiveAmount(){
		return xpGiveAmount;
	}

	public int GetAIMaxHealth() {
		return aiMaxHealth;
	}

	public int GetAIHealth() {
		return aiHealth;
	}

	public string GetAINameText(){
		return aiName;
	}
}
