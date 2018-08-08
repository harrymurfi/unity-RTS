using UnityEngine;
using System.Collections;

public class LineOfSight : MonoBehaviour {

	public GameObject unit;

	void Start()
	{
		unit = transform.parent.gameObject;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 12)
		{
		}
	}
}
