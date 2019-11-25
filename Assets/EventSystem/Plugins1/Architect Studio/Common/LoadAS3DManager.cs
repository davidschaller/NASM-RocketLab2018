using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using JsonFx.Json;

public class LoadAS3DManager : AS3DNet
{
    private void Awake()
    {
        gameList = new List<GameData>();
    }

    private static GameData gameData;
    public static GameData oGameData
    {
        get
        {
            return gameData;
        }
        set
        {
            gameData = value;
        }
    }

    private static List<GameData> gameList;
    public static List<GameData> GameList
    {
        get
        {
            return gameList;
        }
        set
        {
            gameList = value;
        }
    }

    public void JoinGame(string p_key)
    {
        WWWForm form = new WWWForm();
        form.AddField("key", p_key);

        StartCoroutine(Post("JoinGame", form, LoadGameData));
    }

    public void LoadGame(string p_gameId)
    {
        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameId);

        StartCoroutine(Post("LoadGame", form, LoadGameData));
    }

    protected void LoadGameData(string p_input)
    {
        if (!string.IsNullOrEmpty(p_input))
        {
            Debug.Log("Trying to join the game. Server returned: " + p_input);

            if (!p_input.Contains("cannot find game"))
            {
                GameData result = JsonReader.Deserialize<GameData>(p_input);

                if (result != null)
                {
                    LoadAS3DManager.oGameData = result;
                }
                else
                {
                    Debug.Log("Coudn't deserialize game data");
                }
            }
            else
                Debug.Log("The game was not found");
        }
        else
            Debug.Log("Couldn't load the game");
    }

    public IEnumerator LoadGames()
    {
        Debug.Log("Loading game Items...");

        WWW w = new WWW(serverUrl + "LoadGames");

        while (!w.isDone)
            yield return w;

        if (w.error != null)
            Debug.LogError("WWW ERROR: " + w.error);
        else
        {
            string result = w.text.Trim();
            Debug.Log(result);

            if (!string.IsNullOrEmpty(result))
            {
                GameData[] gameDataArray = JsonReader.Deserialize<GameData[]>(result);
                if (gameDataArray != null)
                {
                    List<GameData> allDataList = new List<GameData>();
                    allDataList.AddRange(gameDataArray);
                    GameList = allDataList;

                    GameList.Sort(delegate(GameData p1, GameData p2) { return p2.oInfo.rating.CompareTo(p1.oInfo.rating); });
                }


                for (int i = 0; i < GameList.Count; i++)
                {
                    StartCoroutine(LoadScreenShot(i));
                }
            }
        }
    }

    private string imagesUrl = "http://architectstudio3d.org/DB2/ScreenShots/";

    private IEnumerator LoadScreenShot(int p_index)
    {
        int gameId = GameList[p_index].GameId;
        string url = imagesUrl + gameId + ".png";

        WWW www = new WWW(url);

        while (!www.isDone)
            yield return 0;

        if (www.texture != null)
        {
            GameList[p_index].Image = www.texture;
        }
    }

    public IEnumerator CheckKey(int p_gameID, string p_key)
    {
        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);
        form.AddField("key", p_key);

        WWW w = new WWW(serverUrl + "CheckKey", form);

        while (!w.isDone)
            yield return w;

        if (w.error != null)
            Debug.LogError("WWW ERROR: " + w.error);
        else
        {
            string result = w.text.Trim();
            Debug.Log(result);

            if (!string.IsNullOrEmpty(result))
            {
                //gameData.KeyCheckResult = result;
            }
        }        
    }
}

