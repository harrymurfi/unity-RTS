using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils 
{
	public static Quaternion standardRotation = Quaternion.Euler(new Vector3(0, 45, 0));

	public static Color greenGhostColor = new Color(0, 1, 0, 0.25f);
	public static Color invalidColor = new Color(1, 0, 0, 0.25f);

	public static Dictionary<string, ResourceInfo> buildingCostTable = new Dictionary<string, ResourceInfo>()
	{
		{"Main Building", new ResourceInfo(0, 0, 0, 0)},
		{"Cropland", new ResourceInfo(70, 90, 70, 20)},
		{"Woodcutter", new ResourceInfo(40, 100, 50, 60)},
		{"Clay Pit", new ResourceInfo(80, 40, 80, 50)},
		{"Iron Mine", new ResourceInfo(100, 80, 30, 60)},
		{"Warehouse", new ResourceInfo(100, 100, 100, 100)},
		{"Granary", new ResourceInfo(100, 100, 100, 100)},
		{"Barracks", new ResourceInfo(100, 80, 30, 60)},
	};

	public static Dictionary<string, ResourceInfo> unitCostTable = new Dictionary<string, ResourceInfo>()
	{
		{"Villager", new ResourceInfo(100, 100, 100, 100)},
		{"Legionnaire", new ResourceInfo(120, 100, 150, 30)},
		{"Praetorian", new ResourceInfo(100, 130, 160, 70)},
	};

	public static Entity FindEntityAt(Camera camera, Vector3 position)
	{
		RaycastHit hit;

		if(Physics.Raycast(camera.ScreenPointToRay(position), out hit, 100))
		{
			Entity entity = hit.transform.gameObject.GetComponent<Entity>();
			if(entity != null)
			{
				return entity;
			}
		}

		return null;
	}

	public static Bounds GetViewportBounds(Camera camera, Vector3 inputStart, Vector3 inputEnd)
	{
		Vector3 inputStartView = camera.ScreenToViewportPoint(inputStart);
		Vector3 inputEndView = camera.ScreenToViewportPoint(inputEnd);
		Vector3 min = Vector3.Min(inputStartView, inputEndView);
		Vector3 max = Vector3.Max(inputStartView, inputEndView);

		min.z = camera.nearClipPlane;
		max.z = camera.farClipPlane;

		Bounds bound = new Bounds();
		bound.SetMinMax(min, max);
		return bound;
	}

	public static bool IsWithinBounds(Camera camera, Bounds viewportBounds, Vector3 position)
	{
		Vector3 viewportPoint = camera.WorldToViewportPoint(position);
		return viewportBounds.Contains(viewportPoint);
	}
}

public enum Team
{
	Red, Blue, Green, Yellow,
}

public enum UnitType 
{
	TypeCube, TypeSphere, TypeCapsule, Anaconda, Worker,
}

public enum SelectionState
{
	Disable, Free, OnMove, OnAttackMode, OnBuild,
}

public enum UnitState
{
	None, Idle, Standby, Move, Stop, Aggresive, AggresiveMove, Chase, Attack, Patrol, Hold, Build, Work, Dead,
}

public enum BuildingState
{
	None, Hover, Prepare, Construct, Complete, Destroyed,
}

public enum ResourceType
{
	Wood, Clay, Iron, Crop,
}
