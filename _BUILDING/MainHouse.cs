using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainHouse : Entitas
{
	Renderer rend;
	NavMeshObstacle obstacle;
	SpriteRenderer marker;
	public Image hpbarImage;
	public HPBar hpBar;
	public BuildTimeBar buildBar;
	public Transform trainBar;

	public BuildingState currentState;
	Vector3 anchor;
	bool invalidLocation;
	public float buildTime;
	float startBuildTime;
	public float percentBuilt;

	public Entitas builder;
	public bool isBuilderExist;

	public GameObject villagerPrefab;
	public Transform spawnPoint;
	float trainTime = 5;
	ResourceManager resoManager;
	public int inQueue;
	public float trainingPercent;
	Vector3 trainScale = Vector3.one;
	float trainStartingTime;
	bool inTraining;


	protected override void Start()
	{
		base.Start();
		instanceID = GetInstanceID();
		rend = GetComponentInChildren<Renderer>();
		obstacle = GetComponent<NavMeshObstacle>();
		marker = GetComponentInChildren<SpriteRenderer>();
		marker.enabled = false;
		hpbarImage.enabled = false;
		hpBar = hpbarImage.GetComponent<HPBar>();
		resoManager = GameObject.FindGameObjectWithTag("Player").GetComponent<ResourceManager>();
	}

	void Update()
	{
		TrainUnit();
	}

	void TrainUnit()
	{
		if(!inTraining)
		{
			if(inQueue > 0)
			{
				trainStartingTime = Time.time;
				inTraining = true;
			}
		}
		else
		{
			if(Time.time < trainStartingTime + trainTime)
			{
				trainingPercent = (Time.time - trainStartingTime) / trainTime;
			}
			else
			{
				inQueue -= 1;
				trainingPercent = 0;
				inTraining = false;
				Instantiate(villagerPrefab, spawnPoint.position, Quaternion.identity);
			}
		}
	}

	public void AddTrainingQueueu()
	{
		inQueue += 1;
	}

	public override void OnSelected()
	{
		if(currentState != BuildingState.Destroyed)
		{
			marker.enabled = true;
			hpbarImage.enabled = true;
		}
	}

	public override void OnDeselected()
	{
		if(currentState != BuildingState.Destroyed)
		{
			marker.enabled = false;
			hpbarImage.enabled = false;
		}
	}
}
