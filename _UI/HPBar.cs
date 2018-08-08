using UnityEngine;

public class HPBar : MonoBehaviour
{
	[HideInInspector]
	public Transform anchor;
	public Vector3 offset = new Vector3(-17.5f, -10f, 0f);
	Vector3 hitbarScale = Vector3.one;

	void Start()
	{
//		transform.SetParent(UIController00.instance.attributesContainer.transform, false);
	}

	void LateUpdate()
	{
		if(anchor != null)
		{
			transform.position = Camera.main.WorldToScreenPoint(anchor.position) + offset;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void ChangeHitbarScale(float scale)
	{
		hitbarScale.x = scale;
		transform.localScale = hitbarScale;
	}
}
