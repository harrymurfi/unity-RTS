using UnityEngine;
using UnityEngine.UI;

public class HitpointBar : MonoBehaviour
{
	public Transform anchor;

	void Update()
	{
		if(anchor != null) 
			transform.position = Camera.main.WorldToScreenPoint(anchor.position);
	}
}
