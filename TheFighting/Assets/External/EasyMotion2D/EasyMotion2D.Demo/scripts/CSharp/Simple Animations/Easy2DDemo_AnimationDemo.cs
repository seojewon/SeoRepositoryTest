using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class Easy2DDemo_AnimationDemo : MonoBehaviour {

	public SpriteAnimation spriteAnimation;
	public string[] animatinoNames = null;
	private string currAnim = "";

	// Use this for initialization
	void Start () {

		//Get SpriteAnimation for GameObject
		spriteAnimation = GetComponent<SpriteAnimation>();


		List<string> tmp = new List<string>();

		//Get all clips assigned in SpriteAnimation
		SpriteAnimationClip[] clips = SpriteAnimationUtility.GetAnimationClips( spriteAnimation );
		foreach( SpriteAnimationClip clip in clips)
		{
			tmp.Add( clip.name);
		}

		animatinoNames = tmp.ToArray();
		currAnim = spriteAnimation.clip.name;

	}


	Vector2 pos = Vector2.zero;

	void OnGUI()
	{
		GUILayout.BeginArea (new Rect (25, 45, 150, 500));
		
		GUILayout.BeginHorizontal();
		
		pos = GUILayout.BeginScrollView(pos);

		for( int i = 0, e = animatinoNames.Length; i < e; i++)
		{
			//If click button, then play this clip.
			if ( GUILayout.Button( animatinoNames[i] ) )
			{
				//stop all playing clips.
				spriteAnimation.StopAll();

				//play new clip by name.
				spriteAnimation.Play( animatinoNames[i] );

				spriteAnimation.CalculateData();

				//Get clip's playing state
				SpriteAnimationState state = spriteAnimation[ animatinoNames[i] ];
				//Set play speed to 1.
				//state.speed = 1f;

				currAnim = animatinoNames[i];

//				if ( state.speed > 0)
//					state.time = 0f;
//				else
//					state.time = state.length;
			}
		}

		GUILayout.EndScrollView();

		GUILayout.EndHorizontal();


		//if click "stop" button, then stop all playing clips.
		if ( GUILayout.Button( "Stop" ) )
		{
			spriteAnimation.StopAll();
		}


		SpriteAnimationState clipState = spriteAnimation[ currAnim ];

		if ( clipState == null )
		{
			return;
		}


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
		float startTime = GUILayout.HorizontalSlider( clipState.startTime, 0f, clipState.endTime);
		if ( GUI.changed )
		{
			spriteAnimation.SetClipRange( clipState.clip.name, startTime, clipState.endTime);
		}

		GUILayout.Label( "EndTime:" + clipState.endTime.ToString() );
		float endTime = GUILayout.HorizontalSlider( clipState.endTime, clipState.startTime, clipState.clip.length);
		if ( GUI.changed )
		{
			spriteAnimation.SetClipRange( clipState.clip.name, clipState.startTime, endTime);
		}

		GUILayout.EndArea();
	}

}
