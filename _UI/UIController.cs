using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIController : MonoBehaviour
{
	public static UIController instance { get; private set; }

	public CommandController commandController;
	public ActionPanelController actionController;
	public Transform attributesContainer;

	public Text[] ProductionHUD;
	public Text[] ProductionRateHUD;

	public string wareHouseCap = "/800";
	public string granaryCap = "/800";

	public Texture2D normalCursor;
	public Texture2D attackCursor;

	void Awake()
	{
		if(instance == null) instance = this;
		else Destroy(gameObject);
	}

	void Start()
	{
		commandController = GetComponentInChildren<CommandController>();
		actionController = GetComponentInChildren<ActionPanelController>();
	}

	public void UpdateProductionHud(ResourceInfo resources)
	{
		ProductionHUD[0].text = resources.wood.ToString() + wareHouseCap;
		ProductionHUD[1].text = resources.clay.ToString() + wareHouseCap;
		ProductionHUD[2].text = resources.iron.ToString() + wareHouseCap;
		ProductionHUD[3].text = resources.crop.ToString() + granaryCap;
	}

	public void UpdateProductionRateHud(ResourceType type, int amount)
	{
		switch(type)
		{
		case ResourceType.Wood: ProductionRateHUD[0].text = '+' + amount.ToString(); break;
		case ResourceType.Clay: ProductionRateHUD[1].text = '+' + amount.ToString(); break;
		case ResourceType.Iron: ProductionRateHUD[2].text = '+' + amount.ToString(); break;
		case ResourceType.Crop: ProductionRateHUD[3].text = '+' + amount.ToString(); break;
		}
	}

	public void ChangeCursor(string name)
	{
		switch(name)
		{
		case "attack":
			{
				Cursor.SetCursor(attackCursor, Vector2.zero, CursorMode.Auto);
				break;
			}
		default:
			{
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				break;
			}
		}
	}
}
