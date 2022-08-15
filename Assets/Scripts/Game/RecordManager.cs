using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    //RecordManager用来协助记录玩家操作信息。后续可以合并到GameManager中去，只需要把这里提供的接口和其他脚本中引用这里的部分稍作修改即可。

    //定义事件
    public delegate void ActionCall(int playerNum, int actionNum);//请注意：这里的 playerNum 输入的是那个玩家的 seatNum 。如果需要更改，记得要连 Player 类中的 AddActionRecord() 一并修改
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
    /// 调用事件发起“广播”
    /// </summary>
    /// <param name="playerNum">行动的玩家序号</param>
    /// <param name="actionNum">玩家的行动类型</param>
    public void CallActionRecord(int playerNum,int actionNum)
    {
        if (ActionRecords != null)
            ActionRecords(playerNum, actionNum);
        else
            Debug.Log("NULL delegate"); 
    }
}
