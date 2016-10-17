using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class Capture : MonoBehaviour {
	[SerializeField] string deviceName;
	public RawImage webcamTexture;
	public RawImage processedTexture;
	Texture2D texture;
	WebCamTexture wct;

	// Use this for initialization
	void Start () {
		
	}

	void LoadFromResource(){
		texture = Resources.Load<Texture2D> ("0");
		var bits = texture.GetPixels ();
		bits = ReverseXPixels (bits, texture.width, texture.height);
		texture.SetPixels(bits);
		texture.Apply();
		var tempText = texture;
		webcamTexture.texture = tempText;
	}

	void LoadFromResource2(){
		texture = Resources.Load<Texture2D> ("test");
		var bits = texture.GetPixels ();
		bits = ReverseXPixels (bits, texture.width, texture.height);
		texture.SetPixels(bits);
		texture.Apply();

		webcamTexture.texture = texture;
	}
		
	bool StartWebcam(){
		WebCamDevice[] devices = WebCamTexture.devices;
		if (devices.Length > 0) {
			deviceName = devices [0].name;
			wct = new WebCamTexture (deviceName, 1280, 720, 12);
			texture = new Texture2D(wct.width, wct.height);
			webcamTexture.texture = texture;
			wct.Play ();
			return true;
		} else {
			return false;
		}
	}

	void Update(){
		if (wct != null && texture != null) {
			var bits = wct.GetPixels ();
			bits = ReverseXPixels (bits, wct.width, wct.height);

			texture.SetPixels(bits);
			texture.Apply();
		}
	}

	void OnGUI() {      
		if (GUI.Button (new Rect (10, 10, 100, 30), "Start webcam"))
			StartWebcam ();

		if (GUI.Button (new Rect (10, 50, 100, 30), "rm bg"))
			RemoveBgFromWebcamTexture ();

		if (GUI.Button (new Rect (Screen.width / 2, 10, 100, 30), "Reload"))
			SceneManager.LoadScene (0);

		if (GUI.Button (new Rect (Screen.width - 110, 10, 100, 30), "Load res"))
			LoadFromResource ();

		if (GUI.Button (new Rect (Screen.width - 110, 50, 100, 30), "Load test"))
			LoadFromResource2 ();

		if (GUI.Button (new Rect (Screen.width - 110, 90, 100, 30), "rm bg"))
			RemoveBgFromTexture ();


	}

	void RemoveBgFromWebcamTexture()
	{
		Texture2D texture = new Texture2D(wct.width, wct.height);
		var bits = wct.GetPixels ();

		texture.SetPixels(bits);
		texture.Apply();
		SaveTextureToFile (texture, "0");

		var colorToRemove = bits[0];
		for (int i = 0; i < bits.Length; i++) {
			if ( CheckColor(bits[i], colorToRemove) ) {
				bits [i] = Color.clear;
			}
		}

		texture.SetPixels(bits);
		texture.Apply();
		SaveTextureToFile (texture, "1");

		processedTexture.texture = texture;
	}

	void RemoveBgFromTexture()
	{
		var bits = texture.GetPixels ();

		var colorToRemove = bits[0];
		for (int i = 0; i < bits.Length; i++) {
			if ( CheckColor(bits[i], colorToRemove) ) {
				bits [i] = Color.clear;
			}
		}

		texture.SetPixels(bits);
		texture.Apply();
		SaveTextureToFile (texture, "2");

		processedTexture.texture = texture;
	}

	Color[] ReverseXPixels(Color[] bits, int w, int h){
		Color[] ret = new Color[w * h];

		for (int i = 0; i < h; i++) {
			for (int j = 0; j < w; j++) {
				ret[i * w + j] = bits[(w - j - 1) + (w * i)];
			}
		}

		return ret;
	}

	bool CheckColor(Color color1, Color color2){
		var threshold = 0.2f;
		bool isRedGood = Mathf.Abs(color1.r - color2.r) <= threshold;
		bool isGreenGood = Mathf.Abs(color1.g - color2.g) <= threshold;
		bool isBlueGood = Mathf.Abs(color1.b - color2.b) <= threshold;
		bool isAlphaGood = Mathf.Abs(color1.a - color2.a) <= threshold;
		return isRedGood && isGreenGood && isBlueGood && isAlphaGood;
	}

	void SaveTextureToFile( Texture2D texture, string fileName)
	{
		Debug.Log (Application.dataPath);
		System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/"+fileName+".png", texture.EncodeToPNG()); 
	}
}