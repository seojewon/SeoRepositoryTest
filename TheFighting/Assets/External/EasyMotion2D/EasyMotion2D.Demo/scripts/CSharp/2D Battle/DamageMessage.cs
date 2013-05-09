using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EasyMotion2D;


public class DamageMessage : MonoBehaviour
{
    public BitmapFontTextRenderer text;
    public float lifeTime = 1;

    public void Start()
    {
        Invoke("DestroySelf", lifeTime);
    }

    public void DestroySelf()
    {
        Destroy( gameObject);
    }

    public void SetText( string text )
    {
        this.text.text = text;             
    }
}
