using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TrainCommand : Command
{
	void Start()
	{
		this.tooltips = "Train " + name;
		GetComponent<Button>().onClick.AddListener(() => CommandController.instance.TrainCommand(name));
	}
}
