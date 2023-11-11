using System.Collections;
using System.Collections.Generic;
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
        player,
        ball,
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

    
    
    public GameObject PassStations;
    [Header("Ontrail")]
    [Tooltip("�ȶ��켣���м�վ��")] public List<GameObject> list_passStations;
    [Tooltip("վ�����ƶ��ٶ�")] public List<float> list_speed;

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (used)
            return;
        if(collision.CompareTag("Player") && enterType == EnterType.player)
        {
            CallHandleTrigger();
        }
        if (collision.CompareTag("Ball") && enterType == EnterType.ball)
        {
            CallHandleTrigger();
        }
    }
    void CallHandleTrigger()
    {
        bool whetherUse = InteractiveBall.Instance.HandleTrigger(this);
        if (usedOnce && whetherUse)
            used = true;
        visable = !used;
        OnValidate();
    }
    private void Awake()
    {
        list_passStations.Clear();
        for (int i=0;i< PassStations.transform.childCount;i++)
        {
            GameObject child = PassStations.transform.GetChild(i).gameObject;
            if (child.activeSelf)
                list_passStations.Add(child);
        }
    }

    public void OnValidate()
    {
        PassStations = gameObject.transform.Find("PassStations").gameObject;
        GetComponent<SpriteRenderer>().enabled = visable;
        for (int i = 0; i < PassStations.transform.childCount; i++)
        {
            GameObject child = PassStations.transform.GetChild(i).gameObject;
            child.GetComponent<SpriteRenderer>().enabled = visable;
        }
    }
}
