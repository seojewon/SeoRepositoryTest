using UnityEngine;
using System.Collections;
using SmoothMoves;

public enum State
{
    STAND = 0,
    GUARD,
    ATTACK
}

public enum CharacterType
{
    IPPO = 0,
    SENDO
}

public class csInGameManager : MonoBehaviour {
    static public csInGameManager m_Instance = null; 

    private GameObject mGameScene1 = null;
    private GameObject mGameScene2 = null;

    private bool mGameScene1View = false;
    private bool mGameScene2View = false;

    private float mChangeTime = 0.0f;
    private float mEffectTime = 0.0f;
    private float mBounceTime = 0.0f;

    private GameObject mFighting = null;
    private bool mBounce = false;
    private GameObject mEffect1 = null;
    private GameObject mEffect2 = null;
    private bool mEffectChange = false;
    private GameObject mEndTexture = null;
    private bool mEndTextureView = false;

    void Awake()
    {
        mGameScene1 = GameObject.Find("GameScene1");
        if (mGameScene1 != null) mGameScene1View = true;
        else Debug.Log("GameScene1 is null"); 

        mGameScene2 = GameObject.Find("GameScene2");
        if (mGameScene2 != null)
        {
            mGameScene2.SetActive(false);

            mFighting = mGameScene2.transform.FindChild("FightingTexture").gameObject;
            if (mFighting == null) Debug.Log("FightingTexture is null");
            
            mEffect1 = mGameScene2.transform.FindChild("EffectTexture1").gameObject;
            if (mEffect1 == null) Debug.Log("EffectTexture1 is null");

            mEffect2 = mGameScene2.transform.FindChild("EffectTexture2").gameObject;
            if (mEffect2 == null) Debug.Log("EffectTexture2 is null");

            mEndTexture = mGameScene2.transform.FindChild("EndTexture").gameObject;
            if (mEndTexture != null) mEndTexture.SetActive(false);
            else Debug.Log("EndTexture is null");
        }
        else Debug.Log("GameScene2 is null");
    }

	// Use this for initialization
	void Start () {
        m_Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
        if (mGameScene1View == true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                mGameScene1View = false;
                mGameScene2View = true;
                SetChangeGameScene(mGameScene1, false);
                SetChangeGameScene(mGameScene2, true);
                if (mEndTextureView == true) mEndTextureView = false;
                mEndTexture.SetActive(false);
            }
        }

        if (mGameScene2View == true)
        {
            if (mEndTextureView == false)
            {
                if (Time.time - mChangeTime > 4.0f)
                {
                    mChangeTime = Time.time;
                    mEndTextureView = true;
                    mEndTexture.SetActive(true);
                }

                if (Time.time - mEffectTime > 0.1f)
                {
                    mEffectTime = Time.time;
                    SetChangeEffectTexture();                    
                }

                if (Time.time - mBounceTime > 0.08f)
                {
                    mBounceTime = Time.time;
                    BounceTexture();
                }
            }
            else if (mEndTextureView == true)
            {
                if (Time.time - mChangeTime > 3.0f)
                {
                    mGameScene1View = true;
                    mGameScene2View = false;
                    SetChangeGameScene(mGameScene1, true);
                    SetChangeGameScene(mGameScene2, false);
                }
            }
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
	}

    void SetChangeGameScene(GameObject _g, bool _State)
    {
        if (_g != null)
        {
            _g.SetActive(_State);
            
            mChangeTime = Time.time;
            mEffectTime = Time.time;
            mBounceTime = Time.time;
        }
        else Debug.Log("ChangeGameObject is null");
    }

    void BounceTexture()
    {
        if (mBounce == false)
        {
            mBounce = true;
            mFighting.transform.localPosition = new Vector3(mFighting.transform.localPosition.x - 5.0f, 5.0f, mFighting.transform.localPosition.z);
        }
        else
        {
            mBounce = false;
            mFighting.transform.localPosition = new Vector3(mFighting.transform.localPosition.x + 5.0f, -5.0f, mFighting.transform.localPosition.z);
        }
    }

    void SetChangeEffectTexture()
    {
        if (mEffectChange == true)
        {
            mEffectChange = false;
            mEffect1.SetActive(true);
            mEffect2.SetActive(false);
        }
        else
        {
            mEffectChange = true;
            mEffect1.SetActive(false);
            mEffect2.SetActive(true);
        }
    }

    public void PlayBoneAnimation(GameObject _g, State _state)
    {
        if (_g.GetComponent<csCharacterControl>().mBoneAnimation == null)
        {
            Debug.Log("BoneAnimation is null");
            return;
        }

        if (_state != State.STAND) _g.GetComponent<csCharacterControl>().mState = _state;
        if (_state == State.ATTACK) _g.GetComponent<csCharacterControl>().mBoneAnimation.Play("Attack01");
        if (_state == State.GUARD) _g.GetComponent<csCharacterControl>().mBoneAnimation.Play("Guard01");
    }

    public void CheckState(GameObject _g)
    {
        if (_g.GetComponent<csCharacterControl>().mBoneAnimation == null)
        {
            Debug.Log("BoneAnimation is null");
            return;
        }

        if (_g.GetComponent<csCharacterControl>().mState != State.STAND && !_g.GetComponent<csCharacterControl>().mBoneAnimation.isPlaying)
        {
            _g.GetComponent<csCharacterControl>().mState = State.STAND;
            _g.GetComponent<csCharacterControl>().mBoneAnimation.Play("Idle");
        }
    }
}
