using System;
using System.Collections;
using UnityEngine;

public class BotLogic : MonoBehaviour
{
    public enum BotType { GreenSpaceship, RedSpaceship };
    public enum BotState { patrol, chase, attack };

    public BotType enemyType;

    public float rotateMinSpeed = 70f;
    public float rotateMaxSpeed = 90f;

    public int maxHealth = 3;
    public int damage = 1;
    public float attackInterval = 1.0f;

    private GameObject planet;//the planet GameObject that the spaceship rotates around
    private Vector3 center;//the center (position) of the planet
    private float angle;//rotation angle
    private float rotateSpeed;//rotation speed (around the planet)
    private int health;//current spaceship's health
    private Vector3 axis;//rotation axis around the planet

    private string enemyTag;//enemy's tag name

    private BotState state = BotState.patrol;//bot's current state
    private GameObject currentTarget = null;//bot's current enemy target

    private float nextAttack = 0f;//time for next attack

    private AudioSource audioSource;//bot's audio source for attack sounds

    void Start()
    {
        enemyTag = Enum.GetName(typeof(BotType), enemyType);
        audioSource = gameObject.GetComponent<AudioSource>();
        center = planet.transform.position;//where to rotate around
        rotateSpeed = UnityEngine.Random.Range(rotateMinSpeed, rotateMaxSpeed);//calculate random spaceship's rotation speed (around planet)

        health = maxHealth;//set current health to max health

        //pick a random axis to rotate around the planet
        int randAxis = UnityEngine.Random.Range(0, 4);
        switch (randAxis)
        {
            case 1:
                axis = Vector3.down;
                break;
            case 2:
                axis = Vector3.left;
                break;
            case 3:
                axis = Vector3.right;
                break;
            default:
                axis = Vector3.up;
                break;
        }
    }

    void Update()
    {
        if(Time.timeScale == 0f)
        {
            return;
        }

        //check if spaceship is dead
        if(health == 0)
        {
            gameObject.SetActive(false);//deactivate object
            Invoke("ClearObject", 1f);//Destroy it after 1s (for other bots to clean up this object as their target)
            return;
        }

        //check if current target bot is dead
        if(currentTarget != null)
        {
            if(!currentTarget.activeSelf)
            {
                currentTarget = null;
            }
        }

        //decide next action, based on state
        switch(state)
        {
            case BotState.patrol:
                Patrol();
                break;
            case BotState.chase:
                Chase();
                break;
            case BotState.attack:
                Attack();
                Chase();
                break;
        }
    }

    /**
     * Patrol Action. During this state, the bot simply rotates
     * around the target planet, in its random rotation speed
     * and random axis.
     */
    void Patrol()
    {
        //check if there are any enemy spaceships
        if(FindClosestEnemy())
        {
            state = BotState.chase;//go to chase state, if there are enemies
            return;
        }

        angle = rotateSpeed * Time.deltaTime;//calculate movement angle

        transform.RotateAround(center, axis, angle);//rotate around planet
    }

    /**
     * Chase Action. During this state, the bot chases the closest
     * enemy spaceship. It keeps rotating around the planet while
     * doing the chase.
     */
    void Chase()
    {
        //if target enemy is dead (or not exists), goto patrol state
        if(currentTarget == null)
        {
            state = BotState.patrol;
            return;
        }

        //if target is in attack range, goto attack state
        if (CheckInAttackRange())
        {
            state = BotState.attack;
        }
        //else find the closest enemy
        else
        {
            state = BotState.chase;
            FindClosestEnemy();
        }

        angle += rotateSpeed * Time.deltaTime;//calculate attacking rotation speed/angle
        angle = Mathf.Clamp(angle, 0f, 1f);//clamp the value (prevent too high speed)

        if (Vector3.Distance(currentTarget.transform.position, transform.position) > 3f)//check if to close to enemy, so to stop going towards him
        {
            //calculate new rotation axis, around the planet towards the target enemy
            Vector3 newAxis = Quaternion.LookRotation(currentTarget.transform.position - transform.position, transform.up).eulerAngles;
            transform.RotateAround(center, newAxis, angle);
        }
        else
        {
            transform.RotateAround(center, axis, angle);
        }
    }

    /**
     * Attack Action. During this state the bot attacks its
     * target enemy. The attack occurs every specified
     * interval of seconds.
     */
    void Attack()
    {
        //if target enemy is dead (or not exists), goto patrol state
        if (currentTarget == null)
        {
            state = BotState.patrol;
            return;
        }

        //check for interval timer
        if(Time.time > nextAttack)
        {
            nextAttack = Time.time + attackInterval;//calculate next attack time
            currentTarget.SendMessage("ApplyDamage", damage);//send attack event message (to apply damage to enemy)

            //draw laser beam (as line)
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 0.12f;
            lineRenderer.startColor = Color.cyan;
            lineRenderer.endColor = Color.blue;
            lineRenderer.SetPositions(new Vector3[] { transform.position, currentTarget.transform.position });
            StartCoroutine(RemoveLine(lineRenderer, 0.2f));//destroy line renderer component (after 200ms)

            //play laser fire sound
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }

    /**
     * Destroys the specified LineRenderer component
     * after the specified time.
     */
    IEnumerator RemoveLine(LineRenderer line, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(line);
    }

    //Destroys the current Spaceship GameObject
    void ClearObject()
    {
        Destroy(gameObject);
    }

    /**
     * Finds the closest (to the current GameObject) 
     * enemy Spaceship. Returns true if any enemy was
     * found or false otherwise.
     */
    bool FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);//get all enemy GameObject's

        Vector3 currentPosition = transform.position;//current bot's position
        float distance = Mathf.Infinity;//distance between current bot and closest enemy bot
        currentTarget = null;//current enemy bot
        foreach (GameObject enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - currentPosition;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                currentTarget = enemy;
                distance = curDistance;
            }
        }

        return currentTarget != null;//return if any enemy found
    }

    /**
     * Finds if the current bot is in range to attack the
     * target enemy bot (if it exists). Range is 5.0f units.
     */
    bool CheckInAttackRange()
    {
        if(currentTarget == null)
        {
            return false;
        }

        RaycastHit hitInfo;
        if(Physics.Linecast(transform.position, currentTarget.transform.position, out hitInfo))
        {
            GameObject theObject = hitInfo.collider.gameObject;
            
            if(theObject.CompareTag(enemyTag) && hitInfo.distance <= 5f)
            {
                return true;
            }
        }

        return false;
    }

    /**
     * Sets the target planet around which the current bot
     * rotates.
     */
    public void SetPlanet(GameObject p)
    {
        planet = p;
    }

    /**
     * Applies the specified damage to the current bot.
     */
    public void ApplyDamage(int dmg)
    {
        if (dmg > 0)
            health -= dmg;
        else
            health += dmg;
    }
}
