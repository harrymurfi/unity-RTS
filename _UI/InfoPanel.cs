using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InfoPanel : MonoBehaviour
{
	public Text Tname;
	public Text Tlevel;
	public Text Tproduction;
	public Text Thealth;

	void Start()
	{
		Entitas entity = Selection.instance.selectedEntities.Values.First();
		if(entity is ResourceField) 
		{
			//print("is resources field");
			Thealth.text = entity.healthPoint.ToString() + '/' + entity.maxHealth.ToString();
			Building b = (Building) entity;
			Tlevel.text = "Lv " + b.level.ToString();
			ResourceField rf = (ResourceField) entity;
			Tproduction.text = '+' + rf.production.ToString();

		}
		else if(entity is Villager)
		{
			//print("is villager");
			Thealth.text = entity.healthPoint.ToString() + '/' + entity.maxHealth.ToString();
		}
		else if(entity is Unit)
		{
			//print("is villager");
			Thealth.text = entity.healthPoint.ToString() + '/' + entity.maxHealth.ToString();
		}
	}
}
