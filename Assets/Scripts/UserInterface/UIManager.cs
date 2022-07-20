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

    //����ģʽ
    public static UIManager instance;

    //��Ч�����
    public Queue<GameObject> textEffectsPool = new Queue<GameObject>();

    private void Awake()
    {
        //����ģʽ��ʼ��
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
        //����һЩ��ս��Ȼع��ʼ���Ĳ���
        if (Time.timeScale <= 0)
            return;
        InitialPanelManager.instance.CallInitialPanel();
        GameManager.instance.Restart();
        CardManager.instance.Restart();
        //ɾ�����������ϵ�AI������
        for (int i = 0; i < playerObjects.Count; i++)
        {
            playerObjects[i].gameObject.SetActive(false);
            cardsSetPanels[i].SetActive(false);
        }
        //�������Ĺ�����
        for (int i = 0; i < communityCards.Count; i++)
        {
            communityCards[i].sprite = Resources.Load<Sprite>("Cards/emptyPlace");
        }
        //���LOG
        logText.text = "LOG:";
    }
    void Speed_OnSliderValueChanged(float value)
    {
        GolbalVar.speedFactor = value;
        speedValueText.text = (2 * GolbalVar.speedFactor).ToString();
    }

    /// <summary>
    /// �����־���·��� LOG & DETAILS ��
    /// </summary>
    /// <param name="log">Ҫ������ı����ݣ����Զ��ھ��׻��У�</param>
    public void PrintLog(string log)
    {
        logText.text = logText.text + "\n" + log;
    }

    /// <summary>
    /// �������а����ݣ�����һ���Ѿ��ź����playerList
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
    /// ����������ʾ������
    /// </summary>
    /// <param name="card">Ҫ��ʾ�Ĺ�����</param>
    /// <param name="cardPlace">�����ҵ���λ����0��ʼ����4����</param>
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
    /// �������Ϣ�����ʾ�ں��ʵ�����㣬������������ PlayerObject
    /// </summary>
    /// <param name="p">Player</param>
    /// <returns>PlayerObject ����</returns>
    public PlayerObject SetPlayerOnSeat(Player p)
    {
        if (p.seatNum < 0 || p.seatNum > 7)
        {
            PrintLog("����δ���������������λ�ţ�"+p.seatNum);
            return null;
        }
        GameObject po = playerObjects[p.seatNum];
        po.GetComponent<PlayerObject>().InitializeThePlayer(p);
        po.SetActive(true);
        return po.GetComponent<PlayerObject>();
    }

    /// <summary>
    /// ���½���
    /// </summary>
    /// <param name="change">���ر䶯�������Ϊ���������ȫ�������Ӯ��</param>
    /*public void UpdateCoinsPool(int change)
    {

        if (change < 0)
            GolbalVar.pot = 0;
        else
            GolbalVar.pot += change;

        coinPoolText.text = GolbalVar.pot.ToString();
    }*/
    //������

    /// <summary>
    /// ���½���
    /// </summary>
    public void UpdateCoinsPool()
    {
        coinPoolText.text = GolbalVar.pot.ToString();
    }

    /// <summary>
    /// ֱ�Ӷ�ȡ���µ�ǰ����
    /// </summary>
    public void UpdateGameRounds()
    {
        gamesCountText.text = "GAMES: " + GolbalVar.curRoundNum + "/" + GolbalVar.totalRoundNum;
    }

    /// <summary>
    /// ��ʾ֧����������ֵ���Ч
    /// </summary>
    /// <param name="p">�ĸ����֧������</param>
    /// <param name="betCount">��ע���˶���</param>
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
    /// ������õ�����ʾ����
    /// </summary>
    /// <param name="p">��ʾ�Ƶ�Player������finalCards�Ѿ����ú�������</param>
    public void ShowFinalCardSet(Player p)
    {
        int seat = p.seatNum;
        if (seat < 0 || seat > 7)
        {
            Debug.Log("��λ�Ŵ��󣡴�����룺" + seat);
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
            Debug.Log("�������󣡴���������" + fc.Count);
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
