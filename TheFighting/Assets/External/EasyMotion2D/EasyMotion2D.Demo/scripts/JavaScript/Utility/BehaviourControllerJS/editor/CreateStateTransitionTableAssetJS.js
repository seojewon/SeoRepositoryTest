

@MenuItem( "Assets/Create/State Transition Table_JS" )
public static function CreateCollection()
{
	var asset : StateTransitionTableAssetJS;
	var name : String  = "NewStateTransitionTableJS";
	var nameIdx : int = 0;
	
	while( System.IO.File.Exists( Application.dataPath + "/" + name + nameIdx + ".asset" ) )
	{
		nameIdx++;
	}

	asset = ScriptableObject.CreateInstance(StateTransitionTableAssetJS);
	AssetDatabase.CreateAsset( asset, "Assets/" + name + nameIdx + ".asset" );
	
	EditorUtility.FocusProjectWindow();
	Selection.activeObject = asset;
}


