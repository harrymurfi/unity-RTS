using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationsManager : MonoBehaviour {

	Dictionary<string, List<Component>> listeners = new Dictionary<string, List<Component>>();

	public void AddListener(Component sender, string notificationName)
	{
		// add listeners to dictionary
		if(!listeners.ContainsKey(notificationName))
			listeners.Add(notificationName, new List<Component>());

		// add obj to listeners list
		listeners[notificationName].Add(sender);
	}

	public void RemoveListener(Component sender, string notificationName)
	{
		// if no key in dictionary exist, then exist
		if(!listeners.ContainsKey(notificationName))
			return;

		// cycle through listeners and identify component, then remove
		for(int i = listeners[notificationName].Count - 1; i >= 0; i--)
		{
			// check InstanceID
			if(listeners[notificationName][i].GetInstanceID() == sender.GetInstanceID())
				listeners[notificationName].RemoveAt(i);
		}
	}

	public void PostNotification(Component sender, string notificationName)
	{
		// if no key in dictionary exist, then exist
		if(!listeners.ContainsKey(notificationName))
			return;

		// else post notification to all matching listener
		foreach(Component listener in listeners[notificationName])
			listener.SendMessage(notificationName, sender, SendMessageOptions.DontRequireReceiver);
		
	}

	public void ClearListeners()
	{
		listeners.Clear();
	}

	public void RemoveRedundancies()
	{
		Dictionary<string, List<Component>> tempListeners = new Dictionary<string, List<Component>>();

		foreach(KeyValuePair<string, List<Component>> item in listeners)
		{
			for(int i = item.Value.Count - 1; i >= 0; i--)
			{
				if(item.Value[i] == null)
					item.Value.RemoveAt(i);
			}

			// if item remain in list for notification, then add to temp dictionary
			if(item.Value.Count > 0)
				tempListeners.Add(item.Key, item.Value);
		}

		// replace listeners dictionary with new one
		listeners = tempListeners;
	}

	void OnLevelWasLoaded()
	{
		RemoveRedundancies();
	}
}
