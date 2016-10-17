using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TextureScript : MonoBehaviour {

	public Texture2D a;
	Color[] colors;
	int NuCol;

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


		a.SetPixels(colors);
		//a.SetPixels(bigColorArray);
		a.filterMode = FilterMode.Bilinear;
		a.wrapMode = TextureWrapMode.Clamp;
		a.Apply();
		if (GetComponent<Renderer> ()) {
			GetComponent<Renderer> ().material.mainTexture = a;
		}

		//for Unity UI need a sprite instead of texture (size needs to match texture2D)
		Sprite mySprite = Sprite.Create(a,new Rect(0,0,NuCol,height),new Vector2(0,0),40);
		if (GetComponent<Image> ()) {
			GetComponent<Image> ().sprite = mySprite;
			GetComponent<Image> ().material.mainTexture = a;
		}	
	}

}
