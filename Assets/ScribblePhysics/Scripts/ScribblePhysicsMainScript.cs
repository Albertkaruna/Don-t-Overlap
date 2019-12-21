//***************************************************************//
// ScribblePhysicsMainScript
//
// This script scans for mouse/touch input and instantiates a new scribble if necessary.
// Relevant user settings can be set in the unity editor or through an external script:
//	1) define a new variable that refers to the ScribblePhysicsMainScript:
//		SPMS = gameObjectThatHoldsTheMainScript.GetComponent<ScribblePhysicsMainScript>();
//
//	2) all settings that defines new scribbles can now be accessed/changed as follows
//		SPMS.lineWidth = 0.6;
//		SPMS.physicsMaterial = userDefinedPhysicsMaterial;
//		etc. 
//
// The following variables can be set:
//
// * Rect canvasRect
// 'canvasRect' holds the rect that defines the drawing canvas.
// rect-sizes will be interpreted as relative screen-sizes
// e.g.: 
// Rect(0,0,1,1) --> use the entire screen
// Rect(0,0,1,0.5) --> use the bottom half of the screen
// Rect(0,0,0.5,1) --> use the right half of the screen
//
// * Camera mainCamera
// The GameObject that holds the main camera.
// If this variable is left empty, than it is assumed that
// main camera is attached to the current gameObject
//
// * float cameraOrthographicSize
// The main camera will be put in orthographic view. 
// The (initial) size of this view can be set through this variable
// 
// * float drawDistanceFromCamera
// The drawing distance from the camera.
//
// * float lineWidth
// All new scribbles will be drawn using this width
//
// * float lineDepth
// All new scribbles will be draw using this depth value. 
// Usually it won't be necessary to change this value
//
// * float density
// Bigger scribbles will have a bigger mass.
// The density is used to calculate this mass.
//
// * Material textureMaterial
// The texture material to be used.
//
// * PhysicMaterial physicsMaterial
// the physics material to be used
//
// * bool scribbleIsDynamic
// 'true': New scribbles will be dynamic, e.g. obey laws of gravity
// 'false' (default): New scribbles will be static
//
// * bool intersectionAllowed
// 'true': Each new scribble can be drawn right through all other 
//		objects in the scene. This can result in unpredictable 
//		and unrealistic behaviour from unity's physics engine.
// 'false' (default): Each new scribble can not intersect with other
//		objects in the scene. Hitting an object will terminate 
//		drawing mode and the scribble will be finalized.
//
// * bool smoothScribble
//	'true' (default): The scribble will be smoothed after completion
//	'false': The scribble will not be smoothed after completion
//
//***************************************************************//

//***************************************************************//
//
using UnityEngine;
using System.Collections;

//
//***************************************************************//

public class ScribblePhysicsMainScript : MonoBehaviour
{
	
	//***************************************************************//
	//
	public Rect canvasRect = new Rect (0f, 0f, 1f, 1f);
	public Camera mainCamera;
	public float cameraOrthographicSize = 20f;
	public float drawDistanceFromCamera = 20f;

	[Range (0.1f, 2.0f)]
	public float lineWidth = 0.5f;
	[Range (0.1f, 2.0f)]
	public float lineDepth = 1.0f;
	[Range (0.1f, 5.0f)]
	public float density = 1.0f;
	public Material textureMaterial;
	public PhysicMaterial physicsMaterial;
	
	public bool scribbleIsDynamic = false;
	public bool intersectionAllowed = false;
	public bool smoothScribble = true;
	
	private Touch touch;
	private int touchId = -1;
	//

	//***************************************************************//
	//
	void Start ()
	{
		// put the (main) camera in orthographic view, 
		// and give it a size of 'cameraOrthographicSize'.
		// also make sure it faces forward.
		if (!mainCamera) {
			mainCamera = gameObject.GetComponent<Camera> ();
		}
		mainCamera.GetComponent<Camera> ().orthographic = true;
		mainCamera.GetComponent<Camera> ().orthographicSize = cameraOrthographicSize;
		mainCamera.transform.rotation = Quaternion.identity;
		 
		// recalculate the canvasRect to real screensizes
		canvasRect.width *= Screen.width;
		canvasRect.height *= Screen.height;
	
		// Set the ambient light to clear and bright white
		RenderSettings.ambientLight = Color.white;
	}
	//
	//***************************************************************//
	
	
	
	//***************************************************************//
	//
	void Update ()
	{
		// check if there's a new user input, within the defined drawing canvas
		touchId = -1;

		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began && canvasRect.Contains (touch.position)) {
				touchId = touch.fingerId; // remember the touchID, the new scribble needs it to keep track of the movement of this specific gesture
				GameObject newObject = new GameObject ();
				if (scribbleIsDynamic) {
					newObject.name = "DynamicScribble";
				} else {
					newObject.name = "StaticScribble";
                    newObject.transform.parent = GameController.instance.scribbleHolder.transform;
				}
				newObject.AddComponent<ScribblePhysicsObjectScript> ();
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetTextureMaterial (textureMaterial);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetDrawDistance (drawDistanceFromCamera);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetCameraObject (mainCamera);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetLineWidth (lineWidth);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetLineDepth (lineDepth);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetMaterialDensity (density);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetScribbleIsDynamic (scribbleIsDynamic);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetSmoothScribble (smoothScribble);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetTouchId (touchId);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetIntersectionAllowed (intersectionAllowed);
				newObject.GetComponent<ScribblePhysicsObjectScript> ().SetPhysicsMaterial (physicsMaterial);
			}
		} 

		if (Input.GetMouseButtonDown (0) && canvasRect.Contains (Input.mousePosition) && touchId == -1) {
			touchId = -1; // setting value to -1, because there is no touchID
			GameObject newObject = new GameObject ();
			if (scribbleIsDynamic) {
				newObject.name = "DynamicScribble";
			} else {
				newObject.name = "StaticScribble";
                newObject.transform.parent = GameController.instance.scribbleHolder.transform;
            }
			newObject.AddComponent<ScribblePhysicsObjectScript> ();
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetTextureMaterial (textureMaterial);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetDrawDistance (drawDistanceFromCamera);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetCameraObject (mainCamera);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetLineWidth (lineWidth);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetLineDepth (lineDepth);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetMaterialDensity (density);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetScribbleIsDynamic (scribbleIsDynamic);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetSmoothScribble (smoothScribble);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetTouchId (touchId);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetIntersectionAllowed (intersectionAllowed);
			newObject.GetComponent<ScribblePhysicsObjectScript> ().SetPhysicsMaterial (physicsMaterial);
		}

	}
	//
	//***************************************************************//
}
