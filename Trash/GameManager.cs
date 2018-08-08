using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NotificationsManager))]
public class GameManager : MonoBehaviour {

	static GameManager instance;
	static NotificationsManager notification;
	static SaveManager saveLoadManager;

	public static GameManager Instance
	{
		get
		{
			if(instance == null)
				instance = new GameObject("GameManager").AddComponent<GameManager>();

			return instance;
		}
	}

	public static NotificationsManager Notification
	{
		get
		{
			if(notification == null)
				notification = instance.GetComponent<NotificationsManager>();

			return notification;
		}
	}

	public static SaveManager SaveLoadManager
	{
		get
		{
			if(saveLoadManager == null)
				saveLoadManager = instance.GetComponent<SaveManager>();

			return saveLoadManager;
		}
	}

	void Awake()
	{
		if((instance) && (instance.GetInstanceID() != this.GetInstanceID()))
		{
			DestroyImmediate(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public void RestartGame()
	{
		
	}

	public void QuitGame()
	{
		
	}

	public void SaveGame()
	{
		
	}

	public void LoadGame()
	{
		
	}
}
