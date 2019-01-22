using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeDateDisplayManager : MonoBehaviour {

	[SerializeField] TextMeshProUGUI timeText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeText.text = TimeKeeper.DayOfWeek.ToString() + ", " + TimeKeeper.FormattedTime;
	}
}
