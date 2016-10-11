using UnityEngine;
using System.Collections;

//Adapted from http://wiki.unity3d.com/index.php?title=MouseOrbitImproved#Code_C.23

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbit : MonoBehaviour {

	//adjust to not use a transform because when imported, objects transform position is different than center of object, 
	//so use bounds instead
	public Transform target;
	public Bounds targetBounds;
	//distance should be based on the bounds extents:
	public float distance = 5.0f;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;

	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;

	public float distanceMin = .5f;
	public float distanceMax = 30f;

	private Rigidbody rigidbody;

	float x = 0.0f;
	float y = 0.0f;

	// Use this for initialization
	void Start () 
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		rigidbody = GetComponent<Rigidbody>();

		// Make the rigid body not change rotation
		if (rigidbody != null)
		{
			rigidbody.freezeRotation = true;
		}
	}

	void LateUpdate () 
	{
		if(Input.GetMouseButton(1)){
			//change target to otherTarget
			if (target) 
			{
				//based on our target we want a bounds (setup done once object to focus on is double clicked... (OnClick script)
//				targetBounds = CalculateBoundsT (target); //need this to happen once
//				Vector3 cp = targetBounds.ClosestPoint (transform.position); //need this to happen once
//				distance = Vector3.Distance (cp, targetBounds.center) + 3.0f; //need this to happen once

				x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

				y = ClampAngle(y, yMinLimit, yMaxLimit);

				Quaternion rotation = Quaternion.Euler(y, x, 0);


				distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);

				RaycastHit hit;

				if (Physics.Linecast (targetBounds.center, transform.position, out hit)) 
				{
					distance -=  hit.distance;
				}
				Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);

				Vector3 position = rotation * negDistance + targetBounds.center;

				transform.rotation = rotation;
				transform.position = position;

			}
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	//only tested with transforms which are boxes and have no children
	Bounds CalculateBoundsT(Transform tgo){
		Bounds b = new Bounds (tgo.transform.position, Vector3.zero);
		Object[] rList = tgo.GetComponentsInChildren (typeof(Renderer));
		foreach (Renderer r in rList) {
			b.Encapsulate (r.bounds);
		}
		return b;
	}


//	public static Transform WhatAmILookingAt(){
//		if (target) {
//			return target;
//		}
//		if (!target) {
//			return GameObject.Find("183space").transform;
//		}
//	}

}