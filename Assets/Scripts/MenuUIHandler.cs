using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{
    public static MenuUIHandler Instance;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    public string playerName;
    public string bestPlayerNameAndScore;

    private GameObject gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadNameValueChange();
        LoadBestScore();
    }

    private void Start()
    {
        if (bestPlayerNameAndScore != null)
        {
            Debug.Log("Load name");
            LoadName();
            if (!string.IsNullOrEmpty(bestPlayerNameAndScore))
                bestScoreText.text = bestPlayerNameAndScore;
        }
    }

    public void FindGameManager()
    {
        gameManager = GameObject.Find("MainManager");
        if (gameManager != null)
        {
            MainManager gameManagerScript = gameManager.GetComponent<MainManager>();
            bestPlayerNameAndScore = gameManagerScript.BestScoreText();
            SaveName();
        }
    }

    public void StartNew()
    {
        playerName = nameField.text;
        SaveName();
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void LoadNameValueChange()
    {
        LoadName();
        nameField.text = playerName;
    }

    private void LoadBestScore()
    {
        if (!string.IsNullOrEmpty(playerName))
            bestScoreText.text = "Best Score : " + playerName;
    }

    [System.Serializable]
    class SaveData
    {
        public string playerName;
        public string bestPlayerNameAndScore;
    }
    public void SaveName()
    {
        SaveData data = new SaveData();
        data.playerName = playerName;
        data.bestPlayerNameAndScore = bestPlayerNameAndScore;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadName()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            playerName = data.playerName;
            bestPlayerNameAndScore = data.bestPlayerNameAndScore;
        }
    }

    public void ClearRecord()
    {
        File.Delete(Application.persistentDataPath + "/savefile.json");
        playerName = "";
        bestPlayerNameAndScore = "";
        PlayerPrefs.DeleteAll();
        //bestScoreText.text = "Record Deleted";
    }
}
