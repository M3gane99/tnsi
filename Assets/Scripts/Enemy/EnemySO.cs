using UnityEngine;

[CreateAssetMenu(fileName ="Enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private int points = 10;
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private float speed = 4f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyLaser;

    //Motherships are requirements to complete levels
    [SerializeField] private bool isMotherShip = false;

    public GameObject GetEnemy()
    {
        return enemyPrefab;
    }

    //Load ScriptableObject's values into prefab
    public void Initialize()
    {
        enemyPrefab.GetComponent<SpriteRenderer>().sprite = sprite;
        enemyPrefab.GetComponent<Enemy>().points = points;
        enemyPrefab.GetComponent<Enemy>().hitPoints = hitPoints;
        enemyPrefab.GetComponent<Enemy>().speed = speed;
        enemyPrefab.GetComponent<Enemy>().laser = enemyLaser;
        enemyPrefab.GetComponent<Enemy>().isMotherShip = isMotherShip;
    }
}
