using UnityEngine;
using UnityEngine.UI;

public class CancelCommand : Command
{
	void Start()
	{
		this.tooltips = name;
		GetComponent<Button>().onClick.AddListener(() => CommandController.instance.CancelCommand());
		Selection.instance.cancelEvent += OnCancelBuild;
	}

	public void OnCancelBuild()
	{
		CommandController.instance.CancelCommand();
		Selection.instance.cancelEvent -= OnCancelBuild;
	}
}
