using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;

public class PackSprites : MonoBehaviour {

	public SpriteGroup[] groups;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () 
	{
		if ( GUI.Button( new Rect(0,0,100,20), "Pack Sprites" ) )
		{
			List<Sprite> sprs = new List<Sprite>();
			foreach( SpriteGroup group in groups)
			{
				sprs.AddRange( group.frames );
			}

			if ( Sprite.PackSprites( sprs.ToArray(), 1024, false) )
			{
				Resources.UnloadUnusedAssets();
			}
		}
	}
}
