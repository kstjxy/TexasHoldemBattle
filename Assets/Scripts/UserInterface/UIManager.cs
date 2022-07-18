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

    [Header("CommunityCards_Image")]
    public List<Image> communityCards;

    [Header("UI Components_Texts")]
    public Text coinPoolText;
    public Text gamesCountText;
    public Text speedValueText;
    public Text logText;
    public Text countDownText;
    public List<Text> rankingList;

    [Header("PositionsForSeatInRect")]
    public RectTransform tableRect;

    List<Vector2> positions;

    //单例模式
    public static UIManager instance;

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
        positions = new List<Vector2>() { new Vector2(-240, 165), new Vector2(-400, 80), new Vector2(-400, -40), new Vector2(-240, -140), new Vector2(140, -140), new Vector2(300, -40), new Vector2(300, 80), new Vector2(140, 165) };
    }

    private void Update()
    {
        if (GolbalVar.gameStatusCounter > -2)
            countDownText.text = "COUNTDOWN: " + (2 * GolbalVar.speedFactor - GameManager.timer).ToString();
    }
    //unfinished
    void Pause_ButtonClicked()
    {
        pausePanelAnimator.Play("Paused", 0, 0);
        Time.timeScale = 0;
    }
    //unfinished
    void Continue_ButtonClicked()
    {
        if (Time.timeScale > 0)
            return;
        Time.timeScale = 1;
        pausePanelAnimator.Play("Continue", 0, 0);
    }
    //unfinished
    void Restart_ButtonClicked()
    {
        //进行一些清空进度回归初始化的操作
        if (Time.timeScale <= 0)
            return;
        InitialPanelManager.instance.CallInitialPanel();
        GameManager.instance.Restart();
        CardManager.instance.Restart();
        //删除所有牌桌上的AI
        for (int i = 0; i < PlayerManager.instance.seatedPlayers.Count; i++)
        {
            Destroy(PlayerManager.instance.seatedPlayers[i].playerObject.gameObject);
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
        GameObject po = Instantiate(Resources.Load<GameObject>("Prefabs/Player_Prefab"));

        if (p.seatNum < 0 || p.seatNum > 7)
        {
            PrintLog("有人未上座！！错误的座位号："+p.seatNum);
        }
        po.transform.SetParent(tableRect);
        RectTransform poRT = po.GetComponent<RectTransform>();
        poRT.anchoredPosition = positions[p.seatNum];
        poRT.localScale = new Vector3(1, 1, 1);
        po.GetComponent<PlayerObject>().InitializeThePlayer(p);
        return po.GetComponent<PlayerObject>();
    }

    /// <summary>
    /// 更新奖池
    /// </summary>
    /// <param name="change">奖池变动清情况，为负数则代表全部被玩家赢走</param>
    public void UpdateCoinsPool(int change)
    {

        if (change < 0)
            GolbalVar.pot = 0;
        else
            GolbalVar.pot += change;

        coinPoolText.text = GolbalVar.pot.ToString();
    }

    /// <summary>
    /// 直接读取更新当前场数
    /// </summary>
    public void UpdateGameRounds()
    {
        gamesCountText.text = "GAMES: " + GolbalVar.curRoundNum + "/" + GolbalVar.totalRoundNum;
    }
}
