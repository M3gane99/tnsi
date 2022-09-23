using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserFactory : MonoBehaviour
{
    private List<GameObject> lasers;

    public void Start()
    {
        lasers = new List<GameObject>();
    }

    public GameObject GetLaser(GameObject laserReference,bool forPlayer = false)
    {
        if (laserReference.GetComponent<Laser>() == null)
            Debug.LogError($"LaserFactory: The gameobject {laserReference} does not have a Laser component.");

        Sprite laserSprite = laserReference.GetComponent<SpriteRenderer>().sprite;
        Laser laser;
        for (int i=0; i < lasers.Count; i++)
        {
            if (lasers[i].gameObject.activeInHierarchy)
                continue;

            laser = lasers[i].GetComponent<Laser>();

            if (forPlayer && laser.GetIsPlayers)
            {
                lasers[i].transform.SetParent(GameObject.FindGameObjectWithTag("Lasers").transform);
                return lasers[i];
            }
                
            else if (!forPlayer && lasers[i].GetComponent<SpriteRenderer>().sprite == laserSprite)
            {
                lasers[i].transform.SetParent(GameObject.FindGameObjectWithTag("Lasers").transform);
                return lasers[i];
            }

        }
        GameObject tmp = Instantiate(laserReference);
        lasers.Add(tmp);
        return tmp;
        
    }

    public void RemoveLasers()
    {
        foreach (GameObject laser in lasers)
            Destroy(laser);
        lasers.Clear();
    }

}
