using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour {
	//This is an alernate code to enemyControl from before
	//With this code, enemy movement should be more controlled than usual 
	//While still retaining randomness

	public float speed;
	private float waitTime;
	public float startWaitTime;

	Vector2 moveSpot;

	//These variables hold the min and max x and y cords
	//That our enemy can go to
	//This is to make it so that it doesnt reach the edge of the screen
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;

	//This is very similar to the code that is present in reflecting the shield.
	private float timeBtwShots;
	public float startTimeBtwShots;

	//Stuff that goes in the inspector to make this script work well
	//These are typically the other components of the enemies
	public GameObject projectile;
	public Transform player;

	///Health Bar Variables
	public float currentHealth { get; set; }
	public float maxHealth { get; set; }

	//The player gameobject, which we will get the script from
	public GameObject playerObject;


	// Use this for initialization
	void Start () {

		waitTime = startWaitTime;
		moveSpot = new Vector2(Random.Range(minX,maxX), Random.Range(minY,maxY));

		timeBtwShots = startTimeBtwShots;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerObject = GameObject.Find("Player");
		//The Maximum Health the enemy has
		maxHealth = 5f;
		//This is to reset the value of the health to full health every time the game is loaded.
		currentHealth = maxHealth;

	}
	
	// Update is called once per frame
	void Update () {

		transform.position = Vector2.MoveTowards(transform.position, moveSpot, speed * Time.deltaTime);

		if(Vector2.Distance(transform.position, moveSpot) < 0.2f)
		{
			//Is it time for the enemy to move to a new position?
			//If so,
			if (waitTime <= 0 )
			{
				moveSpot = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
				//Reset the wait time back to its initial waitTime
				waitTime = startWaitTime;
			}
			else
			{
				waitTime -= Time.deltaTime;
			}
		}

		//Like in attacking with our player, we want to prevent constant shooting.
		if (timeBtwShots <= 0)
		{
			//Instantiate(what we want to spawn, where, and what rotation)
			Instantiate(projectile, transform.position, Quaternion.identity);

			//We can edit startTimeBtwShots without having to edit in on visual studio
			//and just do it in the inspector.
			timeBtwShots = startTimeBtwShots;
		}
		else
		{
			timeBtwShots -= Time.deltaTime;
		}
	}

	float CalculateHealth()
	{
		return currentHealth / maxHealth;
	}

	//This function deals damage to the game object it is attached to
	//By "deal damage", i technically mean that this function will subtract
	//damageValue from currentHealth
	//In this version, I basically got rid of any mention of the health bar ui
	//since it wont be present for enemies
	public void DealDamage(float damageValue)
	{

		//Deal damage to the health bar
		currentHealth -= damageValue;

		//If the character is out of health, they die
		if (currentHealth <= 0)
		{

			print("Enemy died");
			Destroy(gameObject);
		}





	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "projectile" && other.gameObject.GetComponent<ProjectileControl>().damageEnemies == true)
		{
			DealDamage(5);
			Destroy(other.gameObject);
			playerObject.GetComponent<PlayerControl>().fillBar(5);
		}


	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "player_projectile")
		{
			DealDamage(5);
			Destroy(other.gameObject);
		}
	}

}



