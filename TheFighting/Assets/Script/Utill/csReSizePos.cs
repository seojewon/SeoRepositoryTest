using UnityEngine;
using System.Collections;

public enum _RESIZETYPE
{
    RT_NONE = 0,
    RT_STATIC_XY_SIZE = 1,             // At Main Root  //������ ������ �ڵ����� �����ǵ� ��������. 
    RT_STATIC_YY_SIZE = 2,             // At static child 
    RT_DYNAMIC_YY_SIZE = 3,            // At Dynamic child  Y_Base
    RT_MAX
}


public class csReSizePos : MonoBehaviour
{
    public bool m_IsVertical = true; //Vertical(���ΰ� ���)
    public _RESIZETYPE m_ResizeType = _RESIZETYPE.RT_NONE;

    [HideInInspector] public Vector3 m_RepairSize = Vector3.zero; //Parent�� �ٲ� �� ���� ������ ������ ���� ���� 
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
        m_RepairLPos.x = this.transform.localPosition.x;    //--> UI Root (2D) : Camera2D�� ���ϵ� �������� ���۵Ǿ� �ֱ� �����̴�.
        m_RepairLPos.y = this.transform.localPosition.y;
        m_RepairLPos.z = this.transform.localPosition.z;

        m_RepairSize.x = this.transform.localScale.x;
        m_RepairSize.y = this.transform.localScale.y;
        m_RepairSize.z = this.transform.localScale.z;
    }

    //Instantiate() //�� ���̿� ȣ��Ǵ� �� �ϴ�. 
    //RT_DYNAMIC_YY_SIZE �϶� Instantiate()�ϸ� �ʱ���ġ�� �׻� ������ �ٲ��� ������ ������ ���� �Ѵ�. : m_RepairLPos

    // Use this for initialization
    void Start() //������ Object �� �����ǰ� �� ����, After Create
    {
        if (m_IsVertical == false)
        {
            m_BasicXX = 1280.0f;  
            m_BasicYY = 720.0f;
        }

        MyStart();
    }

    public void MyStart()      //--> csReSizePos : Start()�� �ֻ����� ���� ȣ��ȴٴ� ������ �����ϱ�.... (���ķ� ȣ��Ǿ�� �ϴ� ��� Class:Start()�Լ����� �ݵ�� MyStart()�� ���� ���� ȣ���� �ش�. 
    {
        if (m_TopSTOneFlag == true) //�ֿ켱���� �ѹ��� ȣ��ǰ� �ϱ� ���Ͽ�...
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
                m_CacValueXX = m_CacValueXX * m_ScaleXX;         //������ ������ �ڵ����� �����ǵ� ��������. 

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

                this.transform.localPosition = m_RepairLPos;  //Instantiate() //�ÿ� m_RepairLPos�� ������ �´�. 
                //Instantiate()���� m_RepairLPos�� �������� ������ Awake()�� �� �� ó�� ���� �� ��ġ�� ���õ� ���̴�.
            }

            m_TopSTOneFlag = false; //�ѹ��� //�ֿ켱���� �ѹ��� ȣ��ǰ� �ϱ� ���Ͽ�...
        }
    }

}
