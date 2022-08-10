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
        //单例模式初始化
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    /// <summary>
    /// 在游戏开始时执行
    /// </summary>
    public void StartButtonClicked()
    {
        //根据用户的输入进行游戏全局的初始化：包括选择的AI玩家上座（实例化）、相关参数设置读取并提交给GameManager
        if (PlayerManager.instance.SeatPlayers())
        {
            UIManager.instance.PrintLog("初始筹码数量：<color=#FFCAC9>" + InitialChips.text + "</color>");
            UIManager.instance.PrintLog("最低小盲注数：<color=#FFCAC9>" + smallBlindInjection.text + "</color>");
            UIManager.instance.PrintLog("游戏最大次数：<color=#C9FFDD>" + maximumGames.text + "</color>");
            UIManager.instance.PrintLog("每轮加注限制：<color=#C9FFF9>" + raisisLimit.text + "</color>\n游戏开始！");

            

            if (smallBlindInjection.text == "")
            {
                CallStartErrorLog("最小下注筹码数不能为空！");
                return;
            }
            GlobalVar.minBetCoin = int.Parse(smallBlindInjection.text);
            if (GlobalVar.minBetCoin <= 0)
            {
                CallStartErrorLog("最小下注筹码数必须为正数！");
                return;
            }
            if (InitialChips.text == "")
            {
                CallStartErrorLog("初始筹码数不能为空！");
                return;
            }
            GlobalVar.initCoin = int.Parse(InitialChips.text);
            if (GlobalVar.initCoin < 2*GlobalVar.minBetCoin)
            {
                CallStartErrorLog("初始筹码数最少为最小下注筹码数的两倍！");
                return;
            }
            if (maximumGames.text == "")
            {
                CallStartErrorLog("最大游戏局数不能为空！");
                return;
            }
            GlobalVar.totalRoundNum = int.Parse(maximumGames.text);
            if (GlobalVar.totalRoundNum <= 0)
            {
                CallStartErrorLog("最大游戏局数必须为正数！");
                return;
            }
           
            if (raisisLimit.text == "")
            {
                CallStartErrorLog("最大加注次数不能为空！");
                return;
            }
            GlobalVar.maxBetCount = int.Parse(raisisLimit.text);
            if (GlobalVar.maxBetCount <= 0)
            {
                CallStartErrorLog("最大加注次数必须为正数！");
                return;
            }
            GlobalVar.speedFactor = UIManager.instance.speedValueSlider.value;

            for (int i = 0; i < PlayerManager.instance.seatedPlayers.Count; i++)
            {
                PlayerManager.instance.seatedPlayers[i].playerObject = UIManager.instance.SetPlayerOnSeat(PlayerManager.instance.seatedPlayers[i]);
            }
            GameManager.instance.Setting();
            UIManager.instance.UpdateGameRounds();
            panelAnimator.Play("GameStarted");//使用动画让Panel退出画面
        }
    }

    /// <summary>
    /// 读到的 Player 通过这个方法显示在初始Panel上提供用户选择
    /// </summary>
    /// <param name="p">读取到的Player</param>
    public void AddSelectablePlayerButton(Player p)
    {
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x + 100, panelRect.sizeDelta.y);
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/SelectButton_Prefab"), panelRect);
        go.GetComponent<PlayerSelectButton>().InitializeSelectButton(p);
    }

    /// <summary>
    /// (用于多线程，自动判断allPlayer是否有增加)读到的 Player 通过这个方法显示在初始Panel上提供用户选择
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
    /// 在Restart的时候调用，将初始设置窗口叫回来
    /// </summary>
    public void CallInitialPanel()
    {
        ResetAllTheButtons();
        panelAnimator.Play("StartInitializing");
    }

    /// <summary>
    /// 在开始的时候出现人数选择问题弹出提示
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
    /// 刷新所有Button的UI
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
