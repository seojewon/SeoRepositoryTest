var spriteAnimation : EasyMotion2D.SpriteAnimation;
var clipNames : Array;

// Use this for initialization
function Start () {
	

	//Get SpriteAnimation for GameObject
	spriteAnimation = GetComponent(EasyMotion2D.SpriteAnimation);


	clipNames = new Array();    
	//Get all clips assigned in SpriteAnimation
	var clips : EasyMotion2D.SpriteAnimationClip[] = EasyMotion2D.SpriteAnimationUtility.GetAnimationClips( spriteAnimation );
	for( var clip : EasyMotion2D.SpriteAnimationClip in clips)
	{
		clipNames.push( clip.name );
	}

	currAnim = spriteAnimation.clip.name;
}



var pos : Vector2 = Vector2.zero;
var currAnim : String = "";

function OnGUI()
{
	GUILayout.BeginArea( Rect(25, 45, 150, 500));
	
	GUILayout.BeginHorizontal();
	
	pos = GUILayout.BeginScrollView(pos);

	for( var clipName : String in clipNames)
	{
		//If click button, then play this clip.
		if ( GUILayout.Button( clipName ) )
		{
			//stop all playing clips.
			spriteAnimation.StopAll();

			//play new clip by name.
			spriteAnimation.Play( clipName );
			//Get clip's playing state
			var state : EasyMotion2D.SpriteAnimationState = spriteAnimation[ clipName ];
			//Set play speed to 1.
			state.speed = 1f;

			currAnim = clipName;
		}
	}

	GUILayout.Space(20);



	GUILayout.EndScrollView();

	GUILayout.EndHorizontal();


	//if click "stop" button, then stop all playing clips.
	if ( GUILayout.Button( "Stop" ) )
	{
		spriteAnimation.StopAll();
	}


	var clipState : EasyMotion2D.SpriteAnimationState = spriteAnimation[ currAnim ];


	//Pause and resume clip
	if ( GUILayout.Button( clipState.isPaused ? "Resume" : "Pause" ) )
	{
		if ( clipState.isPaused )
		{
			spriteAnimation.Resume( clipState.clip.name );
		}
		else
		{
			spriteAnimation.Pause( clipState.clip.name );
		}
	}			



	//Display current playing clip's state.
	GUILayout.Label( "Name:" + clipState.clip.name );
	GUILayout.Label( "Length:" + clipState.length.ToString() );
	GUILayout.Label( "WrapMode:" + clipState.wrapMode.ToString() );
	GUILayout.Label( "Time:" + clipState.time.ToString() );
	GUILayout.Label( "Speed:" + clipState.speed.ToString() );
	clipState.speed = GUILayout.HorizontalSlider( clipState.speed, -5f, 5f);



	//Set play time range in clip 
	GUILayout.Label( "StartTime:" + clipState.startTime.ToString() );
	var startTime : float = GUILayout.HorizontalSlider( clipState.startTime, 0f, clipState.endTime);
	if ( GUI.changed )
	{
		spriteAnimation.SetClipRange( clipState.clip.name, startTime, clipState.endTime);
	}

	GUILayout.Label( "EndTime:" + clipState.endTime.ToString() );
	var endTime : float = GUILayout.HorizontalSlider( clipState.endTime, clipState.startTime, clipState.clip.length);
	if ( GUI.changed )
	{
		spriteAnimation.SetClipRange( clipState.clip.name, clipState.startTime, endTime);
	}


	GUILayout.EndArea();
}
