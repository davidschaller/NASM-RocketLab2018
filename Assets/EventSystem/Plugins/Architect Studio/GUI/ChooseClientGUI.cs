using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChooseClientGUI : MonoBehaviour
{
    public GUISkin selectionSkin,
                   buttonSkin,
                   hintSkin;

    List<int> potentialClients; // preview clients

    private GUIStyle whiteButtonStyle,
                     buttonStyle,
                     selectionButtonStyle;
    void Awake() 
    {
        ArchitectStudioGUI.RegisterStateScript(ArchitectStudioGUI.States.ChooseClient, GetComponent<ChooseClientGUI>());

        potentialClients = new List<int>();

        ClientManager.Init();

        for (int i = 0; i < ClientManager.maxNumOfClients; i++)
        {
            potentialClients.Add(0);
        }

        if (LoadAS3DManager.oGameData != null)
            ClientManager.MyClients = LoadAS3DManager.oGameData.Clients;

        whiteButtonStyle = hintSkin.GetStyle("button");
        buttonStyle = buttonSkin.GetStyle("button");
        selectionButtonStyle = selectionSkin.GetStyle("button");
    }

    void OnEnable()
    {
        EnableClient(newClientIndex);
    }

    void OnDisable()
    {
        //ScreenShotMaker.CreateClientsPicture(ClientManager.MyClients, ClientManager.Clients);
               
        ClientManager.DeactivateAllClients();
    }

    ClientDefinition currentClient;
    void EnableClient(int clientIndex)
    {
        if (currentClient != null)
            currentClient.Deactivate();

        currentClient = ClientManager.Clients[clientIndex];
        this.newClientIndex = clientIndex;
        currentClient.Activate();
    }

    private float newClientPanelWidth = 150f,
                 newClientPanelHeight = 390f,
                 myClientPanelWidth = 150f,
                 myClientPanelHeight = 300f,
                 clientsNeedsWidth = 207f,
                 clientsNeedsHeight = 355f,
                 spaceBetweenMyClients = 50f;

    private int ClientPanel(Vector2 loc, float panelWidth, float panelHeight, int p_panelIndex, int p_inputClientIndex)
    {
        int r_outputClientIndex = 0;

        if (p_panelIndex >= 0)
        {
            r_outputClientIndex = potentialClients[p_panelIndex];
        }
        else
        {
            r_outputClientIndex = p_inputClientIndex;
        }

        Rect imageBox = new Rect(loc.x + panelWidth / 2 - ClientManager.Clients[r_outputClientIndex].image.width / 2, 
            loc.y, panelWidth, ClientManager.Clients[r_outputClientIndex].image.height);

        GUIStyle imageStyle = new GUIStyle();
        imageStyle.alignment = TextAnchor.UpperCenter;

        float imageHeight = ClientManager.Clients[r_outputClientIndex].image.height;

        if (p_panelIndex != -1)           // this is only for "CHANGE CLIENT" case; -1 is Main client's panel index
        {
            imageStyle.padding.left = 10; // need to move image to the right
            imageHeight *= 0.75f;         // ans scale image a little bit
        }
        GUILayout.BeginArea(imageBox, GUIContent.none, imageStyle);
        GUILayout.BeginVertical();

        GUILayout.Label(new GUIContent(ClientManager.Clients[r_outputClientIndex].image),
            GUILayout.Height(imageHeight));
        

        GUILayout.EndVertical();
        GUILayout.EndArea();

        float buttonWidth = selectionButtonStyle.normal.background.width;
        float buttonHeight = selectionButtonStyle.normal.background.height;


        Vector2 leftArrowCenter = new Vector2(loc.x - buttonWidth / 2, loc.y + buttonHeight / 2);
        GUIUtility.RotateAroundPivot(180, leftArrowCenter);
        if (GUI.Button(new Rect(loc.x - buttonWidth - buttonWidth / 2,
                                 loc.y - panelHeight + panelHeight / 3 + buttonHeight / 2,
                                 buttonWidth, buttonHeight), string.Empty, selectionButtonStyle))
        {
            clientNeedsIndex = 0;
            r_outputClientIndex--;
            if (r_outputClientIndex <= 0)
            {
                r_outputClientIndex = ClientManager.Clients.Count - 1;
            }
        }

        GUI.matrix = Matrix4x4.identity;

        if (GUI.Button(new Rect(loc.x + panelWidth - buttonWidth / 2,
                                loc.y + panelHeight - panelHeight / 3 - buttonHeight / 2,
                                buttonWidth, buttonHeight), string.Empty, selectionButtonStyle))
        {
            clientNeedsIndex = 0;
            r_outputClientIndex++;
            if (r_outputClientIndex > ClientManager.Clients.Count - 1)
            {
                r_outputClientIndex = 1;
            }
        }

        

        Rect buttonsBox = new Rect(loc.x, loc.y + panelHeight - panelHeight / 3 + buttonHeight / 2, panelWidth, panelHeight / 3 - buttonHeight / 2);
        GUILayout.BeginArea(buttonsBox);
        GUILayout.BeginVertical();
        if (GUILayout.Button("VIEW CLIENT'S NEEDS", whiteButtonStyle, GUILayout.Height(25)))
        {
            if (r_outputClientIndex == clientNeedsIndex)
            {
                showClientsNeeds = !showClientsNeeds;
            }
            else
            {
                clientNeedsIndex = r_outputClientIndex;
                showClientsNeeds = true;
            }
        }
        
        if (GUILayout.Button("CHOOSE THIS CLIENT", whiteButtonStyle, GUILayout.Height(25)) && newClientIndex > 0)
        {
            showClientsNeeds = false;

            if (p_panelIndex == -1)
            {
                if (ClientManager.MyClients.Count < ClientManager.maxNumOfClients)
                {
                    ClientManager.MyClients.Add(r_outputClientIndex);
                    showNewClientPanel = false;
                }
            }
            else
            {
                ClientManager.MyClients[p_panelIndex] = r_outputClientIndex;
                r_outputClientIndex = 0;
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();

        return r_outputClientIndex;
    }

    bool showClientsNeeds = false,
         showNewClientPanel = false;

    int newClientIndex = 1,
        clientNeedsIndex = 0;

    void OnGUI()
    {
        GUI.depth = 1;

        // show this only for the first time or if new client was just added
        if (ClientManager.MyClients.Count == 0 || showNewClientPanel)
        {
            newClientIndex = ClientPanel(new Vector2(170, 100), newClientPanelWidth, newClientPanelHeight, -1, newClientIndex);
        }
        // if client's list is not full and new client panel was canceled, show the question
        else if (ClientManager.MyClients.Count > 0 && ClientManager.MyClients.Count < ClientManager.maxNumOfClients && !showNewClientPanel)
        {
            QuestionPanel(new Vector2(200, 50), newClientPanelWidth, newClientPanelHeight);
        }

        if (showClientsNeeds && clientNeedsIndex > 0)
        {
            ClientsNeeds(new Vector2(10, ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Gallery ? 130 : 80), clientsNeedsWidth, clientsNeedsHeight, clientNeedsIndex);
        }

        GUILayout.BeginHorizontal();

        for (int i = 0; i < ClientManager.MyClients.Count; i++)
        {
            if (ClientManager.MyClients[i] != 0)
            {
                MyClientPanel(new Vector2(Screen.width - (myClientPanelWidth + spaceBetweenMyClients) * (i + 1), 100), myClientPanelWidth, myClientPanelHeight, ClientManager.MyClients[i], i);
            }
            else if (ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Design)
            {
                potentialClients[i] = ClientPanel(new Vector2(Screen.width - (myClientPanelWidth + spaceBetweenMyClients) * (i + 1), 100), myClientPanelWidth, myClientPanelHeight, i, 0);
            }
        }
        GUILayout.EndHorizontal();

    }

    private void MyClientPanel(Vector2 loc, float panelWidth, float panelHeight, int clientIndex, int panelIndex)
    {
        float buttonHeight = buttonStyle.normal.background.height;

        Rect imageBox = new Rect(loc.x + panelWidth / 2 - ClientManager.Clients[clientIndex].image.width / 2,
            loc.y, panelWidth, ClientManager.Clients[clientIndex].image.height);

        GUIStyle imageStyle = new GUIStyle();
        imageStyle.alignment = TextAnchor.UpperCenter;
        float imageHeight = ClientManager.Clients[clientIndex].image.height * 0.75f;
        imageStyle.padding.left = 10;

        GUILayout.BeginArea(imageBox, GUIContent.none, imageStyle);
        GUILayout.Label(new GUIContent(ClientManager.Clients[clientIndex].image), GUILayout.Height(imageHeight));
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(loc.x, loc.y + panelHeight - panelHeight / 3 + buttonHeight / 2, panelWidth, panelHeight));
        GUILayout.BeginVertical();

        if (GUILayout.Button("VIEW CLIENT'S\nNEEDS", whiteButtonStyle, GUILayout.Height(30), GUILayout.Width(120)))
        {
            if (clientIndex == clientNeedsIndex)
            {
                showClientsNeeds = !showClientsNeeds;
            }
            else
            {
                clientNeedsIndex = clientIndex;
                showClientsNeeds = true;
            }
        }

        if (ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Design)
        {
            if (GUILayout.Button("CHANGE CLIENT", whiteButtonStyle, GUILayout.Height(20), GUILayout.Width(120)))
            {
                ClientManager.MyClients[panelIndex] = 0;
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void ClientsNeeds(Vector2 loc, float panelWidth, float panelHeight, int index)
    {
        GUI.Box(new Rect(loc.x, loc.y, panelWidth, panelHeight), GUIContent.none, hintSkin.GetStyle("needsbox"));

        GUILayout.BeginArea(new Rect(loc.x + 15, loc.y + 20, panelWidth - 20, panelHeight - 20));
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name:\t", hintSkin.GetStyle("needsheader"));
        GUILayout.Label(ClientManager.Clients[index].name, hintSkin.GetStyle("needstext"), GUILayout.Width(panelWidth - 20));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Job:\t", hintSkin.GetStyle("needsheader"));
        GUILayout.Label(ClientManager.Clients[index].job, hintSkin.GetStyle("needstext"), GUILayout.Width(panelWidth - 20));
        GUILayout.EndHorizontal();


        GUILayout.Label("Needs/Interests:", hintSkin.GetStyle("needsheader"));
        foreach (string item in ClientManager.Clients[index].needsAndInterests)
        {
            GUILayout.Label("* " + item, hintSkin.GetStyle("needstext"), GUILayout.Width(panelWidth - 20));
        }

        GUILayout.Label("\n", hintSkin.GetStyle("needstext"));

        GUILayout.Label(ClientManager.Clients[index].description.text, hintSkin.GetStyle("needstext"),
            GUILayout.ExpandHeight(true), GUILayout.Width(panelWidth - 30));


        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    void QuestionPanel(Vector2 loc, float panelWidth, float panelHeight)
    {
        GUILayout.BeginArea(new Rect(loc.x, loc.y + panelHeight / 3, panelWidth, panelHeight));

        GUIStyle labelStyle = new GUIStyle(hintSkin.GetStyle("label"));
        labelStyle.alignment = TextAnchor.UpperCenter;

        GUI.Label(new Rect(0, 0, panelWidth, panelHeight),
            "Do you want to add another person to your client family?", labelStyle);

        if (GUI.Button(new Rect(panelWidth / 2 - 30, 110, 60, 25),
                "Yes", whiteButtonStyle))
        {
            showNewClientPanel = true;
        }

        else if (GUI.Button(new Rect(panelWidth / 2 - 60, 140, 120, 50),
                    "NO, CHOOSE\nLOCATION NEXT", whiteButtonStyle))
        {
            CompleteMyList();
        }

        GUILayout.EndArea();
    }

    // MyList should be filled up at the end of this stage
    void CompleteMyList()
    {
        ClientManager.CompleteList();

        ArchitectStudioGUI.SetState(ArchitectStudioGUI.States.Layout);
    }


    /// <summary>
    /// All clients has been picked already / At least one client is picked and next State is selected
    /// </summary>
    /// <returns></returns>
    internal static bool IsFinished()
    {
        return ClientManager.IsMyClientsListComplete && ClientManager.IsValid;
    }
}
