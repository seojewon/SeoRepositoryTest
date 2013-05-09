using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateObjectBehaviourAsset
{

	[MenuItem( "Assets/Create/State Transition Table" )]
	public static void CreateCollection()
	{
		StateTransitionTableAsset asset;
		string name = "NewStateTransitionTable";
		int nameIdx = 0;
		
		while( System.IO.File.Exists( Application.dataPath + "/" + name + nameIdx + ".asset" ) )
		{
			nameIdx++;
		}

		asset = ScriptableObject.CreateInstance<StateTransitionTableAsset>();
		AssetDatabase.CreateAsset( asset, "Assets/" + name + nameIdx + ".asset" );
		
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}
}


