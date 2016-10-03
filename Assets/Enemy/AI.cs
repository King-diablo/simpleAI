using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {

  private NavMeshAgent navMesh;
  public Transform Player;
  public Transform[] randomPoints;

//protected Animator animator;

  private float playerDist, randomPointDist;
  public int currentRandomPoint;
  private bool chasing, chaseTime, attacking;
  private float chaseStopwatch, attackingStopwatch;

  public float perceptionDistance = 30, chaseDistance = 20, attackDistance = 1, walkVelocity = 2, chaseVelocity = 6, attackTime = 1.5f, enemyDamage = 50;

  public bool seeingPlayer;
  public float enemyLife;
  public float totalEnemyLife = 100;
  public string GameOverSceneName;

  void Start () {
	    //animator = GetComponent<Animator>();
		currentRandomPoint = Random.Range(0, randomPoints.Length);
	 	navMesh = transform.GetComponent<NavMeshAgent>();
		enemyLife = totalEnemyLife;
	}

  void Update () {

    playerDist = Vector3.Distance(Player.transform.position, transform.position);
    randomPointDist = Vector3.Distance(randomPoints[currentRandomPoint].transform.position, transform.position);
    RaycastHit hit;

    Vector3 startRay = transform.position;
    Vector3 endRay = Player.transform.position;
    Vector3 direction = endRay - startRay;

    if (Physics.Raycast (transform.position, direction, out hit, 1000) && playerDist < perceptionDistance ) {
      if (hit.collider.gameObject.CompareTag("Player")) {
        seeingPlayer = true;
      } else {
        seeingPlayer = false;
      }
    }

    if ( playerDist > perceptionDistance)
      walk();

 	if ( playerDist <= perceptionDistance && playerDist > chaseDistance) {
		if (seeingPlayer == true)
			look();
		else
			walk();
	}

    if ( playerDist <= chaseDistance && playerDist > attackDistance) {
			if (seeingPlayer == true) {
				chase();
				chasing = true;
			} else {
				walk();
			}
		}

    if (playerDist <= attackDistance && seeingPlayer == true)
			attack();

    if (randomPointDist <= 8) {
      currentRandomPoint = Random.Range(0, randomPoints.Length);
	  walk();
	}

    if (chaseTime == true)
      chaseStopwatch += Time.deltaTime;

    if (chaseStopwatch >= 5 && seeingPlayer == false) {
      chaseTime = false;
      chaseStopwatch = 0;
      chasing = false;
    }

    if (attacking == true)
      attackingStopwatch += Time.deltaTime;

    if (attackingStopwatch >= attackTime && playerDist <= attackDistance) {
	  attacking = true;
      attackingStopwatch = 0;
	  PLAYER.life = PLAYER.life - enemyDamage;

	  if (PLAYER.life < 5)
		Application.LoadLevel(GameOverSceneName);
	  else if (attackingStopwatch >= attackTime && playerDist > attackDistance) {
		attacking = false;
		attackingStopwatch = 0;
	   }

	}
}

  void OnCollisionEnter (Collision col) {
      if(col.transform.tag == "Bullet"){
        enemyLife -= 10;
        if (enemyLife <= 0) {
        	//animator.SetBool("Dying" , true);
            seeingPlayer = false;
            walkVelocity = 0;
            attackDistance = 0;
            chaseVelocity = 0;
            chaseDistance = 0;
            enemyDamage = 0;
			currentRandomPoint = 0;
        }
    }
    }

    void walk () {

  		if (chasing == false) {
  			//animator.SetFloat("Speed", 0.5f);
  		    //animator.SetBool("Attack" , false);
  			navMesh.acceleration = 4;
  			navMesh.speed = walkVelocity;
  			navMesh.destination = randomPoints[currentRandomPoint].position;

  		} else if (chasing == true) {
  			//animator.SetFloat("Speed", 0.5f);
  			//animator.SetBool("Attack" , false);
  			chaseTime = true;
  		}
  	}

  	void look () {
  		navMesh.speed = 0;
  		transform.LookAt (Player);
  	}

  	void chase () {
  		//animator.SetFloat("Sprint", 0.4f);
  		//animator.SetBool("Attack" , false);
  		navMesh.acceleration = 8;
  		navMesh.speed = chaseVelocity;
  		navMesh.destination = Player.position;
  	}

  	void attack () {
  		//animator.SetBool("Attack" , true);
  		navMesh.acceleration = 0;
  		navMesh.speed = 0;
  		attacking = true;
  	}

}
