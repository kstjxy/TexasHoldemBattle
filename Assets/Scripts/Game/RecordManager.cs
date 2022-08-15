using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    //RecordManager����Э����¼��Ҳ�����Ϣ���������Ժϲ���GameManager��ȥ��ֻ��Ҫ�������ṩ�Ľӿں������ű�����������Ĳ��������޸ļ��ɡ�

    //�����¼�
    public delegate void ActionCall(int playerNum, int actionNum);//��ע�⣺����� playerNum ��������Ǹ���ҵ� seatNum �������Ҫ���ģ��ǵ�Ҫ�� Player ���е� AddActionRecord() һ���޸�
    public event ActionCall ActionRecords;
    public static RecordManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    /// <summary>
    /// �����¼����𡰹㲥��
    /// </summary>
    /// <param name="playerNum">�ж���������</param>
    /// <param name="actionNum">��ҵ��ж�����</param>
    public void CallActionRecord(int playerNum,int actionNum)
    {
        if (ActionRecords != null)
            ActionRecords(playerNum, actionNum);
        else
            Debug.Log("NULL delegate"); 
    }
}
