var spriteAnimation : EasyMotion2D.SpriteAnimation;
var clipNames : Array;

//current selected animation state
var currState : EasyMotion2D.SpriteAnimationState = null;

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

	if ( spriteAnimation.clip != null )
		currState = spriteAnimation[ spriteAnimation.clip.name ];
}




var  pos : Vector2 = Vector2.zero;

function OnGUI()
{
	GUILayout.BeginArea (new Rect (25, 45, 150, 500));
	
	GUILayout.BeginHorizontal();		
	pos = GUILayout.BeginScrollView(pos);



	for( var clipName : String in clipNames )
	{
		//If click button, then select this clip.
		if ( GUILayout.Button( clipName ) )
		{
			//change current select animation state
			currState = spriteAnimation[ clipName ];
		}
	}


	GUILayout.EndScrollView();
	GUILayout.EndHorizontal();


	//if current select animation state is not null
	if ( currState != null )
	{

		var clipState : EasyMotion2D.SpriteAnimationState = currState;

		//enable or disable clip
		if ( GUILayout.Button( clipState.enabled ? "Disable" : "Enable" ) )
		{
			clipState.enabled = !clipState.enabled;
		}	


		//Pause or resume clip
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



		//Display current playing animation's state.
		GUILayout.Label( "Name:" + clipState.clip.name );
		GUILayout.Label( "Length:" + clipState.length.ToString() );
		GUILayout.Label( "WrapMode:" + clipState.wrapMode.ToString() );
		GUILayout.Label( "Time:" + clipState.time.ToString() );
		GUILayout.Label( "Speed:" + clipState.speed.ToString() );


		clipState.speed = GUILayout.HorizontalSlider( clipState.speed, -5f, 5f);


		GUILayout.Label( "Weight:" + clipState.weight.ToString() );
		var weight : float = GUILayout.HorizontalSlider( clipState.weight, 0f, 1f);
		//if adjust the weight, then assgin the new value the animation state
		if ( GUI.changed )
		{
			clipState.weight = weight;
		}

		GUILayout.Label( "Layer:" + clipState.layer.ToString() );
		var layer : int = GUILayout.HorizontalSlider( clipState.layer, -100f, 100f);
		//if adjust the layer, then assgin the new value the animation state
		if ( GUI.changed )
		{
			clipState.layer = layer;
		}
	}
	GUILayout.EndArea();
}