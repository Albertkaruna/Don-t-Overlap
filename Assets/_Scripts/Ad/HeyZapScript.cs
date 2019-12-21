using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Heyzap;


public class HeyZapScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        HeyZapInit();
        //HeyzapAds.ShowMediationTestSuite();
    }
	
	// Update is called once per frame
	void Update () {

        //You can set a listener on HZInterstitialAd, HZVideoAd, HZIncentivizedAd, and HZOfferWallAd.

        HZIncentivizedAd.AdDisplayListener listenerReward = delegate (string adState, string adTag) {
            if (adState.Equals("incentivized_result_complete"))
            {
                // The user has watched the entire video and should be given a reward.
            }
            if (adState.Equals("incentivized_result_incomplete"))
            {
                // The user did not watch the entire video and should not be given a   reward.
            }
        };

        HZIncentivizedAd.SetDisplayListener(listenerReward);

        HZInterstitialAd.AdDisplayListener listenerInterstitial = delegate (string adState, string adTag) {
            if (adState.Equals("show"))
            {
                // Sent when an ad has been displayed.
                // This is a good place to pause your app, if applicable.
            }
            if (adState.Equals("hide"))
            {
                // Sent when an ad has been removed from view.
                // This is a good place to unpause your app, if applicable.
            }
            if (adState.Equals("click"))
            {
                // Sent when an ad has been clicked by the user.
            }
            if (adState.Equals("failed"))
            {
                // Sent when you call `show`, but there isn't an ad to be shown.
                // Some of the possible reasons for show errors:
                //    - `HeyzapAds.PauseExpensiveWork()` was called, which pauses 
                //      expensive operations like SDK initializations and ad
                //      fetches, andand `HeyzapAds.ResumeExpensiveWork()` has not
                //      yet been called
                //    - The given ad tag is disabled (see your app's Publisher
                //      Settings dashboard)
                //    - An ad is already showing
                //    - A recent IAP is blocking ads from being shown (see your
                //      app's Publisher Settings dashboard)
                //    - One or more of the segments the user falls into are
                //      preventing an ad from being shown (see your Segmentation
                //      Settings dashboard)
                //    - Incentivized ad rate limiting (see your app's Publisher
                //      Settings dashboard)
                //    - One of the mediated SDKs reported it had an ad to show
                //      but did not display one when asked (a rare case)
                //    - The SDK is waiting for a network request to return before an
                //      ad can show
            }
            if (adState.Equals("available"))
            {
                // Sent when an ad has been loaded and is ready to be displayed,
                //   either because we autofetched an ad or because you called
                //   `Fetch`.
            }
            if (adState.Equals("fetch_failed"))
            {
                // Sent when an ad has failed to load.
                // This is sent with when we try to autofetch an ad and fail, and also
                //    as a response to calls you make to `Fetch` that fail.
                // Some of the possible reasons for fetch failures:
                //    - Incentivized ad rate limiting (see your app's Publisher
                //      Settings dashboard)
                //    - None of the available ad networks had any fill
                //    - Network connectivity
                //    - The given ad tag is disabled (see your app's Publisher
                //      Settings dashboard)
                //    - One or more of the segments the user falls into are
                //      preventing an ad from being fetched (see your
                //      Segmentation Settings dashboard)
            }
            if (adState.Equals("audio_starting"))
            {
                // The ad about to be shown will need audio.
                // Mute any background music.
            }
            if (adState.Equals("audio_finished"))
            {
                // The ad being shown no longer needs audio.
                // Any background music can be resumed.
            }
        };

        HZInterstitialAd.SetDisplayListener(listenerInterstitial);

        if (GameController.adCount == 3)
        {
            if (!HZVideoAd.IsAvailable())
            {
                FetchVideoAd();
            }
        }

        if (GameController.adCount >= 5)
        {
            GameController.adCount = 0;
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                ShowInterstitialAd();
            }
            else
            {
                ShowVideoAd();
            }
        }        
    }

    void HeyZapInit()
    {
        HeyzapAds.Start("c0d047674442aff18af093539748d8ae", HeyzapAds.FLAG_NO_OPTIONS);
        FetchVideoAd();
    }

    public void ShowInterstitialAd()
    {
        HZInterstitialAd.Show();
    }

    public void FetchVideoAd()
    {
        HZVideoAd.Fetch();
    }

    public void ShowVideoAd()
    {
        if (HZVideoAd.IsAvailable())
        {
            HZVideoAd.Show();
        }
        else
        {
            FetchVideoAd();
        }
    }

    public void FetchRewardedAd()
    {
        HZIncentivizedAd.Fetch();
    }

    public void ShowRewardedAd()
    {
        if (HZIncentivizedAd.IsAvailable())
        {
            HZIncentivizedAd.Show();
        }
    }

    public void ShowBannerAd()
    {
        HZBannerShowOptions showOptions = new HZBannerShowOptions();
        showOptions.Position = HZBannerShowOptions.POSITION_TOP;
        HZBannerAd.ShowWithOptions(showOptions);
    }

    public void HideBannerAd()
    {
        HZBannerAd.Hide();
    }

    public void DestroyBannerAd()
    {
        HZBannerAd.Destroy();
    }
}
