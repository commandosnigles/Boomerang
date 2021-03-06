﻿using UnityEngine;
using System.Collections;

public class Boomerang: MonoBehaviour {

	public float Roundness = 3f;
	public float Range = 20f;
	public float Rate = 10f;
	public float RotationSpeed = 6;

	public GameObject[] targetList;
	
	private GameObject StartingOrigin;
	private GameObject StartingTarget;
	private GameObject origin;
	private GameObject target;
	private float startTime;
	private float percentDone;
	private Vector3 distance;
	private Vector3 handle;
	private Vector3 handleDistance;
	private Vector3 bisector;
	private short handleDirection = 1;

	void Awake () {
		//make sure there is a nonkinematic rigidbody
		if (this.rigidbody == null){
			this.gameObject.AddComponent("Rigidbody");
		}
		this.rigidbody.isKinematic = false;
		

	}

	void Start () {
		Throw ();
	}

	void Throw () {

		Destroy (GameObject.Find ("Origin"));
		Destroy (GameObject.Find ("Target"));

		StartingOrigin = new GameObject("Origin");
		StartingOrigin.transform.position = transform.position;

		if (targetList == null) {
			StartingTarget = new GameObject("Target");
			StartingTarget.transform.position = transform.position + Range * transform.forward;
		}
		else {
			StartingTarget = targetList[0];
		}


		startTime = Time.time;
		origin = StartingOrigin;
		target = StartingTarget;
		handle = new Vector3(transform.forward.z,0,-transform.forward.x) * Mathf.Min(Roundness, 0.75f*Range);
	}

	void Update () {

		//SWITH DIRECTION AT PERCENTDONE = 1\\
		if (percentDone == 1f) {
			NewTarget (target, origin);
		}

		RefreshPosition();

	}

	void RefreshPosition(){
		//FIND RAYS\\
		
		//find distance between origin and target 
		distance = target.transform.position-origin.transform.position;

		//create a ray between origin and the end of the handle
		handleDistance = (StartingTarget.transform.position + handle - StartingOrigin.transform.position);
		
		//calculate the time passed since the last direction reversal
		float timePassed = Time.time - startTime;
		//calculate how far along the object would be traveling in a straight line from origin to target
		percentDone = Mathf.Clamp01 (timePassed*Rate/distance.magnitude);
		float percentLeft = 1 - percentDone;
		
		//FIND THE POINT ALONG THE BISECTOR\\
		Vector3 bezierPoint;
		//find the ray from handleDistance to handle for a given percent of the cycle
		if (handleDirection > 0) {
			bisector = percentLeft*handleDistance - percentDone*handle;
			bezierPoint = StartingOrigin.transform.position + percentDone * handleDistance + percentDone * bisector;
		}
		else {
			bisector = percentDone*handleDistance - percentLeft*handle;
			bezierPoint = StartingOrigin.transform.position + percentLeft * handleDistance + percentLeft * bisector;
		}
		//Vector3 moveVector = bezierPoint - transform.position;
		//Quaternion rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation (moveVector,Vector3.up),RotationSpeed);
		//rotation = Quaternion.Euler (rotation.eulerAngles.x,0,rotation.eulerAngles.z);
		//transform.rotation = rotation;
		transform.position = bezierPoint;
	}

	/// <summary> Sets a new target and origin, and reverses the handle. </summary>
	void NewTarget (GameObject _origin,GameObject _target) {
		//reset timer
		startTime = Time.time;
		//set new origin
		origin = _origin;
		//set new target
		target = _target;
		//reverse the direction of travel
		handleDirection *= -1;
		handle *= -1;

	}

//	void OnDrawGizmos() {
//
//		Gizmos.color = Color.black;
//		Gizmos.DrawRay(StartingOrigin.transform.position, handleDistance);
//		Gizmos.DrawRay(StartingTarget.transform.position, handle);
//		
//		Gizmos.color = Color.green;
//		if (handleDirection > 0)
//			Gizmos.DrawRay(StartingOrigin.transform.position + percentDone * handleDistance , bisector);
//		else
//			Gizmos.DrawRay(StartingOrigin.transform.position + (1-percentDone) * handleDistance , bisector);
//
//	}
}
