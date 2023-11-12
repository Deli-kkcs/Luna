using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class MyTrigger : MonoBehaviour
{
    [Header("����")]
    public bool visable;

    [Header("�Ƿ�ֻ����һ��")]
    public bool usedOnce;
    [SerializeField][Header("�Ƿ��Ѿ�������")]
    private bool used = false;
    public enum EnterType
    {
        Player,
        Ball,
    };
    public EnterType enterType;
    public enum EffectType
    {
        None,//��Ч��
        GuildAhead,//�������ǰ��
        ChasePlayer,//�������
        Idle,//ͣ��ԭ��
        OnTrail,//���ռȶ��켣ǰ��
    }
    public EffectType effectType;

    [Header("GuildAhead")]
    [Tooltip("����ҵ��������")] public Vector2 delta_ahead;


    [HideInInspector]
    public GameObject PassStations;
    [HideInInspector]
    /*[Tooltip("�ȶ��켣���м�վ��")] */public List<GameObject> list_passStations;
    [Header("Ontrail")]
    [Tooltip("վ�����ƶ��ٶ�")]public List<float> list_passSpeed = new();

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (used)
            return;
        if(collision.CompareTag("Player") && enterType == EnterType.Player)
        {
            CallHandleTrigger();
        }
        if (collision.CompareTag("Ball") && enterType == EnterType.Ball)
        {
            CallHandleTrigger();
        }
    }
    void CallHandleTrigger()
    {
        bool whetherUse = InteractiveBall.Instance.HandleTrigger(this);
        if (usedOnce && whetherUse)
        {
            used = true;
            visable = false;
        }
        OnValidate();
    }
    private void Awake()
    {
        list_passStations.Clear();
        for (int i = 0; i < PassStations.transform.childCount; i++)
        {
            GameObject child = PassStations.transform.GetChild(i).gameObject;
            if (child.activeSelf)
                list_passStations.Add(child);
        }
    }

    public void OnValidate()
    {
        #region visable
        PassStations = transform.Find("PassStations").gameObject;
        GetComponent<SpriteRenderer>().enabled = visable;
        for (int i = 0; i < PassStations.transform.childCount; i++)
        {
            GameObject child = PassStations.transform.GetChild(i).gameObject;
            child.GetComponent<SpriteRenderer>().enabled = visable;
        }
        #endregion

        #region name
        gameObject.name = "Trigger_" + enterType + "_"+  effectType.ToString();
        #endregion

        #region onTrail
        if (effectType == EffectType.OnTrail)
        {
            PassStations.SetActive(true);
            
        }
        else
            PassStations.SetActive(false);
        #endregion


        //AttributeCollection collection = TypeDescriptor.GetAttributes(list_passSpeed,false);
        //Debug.Log(collection[0]);
        //Debug.Log(collection[1]);
        //Debug.Log(collection[2]);
        //Debug.Log(collection[3]);
    }
}
