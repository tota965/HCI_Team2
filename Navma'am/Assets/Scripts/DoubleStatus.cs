using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoubleStatus : MonoBehaviour {

	public GameObject RoomPanelText;
	static string startStatus;
	static string startRoomStatus;

	GameObject button;

	void OnEnable()
	{
		OnRoomClick.OnDClicked += TextUpdate;
		OnRoomClick.OnDClicked += ViewButton;
		OnRoomClick.OnDClicked += RoomTextUpdate;

	}

	void OnDisable()
	{
		OnRoomClick.OnDClicked -= TextUpdate;
		OnRoomClick.OnDClicked -= ViewButton;
		OnRoomClick.OnDClicked -= RoomTextUpdate;
	}

	void Start(){
		startStatus = gameObject.GetComponent<Text> ().text;
		startRoomStatus = RoomPanelText.GetComponent<Text> ().text;
		button = GameObject.Find ("View Graph (1)");
		button.SetActive (false);
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.C)) {
			gameObject.GetComponent<Text> ().text = startStatus;
			RoomPanelText.GetComponent<Text> ().text = startRoomStatus;
		}
	}

	//Display status for what room the Camera focuses on... using onclick script to find out what the object is
	//could be more elegant (access from camera)
	//also uses substring of name to access the room number (last 4 digits of name)
	//doesn't update yet when switching to main camera
	string rNo;
	void TextUpdate(){
		print ("the event was executed for " + gameObject.name);
		//Transform ct = Camera.main.gameObject.GetComponent<MouseOrbit> ().target;
		GameObject go = OnRoomClick.cgo;
		rNo = go.name.Substring (go.name.Length - 4, 4);
		if(go) 
			gameObject.GetComponent<Text> ().text = "Current Room: " + rNo;
	}

	//Add text to the Summary for Room
	void RoomTextUpdate(){

		GameObject go = OnRoomClick.cgo;

		rNo = go.name.Substring (go.name.Length - 4, 4);
		if (go) {
			string text = "Summary for Room ";
			RoomPanelText.GetComponent<Text> ().text = text + rNo;
			text = RoomPanelText.GetComponent<Text> ().text;
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();
			sb.AppendLine (text);

			float CurrentT = 0.0f;
			foreach (BmsSpace s in roomManager.spaces) {
				string number = s.name.Substring(4,4);
				if (number == rNo) {
					CurrentT = s.temp;
				}
			}

			sb.AppendLine ("\nCurrent Temperature: " + CurrentT +"C");
			sb.AppendLine ("\nCurrent Fan Status: " + "On");
			sb.AppendLine ("\nCurrent CO2: " + 450 + "ppm");
			sb.AppendLine ("\nGraph: " + "Temperature");
			text = sb.ToString();

			RoomPanelText.GetComponent<Text> ().text = text;
		}
	}

	void ViewButton(){
		button.SetActive (true);
		if(rNo == "2003")
		{
			GameObject data = GameObject.Find ("DataManager");
			button.GetComponent<Button> ().onClick.AddListener (data.GetComponent<roomManager> ().TogglePanel);
		}
	}
}
