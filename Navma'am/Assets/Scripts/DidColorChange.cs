using UnityEngine;
using System.Collections;

//for subscribing dupulicated and normal reversed geometry to the color change event

public class DidColorChange : MonoBehaviour {

	public GameObject mirrorObject;
	public Renderer rend;
	Color mycolor;

	void Start(){
		rend = GetComponent<Renderer> ();
		rend.enabled = true;
		mycolor = mirrorObject.GetComponent<Renderer> ().material.color;
		rend.material.color = mycolor;
	}



	void OnEnable()
	{
		ColorChange.OnChanged += Check;

	}

	void OnDisable()
	{
		ColorChange.OnChanged -= Check;

	}

	void Check(){
		//print ("the event was executed for " + gameObject.name);
		mycolor = mirrorObject.GetComponent<Renderer> ().material.color;
		rend.material.color = mycolor;
		//print ("the event was executed for " + gameObject.name);
	}
}
