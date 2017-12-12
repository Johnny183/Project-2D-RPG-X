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
		AIDamaged();

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
		UpdateUIPositions(yOffaxisHP, yOffaxisName);
		UpdateUIValues(displayUIName);

		if(targetSpotted){
			// Visual Range amplifier if we are chasing the target
			targetInRange = IsTargetInVisualRange(visualRange * 1.5f, targetSpotted);
		} else {
			// If we have yet to spot the target, normal visual range.
			targetInRange = IsTargetInVisualRange(visualRange, targetSpotted);
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
				Move(dir);
			}

			if(Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < 1f){
				currentWaypoint++;
			}
		}
	}
	
	public void SetFriendlyTarget(GameObject value){
		friendlyTarget = value;
	}

	public void SetTargetSpotted(bool value){
		targetSpotted = value;
	}
	
	public bool GetTargetSpotted(){
		return targetSpotted;
	}
}
