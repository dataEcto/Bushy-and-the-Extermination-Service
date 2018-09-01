using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControl : MonoBehaviour
{

	public enum PlayerState { SHIELD, GUN }

	///Movement Variables
	Rigidbody2D player_rb;
	public float speed;
	private Vector2 moveVelocity;

	///Health Bar Variables
	public Slider healthBar;
	public float currentHealth { get; set; }
	public float maxHealth { get; set; }
	public TextMeshProUGUI healthBarText;

	//Progress Bar Variables
	public Slider progressBar;
	public float currentProgress { get; set; }
	public float maxProgress { get; set; }
	public TextMeshProUGUI progressBarText;

	///Attacking Variables
	float timeBtwAttack;
	public float startTimeBtwAttack;
	private Animator player_anim;

	//Gun Variables
	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	//State Control
	public PlayerState state = PlayerState.SHIELD;

	void Start()
	{
		player_rb = GetComponent<Rigidbody2D>();
		player_anim = GetComponent<Animator>();

		//The Maximum Health the player has
		maxHealth = 100f;
		//This is to reset the value of the health bar to full health every time the game is loaded.
		currentHealth = maxHealth;
		///Visual representation of the health.
		//Get the value of the slider object, 
		//set it to calculate health
		//CalculateHealth() is mainly used when the damage function and healing function are called
		healthBar.value = CalculateHealth();
		//Display the intial health
		healthBarText.text = "Health: " + currentHealth;


		//The maximum amount of points needed to fill out the Progress Bar
		maxProgress = 20f;
		//Reset the progress bar to 0 everytime the game starts
		currentProgress = 0;
		//Similar to the healtbar, we need to set the value of the Progress Bar/Slider object
		progressBar.value = CalculateProgress();
		//Display text for the progress bar. For now, we can have it set to nothing
		//But once it is full, we will
		progressBarText.text = "";

		//Camera testing stuff
		float height = 2 * Camera.main.orthographicSize;
		float width = height * Camera.main.aspect;
		print("The Height is " + height);
		print("the width is " + width);



	}

	//Consists of parts of movement code and attacking code
	void Update()
	{
		///NEW Movement Code
		//This variable detects where we want to move
		//This is a quick way to map movement! 
		//Left Arrow key - Horizontal input becomes -1, Right Arrow key, 1, and so on with the Vertical
		//In Edit -> Project settings -> input you can change these inputs.
		Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));
		//Calculates the speed;
		//While also normalizing diagonal speed
		moveVelocity = moveInput.normalized * speed;

		//Displays the CURRENT health
		healthBarText.text = "Health: " + Mathf.Round(currentHealth);

		//Display a prompt when the player reaches the max amount of Progress
		if (currentProgress == maxProgress)
		{
			progressBarText.text = "Press L!";
			if (Input.GetKey(KeyCode.L))
			{
				Debug.Log("Get Weapon");
				state = PlayerState.GUN;
			}
		}


		///Attacking Code
		//Time between attack starts off as StartTimeBtwAttack everytime it is less than 0
		//It seems like a paradox in the way its written here, actually.
		if (timeBtwAttack <= 0)
		{


			if (state == PlayerState.SHIELD)
			{
				//Enable the attack
				//Important to note that I am using place holder assets right now
				//So the sheild hitbox is a seperate object. This could server to be problematic once art is implemented
				//Solution:
				//Look back at the blackthornpord sword tutorial and recreate the code from there. This code right now
				//is already based on that after all. 
				if (Input.GetKey("space"))
				{
					player_anim.SetTrigger("attack");

					Debug.Log("Attacking");

					timeBtwAttack = startTimeBtwAttack;

				}
			}

			if (state == PlayerState.GUN)
			{
				//Decrease the bar over time
				lowerBar(5f);
				//Enable the attack
				if (Input.GetKey("space"))
				{
					lowerBar(5f);
					Debug.Log("Attacking with Gun");
					Fire();
					timeBtwAttack = startTimeBtwAttack;

				}
				if (currentProgress == 0)
				{
					state = PlayerState.SHIELD;
				}
			}



		}

		else
		{
			//Start counting down to allow attack
			timeBtwAttack -= Time.deltaTime;

		}


	}

	//Fixedupdate is called on every physics step in our game.
	void FixedUpdate()
	{

		player_rb.MovePosition(player_rb.position + moveVelocity * Time.fixedDeltaTime);

	}

	//The function that calculates what the health is currently.
	//What it does is take the health at the moment, then divide it by max health
	float CalculateHealth()
	{
		return currentHealth / maxHealth;
	}

	//Same as above
	float CalculateProgress()
	{
		return currentProgress / maxProgress;
	}

	//This method will fill up the progress bar
	//Whenever the reflected projectile hits an enemy!
	//This method will be called in EnemyControl
	//Because there is code in there that focues on what to do
	//When the enemy is hit
	public void fillBar(float progressValue)
	{
		currentProgress += progressValue;
		progressBar.value = CalculateProgress();

		if (currentProgress > maxProgress)
		{
			currentProgress = maxProgress;

		}
	}

	void Fire()
	{
		// Create the Bullet from the Bullet Prefab
		var bullet = (GameObject)Instantiate(
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		// Add velocity to the bullet
		bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * 6;

		// Destroy the bullet after 2 seconds
		Destroy(bullet, 2.0f);
	}

	//This function lowers the bar when the weapon is active
	public void lowerBar(float lowerValue)
	{
		currentProgress -= lowerValue * Time.deltaTime;
		progressBar.value = CalculateProgress();

		if (currentProgress <= 0)
		{
			currentProgress = 0;
			state = PlayerState.SHIELD;

		}
	}

	//This function deals damage to the game object it is attached to
	//By "deal damage", i technically mean that this function will subtract
	//damageValue from currentHealth
	public void DealDamage(float damageValue)
	{

		//Deal damage to the health bar
		currentHealth -= damageValue;
		//Same as from start
		healthBar.value = CalculateHealth();

		//If the character is out of health, they die
		if (currentHealth <= 0)
		{
			//This displays the health to be 0.
			//This is to prevent negative numbers to show up
			healthBarText.text = "Health: " + 0;
			print("You Died");
			Destroy(gameObject);
		}

	}

	//This code restores health
	//Used for fixing the damage taking while reflecting issue
	public void RestoreHealth(float healthGained)
	{
		Debug.Log("Healing");
		//Prevent the player from restoring full health
		if (currentHealth >= maxHealth)
		{
			currentHealth = maxHealth;
			Debug.Log("Health has been restored to full");
		}

		currentHealth += healthGained;
		healthBar.value = CalculateHealth();


	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "projectile")
		{
			print("Take Damage");
			DealDamage(5);
		}
	}

}