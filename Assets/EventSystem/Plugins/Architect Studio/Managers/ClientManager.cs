using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ClientManager
{
    public static int maxNumOfClients = 4;

    private static List<ClientDefinition> clients = new List<ClientDefinition>();
    public static List<ClientDefinition> Clients
    {
        get
        {
            return clients;
        }
        private set { }
    }

    private static List<int> myClients = new List<int>();
    public static List<int> MyClients
    {
        get
        {
            return myClients;
        }
        set
        {
            myClients = value;
        }
    }
	
	public static void Init ()
	{
        ClientDefinition[] clientsArray = (ClientDefinition[])GameObject.FindObjectsOfType(typeof(ClientDefinition));
        if (clientsArray != null)
        {
            clients.AddRange(clientsArray);
            clients.Sort(delegate(ClientDefinition p1, ClientDefinition p2)
            { return p1.clientSortKey.CompareTo(p2.clientSortKey); });
        }
		DeactivateAllClients();
	}

    public static bool IsValid
    {
        get
        {
            if (myClients.Count == 0)
                return false;

            foreach (int client in myClients)
            {
                if (client > 0)
                    return true;
            }

            return false;
        }
    }

    public static bool IsMyClientsListComplete
    {
        get
        {
            return myClients.Count == maxNumOfClients;
        }
    }
	
	public static void DeactivateAllClients ()
	{
		foreach(ClientDefinition c in clients)
		{
			if (c != null)
				c.gameObject.SetActiveRecursively(false);
		}
	}

	public static void ActivateAllClients ()
	{
		foreach(ClientDefinition c in clients)
		{
			if (c != null)
				c.gameObject.SetActiveRecursively(true);
		}
	}

    internal static void CompleteList()
    {
        int numOfEmptyClients = maxNumOfClients - ClientManager.MyClients.Count;

        for (int i = 0; i < numOfEmptyClients; i++)
        {
            ClientManager.MyClients.Add(0);
        }
    }
}
