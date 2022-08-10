using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialPanelManager : MonoBehaviour
{
    [Header("Anim")]
    public Animator panelAnimator;
    public Text startErrorLog;
    public Text LuaLog;
    public Text WebLog;

    [Header("RectTransform")]
    public RectTransform panelRect;

    [Header("InputFields")]
    public InputField InitialChips;
    public InputField smallBlindInjection;
    public InputField maximumGames;
    public InputField raisisLimit;
    public InputField ipAdress;
    public InputField portNum;
    public InputField MaxPlayerNum;


    public static InitialPanelManager instance;
    private int lenOfPlayers = 0;

    private void Awake()
    {
        //����ģʽ��ʼ��
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    /// <summary>
    /// ����Ϸ��ʼʱִ��
    /// </summary>
    public void StartButtonClicked()
    {
        //�����û������������Ϸȫ�ֵĳ�ʼ��������ѡ���AI���������ʵ����������ز������ö�ȡ���ύ��GameManager
        if (PlayerManager.instance.SeatPlayers())
        {
            UIManager.instance.PrintLog("��ʼ����������<color=#FFCAC9>" + InitialChips.text + "</color>");
            UIManager.instance.PrintLog("���Сäע����<color=#FFCAC9>" + smallBlindInjection.text + "</color>");
            UIManager.instance.PrintLog("��Ϸ��������<color=#C9FFDD>" + maximumGames.text + "</color>");
            UIManager.instance.PrintLog("ÿ�ּ�ע���ƣ�<color=#C9FFF9>" + raisisLimit.text + "</color>\n��Ϸ��ʼ��");

            

            if (smallBlindInjection.text == "")
            {
                CallStartErrorLog("��С��ע����������Ϊ�գ�");
                return;
            }
            GlobalVar.minBetCoin = int.Parse(smallBlindInjection.text);
            if (GlobalVar.minBetCoin <= 0)
            {
                CallStartErrorLog("��С��ע����������Ϊ������");
                return;
            }
            if (InitialChips.text == "")
            {
                CallStartErrorLog("��ʼ����������Ϊ�գ�");
                return;
            }
            GlobalVar.initCoin = int.Parse(InitialChips.text);
            if (GlobalVar.initCoin < 2*GlobalVar.minBetCoin)
            {
                CallStartErrorLog("��ʼ����������Ϊ��С��ע��������������");
                return;
            }
            if (maximumGames.text == "")
            {
                CallStartErrorLog("�����Ϸ��������Ϊ�գ�");
                return;
            }
            GlobalVar.totalRoundNum = int.Parse(maximumGames.text);
            if (GlobalVar.totalRoundNum <= 0)
            {
                CallStartErrorLog("�����Ϸ��������Ϊ������");
                return;
            }
           
            if (raisisLimit.text == "")
            {
                CallStartErrorLog("����ע��������Ϊ�գ�");
                return;
            }
            GlobalVar.maxBetCount = int.Parse(raisisLimit.text);
            if (GlobalVar.maxBetCount <= 0)
            {
                CallStartErrorLog("����ע��������Ϊ������");
                return;
            }
            GlobalVar.speedFactor = UIManager.instance.speedValueSlider.value;

            for (int i = 0; i < PlayerManager.instance.seatedPlayers.Count; i++)
            {
                PlayerManager.instance.seatedPlayers[i].playerObject = UIManager.instance.SetPlayerOnSeat(PlayerManager.instance.seatedPlayers[i]);
            }
            GameManager.instance.Setting();
            UIManager.instance.UpdateGameRounds();
            panelAnimator.Play("GameStarted");//ʹ�ö�����Panel�˳�����
        }
    }

    /// <summary>
    /// ������ Player ͨ�����������ʾ�ڳ�ʼPanel���ṩ�û�ѡ��
    /// </summary>
    /// <param name="p">��ȡ����Player</param>
    public void AddSelectablePlayerButton(Player p)
    {
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x + 100, panelRect.sizeDelta.y);
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/SelectButton_Prefab"), panelRect);
        go.GetComponent<PlayerSelectButton>().InitializeSelectButton(p);
    }

    /// <summary>
    /// (���ڶ��̣߳��Զ��ж�allPlayer�Ƿ�������)������ Player ͨ�����������ʾ�ڳ�ʼPanel���ṩ�û�ѡ��
    /// </summary>
    public void UpdatePlayerButton()
    {
        if (PlayerManager.instance.allPlayers.Count == lenOfPlayers) return;
        int old = lenOfPlayers;
        int nowLen = PlayerManager.instance.allPlayers.Count;
        lenOfPlayers = nowLen;
        for (int i = old; i < nowLen; i++)
        {
            Player p = PlayerManager.instance.allPlayers[i];
            panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x + 100, panelRect.sizeDelta.y);
            GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/SelectButton_Prefab"), panelRect);
            go.GetComponent<PlayerSelectButton>().InitializeSelectButton(p);
        }
    }


    /// <summary>
    /// ��Restart��ʱ����ã�����ʼ���ô��ڽл���
    /// </summary>
    public void CallInitialPanel()
    {
        ResetAllTheButtons();
        panelAnimator.Play("StartInitializing");
    }

    /// <summary>
    /// �ڿ�ʼ��ʱ���������ѡ�����ⵯ����ʾ
    /// </summary>
    /// <param name="log"></param>
    public void CallStartErrorLog(string log)
    {
        WebLog.text = log;
        WebLog.GetComponent<Animator>().Play("showLog", 0, 0);
    }
    public void CallLuaLog(string log)
    {
        WebLog.text = log;
        WebLog.GetComponent<Animator>().Play("LuaLog", 0, 0);
    }
    public void CallWebLog(string log)
    {
        WebLog.text = log;
        WebLog.GetComponent<Animator>().Play("WebLog", 0, 0);
    }

    /// <summary>
    /// ˢ������Button��UI
    /// </summary>
    public void ResetAllTheButtons()
    {
        for (int i = 0; i < panelRect.childCount; i++)
        {
            panelRect.GetChild(i).GetComponent<PlayerSelectButton>().RefreshButtonUI();
        }
    }

    public void ResetAllAvatar()
    {
        for (int i = 0; i < panelRect.childCount; i++)
        {
            panelRect.GetChild(i).GetComponent<PlayerSelectButton>().RefreshButtonUI();
        }


    }
}
