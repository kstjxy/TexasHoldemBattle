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
        //ִ�и�ע����
        //��Ҳ����ѽ����������ѭ��
        this.GetComponent<Animator>().Play("Ended");
    }

    void RaiseButtonClicked()
    {
        //ִ�м�ע����
        int sumOfRaise = int.Parse(raiseInput.text);//ȡ�ü�ע���
        //��Ҳ����ѽ����������ѭ��
        this.GetComponent<Animator>().Play("Ended");
    }
    void PassButtonClicked()
    {
        //ִ�й��Ʋ���
        //��Ҳ����ѽ����������ѭ��
        this.GetComponent<Animator>().Play("Ended");
    }
    void LeaveClicked()
    {
        //ִ���볡����
        //��Ҳ����ѽ����������ѭ��
        this.GetComponent<Animator>().Play("Ended");
    }

    /// <summary>
    /// ��������������˹�������庰����
    /// </summary>
    public void CallManualPanel()
    {
        this.GetComponent<Animator>().Play("Start");
        followCostText.text = GlobalVar.curBetCount.ToString();
    }
}
