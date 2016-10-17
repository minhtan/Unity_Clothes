using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class Capture : MonoBehaviour {
	[SerializeField] string deviceName;
	public RawImage webcamTexture;
	public RawImage processedTexture;

	WebCamTexture wct;

	// Use this for initialization
	void Start () {
		WebCamDevice[] devices = WebCamTexture.devices;
		deviceName = devices[0].name;
		wct = new WebCamTexture(deviceName, 1080, 720, 12);
		webcamTexture.texture = wct;
		wct.Play();
	}

	void OnGUI() {      
		if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
			RemoveBg();

	}

	void RemoveBg()
	{
		Texture2D snap = new Texture2D(wct.width, wct.height);
		var bits = wct.GetPixels ();

		snap.SetPixels(bits);
		snap.Apply();
		SaveTextureToFile (snap, "0");

		var colorToRemove = bits[0];
		for (int i = 0; i < bits.Length; i++) {
			if ( CheckColor(bits[i], colorToRemove) ) {
				bits [i] = Color.clear;
			}
		}

		snap.SetPixels(bits);
		snap.Apply();
		SaveTextureToFile (snap, "1");

		processedTexture.texture = snap;
	}

	bool CheckColor(Color color1, Color color2){
		var threshold = 0.3f;
		bool isRedGood = Mathf.Abs(color1.r - color2.r) <= threshold;
		bool isGreenGood = Mathf.Abs(color1.g - color2.g) <= threshold;
		bool isBlueGood = Mathf.Abs(color1.b - color2.b) <= threshold;
		bool isAlphaGood = Mathf.Abs(color1.a - color2.a) <= threshold;
		return isRedGood && isGreenGood && isBlueGood && isAlphaGood;
	}

	void SaveTextureToFile( Texture2D texture, string fileName)
	{
		Debug.Log (Application.dataPath);
		System.IO.File.WriteAllBytes(Application.dataPath + "/"+fileName+".png", texture.EncodeToPNG()); 
	}
}