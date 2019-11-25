#define FortMcHenry
//#define Crypto
//#define Texas
//#define ETS1


using UnityEngine;
using System.Collections;


public enum Projects
{
	FortMcHenry,
	Texas,
	Crypto,
	ETS1,
}
public static class ProjectManager
{
#if FortMcHenry
	public static Projects project = Projects.FortMcHenry;
#elif Texas
	public static Projects project = Projects.Texas;
#elif Crypto
	public static Projects project = Projects.Crypto;
#elif ETS1
	public static Projects project = Projects.ETS1;
#endif
}
