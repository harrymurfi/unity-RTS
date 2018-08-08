using UnityEngine;
using System.Collections;

public class RTSCamera : MonoBehaviour {

	public float scrollSpeed = 2;
	float margin = 20;
	float marginTop;
	float marginBottom;
	float marginRight;
	float marginLeft;

	Vector3 newPosition;
	Vector3 movement;

	[HideInInspector]
	public bool canScrollView = true;

	void Start()
	{
		SetCameraMargin();
		newPosition = transform.position;
	}

	void Update()
	{
		if(canScrollView)
		{
			Vector3 mouse = Input.mousePosition;
			
			if(mouse.y > marginTop || Input.GetKey(KeyCode.W))
				movement.z += scrollSpeed * Time.deltaTime;
			else if(mouse.y < marginBottom || Input.GetKey(KeyCode.S))
				movement.z -= scrollSpeed * Time.deltaTime;
			else
				movement.z = 0;
			
			if(mouse.x > marginRight || Input.GetKey(KeyCode.D))
				movement.x += scrollSpeed * Time.deltaTime;
			else if(mouse.x < marginLeft || Input.GetKey(KeyCode.A))
				movement.x -= scrollSpeed * Time.deltaTime;
			else
				movement.x = 0;

//			if(Input.GetKey(KeyCode.W))
//				movement.z += scrollSpeed * Time.deltaTime;
//			else if(Input.GetKey(KeyCode.S))
//				movement.z -= scrollSpeed * Time.deltaTime;
//			else
//				movement.z = 0;
//
//			if(Input.GetKey(KeyCode.D))
//				movement.x += scrollSpeed * Time.deltaTime;
//			else if(Input.GetKey(KeyCode.A))
//				movement.x -= scrollSpeed * Time.deltaTime;
//			else
//				movement.x = 0;
			
			movement.z = Mathf.Clamp(movement.z, -0.8f, 0.8f);
			movement.x = Mathf.Clamp(movement.x, -0.8f, 0.8f);
			
			newPosition = newPosition + movement;
			newPosition.x = Mathf.Clamp(newPosition.x, 0, 40);
			newPosition.z = Mathf.Clamp(newPosition.z, -35, 10);
			transform.position = Vector3.Lerp(transform.position, newPosition, 0.2f);
		}
	}

	public void SetCameraMargin()
	{
		marginTop = Screen.height - margin;
		marginBottom = margin;
		marginRight = Screen.width - margin;
		marginLeft = margin;
	}
}
