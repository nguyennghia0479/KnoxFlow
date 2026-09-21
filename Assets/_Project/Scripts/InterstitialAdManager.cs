using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class InterstitialAdManager : MonoBehaviour
{
    // Google test Interstitial Ad Unit ID cho Android
    private const string TEST_AD_UNIT_ID = "ca-app-pub-3940256099942544/1033173712";

    private InterstitialAd interstitialAd;
    private Action afterAdClosed;

    private void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("Google Mobile Ads SDK initialized.");
            LoadInterstitialAd();
        });
    }

    private void LoadInterstitialAd()
    {
        interstitialAd?.Destroy();
        interstitialAd = null;

        AdRequest adRequest = new();
        InterstitialAd.Load(
            TEST_AD_UNIT_ID,
            adRequest,
            (ad, error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Interstitial ad failed to load: " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded successfully.");
                interstitialAd = ad;
                RegisterAdEvents();
            }
        );
    }

    private void RegisterAdEvents()
    {
        interstitialAd.OnAdFullScreenContentClosed += HandleAdClosed;
        interstitialAd.OnAdFullScreenContentFailed += HandleAdFailed;
    }

    public void ShowAd(Action onAdClosed)
    {
        afterAdClosed = onAdClosed;
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready.");
            InvokeAfterAdClosed();
        }
    }

    private void HandleAdClosed()
    {
        Debug.Log("Interstitial ad closed.");
        LoadInterstitialAd();
        InvokeAfterAdClosed();
    }

    private void HandleAdFailed(AdError error)
    {
        Debug.LogError("Interstitial ad failed to show: " + error);
        LoadInterstitialAd();
        InvokeAfterAdClosed();
    }

    private void InvokeAfterAdClosed()
    {
        Action callback = afterAdClosed;
        afterAdClosed = null;
        callback?.Invoke();
    }

    private void OnDestroy()
    {
        interstitialAd?.Destroy();
        interstitialAd = null;
        afterAdClosed = null;
    }
}
