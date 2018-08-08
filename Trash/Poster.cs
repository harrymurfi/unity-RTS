using UnityEngine;
using System.Collections;

public class Poster : MonoBehaviour {

	public NotificationsManager notif;

	void Update()
	{
		if(Input.anyKeyDown && notif != null)
			notif.PostNotification(this, "OnKeyboardInput");
	}
}
