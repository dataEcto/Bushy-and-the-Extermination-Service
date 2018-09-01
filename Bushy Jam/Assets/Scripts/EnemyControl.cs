using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour {

	public float speed;
	public float stoppingDistance;
	public float retreatDistance;
	public float enemyRetreat;

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

	void Start () {
		timeBtwShots = startTimeBtwShots;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerObject = GameObject.Find("Player");
		//The Maximum Health the enemy has
		maxHealth = 5f;
		//This is to reset the value of the health to full health every time the game is loaded.
		currentHealth = maxHealth;
	}
	

	void Update () {

		//If the enemy is far away from the player, move closer
	
			if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
			{
				transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

			}

			//If it is near, stop moving
			else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > retreatDistance)
			{
				transform.position = this.transform.position;
			}

			//It may also want to retreat as well if the player gets closer while its stopped
			else if (Vector2.Distance(transform.position, player.position) < retreatDistance)
			{
				transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);

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







	
	

	
