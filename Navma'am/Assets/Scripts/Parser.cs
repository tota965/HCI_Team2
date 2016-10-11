using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Globalization;
using System.Collections.Generic;

//generic parser for csv data

public class Parser : MonoBehaviour {

	private TextAsset[] datas;
	private TextAsset data;

	public List<IaTemperature> listofRmTemps = new List<IaTemperature>();
	public IaTemperature[][] arrayofRmsTemps;
	public IaTemperature[] roomTemps;

	public string roomName = "Rm2003";
	public string[] roomNames = {"Rm1001" , "Rm3047"}; //will need to be in the same order that the computer reads the text files in...
	CultureInfo provider = new CultureInfo("en-NZ");

	//setup for the graphs
	public GameObject emptyGraphPrefab;
	public static WMG_Axis_Graph graph;
	public WMG_Series series1; //for one series of room data
	public WMG_Series series2; //for set points
	public WMG_Series series3;
	public WMG_Series series4;
	public List<Vector2> series1Data;
	public List<Vector2> series2Data;
	public List<Vector2> series3Data;
	public List<Vector2> series4Data;
	public GameObject canvas;
	public int points = 120;
	public GameObject graphGO;

	void Awake () {
		
		LoadRoomName ();

		LoadAllRoomData ();

		//this looks up the current reading and prints out how many are assigned current for each room just loaded
		foreach (IaTemperature a in roomTemps) {
			int count = 0;
			if (a.isCurrent == true) {
				
				string str = a.room.Substring (a.room.Length - 4, 4);
				Debug.Log ("Room " + str + ": " + a.timestamp + " is the most current reading : " + a.temperature);
				//we need to set the space which is associated with this current temp to have this value

				//make some error if there is anything other 1 assigned
				count++;
				if (count>1)
					Debug.LogError ("Room number of current temp is: " + str + " has " + count + " current temperatures");
				if (count == 0)
					Debug.LogError ("no current temperature assigned for room " + str);
				
			}
		}
		foreach (IaTemperature[] ia in arrayofRmsTemps) {
			foreach (IaTemperature a in ia) {
				int count = 0;
				if (a.isCurrent == true) {
					
					string str = a.room.Substring (a.room.Length - 4, 4);
					Debug.Log ("Room " + str + ": " + a.timestamp + " is the most current reading : " + a.temperature);
					//we need to set the space which is associated with this current temp to have this value

					count++;
					//make some error if there is anything other 1 assigned
					if (count > 1)
						Debug.LogError ("Room number of current temp is: " + str + " has " + count + " current temperatures");
					if (count == 0)
						Debug.LogError ("no current temperature assigned for room " + str);
				}
			}
		}

		//add some check for the error time or temp readings to remove those points...
	}

	Gradient g;
	public InputField input;

	void Start()
	{
		//setup for graphs
		canvas = GameObject.Find ("RoomInfoPanel");
		graphGO = GameObject.Instantiate (emptyGraphPrefab);
		//test attaching graph to a panel
		graphGO.transform.SetParent (canvas.transform, false);
		graph = graphGO.GetComponent<WMG_Axis_Graph> ();
		series1 = graph.addSeries ();
		series2 = graph.addSeries ();
		series3 = graph.addSeries ();
		series4 = graph.addSeries ();
		MakeSeriesList ();
		series1.pointValues.SetList (series1Data);
		series2.pointValues.SetList (series2Data);
		series3.pointValues.SetList (series3Data);
		series4.pointValues.SetList (series4Data);
		series1.seriesName = "Room 2003 Temp";
		series2.seriesName = "Setpoint";
		series3.seriesName = "Maximum Temp";
		series4.seriesName = "Minimum Temp";

		g = new Gradient ();
		GradientColorKey[] gck = gameObject.GetComponent<roomManager> ().g.colorKeys;//get colors not the alphas of the room gradient
		//create the alpha keys (100% alpha)
		GradientAlphaKey[] gak = new GradientAlphaKey[2];
		gak[0].alpha=1.0f;
		gak [1].alpha = 1.0f;
		gak[0].time=0.0f;
		gak [1].time = 1.0f;
		//set the keys
		g.SetKeys(gck,gak);

		float eval = Mathf.InverseLerp (lt, ht, sp); //assuming lt and ht correspond to those numbers in room Manager script

		series2.lineColor = g.Evaluate (eval);
		series2.pointColor = g.Evaluate (eval);
		series3.lineColor = g.Evaluate (1);
		series3.pointColor = g.Evaluate (1);
		series4.lineColor = g.Evaluate (0);
		series4.pointColor = g.Evaluate (0);

		//used to investigate how the labels are made in Graph_Maker package
//		List<WMG_Node> myLabels = new List<WMG_Node>();
//		myLabels = graph.xAxis.GetAxisLabelNodes ();
//
//		print ("there are " + myLabels.Count + " x axis labels");
//		foreach (WMG_Node n in myLabels) {
//			print ("here is a list of labels for x axis ");
//		}
		input.onEndEdit.AddListener(SetPoints);
	}

	/// <summary>
	/// Function to toggle the active state of the graph.
	/// </summary>
	public void ToggleGraph ()
	{
		bool mybool = graphGO.activeSelf;
		graphGO.SetActive (!mybool);
	}


	//setup for series2,series3, and series4:
	float sp = 23;//set point for room (BMSspace check needed here)
	float ht = 25;//high temp range
	float lt = 20;//low temp range

	//series for one graph with setpoints,ht,and lt
	void MakeSeriesList(){
		//first reset the data?
		print ("Series 1 Data has " +series1Data.Count + " items to start");
		int test = 0;
		for (int i = 0; i < points; i++) {
			if (i == 0) {
				series1Data.Clear ();
				series2Data.Clear ();
				series3Data.Clear ();
				series4Data.Clear ();
			}
			if (roomTemps [i].temperature != -100) {
				float y = roomTemps [i].temperature;


				DateTime x = roomTemps [i].timestamp;

				try { 
					float xx = Convert.ToSingle (x.ToOADate ()); //not sure if there is another way to cast...
					Vector2 v = new Vector2 (xx, y);
					series1Data.Add (v);

					//add line for set point, and ranges each with only two values (first and last)
					if (series1Data.Count == 1) {
						//add line for set point, and ranges
						Vector2 b = new Vector2 (xx, sp);
						Vector2 c = new Vector2 (xx, ht);
						Vector2 d = new Vector2 (xx, lt);
						series2Data.Add (b);
						series3Data.Add (c);
						series4Data.Add (d);
					}
					//last number minus any that were set to invalid temperature
					if (series1Data.Count == points - test) {
						Vector2 b = new Vector2 (xx, sp);
						Vector2 c = new Vector2 (xx, ht);
						Vector2 d = new Vector2 (xx, lt);
						series2Data.Add (b);
						series3Data.Add (c);
						series4Data.Add (d);
					}

				} catch (OverflowException) {
					print ("We can't convert the datetime for time: " + roomTemps [i].timestamp + " in room: " + roomTemps [i].room);
				}
			} else
				test++; //count of invalid temperatures
		}
		print ("Series 1 Data has " +series1Data.Count + " items after load");
	}

	public void RefreshGraph()
	{
		
		if (graph) {
			MakeSeriesList ();
			series1.pointValues.SetList (series1Data);
			series2.pointValues.SetList (series2Data);
			series3.pointValues.SetList (series3Data);
			series4.pointValues.SetList (series4Data);
			//graph.Refresh (); //not needed, just need to reset the series data with the above line

			List<WMG_Node> myLabels = new List<WMG_Node> ();
			myLabels = graph.xAxis.GetAxisLabelNodes ();

			WMG_List<string> myOtherLabels = new WMG_List<string> ();
			myOtherLabels = graph.xAxis.axisLabels;

			print ("there are " + myLabels.Count + " x axis labels");
//			foreach (WMG_Node n in myLabels) {
//				print ("here is a list of labels for x axis " + n);
//			}

				for(int i = 0; i< myOtherLabels.Count;i++){
					print ("here is a list of labels for x axis before " + myOtherLabels [i]);
					myOtherLabels [i] = "Updated";
					print ("here is a list of labels for x axis after" + myOtherLabels [i]);
				}

		}
	}

	//for only the string roomName
	void LoadRoomName ()
	{
		data = Resources.Load (roomName) as TextAsset;
		string input = data.text;
		string[] lines = input.Split (new[] {
			'\r',
			'\n'
		}, System.StringSplitOptions.RemoveEmptyEntries);
		int p = lines.Length - 1;
		//there are the same number of events as number of lines subtracting the header
		roomTemps = new IaTemperature[p];
		Debug.Log ("number of datapoints in room " + roomName + " array: " + roomTemps.Length);
		//adjust to remove the header from the sequence...
		for (int i = 1; i <= p; i++) {
			string[] col = lines [i].Split (new[] {
				','
			});
			//set temp to some value which shows there is an error in reading:
			float temp = -100;
			//sets the inside air temp of room 1001 for the first three columns
			//cast column 3 as float
			try {
				temp = Convert.ToSingle (col [2]);
			}
			catch (FormatException) {
				Debug.Log ("Problem converting column temp for line " + (i + 1) + ". Check data");
			}
			catch (OverflowException) {
				Debug.Log ("overflow");
			}
			//lets try to create a DateTime instead of double in OA format (works but using culture info to understand nz time source)
			//DateTime newTime = DateTime.FromOADate(time);
			//Debug.Log ("at line " + (i + 1) + " we have a DateTime value " + newTime + " associated with " + time);
			DateTime newTime = DateTime.Parse (col [0], provider);
			//check if datetime is associated correctly and reading correctly
			//Debug.Log ("at line " + (i +1) + " we have a DateTime value " + newTime + " associated with " + col[0]);
			roomTemps [i - 1] = new IaTemperature (roomName, newTime, temp, false);
			//still have compressor, fans, occupancy, and scheduled available and unused
			//for the first instance of timestamp we want to set it to current, or if the previous datapoint had an error in the temp reading
			//for every instance of our array of temps for the room want to compare it with the previous to see if it has a greater time (works with ascending data)
			if (i == 1 || roomTemps [i - 2].temperature == -100) {
				roomTemps [i - 1].isCurrent = true;
			}
			else
				if (roomTemps [i - 1].timestamp > roomTemps [i - 2].timestamp) {
					roomTemps [i - 1].isCurrent = true;
					roomTemps [i - 2].isCurrent = false;
				}
			//if we set this temp to be most current, unset the last one as current. (works with decending data)
			if (roomTemps [i - 1].isCurrent == true) {
				if (i > 1) {
					roomTemps [i - 2].isCurrent = false;
				}
			}
			//debug which line has what values
			//print(roomTemps[i-1].ToString());
		}
	}

	void LoadAllRoomData ()
	{
		//these need to be in the room data folder:
		//TextAsset data1 = Resources.Load ("RoomData/"roomNames [0]) as TextAsset;
		//TextAsset data2 = Resources.Load ("RoomData/"roomNames [1]) as TextAsset;
		 
		datas = Resources.LoadAll<TextAsset> ("RoomData");

		int num = datas.Length;
		print ("This many text assets were loaded: " + datas.Length);
		arrayofRmsTemps = new IaTemperature[num][];
		for (int i = 0; i < num; i++) {
			string inputx = datas [i].text;
			string[] linesx = inputx.Split (new[] {'\r','\n'}, System.StringSplitOptions.RemoveEmptyEntries);
			int q = linesx.Length - 1;
			//there are the same number of events as number of lines subtracting the header
			arrayofRmsTemps [i] = new IaTemperature[q];
			//roomTemps = new IaTemperature[q];
			Debug.Log ("number of datapoints in room " + datas [i].name + " array: " + arrayofRmsTemps [i].Length);
			for (int o = 1; o <= q; o++) {
				string[] col = linesx [o].Split (new[] {','});
				float temp = -100;
				try {
					temp = Convert.ToSingle (col [2]);
				}
				catch (FormatException) {
					Debug.Log ("Problem converting column temp for line " + (o + 1) + ". Check data");
				}
				catch (OverflowException) {
					Debug.Log ("overflow");
				}
				DateTime newTime = DateTime.Parse (col [0], provider);
				arrayofRmsTemps [i] [o - 1] = new IaTemperature (roomNames [i], newTime, temp, false);

				//for the first instance of timestamp we want to set it to current, or if the previous datapoint had an error in the temp reading
				//for every instance of our array of temps for the room want to compare it with the previous to see if it has a greater time (works with ascending data)
				if (o == 1 || arrayofRmsTemps [i] [o - 2].temperature == -100) {
					arrayofRmsTemps [i] [o - 1].isCurrent = true;
				}
				else
					if (arrayofRmsTemps [i] [o - 1].timestamp > arrayofRmsTemps [i] [o - 2].timestamp) {
						arrayofRmsTemps [i] [o - 1].isCurrent = true;
						arrayofRmsTemps [i] [o - 2].isCurrent = false;
					}
				//if we set this temp to be most current, unset the last one as current. (works with decending data)
				if (arrayofRmsTemps [i] [o - 1].isCurrent == true) {
					if (o > 1) {
						arrayofRmsTemps [i] [o - 2].isCurrent = false;
					}
				}
				//debug which line has what values
				//print(roomTemps[i-1].ToString());

				//debug which line has what values
				//print (arrayofRmsTemps [i] [o - 1].ToString ());
			}
		}
	}

	void Update(){
		
	}

	public void SetPoints(string s){
		int i;
		Int32.TryParse (s, out i);
		print ("Input is: " + s);
		i = Mathf.Clamp (i, 1, 481);//make sure the value is within the range of the dataset
		points = i;
		RefreshGraph ();
	}
}
