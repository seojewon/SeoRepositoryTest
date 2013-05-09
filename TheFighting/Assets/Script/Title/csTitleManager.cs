using UnityEngine;
using System.Collections;

public class csTitleManager : MonoBehaviour {

    private GameObject mText1 = null;
    private GameObject mText2 = null;

    private float mChangeTime = 0.0f;
    private bool mChange = false;

    void Awake()
    {
        mText1 = GameObject.Find("TitleText1");
        if (mText1 != null)
        {
            mText1.SetActive(true);
        }
        else Debug.Log("TitleText1 is null");

        mText2 = GameObject.Find("TitleText2");
        if (mText2 != null)
        {
            mText2.SetActive(false);
        }
        else Debug.Log("TitleText2 is null");
    }

	// Use this for initialization
	void Start () {
        mChangeTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.time - mChangeTime > 0.2f)
        {
             mChangeTime = Time.time;
             if (mChange == false)
             {
                 mChange = true;
                 mText2.SetActive(false);
                 mText2.SetActive(true);
             }
             else
             {
                 mChange = false;
                 mText2.SetActive(true);
                 mText2.SetActive(false);
             }
        }

	    if(Input.GetMouseButtonUp(0))
        {
            Application.LoadLevel("InGame");
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
	}
}
