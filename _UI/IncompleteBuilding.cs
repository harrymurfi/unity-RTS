using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class IncompleteBuilding : MonoBehaviour
{
	public Text Tname;
	public Text Tpercentage;
	Building b;

	void Start()
	{
		Entitas entity = Selection.instance.selectedEntities.Values.First();
		if(entity is Building)
		{
			Tname.text = entity.entityName;
			b = (Building) entity;
			b.OnBuildComplete += OnBuildComplete;
			b.deathEvent += OnBuildingDestroyed;
		}
		else
		{
			print("Not syncronize. Destroy " + name);
			Destroy(gameObject);
		}
	}

	void Update()
	{
		int p = (int) (b.percentBuilt * 100);
		Tpercentage.text = p.ToString() + "%";
	}

	void OnBuildComplete()
	{
		b.OnBuildComplete -= OnBuildComplete;
		CommandController.instance.Create(b.entityName);
		Destroy(gameObject);
	}

	void OnBuildingDestroyed()
	{
		b.deathEvent -= OnBuildingDestroyed;
		Destroy(gameObject);
	}

	void OnDestroy()
	{
		b.OnBuildComplete -= OnBuildComplete;
		b.deathEvent -= OnBuildingDestroyed;
	}
}
