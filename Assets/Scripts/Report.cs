using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Report : MonoBehaviour
{
	private TMP_Text report;
	private GameManager gameManager;

	// Start is called before the first frame update
	void Start()
	{
		report = GameObject.Find("Report").GetComponent<TMP_Text>();

		// Get player prefs
		int trees = PlayerPrefs.GetInt("trees");
		int Co2 = PlayerPrefs.GetInt("co2");
		int time = PlayerPrefs.GetInt("time");
		int money = PlayerPrefs.GetInt("money");
		int hostel = PlayerPrefs.GetInt("hotelBuilt");

		report.text = $"You let {trees} trees remain.{Environment.NewLine}You consumed {Co2} tons of CO2.{Environment.NewLine}You finish the game in {time} seconds.{Environment.NewLine}You earned {money} ${Environment.NewLine}{(hostel == 1 ? "You built the hostel" : "You didn't build the hostel")}";
	}
}
