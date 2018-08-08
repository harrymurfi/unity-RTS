using UnityEngine;
using UnityEngine.EventSystems;

public class Command : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public string tooltips;

	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData)
	{
		Tooltips.instance.ShowTooltips(tooltips);
	}
	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		Tooltips.instance.HideTooltip();
	}

	#endregion

	void OnDestroy()
	{
		Tooltips.instance.HideTooltip();
	}
}
