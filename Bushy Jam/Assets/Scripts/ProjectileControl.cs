using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControl : MonoBehaviour {
	public float speed;
	private Transform player;
	private GameObject playerObject;
	private Vector2 target;

	public bool reflectProjectile;
	//In order to prevent the projectile from damaging the enemy when it isnt reflected
	//I first set up this boolean
	public bool damageEnemies;

	
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerObject = GameObject.Find("Player");
		target = new Vector2(player.position.x, player.position.y);
		//This gets set to false first.
		//Go to ontriggerenter for more
		damageEnemies = false;
	}
	

	void Update () {

		//This is basically the core of making the projectile move.
		//If you want to have a homing projectile towards the player, change "target" with player
		if (reflectProjectile == false)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
	
		}
		//This allows for reflection
		else
		{
			transform.Translate(Vector3.right * speed * Time.deltaTime);

		}
		

		//If the x and y coordinates are equal to the targets coordinates
		if (transform.position.x == target.x && transform.position.y == target.y)
		{
			DestroyProjectile();
			
		}
	}

	void DestroyProjectile()
	{
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "player")
		{
			DestroyProjectile();

		}

		if (collisionInfo.gameObject.tag == "shield_tag" )
		{
			reflectProjectile = true;
			//Now that this is true, the projectile can now damage enemies!
			damageEnemies = true;

			//This function is here because
			//Whenever I reflect the projectile, the player still ends up taking damage
			//So a fix would be to restore health at the same time
			//We use the playerObject that we instantiated earlier
			playerObject.GetComponent<PlayerControl>().RestoreHealth(5);


		}

	}
}
