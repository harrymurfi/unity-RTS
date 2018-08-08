using UnityEngine;

public class HitDisplayController : MonoBehaviour
{
	public GameObject hitPrefab;
	public Transform local;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GameObject go = Instantiate(hitPrefab, Camera.main.WorldToScreenPoint(local.transform.position), Quaternion.identity) as GameObject;
			go.transform.SetParent(transform, false);
		}
	}
}
