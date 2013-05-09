using UnityEngine;
using System.Collections;

public enum _RESIZETYPE
{
    RT_NONE = 0,
    RT_STATIC_XY_SIZE = 1,             // At Main Root  //사이즈 때문에 자동으로 포지션도 맞춰진다. 
    RT_STATIC_YY_SIZE = 2,             // At static child 
    RT_DYNAMIC_YY_SIZE = 3,            // At Dynamic child  Y_Base
    RT_MAX
}


public class csReSizePos : MonoBehaviour
{
    public bool m_IsVertical = true; //Vertical(세로가 길게)
    public _RESIZETYPE m_ResizeType = _RESIZETYPE.RT_NONE;

    [HideInInspector] public Vector3 m_RepairSize = Vector3.zero; //Parent를 바꾼 후 로컬 사이즈 복구를 위한 변수 
    [HideInInspector] public Vector3 m_RepairLPos = Vector3.zero;
    private bool m_TopSTOneFlag = true;         //Top Start OneFlag

    [HideInInspector] public float m_BasicXX = 720.0f;
    [HideInInspector] public float m_BasicYY = 1280.0f;

    private float m_ScaleXX;
    private float m_ScaleYY;

    private float m_CacValueXX;
    private float m_CacValueYY;

    void Awake()  //Before Create 
    {
        m_RepairLPos.x = this.transform.localPosition.x;    //--> UI Root (2D) : Camera2D의 차일드 기준으로 제작되어 있기 때문이다.
        m_RepairLPos.y = this.transform.localPosition.y;
        m_RepairLPos.z = this.transform.localPosition.z;

        m_RepairSize.x = this.transform.localScale.x;
        m_RepairSize.y = this.transform.localScale.y;
        m_RepairSize.z = this.transform.localScale.z;
    }

    //Instantiate() //이 사이에 호출되는 듯 하다. 
    //RT_DYNAMIC_YY_SIZE 일때 Instantiate()하면 초기위치를 항상 셋팅해 줄껀지 말껀지 생각해 봐야 한다. : m_RepairLPos

    // Use this for initialization
    void Start() //이쪽은 Object 가 생성되고 난 다음, After Create
    {
        if (m_IsVertical == false)
        {
            m_BasicXX = 1280.0f;  
            m_BasicYY = 720.0f;
        }

        MyStart();
    }

    public void MyStart()      //--> csReSizePos : Start()가 최상위로 먼저 호출된다는 보장이 없으니까.... (이후로 호출되어야 하는 모든 Class:Start()함수에서 반드시 MyStart()를 제일 먼저 호출해 준다. 
    {
        if (m_TopSTOneFlag == true) //최우선으로 한번만 호출되게 하기 위하여...
        {
            if (m_ResizeType == _RESIZETYPE.RT_DYNAMIC_YY_SIZE)  // At Dynamic child
            {
                this.transform.localScale = m_RepairSize;
            }

            m_ScaleXX = (float)Screen.width / m_BasicXX;
            m_ScaleYY = (float)Screen.height / m_BasicYY;

            if (m_ResizeType == _RESIZETYPE.RT_STATIC_XY_SIZE)  // At Main Root
            {
                m_CacValueXX = this.transform.localScale.x;
                m_CacValueXX = m_CacValueXX * m_ScaleXX;         //사이즈 때문에 자동으로 포지션도 맞춰진다. 

                m_CacValueYY = this.transform.localScale.y;
                m_CacValueYY = m_CacValueYY * m_ScaleYY;

                this.transform.localScale = new Vector3(m_CacValueXX, m_CacValueYY, 1);
            }
            else if (m_ResizeType == _RESIZETYPE.RT_STATIC_YY_SIZE)
            {
                m_CacValueXX = this.transform.localScale.x;
                m_CacValueXX = m_CacValueXX * m_ScaleYY;  //m_ScaleXX;

                m_CacValueYY = this.transform.localScale.y;
                m_CacValueYY = m_CacValueYY * m_ScaleYY;

                this.transform.localScale = new Vector3(m_CacValueXX, m_CacValueYY, 1);

                m_CacValueXX = this.transform.position.x;
                m_CacValueXX = m_CacValueXX * m_ScaleXX;

                m_CacValueYY = this.transform.position.y;
                m_CacValueYY = m_CacValueYY * m_ScaleYY;
                this.transform.position = new Vector3(m_CacValueXX, m_CacValueYY, this.transform.position.z);
            }
            else if (m_ResizeType == _RESIZETYPE.RT_DYNAMIC_YY_SIZE)
            {
                m_CacValueXX = this.transform.localScale.x;
                m_CacValueXX = m_CacValueXX * m_ScaleYY;  //m_ScaleXX;

                m_CacValueYY = this.transform.localScale.y;
                m_CacValueYY = m_CacValueYY * m_ScaleYY;

                this.transform.localScale = new Vector3(m_CacValueXX, m_CacValueYY, 1);

                this.transform.localPosition = m_RepairLPos;  //Instantiate() //시에 m_RepairLPos를 셋팅해 온다. 
                //Instantiate()에서 m_RepairLPos를 셋팅하지 않으면 Awake()의 값 즉 처음 만들 때 위치가 셋팅될 것이다.
            }

            m_TopSTOneFlag = false; //한번만 //최우선으로 한번만 호출되게 하기 위하여...
        }
    }

}
