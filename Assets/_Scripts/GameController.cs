using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
	public static GameController instance;

    public Transform levelsParent;
    [HideInInspector]
    public GameObject[] _levels;
    public Transform numberIndicator, numberMarker, Marker;
    public Animator fader;
    public Color[] colors;
	[HideInInspector]
	public bool canDraw = false;
	[HideInInspector]
	public Collider hitObject;
	[HideInInspector]
	public Vector3 lastPoint;
	private RaycastHit hit;
	[HideInInspector]
	public List<Transform> list = new List<Transform> ();
	private Transform source, destination, currentNumber;
    [HideInInspector]
    public GameObject holder, scribbleHolder;
    [HideInInspector]
    public int lvl_Count = 0;
    public static bool isStarted=false;
    public static int adCount=0;


    void Awake ()
	{
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	// Use this for initialization
	void Start ()
	{
        _levels = new GameObject[levelsParent.childCount];
        for (int i = 0; i < levelsParent.childCount; i++)
        {
            _levels[i] = levelsParent.GetChild(i).gameObject;
        }
        lvl_Count = PreferenceController.instance.GetLevelCount();
        print("Levelcount: " + lvl_Count);
        //		 Get new level at first

        source = GameObject.Find ("Source").GetComponent<Transform> ();
		destination = GameObject.Find ("Destination").GetComponent<Transform> ();
        CreateHolders();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (Physics.Raycast (worldPoint, Vector3.forward, out hit)) {
			if (hit.collider != null && Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
				hitObject = hit.collider;
				CheckWhat ();
			}
		}
	}

	// Check on what thing the raycast is hitting
	void CheckWhat ()
	{
		if (hitObject.name == "Source") {
			print ("Started");
            if (UIManager.instance.Menu.activeSelf)
                UIManager.instance.Menu.SetActive(false);
			canDraw = true;
			hitObject.name = "SourceStarted";
            isStarted = true;
			ChooseNumber ();
		} else if (hitObject.name == "Marker") {
			canDraw = true;
			TempMarker (false);
		} else if (hitObject.tag == "NumberMarker") {
            hitObject.tag = "OldMarker";
            StartCoroutine(EnableMarkerCollider(hitObject));
			hitObject.enabled = false;
			canDraw = true;
		}
	}

    IEnumerator EnableMarkerCollider(Collider col)
    {
        yield return new WaitForSeconds(1f);
        if (col != null)
            col.enabled = true;
    }

    // Does the drawing hit the indicated number or other numbers
    public void NumberReached (Collider hitter)
	{
		if (hitObject != null) {
			if (hitter.name == currentNumber.name) {
				Indicator (false, Vector3.zero);
                hitter.enabled = false;
                ChooseNumber ();
				GameObject t = Instantiate (GameController.instance.numberMarker.gameObject, lastPoint, Quaternion.identity)as GameObject;
				t.transform.parent = holder.transform;
				canDraw = false;
                UIManager.instance.PlayNumberReachedSound();
				print ("Number reached");
			} else {
				print ("is that u");
				Failure ();
			}
		}
	}

	// Choose the new number to complete
	void ChooseNumber ()
	{
		if (list.Count > 0) {
			int temp = Random.Range (0, list.Count - 1);
			currentNumber = list [temp];
			list.Remove (currentNumber);
			Indicator (true, currentNumber.position);
		} else {
			Indicator (true, destination.position);
		}

	}
	// Show or Hide the current number indicator
	void Indicator (bool Show, Vector3 Pos)
	{
		if (Show) {
			numberIndicator.gameObject.SetActive (true);
			numberIndicator.position = Pos;
		} else {
			numberIndicator.gameObject.SetActive (false);
		}

	}

	// Show or Hide the temporary markers when mouse up or mouse down at empry place
	public void TempMarker (bool Show)
	{
		if (Show) {
			Marker.gameObject.SetActive (true);
			Marker.position = lastPoint;
		} else {
			Marker.gameObject.SetActive (false);
		}
	}
	// Victory now load next level
	public void Victory ()
	{
		print ("Victory");
        adCount++;
        UIManager.instance.PlayVictorySound();
        lvl_Count++;
        PreferenceController.instance.SetLevelCount(lvl_Count);
		canDraw = false;
        Indicator (false, Vector3.zero);
        StartCoroutine(DisableOld_AndGet_NewLevel());
	}
	// Level failed now restart the level
	public void Failure ()
	{
		print ("Game Over");
        adCount++;
        UIManager.instance.PlayFailureSound();
        PreferenceController.instance.SetLevelCount(lvl_Count);
        canDraw = false;
        Indicator(false, Vector3.zero);
        StartCoroutine(DisableOld_AndGet_NewLevel());
    }


   

    // Get the new level when the 'Next' or 'Previous' buttons are pressed
    public void GetNewLevelUI(string s)
    {
        UIManager.instance.NextBtn.interactable = false;
        UIManager.instance.PreviousBtn.interactable = false;

        
        StartCoroutine(GetNewLevelNow(s));
    }

    // Disables the current level and gets the new level
    IEnumerator GetNewLevelNow(string s)
    {
        int temp = 0;
        temp = lvl_Count;
        if (s == "Next")
        {
            lvl_Count += 1;
        }
        else if (s == "Previous")
        {
            lvl_Count -= 1;
        }
        yield return new WaitForSeconds(0.1f);
        fader.gameObject.SetActive(true);
        fader.SetBool("fade", true);
        yield return new WaitForSeconds(1f);
        _levels[temp].SetActive(false);
        _levels[lvl_Count].SetActive(true);

        DestryHolders();
        CreateHolders();

        if (source != null)
            Indicator(true, source.position);
        if (source != null)
            source.name = "Source";
        TempMarker(false);
        list.Clear();
        for (int i = 0; i < _levels[lvl_Count].transform.childCount; i++)
        {
            Transform t = _levels[lvl_Count].transform.GetChild(i);
            list.Add(t);
            t.GetComponent<Collider>().enabled = true;
            SetColor(t.GetComponent<SpriteRenderer>());
        }


        yield return new WaitForSeconds(0.5f);
        fader.SetBool("fade", false);
        yield return new WaitForSeconds(1f);
        fader.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        UIManager.instance.MenuButtonsInteractables();
    }

    public void CallDisableOld_AndGet_NewLevel()
    {
        StartCoroutine(DisableOld_AndGet_NewLevel());
    }

    // Load new level if the level was won and disables the old level
    IEnumerator DisableOld_AndGet_NewLevel()
    {
        yield return new WaitForSeconds(0.1f);
        fader.gameObject.SetActive(true);
        fader.SetBool("fade", true);
        yield return new WaitForSeconds(1f);
        if(UIManager.instance.firstPanel.activeSelf)
            UIManager.instance.firstPanel.SetActive(false);
        try
        {
            if(lvl_Count>0)
                _levels[lvl_Count - 1].SetActive(false);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
        GetNewLevel();
        yield return new WaitForSeconds(0.5f);
        fader.SetBool("fade", false);
        yield return new WaitForSeconds(1f);
        fader.gameObject.SetActive(false);
    }
    // Get the level to play
    public void GetNewLevel()
    {
        // lvl_Count = 10; // Remove this line
        if (lvl_Count < _levels.Length)
        {
            _levels[lvl_Count].SetActive(true);
        }
        else
        {
            print("No More Levels, Probably Game End !");
        }

        DestryHolders();
        CreateHolders();

        if(source!=null)
            Indicator(true,source.position);
        if(source!=null)
            source.name = "Source";
        list.Clear();
        TempMarker(false);

        for (int i = 0; i < _levels[lvl_Count].transform.childCount; i++)
        {
            Transform t = _levels[lvl_Count].transform.GetChild(i);
            list.Add(t);
            t.GetComponent<Collider>().enabled = true;
            SetColor(t.GetComponent<SpriteRenderer>());
        }
    }


    public void CreateHolders()
    {
        holder = new GameObject("Holder");
        scribbleHolder = new GameObject("ScribbleHolder");
    }

    public void DestryHolders()
    {
        if (holder != null)
            Destroy(holder);
        if (scribbleHolder != null)
            Destroy(scribbleHolder);
    }

    public void SetColor(SpriteRenderer sprite)
    {
        int r = Random.Range(0, colors.Length);
        sprite.color = colors[r];
    }
}
