using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class FileManager : MonoBehaviour
{
    public static FileManager instance;


    //File Management

    public int CreateSave(int saveID)
    {
        string folderPath = Application.persistentDataPath + "/saves/" + saveID.ToString();
        Directory.CreateDirectory(folderPath);

        //string userString = name + "\n";
        //File.WriteAllText(folderPath + "/save.txt", userString);

        List<int> scores = new List<int>(10);
        for (int i = 0; i < 10; i++)
        {
            scores.Add(0);
        }

        SaveScores(saveID, scores);
        SaveUser(saveID, 0, 0, 0);

        return saveID;
    }

    public void SaveScores(int saveID, List<int> scores)
    {
        string folderPath = Application.persistentDataPath + "/saves/" + saveID.ToString();

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string path = folderPath + "/top_scores.txt";

        string stringToWrite = "";
        foreach (int score in scores)
        {
            stringToWrite += score.ToString() + "\n";
        }


        File.WriteAllText(path, stringToWrite);

        return;
    }

    public void SaveUser(int saveID, int starsAvailable, int energy, int missiles)
    {
        string folderPath = Application.persistentDataPath + "/saves/" + saveID.ToString();

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string path = folderPath + "/user.txt";

        string stringToWrite = starsAvailable.ToString() + "\n";
        stringToWrite += energy.ToString() + "\n";
        stringToWrite += missiles.ToString() + "\n";

        File.WriteAllText(path, stringToWrite);

        return;
    }

    public List<int> LoadScores(int saveID)
    {
        List<int> scores = new List<int>();
        string path = Application.persistentDataPath + "/saves/" + saveID.ToString() + "/top_scores.txt";

        if (!File.Exists(path))
        {
            return scores;
        }

        string[] mapFileString = new string[2];
        mapFileString[1] = File.ReadAllText(path);

        for (int i = 0; i < 10; i++)
        {
            mapFileString = mapFileString[1].Split(new char[] { '\n' }, 2);
            int score = int.Parse(mapFileString[0]);

            scores.Add(score);
        }

        return scores;
    }

    public (int, int, int) LoadUser(int saveID)
    {
        int stars = 0;
        int energy = 0;
        int missiles = 0;
        string path = Application.persistentDataPath + "/saves/" + saveID.ToString() + "/user.txt";

        if (!File.Exists(path))
        {
            return (stars, energy, missiles);
        }

        string[] mapFileString = new string[2];
        mapFileString[1] = File.ReadAllText(path);

        mapFileString = mapFileString[1].Split(new char[] { '\n' }, 2);
        stars = int.Parse(mapFileString[0]);

        mapFileString = mapFileString[1].Split(new char[] { '\n' }, 2);
        energy = int.Parse(mapFileString[0]);

        mapFileString = mapFileString[1].Split(new char[] { '\n' }, 2);
        missiles = int.Parse(mapFileString[0]);


        return (stars, energy, missiles);
    }

    public int GetNextSave()
    {
        string folderPath = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string[] saveFolders = Directory.GetDirectories(folderPath);
        List<int> saveNumbers = new List<int>();
        foreach (string saveFolder in saveFolders)
        {
            saveNumbers.Add((int)char.GetNumericValue(saveFolder[saveFolder.Length - 1]));
        }

        for(int i = 0; i < 4; i++)
        {
            if (!saveNumbers.Contains(i))
            {
                return i;
            }
                
        }

        return -1;
    }
    
    public List<int> GetExistingSaves()
    {
        string folderPath = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string[] saveFolders = Directory.GetDirectories(folderPath);
        List<int> saveNumbers = new List<int>();
        foreach (string saveFolder in saveFolders)
        {
            saveNumbers.Add((int)char.GetNumericValue(saveFolder[saveFolder.Length - 1]));
        }
        return saveNumbers;
    }

    public void DeleteSave(int deleteID)
    {
        string folderPath = Application.persistentDataPath + "/saves/" + deleteID.ToString();

        Directory.Delete(folderPath, true);
    }

    //Sprite Management
    public Sprite[] Big_Meteors;
    public Sprite[] Medium_Meteors;
    public Sprite[] Small_Meteors;

    public Sprite[] Stars;

    public Sprite[] Numbers;

    public Sprite GetMeteorSprite(int size)
    {
        switch (size)
        {
            case 0:
                return Small_Meteors[UnityEngine.Random.Range(0, Small_Meteors.Length)];
            case 1:
                return Medium_Meteors[UnityEngine.Random.Range(0, Medium_Meteors.Length)];
            case 2:
                return Big_Meteors[UnityEngine.Random.Range(0, Big_Meteors.Length)];
            default:
                return Small_Meteors[0];
        }
    }

    public Sprite GetNumber(int number)
    {
            return Numbers[number];
    }

    public Sprite GetStarSprite(int starType)
    {
        return Stars[starType];
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
        // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
