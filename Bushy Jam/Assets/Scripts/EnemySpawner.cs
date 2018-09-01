using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	public enum SpawnState { SPAWNING, WAITING, COUNTING }

	//Viewable in the Inspector
	[System.Serializable]
	public class Wave
	{
		public string Wavename;
		public Transform enemy;
		public int amount;
		public float spawnRate;
	}

	//Array of class Wave
	public Wave[] wavesList;
	//Index of wave
	private int nextWave = 0;

	//An array that holds the transform of spawnPoints
	public Transform[] spawnPoints;



	//Time between waves
	public float timeBetweenWaves = 5f;
	//The countdown time for the wave
	public float waveCountDown;

	//Time it takes for the game to search if the enemies are on screen
	//This makes it so that the game isnt CONSTANTLY searching
	//Thus using up resources.
	private float searchCountDown = 1f;

	//Store the spawn state. The default is the counting one.
	public SpawnState state = SpawnState.COUNTING;

	void Start()
	{
		waveCountDown = timeBetweenWaves;

		if (spawnPoints.Length == 0)
		{
			Debug.LogError("There is no spawn point. Make sure there is at least 1");
		}

	}

	 void Update()
	{
		if (state == SpawnState.WAITING)
		{
			if (EnemyIsAlive() == false)
			{
				//Begin a new round
				WaveCompleted();

			}
			else
			{
				//Return to the start
				return;
			}
		}

		//If it is time to start spawning waves....
		if (waveCountDown <= 0)
		{
			if (state != SpawnState.SPAWNING)
			{
				//Start wave spawning
				//IEnumeators use StartCoroutine
				StartCoroutine(SpawnWave(wavesList[nextWave]));
			}
			
		}

		else
		{
			//Start counting down
			waveCountDown -= Time.deltaTime;
		}
	}

	//A method that determines what to do when a wave is completed
	void WaveCompleted()
	{
		Debug.Log("Wave Complete");

		state = SpawnState.COUNTING;
		waveCountDown = timeBetweenWaves;

		//If the next wave is bigger than whats in the array,
		if (nextWave + 1 > wavesList.Length - 1)
		{
			//...Set it back to 0
			nextWave = 0;
			//We can do about any other function in here. For now
			//It just loops.
			Debug.Log("Completed all waves. Looping.");
		}
		else
		{
			//Increment the wave index
			nextWave++;
		}

		
	}

	//A method to check if there are any on screen enemies
	bool EnemyIsAlive()
	{
		searchCountDown -= Time.deltaTime;

		if (searchCountDown <= 0f)
		{
			//Reset the countdown to default.
			searchCountDown = 1f;
			if (GameObject.FindGameObjectWithTag("enemy") == null)
			{
				return false;
			}

		}

		return true;
	}
	//Spawning method that is made with an IEnumerator to allow delays
	IEnumerator SpawnWave (Wave _wave)
	{
		Debug.Log("Wave: " + _wave.Wavename);
		state = SpawnState.SPAWNING;

		//Spawn
		for (int i = 0; i < _wave.amount; i++)
		{
			SpawnEnemy(_wave.enemy);
			yield return new WaitForSeconds(1f / _wave.spawnRate);
		}

		state = SpawnState.WAITING;

		//IEnumerators need to return a value
		//This line allows us to return nothing
		yield break;
	}

	//This is our secondary spawn method that creates the enemys
	void SpawnEnemy (Transform _enemy)
	{
		//Spawn the enemy
		Debug.Log("Spawning Enemy: " + _enemy.name);
		//From the list of spawn points,
		//The _sp chooses one and takes on that properities
		Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
		//Then the enemy is instantiated from here.
		Instantiate(_enemy, _sp.position, _sp.rotation);
		
	}
}
 