using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BotLogic : MonoBehaviour
{
    public enum BotType { GreenSpaceship, RedSpaceship };
    public enum BotState { patrol, chase, attack };

    public BotType type;
    public BotType enemyType;

    public float rotateMinSpeed = 10f;
    public float rotateMaxSpeed = 20f;
    public float radius = 20f;

    public int maxHealth = 3;
    public int damage = 1;
    public float attackInterval = 1.0f;

    private GameObject planet;
    private Vector3 center;
    private float angle;
    private float rotateSpeed;
    private int health;
    private Vector3 axis;

    public BotState state = BotState.patrol;
    private GameObject currentTarget = null;

    private float nextAttack = 0f;

    // Start is called before the first frame update
    void Start()
    {
        center = planet.transform.position;
        rotateSpeed = UnityEngine.Random.Range(rotateMinSpeed, rotateMaxSpeed);

        health = maxHealth;
        angle += 10f * Time.deltaTime;

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

    // Update is called once per frame
    void Update()
    {
        if(health == 0)
        {
            Destroy(this.gameObject);
            return;
        }

        //check if current bot is dead
        if(currentTarget != null)
        {
            BotLogic bt = currentTarget.GetComponent(typeof(BotLogic)) as BotLogic;
            if(bt == null)
            {
                currentTarget = null;
            }
        }

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

    void Patrol()
    {
        if(FindClosestEnemy())
        {
            state = BotState.chase;
            return;
        }

        angle += rotateSpeed * Time.deltaTime;
        angle = Mathf.Clamp(angle, 0f, 1f);

        transform.RotateAround(center, axis, angle);
    }

    void Chase()
    {
        if(currentTarget == null)
        {
            state = BotState.patrol;
            return;
        }

        if (CheckInAttackRange())
        {
            state = BotState.attack;
        }
        else
        {
            state = BotState.chase;
            FindClosestEnemy();
        }

        angle += rotateSpeed * Time.deltaTime;
        angle = Mathf.Clamp(angle, 0f, 1f);
        Vector3 newAngle = Quaternion.LookRotation(axis, currentTarget.transform.position).eulerAngles;

        transform.RotateAround(center, newAngle, angle);
    }

    void Attack()
    {
        if (currentTarget == null)
        {
            state = BotState.patrol;
            return;
        }

        if(Time.time > nextAttack)
        {
            nextAttack = Time.time + attackInterval;
            currentTarget.SendMessage("ApplyDamage", damage);
            Debug.Log(this.gameObject.name + " attacked");
        }
    }

    bool FindClosestEnemy()
    {
        String enemyTag = System.Enum.GetName(typeof(BotType), enemyType);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        Vector3 currentPosition = transform.position;
        float distance = Mathf.Infinity;
        currentTarget = null;
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

        return currentTarget != null;
    }

    bool CheckInAttackRange()
    {
        RaycastHit hitInfo;
        if(Physics.Linecast(transform.position, currentTarget.transform.position, out hitInfo))
        {
            String enemyTag = System.Enum.GetName(typeof(BotType), enemyType);
            GameObject theObject = hitInfo.collider.gameObject;
            
            if(theObject.CompareTag(enemyTag) && hitInfo.distance <= 5f)
            {
                return true;
            }
        }

        return false;
    }

    public void setPlanet(GameObject p)
    {
        planet = p;
    }

    public void ApplyDamage(int dmg)
    {
        if (dmg > 0)
            health -= dmg;
        else
            health += dmg;
    }
}
