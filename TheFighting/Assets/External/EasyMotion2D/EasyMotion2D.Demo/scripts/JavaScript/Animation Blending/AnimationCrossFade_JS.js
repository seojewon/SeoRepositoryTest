public var spriteAnimation : EasyMotion2D.SpriteAnimation;
var clipNames : Array;


// Use this for initialization
function Start () {

	//Get SpriteAnimation for GameObject
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);

	clipNames = new Array();    
	//Get all clips assigned in SpriteAnimation
	var clips = EasyMotion2D.SpriteAnimationUtility.GetAnimationClips( spriteAnimation );
	for( var clip in clips)
	{
		clipNames.push( clip.name );
	}
}





private var  fadeLength : float = 0.3f;

function OnGUI()
{
	GUILayout.BeginArea (new Rect (25, 45, 150, 500));
	


	GUILayout.BeginVertical();
	
	for( var clipName : String in clipNames)
	{
		//If click button, then CrossFade this clip.
		if ( GUILayout.Button( clipName ) )
		{
			//CrossFade to other clips.
			spriteAnimation.CrossFade( clipName, fadeLength);
		}
	}		

	GUILayout.EndVertical();


	//show the fade length
	GUILayout.Label( "CrossFade Length:" + fadeLength.ToString() );
	//get fade length from GUI
	fadeLength = GUILayout.HorizontalSlider( fadeLength, 0.01f, 1f);

	GUILayout.EndArea();
}