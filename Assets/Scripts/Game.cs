using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    //GameObjects needed to turn UI on/off and to change things in them (e.g text)
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject game;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private Text gameOverTitleText;

    [SerializeField] private List<Text> levelRecords; //Index number + 1 is the number of level

    private Text gamePoints;

    private int HowManyMotherships = 0;
    private List<GameObject> enemies;
    private LaserFactory laserFactory;
    public LaserFactory LaserFactory => laserFactory;

    private int currentLevel = 0;
    public int GetCurrentLevel => currentLevel;

    private int points = 0;
    private static Game instance;

    public static Game Instance => instance;

    public enum State
    {
        Menu,
        Game,
        GameOver,
        GameWin,
    }

    private State state;

    public void SetPoints(int newValue)
    {
        points = newValue;
        gamePoints.text ="Points: " + points.ToString();
    }
    public int GetPoints()
    {
        return points;
    }

    private void CheckState()
    {
        switch (state)
        {
            case State.Menu:
                mainMenu.SetActive(true);
                game.SetActive(false);
                gameUI.SetActive(false);
                gameOver.SetActive(false);
                points = 0;
                currentLevel = 0;
                break;
            case State.Game:
                game.SetActive(true);
                mainMenu.SetActive(false);
                gameUI.SetActive(true);
                gameOver.SetActive(false);
                break;
            case State.GameOver:
                RemoveEnemies();
                laserFactory.RemoveLasers();

                mainMenu.SetActive(false);
                game.SetActive(false);
                gameUI.SetActive(false);
                gameOver.SetActive(true);
                GameObject.FindGameObjectWithTag("Points").GetComponent<Text>().text = $"Points: {points}";
                GameObject.FindGameObjectWithTag("Record").GetComponent<Text>().text = $"Record: {LoadRecords(currentLevel)}";
                gameOverTitleText.text = "Game Over";
                break;
            case State.GameWin:
                RemoveEnemies();
                laserFactory.RemoveLasers();

                mainMenu.SetActive(false);
                game.SetActive(false);
                gameUI.SetActive(false);
                gameOver.SetActive(true);
                GameObject.FindGameObjectWithTag("Points").GetComponent<Text>().text = $"Points: {points}";
                SaveRecord();
                GameObject.FindGameObjectWithTag("Record").GetComponent<Text>().text = $"Record: {LoadRecords(currentLevel)}";
                LoadRecordsToMainMenu();
                gameOverTitleText.text = "You Win!";
                break;
        }
    }

    public void ChangeState(int newState)
    {
        state = (State) newState;
        CheckState();
        
    }
    public void ChangeState(State newState)
    {
        state = newState;
        CheckState();
    }

    private void LoadRecordsToMainMenu()
    {
        int index = 1;
        foreach (Text text in levelRecords)
        {
            text.text = LoadRecords(index);
            index++;
        }
    }

    void Start()
    {
        LoadRecordsToMainMenu();
        if (instance == null)
            instance = this;

        gamePoints = gameUI.transform.GetChild(0).GetComponent<Text>();
        state = State.Menu;
        laserFactory = new LaserFactory();
        enemies = new List<GameObject>();
        laserFactory.Start();
    }

    public int Motherships => HowManyMotherships;
    public void SetMotherShipCount(int value)
    {
        HowManyMotherships = value;
    }
    public static Vector2 GetBounds()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    public void RemoveEnemies()
    {
        foreach (GameObject enemy in enemies)
            Destroy(enemy);
        enemies.Clear();
    }

    public static void LoadLevel(LevelSO level)
    {
        Game.Instance.ChangeState(State.Game);
        Game.Instance.currentLevel = level.levelIndex;
        Game.Instance.SetPoints(0);

        int offsetX = 1;
        int offsetY = 2;
        Vector2 bounds = GetBounds();
        foreach (EnemyRow enemyRow in level.enemyRows)
        {
            offsetX = 2;
            enemyRow.enemy.Initialize();
            for(int i=0; i < enemyRow.howMany; i++)
            {
                GameObject temp = Instantiate(enemyRow.enemy.GetEnemy(), new Vector3(bounds.x * -1 + offsetX, bounds.y-offsetY), Quaternion.identity);
                temp.transform.SetParent(GameObject.FindGameObjectWithTag("Enemies").transform);
                Game.Instance.enemies.Add(temp);
                offsetX += 2;
            }
            offsetY += 2;
        }

        Game.Instance.HowManyMotherships = 0;
        foreach (GameObject enemy in Game.Instance.enemies)
            if (enemy.GetComponent<Enemy>().isMotherShip) Game.Instance.HowManyMotherships++;
    }

    private void SaveRecord()
    {
        List<string> records = LoadRecords();
        if (records.Count < currentLevel)
        {
            for (int i = 1; i <= currentLevel; i++)
            {
                if(records.Count < i)
                {
                    if (i == currentLevel)
                        records.Add(points.ToString());
                    else
                        records.Add("0");
                }
                else
                {
                    if (i == currentLevel)
                        records.Add(points.ToString());
                    else if (records[i - 1] != "")
                        continue;
                    else
                        records.Add("0");
                }

            }
        }
        else
        {
            int.TryParse(records[currentLevel - 1], out int record);
            if (record < points)
                records[currentLevel - 1] = points.ToString();
        }
        StreamWriter writer = new StreamWriter("./records.save");
        foreach (string line in records)
            writer.WriteLine(line);
        writer.Close();
        
    }
    private List<string> LoadRecords()
    {
        if (!File.Exists("./records.save"))
            File.WriteAllText("./records.save", "");

        List<string> lines = new List<string>();
        StreamReader reader = new StreamReader("./records.save");
        string line = "";
        while ((line = reader.ReadLine()) != null)
            lines.Add(line);
        reader.Close();
        if (lines.Count == 0)
            return new List<string>();
        return lines;
    }
    private string LoadRecords(int level)
    {
        if (!File.Exists("./records.save"))
            File.WriteAllText("./records.save", "");

        StreamReader reader = new StreamReader("./records.save");
        string line;
        int index = 1;
        while ((line = reader.ReadLine()) != null)
        {
            if (index == level) break;
            index++;
        }
        if (index != level) return "0"; //If there is not enough levels in file then return 0 (no record)
        reader.Close();
        return line == null ? "0" : line; //It returns 0 if there are no records in file
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
