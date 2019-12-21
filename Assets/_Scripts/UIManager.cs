using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Security.Policy;


public class UIManager : MonoBehaviour
{

	public static UIManager instance;

    private string fbURL= "https://www.facebook.com/hyperzeta";
    private string twitterURL= "https://twitter.com/hyper_zeta";
    private string rateUsURL = "https://play.google.com/store/apps/details?id=com.hyperzeta.dontoverlap";


    public GameObject Menu, instruction;
	public Button NextBtn, PreviousBtn;
	private int doubleClick = 0;
	[HideInInspector]
	public bool letDoubleClick = true;

    public GameObject firstPanel, settingsPanel, infoPanel;
    public AudioSource audioSource,effectAudioSource;
    public Image audioBtnImage;
    public Sprite audioPlaySprite, audioPauseSprite;
    public AudioClip victorySound, failureSound,numberReachedSound;
    public GameObject progressResetPanel,gameQuitPanel;

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
       if(PreferenceController.instance.GetSound()==2)
        {
            audioSource.Pause();
            audioBtnImage.sprite = audioPauseSprite;
        }
        else
        {
            audioSource.Play();
            audioBtnImage.sprite = audioPlaySprite;
        }
    }

	void Update ()
	{
		//if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject () && letDoubleClick) {
		//	doubleClick += 1;
		//	if (Menu.activeSelf) {
		//		Menu.SetActive (false);
		//	}
		//	if (doubleClick >= 2) {
		//		ShowHideMenu ();
		//	}
		//	Invoke ("ResetClick", 0.3f);
		//}

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ShowGameQuitPopup();
        }
	}

	//void ResetClick ()
	//{
	//	doubleClick = 0;
	//}

	public void ShowHideMenu ()
	{
//		Show the menu
		if (!Menu.activeSelf) {
			Menu.SetActive (true);
			MenuButtonsInteractables ();
        }
        else
        {
            Menu.SetActive(false);
        }
	}

	public void MenuButtonsInteractables ()
	{
        if (GameController.instance.lvl_Count == PreferenceController.instance.GetLevelCount())
        {
            NextBtn.interactable = false;
        }
        else
        {
            NextBtn.interactable = true;
        }
        if (GameController.instance.lvl_Count == 0)
        {
            PreviousBtn.interactable = false;
        }
        else
        {
            PreviousBtn.interactable = true;
        }
    }

	public void Previous ()
	{
		GameController.instance.GetNewLevelUI ("Previous");
	}

	public void Next ()
	{
		GameController.instance.GetNewLevelUI ("Next");
	}

	public void Temp ()
	{

		GameController.instance.Victory ();
	}

    void OnApplicationQuit()
    {
        
    }

    public void Play()
    {
        GameController.instance.CallDisableOld_AndGet_NewLevel();
    }

    public void Audio()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.Play();
            audioBtnImage.sprite = audioPlaySprite;
            PreferenceController.instance.SetSound(1);
        }
        else
        {
            audioSource.Pause();
            audioBtnImage.sprite = audioPauseSprite;
            PreferenceController.instance.SetSound(2);
        }
        print(PreferenceController.instance.GetSound());
    }

    public void Facebook()
    {
        Application.OpenURL(fbURL);
    }

    public void Twitter()
    {
        Application.OpenURL(twitterURL);
    }

    public void Info()
    {
        infoPanel.SetActive(true);
    }

    public void Settings()
    {
        settingsPanel.SetActive(true);
    }

    public void ResetGame()
    {
        progressResetPanel.SetActive(true);
    }

    public void FeedbackMail()
    {
        SendEmail();
    }

    public void RateTheGame()
    {
        Application.OpenURL(rateUsURL);
    }

    public void BackPanel(GameObject go)
    {
        if(go.name=="BackInfoPanel")
        {
            infoPanel.SetActive(false);
        }else if(go.name=="BackSettingsPanel")
        {
            settingsPanel.SetActive(false);
        }
    }

    void SendEmail()
    {
        string email = "hyperzetaz@gmail.com";
        string subject = MyEscapeURL("Support");
        string body = MyEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    string MyEscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public void PlayVictorySound()
    {
        effectAudioSource.PlayOneShot(victorySound);
    }

    public void PlayFailureSound()
    {
        effectAudioSource.PlayOneShot(failureSound);
    }

    public void PlayNumberReachedSound()
    {
        effectAudioSource.PlayOneShot(numberReachedSound);
    }

    public void ProgressResetYes()
    {
        PreferenceController.instance.ResetTheGame();
        progressResetPanel.SetActive(false);
        Application.Quit();
    }

    public void ProgressResetNo()
    {
        progressResetPanel.SetActive(false);
    }

    public void ShowGameQuitPopup()
    {
            gameQuitPanel.SetActive(!gameQuitPanel.activeSelf);
    }

    public void GameQuitYes()
    {
        gameQuitPanel.SetActive(false);
        Application.Quit();
    }

    public void GameQuitNo()
    {
        gameQuitPanel.SetActive(false);
    }
}
