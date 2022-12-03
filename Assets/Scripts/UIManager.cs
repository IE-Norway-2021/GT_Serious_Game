using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

	public ProgressBar money;
	public float newMoney;
    public TMP_Text moneyText;
    

	// Start is called before the first frame update
	void Start()
	{
        // Update resource every 1 second
        InvokeRepeating("UpdateResource", 0, 1);
	}

	void UpdateResource()
	{
        money = GameObject.Find("MoneyProgressBar").GetComponent<ProgressBar>();
        moneyText = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
		newMoney = GameObject.Find("GameManager").GetComponent<GameManager>().moneyCount;
		money.current = newMoney;
        moneyText.text = newMoney.ToString();
	}
}
