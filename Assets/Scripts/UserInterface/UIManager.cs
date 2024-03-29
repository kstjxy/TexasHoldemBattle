using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Animators")]
    public Animator pausePanelAnimator;

    [Header("UI Components_InteractElements")]
    public Button pauseButton;
    public Button continueButton;
    public Button restartButton;
    public Slider speedValueSlider;
    public Button showCardsButton;
    public Button saveLogsButton;
    public List<GameObject> playerObjects;
    public List<GameObject> cardsSetPanels;

    [Header("CommunityCards_Image")]
    public List<Image> communityCards;

    [Header("UI Components_Texts")]
    public Text coinPoolText;
    public Text gamesCountText;
    public Text speedValueText;
    public Text logText;
    public Text countDownText;
    public List<Text> rankingList;

    [Header("Attribute")]
    public bool isShowingCards = true;
    public string logSave;
    //单例模式
    public static UIManager instance;

    //Loglst 用于多线程更新UI
    public List<string> logList = new List<string>();
    private int lenOfList = 0; 

    //特效对象池
    public Queue<GameObject> textEffectsPool = new Queue<GameObject>();
    public List<GameObject> activeTextEffects = new List<GameObject>();

    private void Awake()
    {
        //单例模式初始化
        if (instance != null)
            Destroy(this);
        else
            instance = this;

        pauseButton.onClick.AddListener(delegate () { Pause_ButtonClicked(); });
        continueButton.onClick.AddListener(delegate () { Continue_ButtonClicked(); });
        restartButton.onClick.AddListener(delegate () { Restart_ButtonClicked(); });
        showCardsButton.onClick.AddListener(delegate () { ShowingCardsButtonClicked(); });
        saveLogsButton.onClick.AddListener(delegate () { SaveLogsButtonClicked(); });
        speedValueSlider.onValueChanged.AddListener(delegate (float value) { Speed_OnSliderValueChanged(value); });
    }

    private void Update()
    {
        if (GlobalVar.gameStatusCounter > -2)
            countDownText.text = "COUNTDOWN: " + (2 * GlobalVar.speedFactor - GameManager.timer).ToString();
    }

    public void Pause_ButtonClicked()
    {
        pausePanelAnimator.Play("Paused", 0, 0);
        Time.timeScale = 0;
    }

    public void Continue_ButtonClicked()
    {
        if (Time.timeScale > 0)
            return;
        Time.timeScale = 1;
        pausePanelAnimator.Play("Continue", 0, 0);
    }

    void Restart_ButtonClicked()
    {
        //进行一些清空进度回归初始化的操作
        
        InitialPanelManager.instance.CallInitialPanel();
        GameManager.instance.Restart();
        CardManager.instance.Restart();
        //删除所有牌桌上的AI与牌组
        for (int i = 0; i < playerObjects.Count; i++)
        {
            playerObjects[i].gameObject.SetActive(false);
            cardsSetPanels[i].SetActive(false);
        }
        //清空桌面的公共牌
        ClearAllCards();
        //清空LOG
        ClearLog();

        for (int i = 0; i < activeTextEffects.Count; i++)
        {
            activeTextEffects[i].SetActive(false);
            textEffectsPool.Enqueue(activeTextEffects[i]);
        }
        activeTextEffects.Clear();
    }
    /*
    public void StartButtonClicked()
    {
        //根据用户的输入进行游戏全局的初始化：包括选择的AI玩家上座（实例化）、相关参数设置读取并提交给GameManager
        if (PlayerManager.instance.SeatPlayers())
        {
            UIManager.instance.PrintLog("初始筹码数量：<color=#FFCAC9>" + InitialChips.text + "</color>");
            UIManager.instance.PrintLog("最低小盲注数：<color=#FFCAC9>" + smallBlindInjection.text + "</color>");
            UIManager.instance.PrintLog("游戏最大次数：<color=#C9FFDD>" + maximumGames.text + "</color>");
            UIManager.instance.PrintLog("每轮加注限制：<color=#C9FFF9>" + raisisLimit.text + "</color>\n游戏开始！");

            GlobalVar.initCoin = int.Parse(InitialChips.text);
            GlobalVar.minBetCoin = int.Parse(smallBlindInjection.text);
            GlobalVar.totalRoundNum = int.Parse(maximumGames.text);
            GlobalVar.maxBetCount = int.Parse(raisisLimit.text);
            GlobalVar.speedFactor = UIManager.instance.speedValueSlider.value;

            for (int i = 0; i < PlayerManager.instance.seatedPlayers.Count; i++)
            {
                PlayerManager.instance.seatedPlayers[i].playerObject = UIManager.instance.SetPlayerOnSeat(PlayerManager.instance.seatedPlayers[i]);
            }
            GameManager.instance.Setting();
            UIManager.instance.UpdateGameRounds();
            panelAnimator.Play("GameStarted");//使用动画让Panel退出画面
        }
    }*/

        void Speed_OnSliderValueChanged(float value)
    {
        GlobalVar.speedFactor = value;
        speedValueText.text = (2 * GlobalVar.speedFactor).ToString();
    }

    void ShowingCardsButtonClicked()
    {
        isShowingCards = !isShowingCards;
        for (int i = 0; i < PlayerManager.instance.activePlayers.Count; i++)
        {
            PlayerManager.instance.activePlayers[i].playerObject.ShowCards();
        }
    }

    void SaveLogsButtonClicked()
    {
        string type = "txt";
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "文件(*." + type + ")\0*." + type + "";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
        openFileName.title = "新建一个记事本/文本txt文档来保存log内容";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetSaveFileName(openFileName))
        {
            string path = openFileName.file;
            File.WriteAllText(path, null);
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(logSave);
            sw.Flush();
            sw.Close();
        }
    }

    /// <summary>
    /// 输出日志到下方的 LOG & DETAILS 中
    /// </summary>
    /// <param name="log">要输出的文本内容（会自动在句首换行）</param>
    public void PrintLog(string log)
    {
        if (logText.text.Length > 20000)
            ClearLog();
        logText.text = logText.text + "\n" + log;
        log = Regex.Replace(log, @"(<.*?>|</color>)", "");
        logSave = logSave + "\n" + log;
    }

    public void PrintThread(string s)
    {
        logList.Add(s);
    }

    /// <summary>
    /// （用于多线程，线程安全）输出日志到下方的 LOG & DETAILS 中，自动检测logList是否有增加
    /// </summary>
    public void UpdateLog()
    {
        if (logList.Count == lenOfList) return ;
        if (logText.text.Length > 20000)
            ClearLog();
        int oldlen = lenOfList;
        int len = logList.Count;
        lenOfList = len;
        for (int i = oldlen; i < len; i++)
        {
            logText.text = logText.text + "\n" + logList[i] ;
            logSave = logSave + "\n" + logList[i];
        }
                
    }

    /// <summary>
    /// 清空日志
    /// </summary>
    public void ClearLog()
    {
        logText.text = "LOG:";
        logList = new List<string>();
        lenOfList = 0;
    }
    /// <summary>
    /// 更新排行榜内容，接收一个已经排好序的playerList
    /// </summary>
    public void UpdateRankingList()
    {
        List<Player> playerList = PlayerManager.instance.GetRankedPlayers();
        List<int> rank = PlayerManager.instance.GetPlayerRank(playerList);
        if (rank.Count != playerList.Count || playerList.Count > 8)
        {
            Debug.Log("List items Count Error");
            return;
        }
        int i ;
        for (i = 0; i < 8 && i < playerList.Count; i++)//要显示的设置为Active
        {
            rankingList[i].text = rank[i].ToString() + ". " + playerList[i].coin.ToString() + ":" + playerList[i].playerName;
            rankingList[i].gameObject.SetActive(true);
        }
        rankingList[0].gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(340, i * 30);
        for (; i < 8; i++)//其余的要设置为unActive
        {
            rankingList[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 将公共牌显示到牌桌
    /// </summary>
    /// <param name="card">要显示的公共牌</param>
    /// <param name="cardPlace">从左到右的牌位，从0开始，到4结束</param>
    public void ShowCommunityCard(Card card, int cardPlace)
    {
        if (cardPlace < 0 || cardPlace > 4)
            return;
        communityCards[cardPlace].sprite = card.GetSpriteSurface();
        StartCoroutine(FlopAnim(communityCards[cardPlace].GetComponent<RectTransform>()));
    }
    /// <summary>
    /// 清空桌面上的公共牌以及玩家的手牌
    /// </summary>
    public void ClearAllCards()
    {
        Sprite emptyCard = Resources.Load<Sprite>("Cards/emptyPlace");
        for (int i = 0; i < communityCards.Count; i++)
        {
            communityCards[i].sprite = emptyCard;
            //StartCoroutine(FlopAnim(communityCards[i].GetComponent<RectTransform>()));
        }
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].activeSelf)
            {
                playerObjects[i].GetComponent<PlayerObject>().card1Image.sprite = emptyCard;
                playerObjects[i].GetComponent<PlayerObject>().card2Image.sprite = emptyCard;
                //StartCoroutine(FlopAnim(playerObjects[i].GetComponent<PlayerObject>().card1Image.GetComponent<RectTransform>()));
                //StartCoroutine(FlopAnim(playerObjects[i].GetComponent<PlayerObject>().card1Image.GetComponent<RectTransform>()));
                cardsSetPanels[i].SetActive(false);//cardSetPanel与playerObjects是对应的，因此可以放在同一个循环里（如果认为不太合理的话单独分出来也可以）
            }
        }
    }

    public IEnumerator FlopAnim(RectTransform cardImage)
    {
        float originalWidth = cardImage.rect.width;
        float originalHeight = cardImage.rect.height;
        cardImage.sizeDelta = new Vector2(0, cardImage.rect.height);
        while (cardImage.rect.width < originalWidth)
        {
            cardImage.sizeDelta = new Vector2(cardImage.rect.width+0.5f, originalHeight);
            yield return null;
        }
        cardImage.sizeDelta = new Vector2(originalWidth, originalHeight);
    }

    /// <summary>
    /// 让玩家信息面板显示在合适的坐标点，上座，并返回 PlayerObject
    /// </summary>
    /// <param name="p">Player</param>
    /// <returns>PlayerObject 本身</returns>
    public PlayerObject SetPlayerOnSeat(Player p)
    {
        if (p.seatNum < 0 || p.seatNum > 7)
        {
            PrintLog("有人未上座！！错误的座位号："+p.seatNum);
            return null;
        }
        GameObject po = playerObjects[p.seatNum];
        po.GetComponent<PlayerObject>().InitializeThePlayer(p);
        po.SetActive(true);
        return po.GetComponent<PlayerObject>();
    }

    /// <summary>
    /// 更新奖池
    /// </summary>
    public void UpdateCoinsPool()
    {
        coinPoolText.text = GlobalVar.pot.ToString();
    }

    /// <summary>
    /// 直接读取更新当前场数
    /// </summary>
    public void UpdateGameRounds()
    {
        gamesCountText.text = "GAMES: " + GlobalVar.curRoundNum + "/" + GlobalVar.totalRoundNum;
    }

    /// <summary>
    /// 显示支付筹码的文字的特效
    /// </summary>
    /// <param name="p">哪个玩家支付筹码</param>
    /// <param name="betCount">下注下了多少</param>
    public void BetCoinsEffect(Player p, int betCount)
    {
        GameObject effect;
        if (textEffectsPool.Count <= 0)
            effect = Instantiate(Resources.Load<GameObject>("Prefabs/TextEffect"));
        else
            effect = textEffectsPool.Dequeue();
        effect.GetComponent<Text>().text = "-" + betCount.ToString();
        effect.transform.SetParent(playerObjects[p.seatNum].transform);
        effect.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        effect.transform.localScale = Vector2.one;
        effect.SetActive(true);
        effect.GetComponent<Animator>().Play("Start", 0, 0);
        activeTextEffects.Add(effect);
    }

    /// <summary>
    /// 将搭配好的牌显示出来
    /// </summary>
    /// <param name="p">显示牌的Player，牌组finalCards已经设置好五张牌</param>
    public void ShowFinalCardSet(Player p)
    {
        int seat = p.seatNum;
        if (seat < 0 || seat > 7)
        {
            Debug.Log("座位号错误！错误号码：" + seat);
            return;
        }
        if (p.finalCards.Count<5)
        {
            Debug.Log("最终牌组数量错误！");
            return;
        }
        cardsSetPanels[seat].SetActive(true);
        cardsSetPanels[seat].transform.GetChild(0).GetComponent<Image>().sprite = p.finalCards[0].GetSpriteSurface();
        StartCoroutine(FlopAnim(cardsSetPanels[seat].transform.GetChild(0).GetComponent<RectTransform>()));
        cardsSetPanels[seat].transform.GetChild(1).GetComponent<Image>().sprite = p.finalCards[1].GetSpriteSurface();
        StartCoroutine(FlopAnim(cardsSetPanels[seat].transform.GetChild(1).GetComponent<RectTransform>()));
        cardsSetPanels[seat].transform.GetChild(2).GetComponent<Image>().sprite = p.finalCards[2].GetSpriteSurface();
        StartCoroutine(FlopAnim(cardsSetPanels[seat].transform.GetChild(2).GetComponent<RectTransform>()));
        p.playerObject.card1Image.sprite = p.finalCards[3].GetSpriteSurface();
        StartCoroutine(FlopAnim(p.playerObject.card1Image.GetComponent<RectTransform>()));
        p.playerObject.card2Image.sprite = p.finalCards[4].GetSpriteSurface();
        StartCoroutine(FlopAnim(p.playerObject.card2Image.GetComponent<RectTransform>()));
    }
}
