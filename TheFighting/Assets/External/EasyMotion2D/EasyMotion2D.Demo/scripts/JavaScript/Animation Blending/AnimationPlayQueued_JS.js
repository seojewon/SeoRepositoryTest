var spriteAnimation : EasyMotion2D.SpriteAnimation;
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

		//change the wrapmode of clip to once, because if play a looping clip, you can not waiting it complete.
		spriteAnimation[ clip.name ].wrapMode = WrapMode.Once;
	}
}






function OnGUI()
{
	GUILayout.BeginArea (new Rect (25, 45, 150, 500));
	
	GUILayout.BeginVertical();
	
	for( var clipName : String in clipNames)
	{
		//If click button, then play this clip.
		if ( GUILayout.Button( clipName ) )
		{
			//Add selected clip to playing queue
			spriteAnimation.PlayQueued( clipName );
		}
	}


	GUILayout.EndVertical();

	
	GUILayout.EndArea();
}