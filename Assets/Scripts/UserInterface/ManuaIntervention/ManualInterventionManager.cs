using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualInterventionManager : MonoBehaviour
{
    [Header("UI_Elements")]
    public Button followButton;
    public Button raiseButton;
    public Button passButton;
    public Button leaveButton;
    public Text followCostText;
    public InputField raiseInput;

    public static ManualInterventionManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        followButton.onClick.AddListener(delegate () { FollowButtonClicked(); });
        raiseButton.onClick.AddListener(delegate () { RaiseButtonClicked(); });
        passButton.onClick.AddListener(delegate () { PassButtonClicked(); });
        leaveButton.onClick.AddListener(delegate () { LeaveClicked(); });
    }

    void FollowButtonClicked()
    {
        //执行跟注操作
        //玩家操作已结束，请继续循环
        this.GetComponent<Animator>().Play("Ended");
    }

    void RaiseButtonClicked()
    {
        //执行加注操作
        int sumOfRaise = int.Parse(raiseInput.text);//取得加注金额
        //玩家操作已结束，请继续循环
        this.GetComponent<Animator>().Play("Ended");
    }
    void PassButtonClicked()
    {
        //执行过牌操作
        //玩家操作已结束，请继续循环
        this.GetComponent<Animator>().Play("Ended");
    }
    void LeaveClicked()
    {
        //执行离场操作
        //玩家操作已结束，请继续循环
        this.GetComponent<Animator>().Play("Ended");
    }

    /// <summary>
    /// 这个方法用来将人工操作面板喊出来
    /// </summary>
    public void CallManualPanel()
    {
        this.GetComponent<Animator>().Play("Start");
        followCostText.text = GlobalVar.curBetCount.ToString();
    }
}
