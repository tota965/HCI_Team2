using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//we will use this for managing our BmsRooms
//first we need to give our rooms some properties
//from our modeling program, we export a schedule which includes rooms, GUID(ifc), Type of room, level for the room, area, and ifc type (just in case)
//then we need to assign them some geometry

//finally we need to check the status of a sensor and perform some event for that value (varys based on data type temp vs occupied)

public class roomManager : MonoBehaviour {

	//used in LoadData
	public static BmsSpace[] spaces;
	private TextAsset data;
	//title of zone schedule
	public string ZoneList = "ZoneList";

	//used in AssignGeometry
	List<GameObject> geometryList1 = new List<GameObject>();
	//used because we have a normal issue with import:
	List<GameObject> geometryList2 = new List<GameObject>();
	public string[] geoNames = new string[2] {"183space", "183space (1)"};

	//used in GradientSetup
	[Range (0,1)]
	public float evaluate = 0.25f;
	float gradcheck;
	public Gradient g;

	public GameObject panel;

	void Awake () {
		GradiantSetup ();
		LoadData ();
		AssignGeometry ();

	}

	void Start (){
		AssignTemp ();
		CheckTemp ();
		bmsColorChange ();
		AddTriggerAndClick ();
		panel.SetActive (false);
	}

	public void TogglePanel(){
		bool mybool = panel.activeSelf;
		panel.SetActive (!mybool);
	}

	void GradiantSetup ()
	{
		GradientColorKey[] gck;
		GradientAlphaKey[] gak;
		g = new Gradient ();
		gck = new GradientColorKey[5];
		gck [0].color = Color.red;
		gck [1].color = Color.blue;
		gck [2].color = Color.green;
		gck [3].color = Color.yellow;
		gck [4].color = Color.red;
		gck [0].time = 0.0f;
		gck [1].time = 0.25f;
		gck [2].time = 0.5f;
		gck [3].time = 0.75f;
		gck [4].time = 1.0f;
		gak = new GradientAlphaKey[5];
		gak [0].alpha = 0.5f;
		gak [1].alpha = 0.3f;
		gak [2].alpha = 0.2f;
		gak [3].alpha = 0.5f;
		gak [4].alpha = 0.5f;
		gak [0].time = 0.0f;
		gak [1].time = 0.25f;
		gak [2].time = 0.5f;
		gak [3].time = 0.75f;
		gak [4].time = 1.0f;
		g.SetKeys (gck, gak);
		//print ("Gradient evaluated at time 0.25f: " + g.Evaluate (evaluate));
		gradcheck = evaluate;
	}

	void Update (){
		if (gradcheck != evaluate) {
			//print ("Gradient evaluated at time " + evaluate +"f: " + g.Evaluate (evaluate));
			bmsColorChange ();
			gradcheck = evaluate;
		}
	}

	public void LoadData(){
		data = Resources.Load (ZoneList) as TextAsset;
		string input = data.text;
		string[] lines = input.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		int p = lines.Length - 1; //there are the same number of events as number of lines subtracting the header
		spaces = new BmsSpace[p];
		Debug.Log ("Number of spaces in 'spaces' array: " + spaces.Length);

		//adjusted to remove the header from the sequence...
		for (int i = 1; i < lines.Length; i++)	{
			string[] col = lines [i].Split (new[]{ ',' });

			//we need to convert some items (area and floor)
			float area = 0;
			int floor = 0; //defaults which obviously don't match our floor system

			area = Convert.ToSingle (col [4]);
			//assuming the floor is setup the same and its possible to have more than 9 floors, no 1.5 floors, no basements or zeros, 
			//this should work:
			string str = col[3].Substring(5,2);

			Int32.TryParse (str, out floor);

			//since we know the structure of our columns this will work:
			spaces[i-1] = new BmsSpace(col[0],col[1],col[2],floor,area,col[5]);

			//checks the spaces:
			//print(spaces[i-1].ToString());
			//print(spaces[i-1].name);
		}
	}

	public void FloorFilter(int fl){

		foreach (BmsSpace s in spaces) {
			if (s.floor == fl) {
				foreach (GameObject g in s.geometry) {
					g.SetActive (true);
				}
			}
			if (s.floor != fl) {
				foreach (GameObject g in s.geometry) {
					g.SetActive (false);
				}
			}
		}
	}

	public void ViewAll(){
		foreach (BmsSpace s in spaces) {
			foreach (GameObject g in s.geometry) {
				g.SetActive (true);
			}
		}
	}

	void AssignGeometry () {
		//find all the spaces, they are all children of some object
		GameObject parent;
		//GameObject parent1;
		//= GameObject.Find (geoNames[0]);

		parent = GameObject.Find(geoNames[0]);
		foreach (Transform child in parent.transform) {
			geometryList1.Add (child.transform.gameObject);
		}

		parent = GameObject.Find(geoNames[1]);
		foreach (Transform child in parent.transform) {
			geometryList2.Add (child.transform.gameObject);
		}

		print ("Number of Geometries in the list 1: " + geometryList1.Count);
		print ("Number of Geometries in the list 2: " + geometryList2.Count);

		int counting = 0;
		foreach (BmsSpace s in spaces) {
			//using the name find the substring of the room number
			string number = s.name.Substring(4,4);
			//print ("Room number: " + number);
			//array of geometry needs to have same length of number of associated geometries, in this case is 2:
			s.geometry = new GameObject[geoNames.Length];

			foreach (GameObject geo in geometryList1) {
				string room = geo.name.Substring(geo.name.Length -4,4);
				if (number == room) {
					s.geometry[0] = geo;
					counting++;
					//print ("Assigned gameObject: " + g.name + " to BmsSpace " + s.name);
				}
			}

			foreach (GameObject geo in geometryList2) {
				string room = geo.name.Substring(geo.name.Length -4,4);
				if (number == room) {
					s.geometry[1] = geo;
					counting++;
					//print ("Assigned gameObject: " + g.name + " to BmsSpace " + s.name);
				}
			}
			//print ("bmsSpace: " + s.name + " game object: " + s.geometry);
		}
		print ("Number of geometry assigned: " + counting);
	}

	//set the color of the geometry for temperature data
	//really only works for one color at a time
	void bmsColorChange(){
		foreach (BmsSpace s in spaces) {
			if (s.GetHasData() == true) {
				foreach (GameObject geo in s.geometry) {
					Renderer rend = geo.GetComponent<Renderer> ();
					Color mycolor = g.Evaluate (s.evaluateT);
					rend.material.color = mycolor;
					//print ("the color for space " + s.name  + " has been changed to " + geo.GetComponent<Renderer> ().material.color.ToString ());
				}
				print ("the temp for space "+ s.name  + " is set to " + s.temp);
			}
		}
	}

	void AssignTemp(){
		foreach (BmsSpace s in spaces) {
			string number = s.name.Substring(4,4);
			//checks the room held in roomTemps
			foreach (IaTemperature ia in gameObject.GetComponent<Parser>().roomTemps) {
				string str = ia.room.Substring (ia.room.Length - 4, 4);

				if (number == str && ia.isCurrent == true) {
					s.temp = ia.temperature;
					s.hasData = true;
					print (s.name + " has temperature " + s.temp);
				}
			}
			//checks for the array of the room temps
			foreach (IaTemperature[] a in gameObject.GetComponent<Parser>().arrayofRmsTemps) {
				foreach (IaTemperature i in a) {
					string str = i.room.Substring (i.room.Length - 4, 4);
					if (number == str && i.isCurrent == true) {
						s.temp = i.temperature;
						s.hasData = true;
						print (s.name + " has temperature " + s.temp);
					}
				}
			}
		}
	}

	//variables for CheckTemp
	public HighLows tempHL;
	float tempEvaluate;

	//function for taking the temp and returning an evaluation (inverse lerp) value for input
	void CheckTemp (){
		float highT = 25;
		float lowT = 20;
		tempHL = new HighLows (highT,lowT);

		foreach (BmsSpace s in spaces) {
			if (s.hasData == true) {
				s.evaluateT = Mathf.InverseLerp (lowT, highT, s.temp);
				//print ("We have a temp evaluate number at: " + s.evaluateT + " for " + s.name);
			}
		}
	}

	void AddTriggerAndClick(){
		foreach (BmsSpace s in spaces) {
			if (s.hasData == true) {
				if (s.geometry != null) {
					//we only want to add trigger to one of the geometries
					s.geometry[0].AddComponent<MeshCollider> ();
					s.geometry[0].GetComponent<MeshCollider> ().convex = true;
					s.geometry[0].GetComponent<MeshCollider>().isTrigger =true;

					s.geometry[0].AddComponent<OnRoomClick> ();
					print ("The geometry " + s.geometry[0].name + " spaces "+s.name + " had a trigger and mesh collider and on click script added");

				}
			}
		}
	}
}
