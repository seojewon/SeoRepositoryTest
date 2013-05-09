using UnityEngine;
using System.Collections;
using EasyMotion2D;

public class ResourceViewer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public Rect texPosition;
	public int texIndex = 0;
	public int matIndex = 0;
	public int rmsIndex = 0;
	public int sprIndex = 0;

	void OnGUI()
	{
		enumTexture();
		enumMaterial();
		enumRenderModeSetting();
		enumSprite();
	}


	void enumSprite()
	{
		Object[] sprs = Resources.FindObjectsOfTypeAll( typeof(Sprite));

		if ( sprs.Length > 0)
		{
			int count = sprs.Length;

			if ( GUI.Button( new Rect( 200, Screen.height - 130, 30, 30 ), "<" ) )
			{
				sprIndex--;
			}

			if ( GUI.Button( new Rect( 600, Screen.height - 130, 30, 30 ), ">" ) )
			{
				sprIndex++;
			}	

			
			sprIndex = Mathf.Min( Mathf.Max( sprIndex, 0), count - 1);

			if ( count > 0 )
				GUI.Label( new Rect(250,Screen.height - 130, 350, 30), "Sprite:"+sprIndex + "/" + count + ":" + sprs[sprIndex].name );
			else
				GUI.Label( new Rect(250,Screen.height - 130, 350, 30), 0 + "/" + count);
		}
	}


	void enumRenderModeSetting()
	{
		Object[] rms = Resources.FindObjectsOfTypeAll( typeof(SpriteRenderModeSetting));

		if ( rms.Length > 0)
		{
			int count = rms.Length;

			if ( GUI.Button( new Rect( 200, Screen.height - 80, 30, 30 ), "<" ) )
			{
				rmsIndex--;
			}

			if ( GUI.Button( new Rect( 600, Screen.height - 80, 30, 30 ), ">" ) )
			{
				rmsIndex++;
			}	

			
			rmsIndex = Mathf.Min( Mathf.Max( rmsIndex, 0), count - 1);

			if ( count > 0 )
				GUI.Label( new Rect(250,Screen.height - 80, 350, 30), "SpriteRenderModeSetting:"+rmsIndex + "/" + count + ":" + rms[rmsIndex].name );
			else
				GUI.Label( new Rect(250,Screen.height - 80, 350, 30), 0 + "/" + count);
		}
	}


	void enumMaterial()
	{
		Object[] mats = Resources.FindObjectsOfTypeAll( typeof(Material));

		if ( mats.Length >= 0)
		{
			int count = mats.Length;

			if ( GUI.Button( new Rect( 200, Screen.height - 30, 30, 30 ), "<" ) )
			{
				matIndex--;
			}
			
			if ( GUI.Button( new Rect( 600, Screen.height - 30, 30, 30 ), ">" ) )
			{
				matIndex++;
			}			

			
			matIndex = Mathf.Min( Mathf.Max( matIndex, 0), count - 1);

			if ( count > 0 )
				GUI.Label( new Rect(250,Screen.height - 30, 350, 30), "Material:"+matIndex + "/" + count + ":" + mats[matIndex].name );
			else
				GUI.Label( new Rect(250,Screen.height - 30, 350, 30), 0 + "/" + count);
		}
	}


	void enumTexture()
	{
		Object[] texts = Resources.FindObjectsOfTypeAll( typeof(Texture2D));

		if ( texts.Length > 0)
		{
			int count = texts.Length;


			if ( GUI.Button( new Rect( 0, Screen.height - 30, 30, 30 ), "<" ) )
			{
				texIndex--;
			}


			if ( GUI.Button( new Rect( 150, Screen.height - 30, 30, 30 ), ">" ) )
			{
				texIndex++;
			}			

			if ( texIndex < 0)
			{
				texIndex = count - 1;
			}
			if ( texIndex >= count )
			{
				texIndex = 0;
			}

			if ( count > 0 )
			{
				GUI.Label( new Rect(50,Screen.height - 30, 100, 30), "Texture:"+texIndex + "/" + count);

				if ( texts[texIndex] is Texture2D )
				{
					GUI.TextField( new Rect(0,Screen.height - 60, 200, 30), texts[texIndex].name);
					GUI.DrawTexture( texPosition, texts[texIndex] as Texture2D);
				}
			}
		}
	}
}
