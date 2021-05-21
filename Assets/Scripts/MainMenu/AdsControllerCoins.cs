using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Monetization;

public class AdsControllerCoins : MonoBehaviour
{

    public int numberOfCoins;

    private string appId = "4089943";
    private string coinAds = "Coins";
    private int coins = 0;
    private Cronometer cronometer;
    private GameObject readyIndicator;
    private Button button;
    private GodScript god;

    // Start is called before the first frame update
    void Start()
    {
        god = GameObject.Find("GodObject").GetComponent<GodScript>();
        Monetization.Initialize(appId, true);
        cronometer = GameObject.Find("Cronometer").GetComponent<Cronometer>();
        cronometer.setTimer(0);
        readyIndicator = GameObject.Find("Ready Indicator");
        button = GetComponent<Button>();
    }

    private void Update()
    {
        button.enabled = (cronometer.getTime() <= 0) ? true : false;
        readyIndicator.SetActive((cronometer.getTime() <= 0) ? true : false);
    }

    public void showAds()
    {
        if (Monetization.IsReady(coinAds))
        {
            ShowAdPlacementContent ad = null;
            ad = Monetization.GetPlacementContent(coinAds) as ShowAdPlacementContent;
            if (ad != null)
            {
                ad.Show();
            }
            setAction();
        }
    }
    private void setAction()
    {
        god.addCoins(numberOfCoins);

        cronometer.initializeTimer();
    }

    public int getCoins()
    {
        return coins;
    }

    public void setCoins(int coins)
    {
        this.coins = coins;
    }

    public void addCoins(int coins)
    {
        coins += coins;
    }

    public void restCoins(int coins)
    {
        coins -= coins;
    }
}