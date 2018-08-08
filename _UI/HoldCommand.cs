using UnityEngine;
using UnityEngine.UI;

public class HoldCommand : Command
{
	Button btn;

	void Start()
	{
		tooltips = "Hold (H)";
		btn = GetComponent<Button>();
		GetComponent<Button>().onClick.AddListener(Hold);
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.H))
		{
			btn.onClick.Invoke();
		}
	}

	void Hold()
	{
		Selection.instance.SendHold();
	}
}
