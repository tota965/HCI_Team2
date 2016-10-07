using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;


public class TextureScript : MonoBehaviour {

	public Texture2D a;
	Color[] colors;
	int NuCol;

	//can use OnGui if wanted
//	void OnGUI() {
//		NuCol = 5;
//		a = new Texture2D(NuCol*100,1);
//		Color[] colors = new Color[NuCol];
//		colors[0] = Color.red;
//		colors[1] = Color.blue;
//		colors[2] = Color.green;
//		colors[3] = Color.yellow;
//		colors[4] = Color.red;
//
//		a.SetPixels(colors);
//		a.filterMode = FilterMode.Bilinear;
//		a.wrapMode = TextureWrapMode.Clamp;
//		a.Apply();
//		GUI.DrawTexture(new Rect(10, 10, 100, 20), a);
//
//	}


	void Start() {
		NuCol = 5;
		int height = 1;
		a = new Texture2D(NuCol,height);
		colors = new Color[NuCol];
		colors[0] = Color.red;
		colors[1] = Color.blue;
		colors[2] = Color.green;
		colors[3] = Color.yellow;
		colors[4] = Color.red;

//		int lengthWidth = NuCol * 20 * 1;
//		Gradient g = new Gradient ();
//		GradientColorKey[] gck = new GradientColorKey[5];
//		gck [0].color = Color.red;
//		gck [1].color = Color.blue;
//		gck [2].color = Color.green;
//		gck [3].color = Color.yellow;
//		gck [4].color = Color.red;
//		gck [0].time = 0.0f;
//		gck [1].time = 0.25f;
//		gck [2].time = 0.5f;
//		gck [3].time = 0.75f;
//		gck [4].time = 1.0f;
//		GradientAlphaKey[] gak = new GradientAlphaKey[2];
//		gak[0].alpha=1.0f;
//		gak [1].alpha = 1.0f;
//		gak[0].time=0.0f;
//		gak [1].time = 1.0f;
//		//set the keys
//		g.SetKeys(gck,gak);
//		Color[] bigColorArray = new Color[lengthWidth];
//		print ("The length of the bigcolorarray is " + bigColorArray.Length);
//		for (int i = 0; i < bigColorArray.Length; i++) {
//			float value = 0.000f;
//			//value = (i / (bigColorArray.Length - 1));
//			bigColorArray [i] = g.Evaluate (i / (bigColorArray.Length - 1));
//
//
//			print ("color " + bigColorArray [i] + " assigned for " + (i+1) + " and evaluated to be: " + g.Evaluate (i / (bigColorArray.Length - 1)));
//		}


		a.SetPixels(colors);
		//a.SetPixels(bigColorArray);
		a.filterMode = FilterMode.Bilinear;
		a.wrapMode = TextureWrapMode.Clamp;
		a.Apply();
		if (GetComponent<Renderer> ()) {
			GetComponent<Renderer> ().material.mainTexture = a;
			AssetDatabase.CreateAsset (GetComponent<Renderer> ().material, "Assets/Materials/" + "ScaleMaterial.mat");
		}

		//for Unity UI need a sprite instead of texture (size needs to match texture2D)
		Sprite mySprite = Sprite.Create(a,new Rect(0,0,NuCol,height),new Vector2(0,0),40);
		if (GetComponent<Image> ()) {
			GetComponent<Image> ().sprite = mySprite;
			GetComponent<Image> ().material.mainTexture = a;
		}	
	}


}
