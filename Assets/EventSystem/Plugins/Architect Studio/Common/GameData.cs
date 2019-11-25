using System;
using System.Collections.Generic;
using UnityEngine;

// Fake classes for serialization

public class Info
{
    public string name;
    public int rating;

    public int age;
    public DateTime dateOfSubmition;
    public string image;
    public string state;

    public Info(Info p_oldInfo, int p_newRating)
    {
        name = p_oldInfo.name;
        rating = p_newRating;
        age = p_oldInfo.age;
        dateOfSubmition = p_oldInfo.dateOfSubmition;
        image = p_oldInfo.image;
        state = p_oldInfo.state;
    }

    public Info()
    {
        name = string.Empty;
        rating = 0;
        age = 0;
        dateOfSubmition = DateTime.Now;
        image = string.Empty;
        state = string.Empty; 
    }

    public Info(string _name, int _rating, int _age, DateTime _date, string _image,  string _state)
    {
        name = _name;
        rating = _rating;
        age = _age;
        dateOfSubmition = _date;
        image = _image;
        state = _state;
    }
}

public class Exterior
{
    public int id = -1;

    public RoofMarker.MarkerTypes pickedMarkerType = RoofMarker.MarkerTypes.m8Foot;

    public string pickedRoof;

    public int pickedMaterial;
}

public class Interior
{
    public Dictionary<string, string> floorTiles;
    public List<InteriorItem> InteriorItems;
    public List<List<string>> walls;
    public List<string> exteriorCoverings;

    public Interior()
    {
        floorTiles = new Dictionary<string, string>();
        InteriorItems = new List<InteriorItem>();
        walls = new List<List<string>>();
        exteriorCoverings = new List<string>();
    }
}

public class Landscape
{
    public List<SimpleItem> landscapeItems;

    public Landscape()
    {
        landscapeItems = new List<SimpleItem>();
    }

}

public class SimpleItem
{
    public string name;
    public string position;
    public string rect;

    public SimpleItem()
    {
        name = string.Empty;
        position = Common.Vector3ToString(Vector3.zero);
        rect = Common.RectToString(new Rect());
    }

    public SimpleItem(string _name, string _position, string _rect)
    {
        name = _name;
        position = _position;
        rect = _rect;
    }
}

public class InteriorItem : SimpleItem
{
    public float angle;

    public InteriorItem()
    {
    }

    public InteriorItem(string _name, string _position, float _angle, string _rect)
    {
        name = _name;
        position = _position;
        angle = _angle;
        rect = _rect;
    }
}

public class GameData
{
    private bool showGallery = false;
    public bool ShowGallery
    {
        get
        {
            return showGallery;
        }
        set
        {
            showGallery = value;
        }
    }


    private bool loadingComplete = false;
    public bool LoadingComplete
    {
        get
        {
            return loadingComplete;
        }
        set
        {
            loadingComplete = value;
        }
    }

    private string keyCheckResult;
    public string KeyCheckResult
    {
        get
        {
            return keyCheckResult;
        }
        set 
        {
            keyCheckResult = value;
        }
    }

    public string info,
                  gameId,
                  exterior,
                  clients,
                  location,
                  securityKey,
                  interior,
                  landscape;//,total;

    public bool shared = false;


    Texture2D image = null;
    public Texture2D Image
    {
        get
        {
            return image;
        }
        set
        {
            image = value;
        }
    }


    public string SecurityKey
    {
        get
        {
            return securityKey;
        }
        private set { }
    }

    public int GameId
    {
        get
        {
            int result = -1;

            int.TryParse(gameId, out result);

            return result;
        }
        set 
        {
            gameId = value.ToString();
        }
    }
    public List<int> Clients
    {
        get
        {
            if (!string.IsNullOrEmpty(clients))
            {
                return JsonSerializer.DeserializeClients(clients);
            }
            else return new List<int>();
        }
        private set { }
    }

    public int Location
    {
        get
        {
            int result = -1;

            int.TryParse(location, out result);

            return result;
        }
        private set { }
    }

    private Exterior _oExterior;
    public Exterior oExterior
    {
        get
        {
            if (!string.IsNullOrEmpty(exterior) && _oExterior == null)
            {
                _oExterior = JsonSerializer.DeserializeExterior(exterior);
                return _oExterior;
            }
            else if (_oExterior != null) 
            {
                return _oExterior;
            }
            else 
                return null;
        }
    }

    private Interior _oInterior;
    public Interior oInterior
    {
        get
        {
            if (!string.IsNullOrEmpty(interior) && _oInterior == null)
            {
                _oInterior = JsonSerializer.DeserializeInterior(interior);
                return _oInterior;
            }
            else if (_oInterior != null)
            {
                return _oInterior;
            }
            else
                return null;
        }
    }

    private Landscape _oLandscape;
    public Landscape oLandscape
    {
        get
        {
            if (!string.IsNullOrEmpty(landscape) && _oLandscape == null)
            {
                _oLandscape = JsonSerializer.DeserializeLandscape(landscape);
                return _oLandscape;
            }
            else if (_oLandscape != null)
            {
                return _oLandscape;
            }
            else
                return null;
        }
    }

    private Info _oInfo;
    public Info oInfo
    {
        get
        {
            if (!string.IsNullOrEmpty(info) && _oInfo == null)
            {
                _oInfo = JsonSerializer.DeserializeUserInfo(info);
                _oInfo.rating += 1;
                return _oInfo;
            }
            else if (_oInfo != null)
            {
                return _oInfo;
            }
            else
                return null;
        }
    }

    public GameData()
    {
        GameId = -1;
    }
}


