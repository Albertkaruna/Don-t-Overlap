//***************************************************************//
// ScribblePhysicsGUI
//***************************************************************//
//
// The GUI script for the test scene
//
//***************************************************************//

//***************************************************************//
//
using UnityEngine;
using System.Collections;

//
////***************************************************************//

public class ScribblePhysicsGUI : MonoBehaviour
{
	
		
	//***************************************************************//
	// Public variables:
	public GameObject cameraObject;
	public Material[] textureMaterial;
	public PhysicMaterial[] physicsMaterial;
	//
	// Private variables:
	private ScribblePhysicsMainScript SPMS;
	private bool clearDrawingMenuEnabled = false;
	private bool textureMaterialMenuEnabled = false;
	private bool physicsMaterialMenuEnabled = false;
	private string FPS;
	private float FPSupdateTime;
	private bool screenSettingsMenuEnabled = false;
	//
	//***************************************************************//
	
	
	//***************************************************************//
	//
	void Start ()
	{
		// if no cameraObject is given by the user, 
		// then assume that the camera is attached to the current gameObject
		if (!cameraObject) {
			cameraObject = gameObject;
		}
		SPMS = cameraObject.GetComponent<ScribblePhysicsMainScript> ();
	}
	//
	//***************************************************************//
	
	//***************************************************************//
	//
	void OnGUI ()
	{
		int w = Screen.width;
		int h = Screen.height;
		float nb = 6; 		// number of buttons to display;
		float bn = 0; 		// current button number (from left to right)
		float bw = w / nb;	// button width
		float bh = 40; 		// button heigth
		float bd = 5; 		// button distance to edge
	
		SPMS.enabled = true;
		SPMS.canvasRect = new Rect (0, 0, w, h - bh - bd);
	
		int i;
	
		// Frames Per Second
		if (Time.time > FPSupdateTime) {
			FPS = "FPS: " + (1f / Time.deltaTime).ToString ("#.00");
			FPSupdateTime = Time.time + 0.5f; //update every 0.5 seconds
		}
		GUI.Box (new Rect (w * 0.5f - 40f, h - 20f, 80f, 20f), FPS);
	 
	
		// MENU: Static Or Dynamic Menu
		bn = 0f;
		if (SPMS.scribbleIsDynamic && GUI.Button (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Scribble = \nDynamic")) {
			SPMS.scribbleIsDynamic = false;	
		}
		if (!SPMS.scribbleIsDynamic && GUI.Button (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Scribble = \nStatic")) {
			SPMS.scribbleIsDynamic = true;	
		}
	
		// MENU: width
		bn = 1f;
		GUI.Box (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Width = " + SPMS.lineWidth.ToString ("#.0"));
		if (GUI.Button (new Rect (bn * bw + bd, 0.5f * bh + bd, 0.5f * bw - bd, 0.5f * bh), "-")) {
			SPMS.lineWidth -= 0.1f;
		}
		if (GUI.Button (new Rect ((bn + 0.5f) * bw, 0.5f * bh + bd, 0.5f * bw - bd, 0.5f * bh), "+")) {
			SPMS.lineWidth += 0.1f;
		}
		SPMS.lineWidth = Mathf.Clamp (SPMS.lineWidth, 0.1f, 2f);
		
		// MENU: density
		bn = 2f;
		GUI.Box (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Density = " + SPMS.density.ToString ("#.0"));
		if (GUI.Button (new Rect (bn * bw + bd, 0.5f * bh + bd, 0.5f * bw - bd, 0.5f * bh), "-")) {
			SPMS.density -= 0.1f;
		}
		if (GUI.Button (new Rect ((bn + 0.5f) * bw, 0.5f * bh + bd, 0.5f * bw - bd, 0.5f * bh), "+")) {
			SPMS.density += 0.1f;
		}
		SPMS.density = Mathf.Clamp (SPMS.density, 0.1f, 5.0f);

		// MENU: Texture Material
		bn = 3f;
		string currentTextureMaterial;
		if (SPMS.textureMaterial) {
			currentTextureMaterial = SPMS.textureMaterial.name;
		} else {
			currentTextureMaterial = "none";
		}
		if (GUI.Button (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Texture Material = \n" + currentTextureMaterial)) {
			textureMaterialMenuEnabled = !textureMaterialMenuEnabled;
		}
		if (textureMaterialMenuEnabled) {
			cameraObject.GetComponent<ScribblePhysicsMainScript> ().enabled = false;
			i = 0;
			foreach (Material aTextureMaterial in textureMaterial) {
				if (GUI.Button (new Rect (bn * bw + bd, (i + 1) * bh + (i + 2) * bd, bw - 2f * bd, bh), aTextureMaterial.name)) {
					textureMaterialMenuEnabled = !textureMaterialMenuEnabled;
					SPMS.textureMaterial = aTextureMaterial;
				}
				i++;
			}
		}
	
		
		// MENU: Physics Material
		bn = 4f;
		string currentPhysicsMaterial;
		if (SPMS.physicsMaterial) {
			currentPhysicsMaterial = SPMS.physicsMaterial.name;
		} else {
			currentPhysicsMaterial = "none";
		}
		
		if (GUI.Button (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Physics Material = \n" + currentPhysicsMaterial)) {
			physicsMaterialMenuEnabled = !physicsMaterialMenuEnabled;
		}
		if (physicsMaterialMenuEnabled) {
			SPMS.enabled = false;
			i = 0;
			foreach (PhysicMaterial aPhysicsMaterial in physicsMaterial) {
				if (GUI.Button (new Rect (bn * bw + bd, (i + 1) * bh + (i + 2) * bd, bw - 2f * bd, bh), aPhysicsMaterial.name)) {
					physicsMaterialMenuEnabled = !physicsMaterialMenuEnabled;
					SPMS.physicsMaterial = aPhysicsMaterial;
				}
				i++;
			}
		}
		
		// MENU: Intersection Allowed
		bn = 5f;
		if (SPMS.intersectionAllowed && GUI.Button (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Intersection =\n Allowed")) {
			SPMS.intersectionAllowed = false;	
		}
		if (!SPMS.intersectionAllowed && GUI.Button (new Rect (bn * bw + bd, 0f + bd, bw - 2f * bd, bh), "Intersection =\nNot Allowed")) {
			SPMS.intersectionAllowed = true;	
		}
	
		// MENU: Clean
		bn = 0f;
		if (GUI.Button (new Rect (bn * bw + bd, h - bd - bh, bw - 2f * bd, bh), "Clear\n")) {
			clearDrawingMenuEnabled = !clearDrawingMenuEnabled;
		}
		if (clearDrawingMenuEnabled) {
			SPMS.enabled = false;
			GameObject[] allGameObjectsList = FindObjectsOfType (typeof(GameObject)) as GameObject[];
			if (GUI.Button (new Rect (bn * bw + bd, h - 2f * (bd + bh), bw - 2 * bd, bh), "Dynamic Only")) {
				foreach (GameObject aGameObject in allGameObjectsList) {
					if (aGameObject.name == "DynamicScribble")
						Destroy (aGameObject);
				}
				clearDrawingMenuEnabled = !clearDrawingMenuEnabled;
			}
			if (GUI.Button (new Rect (bn * bw + bd, h - 3f * (bd + bh), bw - 2f * bd, bh), "Clear All")) {
				foreach (GameObject aGameObject in allGameObjectsList) {
					if (aGameObject.name == "StaticScribble")
						Destroy (aGameObject);
					if (aGameObject.name == "DynamicScribble")
						Destroy (aGameObject);
				}
				clearDrawingMenuEnabled = !clearDrawingMenuEnabled;
			}
		}
	
		// MENU: ScreenSettings
		bn = 5f;
		string currentDeviceOrientation;
		if (SystemInfo.deviceType == DeviceType.Handheld) {
			currentDeviceOrientation = "(" + Screen.orientation + ")";
		} else {
			currentDeviceOrientation = "";
		}
	
		if (GUI.Button (new Rect (bn * bw + bd, h - bd - bh, bw - 2f * bd, bh), "Screen \n" + currentDeviceOrientation)) {
			screenSettingsMenuEnabled = !screenSettingsMenuEnabled;
		}
		if (screenSettingsMenuEnabled) {
			cameraObject.GetComponent<ScribblePhysicsMainScript> ().enabled = false;
			if (GUI.Button (new Rect (bn * bw + bd, h - 2f * (bd + bh), 0.5f * bw - 2f * bd, bh), "<")) {
				SPMS.cameraOrthographicSize -= 1f;
			}
			if (GUI.Button (new Rect ((bn + 0.5f) * bw + bd, h - 2f * (bd + bh), 0.5f * bw - 2f * bd, bh), ">")) {
				SPMS.cameraOrthographicSize += 1f;
			}
			GUI.Box (new Rect (bn * bw + bd, h - 3f * (bd + bh), bw - 2f * bd, bh), "Screen \nSize = " + SPMS.cameraOrthographicSize);
			SPMS.cameraOrthographicSize = Mathf.Clamp (SPMS.cameraOrthographicSize, 1f, 100f);
			cameraObject.GetComponent<Camera> ().orthographicSize = Mathf.Clamp (SPMS.cameraOrthographicSize, 1f, 100f);
	
			if (SystemInfo.deviceType == DeviceType.Handheld) {
				GUI.Box (new Rect (bn * bw + bd, h - 8f * (bd + bh) - bh, bw - 2f * bd, bh), "Device \nOrientation = \n");
				if (GUI.Button (new Rect (bn * bw + bd, h - 7f * (bd + bh) - bh, bw - 2f * bd, bh), "Portrait Up")) {
					Screen.orientation = ScreenOrientation.Portrait;	
					screenSettingsMenuEnabled = false;
				}
				if (GUI.Button (new Rect (bn * bw + bd, h - 6f * (bd + bh) - bh, bw - 2f * bd, bh), "Portrait Down")) {
					Screen.orientation = ScreenOrientation.PortraitUpsideDown;
					screenSettingsMenuEnabled = false;
				}	
				if (GUI.Button (new Rect (bn * bw + bd, h - 5f * (bd + bh) - bh, bw - 2f * bd, bh), "Landscape Left")) {
					Screen.orientation = ScreenOrientation.LandscapeLeft;	
					screenSettingsMenuEnabled = false;
				}
				if (GUI.Button (new Rect (bn * bw + bd, h - 4f * (bd + bh) - bh, bw - 2f * bd, bh), "Landscape Right")) {
					Screen.orientation = ScreenOrientation.LandscapeRight;	
					screenSettingsMenuEnabled = false;
				}
			}
		}
	}
	//
	//***************************************************************//

}




