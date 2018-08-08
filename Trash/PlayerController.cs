using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	public string playerName;
	public Team playerTeam;

	public List<Entity> playerEntities = new List<Entity>();
	public List<Entity> visibleEntities = new List<Entity>();

}