using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltips : MonoBehaviour
{
	public static Tooltips instance;

	public CanvasGroup tooltipsGroup;
	public Text tooltipsText;
	public bool on;

	void Awake()
	{
		if(instance == null) instance = this;
		else
		{
			if(instance != this) Destroy(gameObject);
		}
	}

	void Start()
	{
		tooltipsGroup.alpha = 0;
		tooltipsGroup.blocksRaycasts = false;
	}

	public void ShowTooltips(string text)
	{
		tooltipsText.text = text;
		tooltipsGroup.alpha = 1;
		on = true;
//		StopAllCoroutines();
//		StartCoroutine(Display());
	}

	public void HideTooltip()
	{
		tooltipsGroup.alpha = 0;
		on = false;
	}

	IEnumerator Display()
	{
		tooltipsGroup.alpha = 1;
		yield return new WaitForSeconds(3);
		while(tooltipsGroup.alpha > 0)
		{
			tooltipsGroup.alpha -= 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
	}
}
