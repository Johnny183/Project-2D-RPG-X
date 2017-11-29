using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableItem : MonoBehaviour {
	
	public Text text;
	public Canvas canvas;
	public Item item;

	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider;
	private GameObject player;
	private float interactDistance;

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = item.icon;
		boxCollider = GetComponent<BoxCollider2D>();
		player = GameObject.FindGameObjectWithTag("Player");
		
		canvas.gameObject.SetActive(true);
		text.gameObject.SetActive(false);
	}

	void Start(){
		interactDistance = GameManager.instance.playerIneractDist;
		text.text = item.name;
	}

	void Update(){
		boxCollider.enabled = false;
		RaycastHit2D target = Physics2D.Linecast(transform.position, player.transform.position, LayerMask.NameToLayer("blockingLayer"));
		boxCollider.enabled = true;

		if(target.distance < interactDistance){
			if(target.transform.CompareTag("Player")){
				Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.x));
				text.transform.position = screenPos;
				text.gameObject.SetActive(true);
			} else {
				text.gameObject.SetActive(false);
			}
		} else {
			text.gameObject.SetActive(false);
			text.text = item.name;
		}
	}

	public void PickUp(GameObject caller) {
		Debug.Log("Picked up: " + item.name);
		bool wasPickedUp = InventoryManager.instance.Add(item);
		if(wasPickedUp){
			Destroy(gameObject);
		} else {
			text.text = "No Inventory Space";
		}
	}
}
