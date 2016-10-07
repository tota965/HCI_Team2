using UnityEngine;
using System.Collections;
using System;

//class for bmsSpace, ifcEntities, and...

public class BmsSpace {
	//for counting
	public static int numOfSpaces;
	//use name of room
	public string name;
	//geometry
	public GameObject[] geometry; 
	//use ifc GUID
	public string ifcGUID;
	//area of space
	public float area;
	//reference floor number
	public int floor;
	//type of space *can depend on how spaces are used, for this implementation, they are room types (offices, etc.)
	public string type;
	public string ifctype;
	//unit for area
	public string areaUnit;
	//does this object actually have data?
	public bool hasData;
	//datatypes we might want: temperature, humidity, airFlow, heat, co2, fire, gas, light, movement, presesure, smoke, sound, particulate
	public float temp;
	//for use to evaluate where the temperature stands on the gradient:
	public float evaluateT;
	public float humidity;
	public float cotwo;
	public float airFlow;
	public bool isFanOn;
	public bool isComp1On;
	public bool isComp2On;
	public bool isRevValveOn;
	//temperature reference
	public float setPoint;
	//occupants
	public int occupancy;
	//is the space in scheduled or unscheduled mode
	public bool isScheduled;

	//default is that a space doesn't have data
	public BmsSpace (string s, string i, string t, int fl, float a, string it)
	{
		name = s;
		ifcGUID = i;
		type = t;
		area = a;
		floor = fl;
		ifctype = it;

		hasData = false;
		//probably should have a destroy function as well to decrease the number as spaces become unavailable.
		numOfSpaces++;
	}

	//constructor with hasdata specified
	public BmsSpace (string s, string i, string t, int fl, float a, string it, bool hd)
	{
		name = s;
		ifcGUID = i;
		type = t;
		area = a;
		floor = fl;
		ifctype = it;

		hasData = hd;


		numOfSpaces++;
	}

	public bool GetHasData(){
		return hasData;
	}

	public void SetHasData(bool x){
		hasData = x;
	}

	public override string ToString()
	{
		return "Space " + name + ": GUID: " + ifcGUID  + ", type: " + type  + ", Floor: " + floor + ", Area: " + area  + ", Data: " + hasData;
	}

}

public struct IaTemperature {

	public string room;
	public DateTime timestamp;
	public float temperature;
	//public string unit;
	public bool isCurrent;

	public IaTemperature (string r, DateTime dt, float t, bool c) {
		room = r;
		timestamp = dt;
		temperature = t;
		//unit = u;
		isCurrent = c;
	}

	public override string ToString ()
	{
		return "Room: " + room + ", Time: " + timestamp + ", Temp: " + temperature + ", Is Current: " + isCurrent;
	}
} 


public struct OaTemperature{
	public string location;
	public float temperature;
	public float humidity;
	public bool isCurrent;

	public OaTemperature (string s, float t, float h, bool c){
		location = s;
		temperature = t;
		humidity = h;
		isCurrent = c;
	}

	public override string ToString ()
	{
		return "Location: " + location + " Temp: " + temperature + " Is Current: " + isCurrent;
	}
}

//this class can be used to store varying temperature and humidity ranges, such as weekly/monthly/yearly average low/highs
//as well as the comfort band high lows in either temp or humidity for chart viewing
public class HighLows {
	public float highT;
	public float lowT;
	public float highH;
	public float lowH;
	//constructor for only temperature
	public HighLows (float ht,float lt){
		highT = ht;
		lowT = lt;
	}
	//constructor for both temperature and humidity
	public HighLows (float ht,float lt,float hh, float lh){
		highT = ht;
		lowT = lt;
		highH = hh;
		lowH = lh;
	}
}

public class CamPosSave {

	private static Vector3 myCamPos;
	public static Vector3 MyCamPos {
		get { return myCamPos; }
		set { myCamPos = value; }
	}

	private static Vector3 mycamrot;
	public static Vector3 MyCamRot {
		get { return mycamrot; }
		set { mycamrot = value; }
	}
}