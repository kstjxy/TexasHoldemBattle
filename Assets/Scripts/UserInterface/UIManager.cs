using System.Collections;
using System.Collections.Generic;
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

    //单例模式
    public static UIManager instance;

    //特效对象池
    public Queue<GameObject> textEffectsPool = new Queue<GameObject>();

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
        speedValueSlider.onValueChanged.AddListener(delegate (float value) { Speed_OnSliderValueChanged(value); });
    }

    private void Update()
    {
        if (GolbalVar.gameStatusCounter > -2)
            countDownText.text = "COUNTDOWN: " + (2 * GolbalVar.speedFactor - GameManager.timer).ToString();
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
        if (Time.timeScale <= 0)
            return;
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
        for (int i = 0; i < communityCards.Count; i++)
        {
            communityCards[i].sprite = Resources.Load<Sprite>("Cards/emptyPlace");
        }
        //清空LOG
        logText.text = "LOG:";
    }
    void Speed_OnSliderValueChanged(float value)
    {
        GolbalVar.speedFactor = value;
        speedValueText.text = (2 * GolbalVar.speedFactor).ToString();
    }

    /// <summary>
    /// 输出日志到下方的 LOG & DETAILS 中
    /// </summary>
    /// <param name="log">要输出的文本内容（会自动在句首换行）</param>
    public void PrintLog(string log)
    {
        logText.text = logText.text + "\n" + log;
    }

    /// <summary>
    /// 更新排行榜内容，接收一个已经排好序的playerList
    /// </summary>
    public void UpdateRankingList()
    {
        List<Player> playerList = GameManager.instance.GetRankedPlayers();
        List<int> rank = GameManager.instance.GetPlayerRank(playerList);
        if (rank.Count != playerList.Count)
        {
            Debug.Log("List items Count Error");
            return;
        }
        for (int i = 0; i < 8 && i < playerList.Count; i++)
        {
            rankingList[i].text = rank[i].ToString() + ". " + playerList[i].coin.ToString() + ":" + playerList[i].playerName;
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
    /// <param name="change">奖池变动清情况，为负数则代表全部被玩家赢走</param>
    /*public void UpdateCoinsPool(int change)
    {

        if (change < 0)
            GolbalVar.pot = 0;
        else
            GolbalVar.pot += change;

        coinPoolText.text = GolbalVar.pot.ToString();
    }*/
    //已弃用

    /// <summary>
    /// 更新奖池
    /// </summary>
    public void UpdateCoinsPool()
    {
        coinPoolText.text = GolbalVar.pot.ToString();
    }

    /// <summary>
    /// 直接读取更新当前场数
    /// </summary>
    public void UpdateGameRounds()
    {
        gamesCountText.text = "GAMES: " + GolbalVar.curRoundNum + "/" + GolbalVar.totalRoundNum;
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
        List<Card> fc = p.finalCards;
        for (int i = 0; i < p.playerCardList.Count; i++)
        {
            for (int j = 0; j < fc.Count; j++)
            {
                if (p.playerCardList[i].Value == fc[j].Value && p.playerCardList[i].cardSuit == fc[j].cardSuit)
                {
                    fc.Remove(fc[j]);
                    break;
                }
            }
        }
        if (fc.Count != 3)
        {
            Debug.Log("牌数错误！错误数量：" + fc.Count);
            return;
        }
        cardsSetPanels[seat].SetActive(true);
        cardsSetPanels[seat].transform.GetChild(0).GetComponent<Image>().sprite = fc[0].GetSpriteSurface();
        StartCoroutine(FlopAnim(cardsSetPanels[seat].transform.GetChild(0).GetComponent<RectTransform>()));
        cardsSetPanels[seat].transform.GetChild(1).GetComponent<Image>().sprite = fc[1].GetSpriteSurface();
        StartCoroutine(FlopAnim(cardsSetPanels[seat].transform.GetChild(1).GetComponent<RectTransform>()));
        cardsSetPanels[seat].transform.GetChild(2).GetComponent<Image>().sprite = fc[2].GetSpriteSurface();
        StartCoroutine(FlopAnim(cardsSetPanels[seat].transform.GetChild(2).GetComponent<RectTransform>()));
    }
}
