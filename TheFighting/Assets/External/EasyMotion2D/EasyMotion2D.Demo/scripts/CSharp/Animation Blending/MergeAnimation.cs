using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class MergeAnimation : MonoBehaviour {

	public SpriteAnimation spriteAnimation;
	public string[] animatinoNames = null;

	private SpriteAnimationState currState = null;

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

		if ( spriteAnimation.clip != null )
			currState = spriteAnimation[ spriteAnimation.clip.name ];
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
				//Get clip's playing state
				currState = spriteAnimation[ animatinoNames[i] ];
			}
		}

		GUILayout.EndScrollView();

		GUILayout.EndHorizontal();



		if ( currState != null )
		{

			SpriteAnimationState clipState = currState;

			//Pause and resume clip
			if ( GUILayout.Button( clipState.enabled ? "Disable" : "Enable" ) )
			{
				clipState.enabled = !clipState.enabled;
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

			GUILayout.Label( "Weight:" + clipState.weight.ToString() );
			float weight = GUILayout.HorizontalSlider( clipState.weight, 0f, 1f);
			if ( GUI.changed )
			{
				clipState.weight = weight;
			}

			GUILayout.Label( "Layer:" + clipState.layer.ToString() );
			int layer = (int)GUILayout.HorizontalSlider( clipState.layer, -100f, 100f);
			if ( GUI.changed )
			{
				clipState.layer = layer;
			}
		}
		GUILayout.EndArea();
	}
}
