using UnityEngine;
using System.Collections;

public class Listener : MonoBehaviour {

	public NotificationsManager notif;

	void Start()
	{
		if(notif != null)
			notif.AddListener(this, "OnKeyboardInput");
	}

	public void OnKeyboardInput()
	{
		Debug.Log(this + "listening to");
	}
}
