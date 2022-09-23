using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelSO : ScriptableObject
{
    public int levelIndex = 0;
    public EnemyRow[] enemyRows;
}

[System.Serializable]
public class EnemyRow
{
    public EnemySO enemy;
    public int howMany = 10;
}