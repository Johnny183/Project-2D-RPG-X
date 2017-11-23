using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
	public string[] randItems;
	public Text text;
	public Canvas canvas;
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private GameObject player;
	private float interactDistance;

	void Start(){
		boxCollider = GetComponent<BoxCollider2D>();
		player = GameObject.FindGameObjectWithTag("Player");

		interactDistance = GameManager.instance.playerIneractDist;
		canvas.gameObject.SetActive(true);
		text.gameObject.SetActive(false);
	}

	void Update(){
		boxCollider.enabled = false;
		RaycastHit2D target = Physics2D.Linecast(transform.position, player.transform.position, blockingLayer);
		boxCollider.enabled = true;

		if(target.distance < interactDistance){
			if(target.transform.tag == "Player"){
				Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.x));
				text.transform.position = screenPos;
				text.gameObject.SetActive(true);
			} else {
				text.gameObject.SetActive(false);
			}
		} else {
			text.gameObject.SetActive(false);
		}
	}

	public void Interact(Transform other){
		int randIndex = Random.Range(0, randItems.Length);
		PlayerController playerController = other.GetComponent<PlayerController>();
		PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
		PlayerExperience playerExperience = other.GetComponent<PlayerExperience>();
		playerHealth.TakeDamage(10);
		playerExperience.AddExp(250);
		playerController.CallPlayerSpeak("You picked up a " + randItems[randIndex], Color.yellow, 0, 4, 0f);
	}
}
