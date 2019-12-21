using UnityEngine;
using System.Collections;

public class PreferenceController : MonoBehaviour
{

	public static PreferenceController instance;

	//	private int levelCount = 0;

	void Awake ()
	{
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
        }
    }

	void Start ()
	{
		if (GetHideIns () != 0) {
			UIManager.instance.instruction.SetActive (false);
		}
	}


	public void SetLevelCount (int levelCount)
	{
		PlayerPrefs.SetInt ("LevelCount", levelCount);
	}

	public int GetLevelCount ()
	{
		return PlayerPrefs.GetInt ("LevelCount", 0);
	}

	public void SetHideIns (int hideIns)
	{		
		PlayerPrefs.SetInt ("HideIns", hideIns);
	}

	public int GetHideIns ()
	{
		return PlayerPrefs.GetInt ("HideIns", 0);
	}

    public void ResetTheGame()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SetSound(int _status)
    {
        PlayerPrefs.SetInt("music", _status);
    }

    public int GetSound()
    {
        return PlayerPrefs.GetInt("music");
    }
}
