using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

	public ProgressBar money;
	public float newMoney;
    public TMP_Text moneyText;
    public ProgressBar co2;
    public float newCo2;
    public TMP_Text co2Text;


	// Start is called before the first frame update
	void Start()
	{
        // Update resource every 1 second
        InvokeRepeating("UpdateResource", 0, 1);
	}

	void UpdateResource()
	{
        // Money
        money = GameObject.Find("MoneyProgressBar").GetComponent<ProgressBar>();
        moneyText = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
		newMoney = GameObject.Find("GameManager").GetComponent<GameManager>().moneyCount;
		money.current = newMoney;
        moneyText.text = newMoney.ToString();

        // CO2
        co2 = GameObject.Find("CO2ProgressBar").GetComponent<ProgressBar>();
        co2Text = GameObject.Find("CO2Count").GetComponent<TMP_Text>();
        newCo2 = GameObject.Find("GameManager").GetComponent<GameManager>().co2Count;
        co2.current = newCo2;
        co2Text.text = newCo2.ToString();
	}
}
