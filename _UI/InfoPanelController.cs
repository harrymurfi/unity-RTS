using UnityEngine;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
	public GameObject[] infoPanelPrefab;
	GameObject temp;

	void Update()
	{
		
	}

	public void CreateInfoPanel(int id)
	{
		temp = Instantiate(infoPanelPrefab[id]);
		temp.transform.SetParent(this.transform, false);
	}

	public void ClearInfoPanel()
	{
		if(temp != null) Destroy(temp.gameObject);
		else Debug.LogWarning("No info panel to deleted.");
	}
}
