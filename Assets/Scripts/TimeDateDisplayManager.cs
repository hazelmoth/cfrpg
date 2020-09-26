using UnityEngine;
using TMPro;

public class TimeDateDisplayManager : MonoBehaviour {

	[SerializeField] private TextMeshProUGUI timeText;

	// Use this for initialization
	private void Start () {
		
	}
	
	// Update is called once per frame
	private void Update () {
		timeText.text = TimeKeeper.DayOfWeek.ToString() + ", " + TimeKeeper.FormattedTime;
	}
}
