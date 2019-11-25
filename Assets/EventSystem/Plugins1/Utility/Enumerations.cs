using UnityEngine;
using System.Collections;

public class Enumerations
{
}

public enum LODState
{
	Hidden,
	Visible,
	Deactivate
}

public enum Gender
{
	Male,
	Female
}

public enum TexasContext
{
	Development,
	WebTestDeployment,
	ClientDeployment	
}

public class ContextManager
{
	static ContextManager main;


	void Init ()
	{
		if (main == null)
			main = this;
	}
	
	private ContextManager ()
	{
		Init();
		context = TexasContext.Development;
	}

	private ContextManager (TexasContext cont)
	{
		Init();
		context = cont;
	}

	
	TexasContext context = TexasContext.Development;
	public static TexasContext CurrentContext
	{
		get
		{
			if (main == null) main = new ContextManager();
			return main.context;
		}
		
		set
		{
			if (main == null) main = new ContextManager();
			main.context = value;
		}
	}
}