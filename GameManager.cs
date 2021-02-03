using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject Menu_C, UI_C;
    public Text Stars_T, Energy_T, Missiles_T, StoreEnergy_T, StoreMissiles_T;
    public Text[] Scores_T;

    List<int> highScores;
    int saveID, totalStars, energy, missiles;

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
        ShowMenu(0);
        
        //StartGame(1, 1);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public GameObject[] Menus_C, Continues_B;
    public Button NewGame_B;
    public Text Resolution_T;

    const int MAIN_MENU = 0;
    const int CONTINUE_GAMES_MENU = 1;
    const int GAME_MENU = 2;
    const int SETTINGS_MENU = 3;
    const int STORE_MENU = 4;
    const int RECORDS_MENU = 5;

    int currentMenu, currentResolution;
    string[] resolutions = new string[] { "1920x1080", "1280x720", "720x480" };

    //Main Menu Function
    public void ShowMenu(int menuNumber)
    {
        switch (menuNumber)
        {
            case MAIN_MENU:
                GetSaveID();
                Menus_C[currentMenu].SetActive(false);
                Menus_C[MAIN_MENU].SetActive(true);
                NewGame_B.interactable = (saveID != -1);

                currentMenu = menuNumber;
                break;
            case CONTINUE_GAMES_MENU:
                Menus_C[currentMenu].SetActive(false);
                Menus_C[CONTINUE_GAMES_MENU].SetActive(true);
                List<int> existingContinues = FileManager.instance.GetExistingSaves();
                for(int i = 0; i < 4; i++)
                {
                    Continues_B[i].SetActive( existingContinues.Contains(i));
                }

                currentMenu = menuNumber;
                break;
            case GAME_MENU:
                Menus_C[currentMenu].SetActive(false);
                Menus_C[GAME_MENU].SetActive(true);
                currentMenu = menuNumber;

                Energy_T.text = energy.ToString();
                Missiles_T.text = missiles.ToString();
                
                break;
            case SETTINGS_MENU:
                Menus_C[currentMenu].SetActive(false);
                Menus_C[SETTINGS_MENU].SetActive(true);

                currentMenu = menuNumber;
                break;
            case STORE_MENU:
                Menus_C[currentMenu].SetActive(false);
                Menus_C[STORE_MENU].SetActive(true);
                currentMenu = menuNumber;

                StoreEnergy_T.text = energy.ToString();
                StoreMissiles_T.text = missiles.ToString();
                Stars_T.text = totalStars.ToString();
                break;
            case RECORDS_MENU:
                Menus_C[currentMenu].SetActive(false);
                Menus_C[RECORDS_MENU].SetActive(true);
                currentMenu = menuNumber;

                int index = 9;
                foreach(Text record in Scores_T)
                {
                    record.text = highScores[index].ToString();
                    index--;
                }
                break;
        }
        
    }

    public void LoadGame(int loadID)
    {
        saveID = loadID;
        highScores = FileManager.instance.LoadScores(saveID);
        (totalStars, energy, missiles) = FileManager.instance.LoadUser(saveID);
        ShowMenu(GAME_MENU);
    }

    public void CreateGame()
    {
        FileManager.instance.CreateSave(saveID);
        highScores = new List<int>();
        for(int i = 0; i<10; i++)
        {
            highScores.Add(0);
        }
        stars = 0;

        ShowMenu(GAME_MENU);
    }

    public void DeleteSave(int deleteID)
    {
        FileManager.instance.DeleteSave(deleteID);
        GetSaveID();
        ShowMenu(CONTINUE_GAMES_MENU);
    }

    public void GetSaveID()
    {
        saveID = FileManager.instance.GetNextSave();
    }

    public void ChangeResolution()
    {
        currentResolution = (currentResolution + 1) % resolutions.Length;
        Resolution_T.text = resolutions[currentResolution];
        switch (currentResolution)
        {
            case 0:
                Screen.SetResolution(1920, 1080, true);
                break;
            case 1:
                Screen.SetResolution(1280, 720, true);
                break;
            case 2:
                Screen.SetResolution(720, 480, true);
                break;
            default:
                break;
        }
        

    }

    public void BuyItem(int item)
    {
        switch (item)
        {
            case 0:
                if(totalStars >= 100)
                {
                    totalStars -= 100;
                    missiles++;
                }
                break;
            case 1:
                if (totalStars >= 500)
                {
                    totalStars -= 500;
                    energy++;
                }
                break;
        }
        ShowMenu(STORE_MENU);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //Main Game
    public GameObject Meteor_Prefab, Ship_Prefab, Pause_H, Hint_H;
    public Transform MainGame;

    public Image Life_I, Missiles_I, Energy_I;
    public Image[] ScoreNumbers_I, StarsNumbers_I, ShieldNumbers_I;

    int meteorCount, meteorMaxNumber, meteorTotalNumber, meteorsDestroyed, score, stars, missilesForShip, energyForShip;
    public bool isPaused = false;

    //// Functions:

    public void StartGame()
    {
        AudioManager.instance.PlayMusic(AudioID.GAME_MUSIC);
        Menu_C.SetActive(false);
        UI_C.SetActive(true);

        missilesForShip = missiles > 9 ? 9 : missiles;
        missiles -= missilesForShip;

        energyForShip = energy > 9 ? 9 : energy;
        energy -= energyForShip;

        Instantiate(Ship_Prefab, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 1f), MainGame).GetComponent<Ship>().StartShip(missilesForShip, energyForShip);

        meteorCount = 0;
        meteorMaxNumber =  5;

        stars = 0;
        score = 0;

        UpdateStars();
        UpdateScore();

        StartCoroutine(MeteorCreator());
        StartCoroutine(PointsCounter());

        Pause(false);
        StartCoroutine(ShowHint());
    }

    public void RemoveMeteorCount(int size)
    {
        meteorCount--;
        if (size == 0)
        {
            meteorsDestroyed++;
            score += 10;
        }else if (size == 1)
        {
            score += 25;
        }
        else
        {
            score += 50;
        }

        UpdateScore();

    }

    public void AddMeteorCount(int number)
    {
        meteorCount += number;
    }

    public void AddStars(int quantity)
    {
        stars += quantity;
        UpdateStars();
    }

    IEnumerator MeteorCreator()
    {
        while (true)
        {
            if (meteorCount < meteorMaxNumber)
            {
                GameObject newMeteor = Instantiate(Meteor_Prefab, MainGame);
                newMeteor.GetComponent<Meteor>().StartNewMeteor();

                meteorCount++;
            }

            yield return new WaitForSeconds(1f);
        }
        
    }

    IEnumerator PointsCounter()
    {
        while (!Ship.instance.dead)
        {
            score++;
            UpdateScore();

            meteorMaxNumber = (score / 500) + 5;

            yield return new WaitForSeconds(0.1f);
        }

    }

    public void UpdateScore()
    {
        ScoreNumbers_I[0].sprite = FileManager.instance.GetNumber((score%100000)/10000);
        ScoreNumbers_I[1].sprite = FileManager.instance.GetNumber((score%10000) / 1000);
        ScoreNumbers_I[2].sprite = FileManager.instance.GetNumber((score%1000) / 100);
        ScoreNumbers_I[3].sprite = FileManager.instance.GetNumber((score%100) / 10);
        ScoreNumbers_I[4].sprite = FileManager.instance.GetNumber(score%10);
    }

    public void UpdateStars()
    {
        //StarsNumbers_I[0].sprite = FileManager.instance.GetNumber((score % 10000) / 10000);
        StarsNumbers_I[0].sprite = FileManager.instance.GetNumber((stars % 1000) / 1000);
        StarsNumbers_I[1].sprite = FileManager.instance.GetNumber((stars % 1000) / 100);
        StarsNumbers_I[2].sprite = FileManager.instance.GetNumber((stars % 100) / 10);
        StarsNumbers_I[3].sprite = FileManager.instance.GetNumber(stars % 10);
    }

    public void UpdateLifes(int currentLifes)
    {
        Life_I.sprite = FileManager.instance.GetNumber(currentLifes);
    }

    public void UpdateMissiles(int currentMissiles)
    {
        Missiles_I.sprite = FileManager.instance.GetNumber(currentMissiles);
    }

    public void UpdateEnergy(int currentEnergy)
    {
        Energy_I.sprite = FileManager.instance.GetNumber(currentEnergy);
    }

    public void UpdateShield(int currentShield)
    {
        ShieldNumbers_I[0].sprite = FileManager.instance.GetNumber((currentShield % 100) / 10);
        ShieldNumbers_I[1].sprite = FileManager.instance.GetNumber(currentShield % 10);
    }

    public void Save()
    {
        FileManager.instance.SaveUser(saveID, totalStars, energy, missiles);
        FileManager.instance.SaveScores(saveID, highScores);
    }

    public void EndGame(int energy, int missiles)
    {
        StopAllCoroutines();

        this.energy += energy;
        this.missiles += missiles;
        
        for (int i = MainGame.childCount - 1; i >= 0; i--)
        {
            Destroy(MainGame.GetChild(i).gameObject);
        }
        Menu_C.SetActive(true);
        UI_C.SetActive(false);

        totalStars += stars;
        highScores.Add(score);
        highScores.Sort();
        highScores.RemoveAt(0);

        Save();

        AudioManager.instance.PlayMusic(AudioID.MENU_MUSIC);
        ShowMenu(GAME_MENU);
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
        Time.timeScale = isPaused ? 0f : 1f;
        Pause_H.SetActive(isPaused);
        Hint_H.SetActive(isPaused);
    }
    
    public IEnumerator ShowHint()
    {
        isPaused = true;
        Time.timeScale =  0f;
        Hint_H.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        Hint_H.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void ReturnToMenu()
    {
        StopAllCoroutines();

        this.energy += energyForShip;
        this.missiles += missilesForShip;

        for (int i = MainGame.childCount - 1; i >= 0; i--)
        {
            Destroy(MainGame.GetChild(i).gameObject);
        }
        Menu_C.SetActive(true);
        UI_C.SetActive(false);

        AudioManager.instance.PlayMusic(AudioID.MENU_MUSIC);
        ShowMenu(GAME_MENU);
    }

}
