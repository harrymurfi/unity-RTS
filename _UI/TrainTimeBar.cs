using UnityEngine;

public class TrainTimeBar : MonoBehaviour
{
	[HideInInspector]
	public Transform anchor;
	public Vector3 offset = new Vector3(-43f, 82f, 0f);
	public Vector3 customScale = Vector3.one;

	void Start()
	{
		ChangeScale(0);
	}

	void Update()
	{
		transform.position = Camera.main.WorldToScreenPoint(anchor.position) + offset;
	}

	public void ChangeScale(float scaleX)
	{
		customScale.x = scaleX;
		transform.localScale = customScale;
	}
}
