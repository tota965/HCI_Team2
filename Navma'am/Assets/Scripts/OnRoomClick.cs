using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OnRoomClick : MonoBehaviour {

	Color original;
	Renderer rend;

	bool one_click = false;
	bool timer_running;
	float firstClickTime = 0;
	//this is how long (framerate dependent) to allow for a double click
	float delay = 1.0f;
	//setup camera and move to camera function
	GameObject go;
	public Camera m;
	Color isSelect;
	Color wasSelect;
	//reset camera doesn't need to be in this class... potentially move out so its not saved as many times as this class is initialized

	public delegate void DoubleClickAction();
	public static event DoubleClickAction OnDClicked;

	//there may be many gameobjects which have a clickable function, so this holds the currently doubleclicked one...
	public static GameObject cgo;

	public GameObject panel;

	void Start () {
		
		rend = GetComponent<Renderer> ();
		original = rend.material.color;
		//for initialization of camera and move to camera function
		go = gameObject;
		m = Camera.main;
		CamPosSave.MyCamPos = m.transform.position;
		CamPosSave.MyCamRot = m.transform.eulerAngles;
		isSelect = Color.white;
		isSelect.a = 0.5f;
		wasSelect = Color.grey;
		wasSelect.a = 0.5f;

	}
	GameObject button;
	void Update () {
		//reset camera
		if (Input.GetKeyDown (KeyCode.C)) {
			m.transform.position = CamPosSave.MyCamPos;
			m.transform.eulerAngles = CamPosSave.MyCamRot;



			//clear the mouse orbit target
			m.gameObject.GetComponent <MouseOrbit>().target = null;
			button = GameObject.Find ("View Graph (1)");
			if (button.activeSelf)
				button.SetActive (false);
			
		}
	}

	void OnMouseEnter() {
		rend.material.color = isSelect;
		//print ("This object is " + gameObject.name);
	}

//	void OnMouseOver() {
//		rend.material.color -= new Color(0.01F, 0.01F, 0.01F) * Time.deltaTime;
//	}


	void OnMouseExit() {
		rend.material.color = wasSelect;
		StartCoroutine (LerpColor());
	}


	float duration = 5; // This will be your time in seconds.
	float smoothness = 0.02f; // This will determine the smoothness of the lerp. Smaller values are smoother. Really it's the time between updates.

	//for fade back to original on exit
	IEnumerator LerpColor()
	{
		float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
		float increment = smoothness/duration; //The amount of change to apply.
		while(progress <= 1)
		{
			rend.material.color = Color.Lerp(wasSelect, original, progress);
			progress += increment;
			yield return new WaitForSeconds(0.01f);
		}
	}

	//for double and single clicking...
	void OnMouseDown(){

		if(!one_click) // first click no previous clicks
		{
			one_click = true;

			firstClickTime = Time.time; // save the current time
			// do one click things:
			timer_running = true;
		} 
		else if ((Time.time - firstClickTime)<= delay)
		{
			//do double click things:
			//FocusCameraOnGameObject (m, go);

			//change the main camera focus to the object here
			//setup for MouseOrbit
			Transform mt = go.transform;
			m.gameObject.GetComponent <MouseOrbit>().target = mt;
			Bounds mybounds = CalculateBounds (go);
			m.gameObject.GetComponent <MouseOrbit> ().targetBounds = mybounds;
			Vector3 cp = mybounds.ClosestPoint (m.transform.position); 
			m.gameObject.GetComponent <MouseOrbit>().distance = Vector3.Distance (cp, mybounds.center) + 3.0f;

			//print("Double CLICK was detected and time between clicks is: " + (Time.time - firstClickTime));
			if(OnDClicked != null){
				//print ("someone is definitely subscribed...");
				cgo = CurrentObject ();
				OnDClicked();
			}

			one_click = false; // found a double click, now reset
		} else print("Time between clicks: " + (Time.time - firstClickTime));


		if(one_click)
		{
			// if the time now is delay seconds more than when the first click started. 
			if (timer_running) {
				if ((Time.time - firstClickTime) > delay) {
					//basically if thats true its been too long and we want to reset so the next click is simply a single click and not a double click.
					one_click = false;
					timer_running = false;
				}
			}
		}
	}

	//calculated in MouseOrbit instead
	Bounds CalculateBounds(GameObject go) {
		Bounds b = new Bounds(go.transform.position, Vector3.zero);
		Object[] rList = go.GetComponentsInChildren(typeof(Renderer));
		foreach (Renderer r in rList) {
			b.Encapsulate(r.bounds);
		}
		return b;
	}

	public GameObject CurrentObject(){
		//print (go.name + "I am the current object.");
		return go;
	}

	//not used anymore because doesn't zoom or rotate 
	//(using MouseOrbit on main camera instead, and just assigning it an object to center and rotate around)
	public void FocusCameraOnGameObject(Camera c, GameObject go) {
		Bounds b = CalculateBounds(go);
		Vector3 max = b.size;
		// Get the radius of a sphere circumscribing the bounds
		float radius = max.magnitude / 2f;
		// Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
		float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(c.fieldOfView * Mathf.Deg2Rad / 2f) * c.aspect) * Mathf.Rad2Deg;
		// Use the smaller FOV as it limits what would get cut off by the frustum        
		float fov = Mathf.Min(c.fieldOfView, horizontalFOV);
		float dist = radius /  (Mathf.Sin(fov * Mathf.Deg2Rad / 2f));
		Debug.Log("Radius = " + radius + " dist = " + dist);

		Vector3 pos = Random.onUnitSphere * dist + b.center;
		c.transform.position = pos;


		//incrementally gets the camera closer to the object by dist
		//c.transform.Translate (0, 0, dist);
		//only makes one transform
		//c.transform.TransformDirection (0, 0, dist);
		//doesn't work:
		//c.transform.SetLocalPositionZ(dist);

		if (c.orthographic)
			c.orthographicSize = radius;

		// Frame the object hierarchy
		c.transform.LookAt(b.center);
	}
}
