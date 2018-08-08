using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ActionPanelController : MonoBehaviour
{
	public GameObject[] actionPanelPrefab;
	GameObject temp;

//	public Dictionary<int, GameObject> actionPanelList = new Dictionary<int, GameObject>();

//	[Serializable]
//	public struct EntityInfo
//	{
//		public int id;
//		public GameObject obj;
//	}
//
//	public EntityInfo[] entitiesList;

	void Update()
	{
		
	}

	public void CreateActionPanel(int id)
	{
		if(id == 99)
		{
			Debug.LogError("Mass Selection");
		}
		if(id < 0 || id >= actionPanelPrefab.Length) 
		{
			Debug.LogError("Iselect id out of range.");
			return;
		}
		temp = Instantiate(actionPanelPrefab[id]);
		temp.transform.SetParent(this.transform, false);
	}

	public void ClearActionPanel()
	{
		if(temp != null) Destroy(temp.gameObject);
		else Debug.LogWarning("No info panel to deleted.");
	}
}
