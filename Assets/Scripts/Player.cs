using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    [SerializeField] private float speed = 1f;
    private Vector2 bounds;
    private AudioSource audio;
    [SerializeField] private GameObject laser;
    void Start()
    {
        //Get x,y coords based on screen size
        bounds = Game.GetBounds();
        Debug.Log("Player loaded. \n" +
                "Screen boundaries: " + bounds);
        audio = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Shoot();
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * speed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * speed * Time.deltaTime;
    }

    void LateUpdate()
    {
        //Keep player on the screen
        Vector3 correctedPosition = transform.position;
        correctedPosition.x = Mathf.Clamp(correctedPosition.x, (bounds.x - 0.5f) * -1, (bounds.x - 0.5f));
        transform.position = correctedPosition;
    }

    private void Shoot()
    {
        GameObject laserTemp = Game.Instance.LaserFactory.GetLaser(laser, true);
        laserTemp.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        laserTemp.gameObject.SetActive(true);
        audio.Play();
    }

}
