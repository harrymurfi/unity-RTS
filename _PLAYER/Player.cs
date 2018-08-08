using UnityEngine;

public class Player : MonoBehaviour
{
	public string PlayerName;
	public TeamColor team;
	public LayerMask playerMask;
	public LayerMask enemyMask;
}

public enum TeamColor
{
	Blue,
	Red,
	Green,
	Yellow
}
