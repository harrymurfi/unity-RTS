using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform lookTarget;
	float offset = 7;

	Vector3 dir, newPosition;

	void Update()
	{
		dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		dir = dir.normalized;
		newPosition = lookTarget.position + dir;
		lookTarget.position = Vector3.Lerp(lookTarget.position, newPosition, Time.deltaTime * 30);
		transform.position = lookTarget.position + new Vector3(0, offset + 7, -offset);
		transform.LookAt(lookTarget.position);
	}
}
