using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int hitPoints;
    public int points;
    public float speed;
    public bool isMotherShip;
    public GameObject laser;

    protected float timeLeft; //Time left to check for laser shot
    protected float originalTimeLeft;
    protected int direction = 1;
    protected Vector2 bounds;
    protected AudioSource audio;
    
    protected virtual void Initialize()
    {
        audio = GetComponent<AudioSource>();
        bounds = Game.GetBounds();
        originalTimeLeft = Random.Range(0.2f, isMotherShip ? 0.5f : 2f);
        timeLeft = 0f; // 0 = Shoot at level start
    }
    
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (hitPoints <= 0) //Enemy is dead
        {
            Game.Instance.SetPoints(Game.Instance.GetPoints() + points);
            gameObject.SetActive(false);

            if (isMotherShip) {
                Game.Instance.SetMotherShipCount(Game.Instance.Motherships - 1);
                int motherships = Game.Instance.Motherships;
                if (motherships <= 0) Game.Instance.ChangeState(Game.State.GameWin);
            }
            
        }
            
        Move();
    }

    protected virtual void Move()
    {
        timeLeft -= Time.deltaTime;

        if (transform.position.x >= (bounds.x - 0.5f))
            direction = -1;
        else if (transform.position.x <= (bounds.x - 0.5f) * -1)
            direction = 1;

        Vector3 enemyPosition = transform.position;
        enemyPosition.x += speed * direction * Time.deltaTime;
        transform.position = enemyPosition;

        if(timeLeft <= 0) //Roll dice for laser shot
        {
            timeLeft = originalTimeLeft;
            int randomNumber = Random.Range(0, 2);
            if (randomNumber == 1)
                Shoot();
        }

        
    }
    protected virtual void Shoot()
    {
        GameObject laserTemp = Game.Instance.LaserFactory.GetLaser(laser);
        laserTemp.transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
        laserTemp.gameObject.SetActive(true);
        audio.Play();
    }

}
