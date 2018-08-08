using UnityEngine;
using UnityEngine.UI;
public class UpgradeCommand : MonoBehaviour
{
	void Start()
	{
		GetComponent<Button>().onClick.AddListener(() => CommandController.instance.UpgradeBuilding());
	}
}
