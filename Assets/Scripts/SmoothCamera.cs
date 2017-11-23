using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour {
	public float offsetX = 0;
	public float offsetY = 0;
	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	private Transform target;
	private bool followCursor = false;
	Camera cam;
	
	void Start(){
		target = GameObject.FindGameObjectWithTag("Player").transform;
		cam = GetComponent<Camera>();
	}

	void FixedUpdate () {
		if(target && !followCursor){
			// Get target position with the added offset
			Vector3 aheadPoint = target.position + new Vector3(offsetX, offsetY, 0);
			// Get target position relative to the world
			Vector3 point = cam.WorldToViewportPoint(target.position);
			// Calculate final distance of delta
            Vector3 delta = aheadPoint - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			// Our destination is our current position + final target position
            Vector3 destination = transform.position + delta;
			// Smooth the transition
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
	}
}
