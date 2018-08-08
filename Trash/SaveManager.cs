using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SaveManager : MonoBehaviour {

	[XmlRoot("GameData")]
	public class GameStateData
	{
		public struct TransformData
		{
			public float X, Y, Z;
			public float rotX, rotY, rotZ;
			public float sclX, sclY, sclZ;
		}

		public class PlayerData
		{
			public TransformData playerTransData;
		}

		public class EnemyData
		{
			public TransformData enemyTransData;
		}

		public PlayerData playerData = new PlayerData();
		public List<EnemyData> enemiesData = new List<EnemyData>();
	}

	public GameStateData gameStateData = new GameStateData();

	// save to xml
	public void Save(string fileName = "GameData.xml")
	{
		// clear enemies data
		gameStateData.enemiesData.Clear();

		// call save start notification
		GameManager.Notification.PostNotification(this, "SaveGamePrepare");

		// saving game
		XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream stream = new FileStream(fileName, FileMode.Create);
		serializer.Serialize(stream, gameStateData);
		stream.Close();

		// call save end notification
		GameManager.Notification.PostNotification(this, "SaveGameCompleted");
	}

	// load from xml
	public void Load(string fileName = "GameData.xml")
	{
		// call load start notification
		GameManager.Notification.PostNotification(this, "LoadGamePrepare");

		// loading
		XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
		FileStream stream = new FileStream(fileName, FileMode.Open);
		gameStateData = serializer.Deserialize(stream) as GameStateData;
		stream.Close();

		// call load end notification
		GameManager.Notification.PostNotification(this, "LoadGameCompleted");
	}
}
