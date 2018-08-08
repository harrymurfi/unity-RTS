using UnityEngine;

public class MainBuilding : Building, ITrainable
{
	public GameObject villagerPrefab;

	ResourceManager resoManager;
	TrainTimeBar trainBar;

	float trainTime = 5;
	public int inQueue;
	public float trainingPercent;
	Vector3 trainScale = Vector3.one;
	float trainStartingTime;
	bool inTraining;

	protected override void Start()
	{
		base.Start();
		trainBar = GetComponentInChildren<TrainTimeBar>();
		trainBar.anchor = transform;
		trainBar.transform.SetParent(UIController.instance.attributesContainer.transform, false);
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void Complete()
	{
		base.Complete();
		TrainUnit();
	}

	public override void Upgrade()
	{
		base.Upgrade();
	}

	public void AddTrainingQueueu()
	{
		AudioController.instance.PlaySingle("training");
		inQueue += 1;
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
			trainBar.ChangeScale(trainingPercent);
		}
	}

	#region ITrainable implementation

	public void Train(string identifier)
	{
		inQueue += 1;
	}

	#endregion
}
