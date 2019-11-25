using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class SaveAS3DManager : AS3DNet
{
    #region Base save methods

    public void RegisterNewGame(Info p_userInfo)
    {
        if (LoadAS3DManager.oGameData == null)
            LoadAS3DManager.oGameData = new GameData();

        string data = JsonSerializer.SerializeInfo(p_userInfo);

        string key = Guid.NewGuid().ToString();
        key = key.Remove(key.IndexOf('-'));

        LoadAS3DManager.oGameData.securityKey = key;

        WWWForm form = new WWWForm();
        form.AddField("key", key);
        form.AddField("info", data);

        Debug.Log("Registering new game with user info = " + data + " and key = " + key);

        StartCoroutine(Post("RegisterNewGame", form, SetGameID));
    }

    public void SaveClients(int p_gameID, List<int> p_myClients)
    {
        string data = JsonSerializer.SerializeClients(p_myClients);

        Debug.Log("Saving clients. Data : " + data + " " + p_gameID);

        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);
        form.AddField("myClients", data);

        StartCoroutine(Post("SaveClients", form, Results));
    }

    public void SaveLocation(int p_gameID, int p_locationIndex)
    {
        Debug.Log("Saving location. Location index : " + p_locationIndex.ToString());

        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);
        form.AddField("locationIndex", p_locationIndex);

        StartCoroutine(Post("SaveLocation", form, Results));
    }

    public void SaveExterior(int p_gameID, BuildingDefinition p_pickedBuilding)
    {
        string data = JsonSerializer.SerializeExterior(p_pickedBuilding);

        Debug.Log("Saving exterior. Data : " + data);

        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);
        form.AddField("exterior", data);

        StartCoroutine(Post("SaveExterior", form, Results));
    }

    public void SaveInterior(int p_gameID, List<InteriorItemDefinition> p_interiorItems, Dictionary<string, string> p_floorTiles, List<List<WallSection>> p_walls)
    {
        string data = JsonSerializer.SerializeInterior(p_interiorItems, p_floorTiles, p_walls, CoveringsManager.Coverings);

        Debug.Log("Saving interior. Data : " + data);

        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);
        form.AddField("interior", data);

        StartCoroutine(Post("SaveInterior", form, Results));
    }

    public void SaveLandscape(int p_gameID, List<ItemDefinition> p_landscapeItems)
    {
        string data = JsonSerializer.SerializeLandscape(p_landscapeItems);

        Debug.Log("Saving landscape. Data : " + data);

        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);
        form.AddField("landscape", data);

        StartCoroutine(Post("SaveLandscape", form, Results));
    }

    public void SaveUserInfo(int p_gameID, Info p_userInfo)
    {
        string data = JsonSerializer.SerializeInfo(p_userInfo);

        Debug.Log("Saving user info. Data : " + data);

        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);
        form.AddField("info", data);

        StartCoroutine(Post("SaveUserInfo", form, Results));
    }

    public void SaveScreenShot(int p_gameID, byte[] p_bytes)
    {
        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);

        string img = Convert.ToBase64String(p_bytes, 0, p_bytes.Length);

        form.AddField("picture", img);

        // Leave this for Python version:
        //form.AddBinaryData("picture", p_bytes, "image/png");

        StartCoroutine(Post("SaveScreenShot", form, Results));
    }

    public void Share(int p_gameID)
    {
        Debug.Log("Sharing is true now");

        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);

        StartCoroutine(Post("Share", form, Results));
    }

    internal void RemoveAllFromGallery()
    {
        StartCoroutine(Post("RemoveAllFromGallery", null, Results));
    }

    internal void Prune(int p_gameID)
    {
        WWWForm form = new WWWForm();
        form.AddField("gameId", p_gameID);

        Debug.Log("Game with id = " + p_gameID + " is removed from Gallery");

        StartCoroutine(Post("Prune", form, Results));
    }

    #endregion

    public enum SaveWindowState
    {
        hide,
        show,
        processing,
        error,
        success
    }

    private SaveWindowState saveWindowState = SaveWindowState.hide;
    public SaveWindowState CurrentSaveState
    {
        get
        {
            return saveWindowState;
        }
        set
        {
            saveWindowState = value;
        }
    }


    private IEnumerator WaitForRegisterAndSave()
    {
        while (LoadAS3DManager.oGameData == null
            || (LoadAS3DManager.oGameData.GameId < 0 && LoadAS3DManager.oGameData.GameId != -500))
        {
            yield return 0;
        }

        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        tex = Common.ResizeTexture(tex, 240, 150, 1);

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        Destroy(tex);

        if (LoadAS3DManager.oGameData.GameId == -500)
        {
            // Change the error message here
            saveWindowState = SaveAS3DManager.SaveWindowState.error;
        }
        else
        {
            SaveScreenShot(LoadAS3DManager.oGameData.GameId, bytes);

            if (ClientManager.MyClients != null)
                SaveClients(LoadAS3DManager.oGameData.GameId, ClientManager.MyClients);

            if (LocationManager.PickedLocation != null)
                SaveLocation(LoadAS3DManager.oGameData.GameId, LocationManager.PickedLocation.id);

            if (BuildingManager.Picked != null)
                SaveExterior(LoadAS3DManager.oGameData.GameId, BuildingManager.Picked);

            if (WallManager.Walls != null && BuildingManager.Picked != null && InteriorDesignGUI.GridItems != null)
                SaveInterior(LoadAS3DManager.oGameData.GameId, InteriorDesignGUI.GridItems, BuildingManager.Picked.FloorTiles, WallManager.Walls);

            if (LandscapingGUI.GridItems != null)
                SaveLandscape(LoadAS3DManager.oGameData.GameId, LandscapingGUI.GridItems);

            saveWindowState = SaveAS3DManager.SaveWindowState.processing;
        }
    }

    public void Save()
    {
        StartCoroutine(WaitForRegisterAndSave());
    }
}

