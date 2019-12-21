// ScribblePhysicsObjectScript
//
// This script defines each scribble: 
// - a new mesh will be created and continuously updated while the 
//   user is still drawing the scribble
// - mouse/touch-coordinates will be stored as nodes, 
//   	e.g. the barebone of the scribble
// - Vertices, triangles, UV's, tangents and normals will all 
//	 continuously be (re)calculated
//
//***************************************************************//
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
//***************************************************************//

public class ScribblePhysicsObjectScript : MonoBehaviour
{

	//***************************************************************//
	// following variables will be set by the 'ScribblePhysicsMainScript'
	// please refer to that script for the usage of these varibles:
	//
	private Material textureMaterial;
	private float drawDistance;
	private int touchId = -1;
	private Camera cam;
	private Vector3 currentPosition;
	private Vector3 oldPosition;
	private bool scribbleIsDynamic;
	private bool smoothScribble;
	private float lineWidth;
	private float lineDepth;
	private float density;
	private Vector3[] newVertices;
	private int[] newTriangles;
	private Mesh newMesh;
	private bool intersectionAllowed = false;
	private PhysicMaterial physicsMaterial;
	private float surfacePenetrationFactor = 0.9f;
	private float maxAngleBetweenBoxColliders = 2f;
	private List<Vector3> pointList = new List<Vector3> ();
	private bool scribbleIsFinished = false;
	
	//
	//***************************************************************//

	//***************************************************************//
	//
	// These functions can be used to set the
	// various private variables used in this script
	//
	public void SetTouchId (int inputVar)
	{
		touchId = inputVar;
	}

	public void SetTextureMaterial (Material inputVar)
	{
		textureMaterial = inputVar;
	}

	public void SetCameraObject (Camera inputVar)
	{
		cam = inputVar;
	}

	public void SetDrawDistance (float inputVar)
	{
		drawDistance = inputVar;
	}

	public void SetLineWidth (float inputVar)
	{
		lineWidth = inputVar;
	}

	public void SetLineDepth (float inputVar)
	{
		lineDepth = inputVar;
	}

	public void SetMaterialDensity (float inputVar)
	{
		density = inputVar;
	}

	public void SetScribbleIsDynamic (bool inputVar)
	{
		scribbleIsDynamic = inputVar;
	}

	public void SetSmoothScribble (bool inputVar)
	{
		smoothScribble = inputVar;
	}

	public void SetIntersectionAllowed (bool inputVar)
	{
		intersectionAllowed = inputVar;
	}

	public void SetPhysicsMaterial (PhysicMaterial inputVar)
	{
		physicsMaterial = inputVar;
	}

	public void SetMaxAngleBetweenBoxColliders (float inputVar)
	{
		maxAngleBetweenBoxColliders = inputVar;
	}

	public void SetSurfacePenetrationFactor (float inputVar)
	{
		surfacePenetrationFactor = inputVar;
	}
	//
	//***************************************************************//
	

	//***************************************************************//
	//
	void Start ()
	{
		// determine start position of the new scribble, based on mouse input
		if (touchId == -1) {
			currentPosition = cam.ScreenToWorldPoint (Input.mousePosition + new Vector3 (0f, 0f, drawDistance));
		}
		// determine start position of the new scribble, based on touch input
		if (touchId != -1) {
			foreach (Touch touch in Input.touches) {
				if (touchId == touch.fingerId) {
					currentPosition = cam.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, 0f) + new Vector3 (0f, 0f, drawDistance));
					oldPosition = currentPosition;
				}
			}
		}
		oldPosition = currentPosition;

		pointList.Clear ();
		pointList.Add (currentPosition);
		gameObject.transform.rotation = Quaternion.identity;
		gameObject.AddComponent<MeshRenderer> ();
		gameObject.GetComponent<MeshRenderer> ().sortingLayerName = "Foreground";
		gameObject.GetComponent<MeshRenderer> ().material = textureMaterial;
		gameObject.AddComponent<MeshFilter> ();
		
	}
	//
	//***************************************************************//

	//***************************************************************//
	//
	// during each update-cycle:
	// - check if the mousebutton has been released (or touch interface has been cancelled):
	//		- Update the scribble to it's final form;
	//		- add all final colliders;
	//		- delete this script, as it is no longer necessary.
	// - If the mousbutton is still pressed (or touch has not been canceled)
	//	 and a movement has been detected:
	//		- add a new node to the array;
	//		- update the scribble;
	//		- add (temporary) colliders;
	// - Also check if the scribble has collided with another object.
	//	 If true, than force the finalization of the scribble
	//	 (only if "intersectionAllowed" is set to false)
	//
	
	void Update ()
	{
		if (!scribbleIsFinished) {
			bool stopDrawing = true;
			RaycastHit rayHit;
			Vector3 deltaMoved;
			float distanceMoved;
		
			// determine next position of a scribble-node, based on mouse input
			if (touchId == -1 && Input.GetMouseButton (0) && GameController.instance.canDraw) {
				stopDrawing = false;
				currentPosition = cam.ScreenToWorldPoint (Input.mousePosition + new Vector3 (0f, 0f, drawDistance));
				deltaMoved = currentPosition - oldPosition;
				distanceMoved = deltaMoved.magnitude;
				// only add a new node if the mouse has been significantly moved
				if (distanceMoved > 0.25f * lineWidth) {
					pointList.Add (currentPosition);
					// update the scribble's mesh and add new colliders
					UpdateScribbleMesh ();
					DeleteAllChildren ();
					AttachCapsuleColliders (pointList);
					// check if a collision has occured with it's surroundings
					if (!intersectionAllowed && Physics.Raycast (oldPosition + deltaMoved.normalized * 0.25f * lineWidth, deltaMoved, out rayHit, distanceMoved) && pointList.Count > 0f) {
						stopDrawing = true;
						pointList [pointList.Count - 1] = rayHit.point;

						if (rayHit.collider.tag == "Number") {							
							GameController.instance.lastPoint = rayHit.transform.position;
							GameController.instance.NumberReached (rayHit.collider);
						} else if (rayHit.collider.name == "Destination") {
                            GameController.isStarted = false;
                            if (GameController.instance.list.Count <= 0) {
								GameController.instance.Victory ();
							} else {
								GameController.instance.Failure ();									
							}
						} else {
							GameController.instance.Failure ();
						}
					}
					oldPosition = currentPosition;
				}
			} // "mouse"
		
			// determine next position of a scribble-node, based on touch input
			if (touchId != -1) {
				// go through all finger id's and check if the right one is still available
				foreach (Touch touch in Input.touches) {
					if (touchId == touch.fingerId && (touch.phase != TouchPhase.Ended || touch.phase != TouchPhase.Canceled) && GameController.instance.canDraw) {
						stopDrawing = false;
						currentPosition = cam.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, drawDistance));
						deltaMoved = currentPosition - oldPosition;
						distanceMoved = deltaMoved.magnitude;
						// only add a new node if finger has been significantly moved
						if (distanceMoved > 0.25f * lineWidth) {
							pointList.Add (currentPosition);
							// update the scribble's mesh and add new colliders
							UpdateScribbleMesh ();
							DeleteAllChildren ();
							AttachCapsuleColliders (pointList);
							// check if a collision has occured with it's surroundings
							if (!intersectionAllowed && Physics.Raycast (oldPosition + deltaMoved.normalized * 0.25f * lineWidth, deltaMoved, out rayHit, distanceMoved) && pointList.Count > 0f) {
								stopDrawing = true;
								pointList [pointList.Count - 1] = rayHit.point;

								if (rayHit.collider.tag == "Number") {							
									GameController.instance.lastPoint = rayHit.transform.position;
									GameController.instance.NumberReached (rayHit.collider);
								} else if (rayHit.collider.name == "Destination") {
                                    GameController.isStarted = false;
									if (GameController.instance.list.Count <= 0) {
										GameController.instance.Victory ();
									} else {
										GameController.instance.Failure ();									
									}
								} else {
									GameController.instance.Failure ();
								}
							}
							oldPosition = currentPosition;
						}
					}
				}
			} // "touch"

			if (Input.GetMouseButtonUp (0) && GameController.instance.canDraw && GameController.isStarted)
            {
				GameController.instance.lastPoint = currentPosition;
				GameController.instance.TempMarker (true);
			}
			// Finalize the scribble:
			// - smooth the array that defines the scribble
			// - update the mesh
			// - add a rigidbody
			// - put constraints on this rigidbody
			// - set rigidbody.useGravity and rigidbody.isKinematic depending whether the scribble is fixed or dynamic
			// - if the scribble is static: delete all (temporary) CapsuleColliders and replace it by a MeshCollider
			// - delete this script, as it is no longer necessary
			if (stopDrawing) {
				GameController.instance.canDraw = false;
				if (pointList.Count <= 1) { // too small to be used as a scribble, therefore destroying the entire object
					Destroy (gameObject);
				} else {
					if (smoothScribble) {
						pointList = SmoothArray (pointList);
						UpdateScribbleMesh ();
					}
					DeleteAllChildren ();
					gameObject.AddComponent<Rigidbody> ();
					gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
					gameObject.GetComponent<Rigidbody> ().mass = CalculateScribbleMass ();
					if (scribbleIsDynamic) {
						gameObject.GetComponent<Rigidbody> ().useGravity = true;
						gameObject.GetComponent<Rigidbody> ().isKinematic = false;
						AttachCapsuleColliders (pointList);    		
					}
					if (!scribbleIsDynamic) {
						gameObject.GetComponent<Rigidbody> ().useGravity = false;
						gameObject.GetComponent<Rigidbody> ().isKinematic = true;
						gameObject.AddComponent<MeshCollider> ();
						gameObject.GetComponent<MeshCollider> ().material = physicsMaterial;
					}
					scribbleIsFinished = true;
				}
			}
		}

	}
	//
	//***************************************************************//
	
	
	//***************************************************************//
	//
	// this function updates the scribble's mesh:
	// - calculate all vertices
	// - calculate all UV's
	// - define all triangles
	// - calculate all tangents
	//
	void UpdateScribbleMesh ()
	{
		newMesh = new Mesh ();
		newMesh.vertices = CalculateScribbleVertices (pointList);
		newMesh.uv = CalculateScribbleUVs (newMesh.vertices);
		newMesh.triangles = CalculateScribbleTriangles (newMesh.vertices);
		newMesh.RecalculateNormals ();	
		newMesh.tangents = CalculateScribbleTangents (newMesh);
		;
		gameObject.GetComponent<MeshFilter> ().mesh = newMesh;
	}
	//
	//***************************************************************//

	
	//***************************************************************//
	//
	// this function deletes all children object, e.g. (temporary) CapsuleColliders, from the gameObject.
	//
	void DeleteAllChildren ()
	{
		for (int i = 0; i < gameObject.transform.childCount; i++) {
			Destroy (gameObject.transform.GetChild (i).gameObject);
		}
	}
	//
	//***************************************************************//

	
	//***************************************************************//
	//
	// this function attaches CapsuleColliders to each segment of the scribble
	// (each segment is defined as the line between the consecutive nodes stored in 'pointList')
	//
	void AttachCapsuleColliders (List<Vector3> pointList)
	{
		Vector3 posA1 = Vector3.zero;	// starting coordinate of segment 'A'
		Vector3 posA2 = Vector3.zero;	// ending coordinate of segment 'A'
		Vector3 posB1 = Vector3.zero;	// starting coordinate of segment 'B' (= 'A' or one of its consecutive segments) 
		Vector3 posB2 = Vector3.zero;	// ending coordinate of segment 'B' (= 'A' or one of its consecutive segments)
		Vector3 dirA = Vector3.zero;
		;	// direction of segment 'A'
		Vector3 dirB = Vector3.zero;
		;	// direction of segment 'B'
		Vector3 dirA1B2 = Vector3.zero;	// the direction between the starting coordinate of 'A' and the ending coordinate of 'B'
		
		// - add capsule colliders to all the segments
		// - if the angle between segments is smaller than 'maxAngleBetweenColliders', 
		//   than create one large capsule collider for these segments, instead of 
		//   several separate smaller ones.
		int i;
		int j;
		GameObject collider;
		for (i = 0; i < pointList.Count - 1; i = j + 1) {
			collider = new GameObject ("CapsuleCollider");
			posA1 = pointList [i]; 
			posA2 = pointList [i + 1];
			dirA = posA2 - posA1;
			for (j = i; j < pointList.Count - 1; j++) {
				posB1 = pointList [j];
				posB2 = pointList [j + 1];
				dirB = posB2 - posB1;
				dirA1B2 = posB2 - posA1;
				if ((Vector3.Angle (dirA, dirB) > maxAngleBetweenBoxColliders) || (Vector3.Angle (dirA1B2, dirB) > maxAngleBetweenBoxColliders) || (Vector3.Angle (dirA1B2, dirA) > maxAngleBetweenBoxColliders)) {
					posB1 = pointList [j - 1];
					posB2 = pointList [j];
					dirB = posB2 - posB1;
					dirA1B2 = posB2 - posA1;
					j--;
					break;
				}
			}
			// create the collider for segment A1B2:
			// - the collider will start at the starting coordinate of segment 'A'
			//   and end at the ending coordinate of segment 'B'
			//   ('B' is either the same as 'A', or one of its consecutive segments)
			//	 The center of the collider is therefore halfway between A1 en B2
			// - the lineWidth will use a surfacePenetrationFactor. This prevents small visual gaps during gameplay 
			// - physicsMaterial will be as defined by the user 
			// - the collider will become a child of the scribble
			collider.transform.Rotate (0f, 0f, Mathf.Rad2Deg * Mathf.Atan (dirA1B2.y / dirA1B2.x));
			collider.transform.position = 0.5f * (posA1 + posB2);
			collider.AddComponent<CapsuleCollider> ();
			collider.GetComponent<CapsuleCollider> ().center = Vector3.zero;
			collider.GetComponent<CapsuleCollider> ().direction = 0; 
			collider.GetComponent<CapsuleCollider> ().radius = 0.5f * lineWidth * surfacePenetrationFactor;
			collider.GetComponent<CapsuleCollider> ().height = dirA1B2.magnitude + surfacePenetrationFactor * lineWidth;
			collider.GetComponent<CapsuleCollider> ().material = physicsMaterial;
			collider.transform.parent = gameObject.transform;
		}
	}
	//
	//***************************************************************//

	
	//***************************************************************//
	//
	// this function returns the coordinates of all the Scribble's Vertices
	//
	Vector3[] CalculateScribbleVertices (List<Vector3> pointList)
	{
		int i;
		Vector3[] vertices = new Vector3[4 * pointList.Count];
		Vector3 k1 = Vector3.zero;
		Vector3 k2 = Vector3.zero;
		Vector3 k = Vector3.zero;
		Vector3 l = Vector3.zero;
		Vector3 m = Vector3.zero;
	
		// calculate all vertices, except the ones at the end of the line
		for (i = 0; i < (pointList.Count - 1); i++) {
			k1 = pointList [i];
			k2 = pointList [i + 1];
			k = k2 - k1;
			l = lineDepth * 0.5f * cam.transform.forward;
			m = lineWidth * 0.5f * Vector3.Normalize (Vector3.Cross (k, l));
			vertices [4 * i + 0] = pointList [i];
			vertices [4 * i + 0] += -m - l;
			vertices [4 * i + 1] = pointList [i];
			vertices [4 * i + 1] += m - l;
			vertices [4 * i + 2] = pointList [i];
			vertices [4 * i + 2] += m + l;
			vertices [4 * i + 3] = pointList [i];
			vertices [4 * i + 3] += -m + l;		
		}
		// now calculate the vertices at the end the line
		i = (pointList.Count - 1);
		vertices [4 * i + 0] = pointList [i];
		vertices [4 * i + 0] += -m - l;
		vertices [4 * i + 1] = pointList [i];
		vertices [4 * i + 1] += m - l;
		vertices [4 * i + 2] = pointList [i];
		vertices [4 * i + 2] += m + l;
		vertices [4 * i + 3] = pointList [i];
		vertices [4 * i + 3] += -m + l;
	
		// first 4 and last 4 vertices must be shifted, due to capsule colliders
		k1 = pointList [0];
		k2 = pointList [1];
		k = (k2 - k1).normalized;
		vertices [0] -= k * lineWidth * 0.5f;
		vertices [1] -= k * lineWidth * 0.5f;
		vertices [2] -= k * lineWidth * 0.5f;
		vertices [3] -= k * lineWidth * 0.5f;
		i = (pointList.Count - 1);
		k1 = pointList [i];
		k2 = pointList [i - 1];
		k = (k2 - k1).normalized;
		vertices [4 * i + 0] -= k * lineWidth * 0.5f;
		vertices [4 * i + 1] -= k * lineWidth * 0.5f;
		vertices [4 * i + 2] -= k * lineWidth * 0.5f;
		vertices [4 * i + 3] -= k * lineWidth * 0.5f;
			
		return vertices;
	}
	//
	//***************************************************************//

	
	//***************************************************************//
	//
	Vector2[] CalculateScribbleUVs (Vector3[] vertices)
	{
		Vector2[] newUV = new Vector2[vertices.Length];
		for (var i = 0; i < newUV.Length; i++) {
			newUV [i] = new Vector2 (vertices [i].x, vertices [i].y);
		}
		return newUV;
	}
	//
	//***************************************************************//

	
	//***************************************************************//
	//
	// this function puts the indices of all vertices in the right order, thereby defining all triangles.
	//
	int[] CalculateScribbleTriangles (Vector3[] vertices)
	{
		int[] triangles = new int[24 * (vertices.Length - 4) + 12];
		int i;
		i = 6 * pointList.Count;
	
		//Beginning of the scribble
		triangles [i + 00] = 0;
		triangles [i + 01] = 1;
		triangles [i + 02] = 2;
		triangles [i + 03] = 0;
		triangles [i + 04] = 2;
		triangles [i + 05] = 3;
	
		//end of the scriblle
		triangles [i + 06] = vertices.Length - 2;
		triangles [i + 07] = vertices.Length - 3;
		triangles [i + 08] = vertices.Length - 4;
		triangles [i + 09] = vertices.Length - 1;
		triangles [i + 10] = vertices.Length - 2;
		triangles [i + 11] = vertices.Length - 4;
	
		for (i = 0; i < (vertices.Length - 4) / 4; i++) {
			//front
			triangles [24 * i + 00] = 4 * i + 4;
			triangles [24 * i + 01] = 4 * i + 5;
			triangles [24 * i + 02] = 4 * i + 0;
			triangles [24 * i + 03] = 4 * i + 5;
			triangles [24 * i + 04] = 4 * i + 1;
			triangles [24 * i + 05] = 4 * i + 0;
			//top
			triangles [24 * i + 06] = 4 * i + 5;
			triangles [24 * i + 07] = 4 * i + 6;
			triangles [24 * i + 08] = 4 * i + 1;
			triangles [24 * i + 09] = 4 * i + 6;
			triangles [24 * i + 10] = 4 * i + 2;
			triangles [24 * i + 11] = 4 * i + 1;
			//back
			triangles [24 * i + 12] = 4 * i + 6;
			triangles [24 * i + 13] = 4 * i + 7;
			triangles [24 * i + 14] = 4 * i + 2;
			triangles [24 * i + 15] = 4 * i + 7;
			triangles [24 * i + 16] = 4 * i + 3;
			triangles [24 * i + 17] = 4 * i + 2;
			//bottom
			triangles [24 * i + 18] = 4 * i + 7;
			triangles [24 * i + 19] = 4 * i + 4;
			triangles [24 * i + 20] = 4 * i + 3;
			triangles [24 * i + 21] = 4 * i + 4;
			triangles [24 * i + 22] = 4 * i + 0;
			triangles [24 * i + 23] = 4 * i + 3;
		}
		return triangles;
	}
	//
	//***************************************************************//

	//***************************************************************//
	//
	// this function calculates all tangents for the scribble.
	//
	Vector4[] CalculateScribbleTangents (Mesh inputMesh)
	{
		int vertexCount = inputMesh.vertexCount;
		Vector3[] vertices = inputMesh.vertices;
		Vector3[] normals = inputMesh.normals;
		Vector2[] texcoords = inputMesh.uv;
		int[] triangles = inputMesh.triangles;
		int trianglesCount = inputMesh.triangles.Length / 3;
		Vector4[] tangents = new Vector4[vertexCount];
	 
		int tri = 0;
		Vector3 sdir;
		Vector3 tdir;
		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
	   
		for (var i = 0; i < trianglesCount; i++) {
			int i1 = triangles [tri];
			int i2 = triangles [tri + 1];
			int i3 = triangles [tri + 2];
	
			Vector3 v1 = vertices [i1];
			Vector3 v2 = vertices [i2];
			Vector3 v3 = vertices [i3];
	
			Vector2 w1 = texcoords [i1];
			Vector2 w2 = texcoords [i2];
			Vector2 w3 = texcoords [i3];
	
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
	
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
	
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
	
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
	
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
	
			float r = 1.0f / (s1 * t2 - s2 * t1);
	
			sdir = new Vector3 ((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			tdir = new Vector3 ((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);
	
			tan1 [i1] += sdir;
			tan1 [i2] += sdir;
			tan1 [i3] += sdir;
	
			tan2 [i1] += tdir;
			tan2 [i2] += tdir;
			tan2 [i3] += tdir;
	
			tri += 3;
		}
	
		Vector3 n;
		Vector3 t;
		for (int i = 0; i < (vertexCount); i++) {
			n = normals [i];
			t = tan1 [i];
			// Gram-Schmidt orthogonalize
			Vector3.OrthoNormalize (ref n, ref t);
			tangents [i].x = t.x;
			tangents [i].y = t.y;
			tangents [i].z = t.z;
			// Calculate handedness
			tangents [i].w = (Vector3.Dot (Vector3.Cross (n, t), tan2 [i]) < 0.0f) ? -1.0f : 1.0f;
		}       
		return tangents; 
	}
	//
	//***************************************************************//
	
	//***************************************************************//
	//
	// This function uses a simple smoothing technique:
	// - first and last points remain the same
	// - remaining points will be recalculated from the old values stored in the
	//		previous/current/next points.
	//
	List<Vector3> SmoothArray (List<Vector3> inputList)
	{
		Vector3 k;
		Vector3 l;
		Vector3 m;
		List<Vector3> outputList = new List<Vector3> ();
		
		// first point remains the same
		outputList.Add (inputList [0]);
		
		// recalculate points, using the old values
		for (int i = 1; i < pointList.Count - 2; i++) {
			k = inputList [i - 1];	// previous point
			l = inputList [i];	// current point
			m = inputList [i + 1];	// next point
			outputList.Add (0.25f * k + 0.5f * l + 0.25f * m);
		}
	
		// last point remains the same
		outputList.Add (inputList [pointList.Count - 1]);
	
		return outputList;
	}
		
	//***************************************************************//
	//
	// Calculate the length of the scribble
	//
	public float GetScribbleLength ()
	{
		float l = 0f;
		for (int i = 0; i < pointList.Count - 1; i++) {
			l += (pointList [i + 1] - pointList [i]).magnitude;
		}
		return l;
	}
	//
	//***************************************************************//

	float CalculateScribbleMass ()
	{
		float scribbleLength = GetScribbleLength ();
		float massa = density * scribbleLength * lineDepth * lineWidth;
		return massa;
	}
}


