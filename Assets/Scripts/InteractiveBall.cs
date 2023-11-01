using System.Collections;
using UnityEngine;

public class InteractiveBall : MonoBehaviour
{
    public enum STATE
    {   
        idle,           //Χ�������ת
        chasingPlayer,        //�ص�������
        //chasingMouse,   //�ص����λ��
        controlled,     //��������
    }

    public GameObject player;
    private STATE state = STATE.controlled;
    private bool canChangeState = true;

    [Header("ÿһ֡��ת�Ƕ�")]
    public float rotateAnglePerFrame;
    private float rotateAngle = 0f;
    [Header("��ת�뾶")]
    public float rotateRadius;
    //[Header("����׷���Ŀ��")]
    private ObservableValue<Vector3, InteractiveBall> target;
    //[Header("С�����ֵ�ж�Ϊ�ѽӽ�")]
    private float nearDistance = 0.01f;
    [SerializeField][Header("׷���ٶ�")]
    private float chaseSpeed = 0.03f;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        target = new(Vector3.zero, this);
        target.Value = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
    }
    private void FixedUpdate()
    {
        switch(state)
        {
            case(STATE.idle):
            {
                MyRotateAround();
                break;
            }
            default:
                break;
        }
    }
    private void Update()
    {
        //����Eʱ���л�target
        if (Input.GetKeyDown(KeyCode.E))
        {
            canChangeState = true;
            if (state == STATE.controlled)
                target.Value = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
            else if(state == STATE.idle || state == STATE.chasingPlayer)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                target.Value = new(mousePos.x, mousePos.y, 0f);
            }
        }
    }
    //idle״̬��Χ�������ת
    void MyRotateAround()
    {
        //TODO �޸���ת�ָ�
        //transform.RotateAround(player.transform.position, new(0, 1, 0), rotateAngle);
        rotateAngle += rotateAnglePerFrame;
        transform.position = new Vector3(player.transform.position.x + rotateRadius * Mathf.Cos(rotateAngle * Mathf.Deg2Rad),
            player.transform.position.y, player.transform.position.z - rotateRadius * Mathf.Sin(rotateAngle * Mathf.Deg2Rad));
        //transform.SetPositionAndRotation(new(transform.position.x,player.transform.position.y,0), Quaternion.identity);
    }
    //chasingPlayer״̬����target�ƶ���������л���idle״̬
    IEnumerator ChasePlayer()
    {
        while(!IsNear())
        {
            target.Value = player.transform.position + new Vector3(rotateRadius, 0f, 0f);
            transform.position = Vector3.MoveTowards(transform.position, target.Value, chaseSpeed);
            yield return new WaitForFixedUpdate();
        }
        state = STATE.idle;
        yield break;
    }
    //controlled״̬����target�ƶ�������󱣳�controlled״̬
    IEnumerator ChaseMouse()
    {
        transform.rotation = Quaternion.identity;
        while (true)
        {
            //��ȡ���λ�õ���������
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.Value = new(mousePos.x,mousePos.y,0f);
            transform.position = Vector3.MoveTowards(transform.position, target.Value, chaseSpeed);
            yield return new WaitForFixedUpdate();
        }
    }
    bool IsNear()
    {
        if ((transform.position - target.Value).magnitude <= nearDistance)
            return true;
        return false;
    }
    //target�仯ʱ���ã����л�״̬
    public void OnTargetChange()
    {
        if(!canChangeState)
            return;
        StopAllCoroutines();
        Debug.Log("Target changed ! oldState : " + state);
        if (state == STATE.controlled)
        {
            state = STATE.chasingPlayer;
            StartCoroutine(nameof(ChasePlayer));
        }
        else if(state == STATE.idle || state == STATE.chasingPlayer)
        {
            state = STATE.controlled;
            StartCoroutine(nameof(ChaseMouse));
        }
        
        canChangeState = false;
    }
}
