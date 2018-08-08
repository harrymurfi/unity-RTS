using UnityEngine;
using UnityEngine.UI;

public class ExtentCommand : Command
{
	void Start()
	{
		this.tooltips = "Open " + name + " Tab";
		GetComponent<Button>().onClick.AddListener(() => CommandController.instance.ExtentCommand(name));
	}
}
