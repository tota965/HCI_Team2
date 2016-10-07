using UnityEngine;
using System.Collections;

public class Geometry : MonoBehaviour {


	bool isActive;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ViewGeo(GameObject go){

		if (go.activeInHierarchy == false)
			isActive = false;
		if (go.activeInHierarchy == true)
			isActive = true;
		go.SetActive (!isActive);
		isActive = !isActive;

	}
}
