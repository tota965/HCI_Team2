using UnityEngine;
using System.Collections;
//using UnityEditor;

public class ColorChange : MonoBehaviour {

	//its not really a change interval, but a number to determine how much change happens (fraction denominator)
	[Range(1,120)]
	public int changeInterval = 60;
	public Renderer rend;

	public GameObject dataManager;
	float random;
	[Range(2.0f,10.0f)]
	public float waittime;
	public delegate void ChangeAction();
	public static event ChangeAction OnChanged;


	// Use this for initialization
	void Start () {

		dataManager = GameObject.Find("DataManager");
		rend = GetComponent<Renderer> ();
		//lets make sure this thing is rendering
		rend.enabled = true;

		PickRandomWaitTime ();

		AssignStartGradientVal ();

		StartCoroutine (TimeChangeControl ());
	}

	void Update () {

	}

	IEnumerator TimeChangeControl ()
	{
		while (random < changeInterval) {
			random++;

			//print("the current random number for " + gameObject.name + " is: " + random);

			yield return new WaitForSeconds (waittime);

			Color mycolor = dataManager.GetComponent<roomManager>().g.Evaluate(random/changeInterval);
			rend.material.color = mycolor;

			//for those subscribed to do something:
			if (OnChanged != null) {
				//print ("something is definitely subscribed...");
				OnChanged ();
			}

			if (random >= changeInterval) {
				random = 0;
			}


		} 

	}

	void AssignStartGradientVal ()
	{
		random = Random.Range (0, changeInterval);
		//print ("random start number for room " + gameObject.name + " is: " + random);
		Color mycolor = dataManager.GetComponent<roomManager> ().g.Evaluate (random / changeInterval);
		rend.material.color = mycolor;
	}
		
	void PickRandomWaitTime(){

		waittime = Random.Range (2.0f, 10.0f);
	}
}
