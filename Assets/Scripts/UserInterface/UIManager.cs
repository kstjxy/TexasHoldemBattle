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
        ClearAllCards();
        //���LOG
        ClearLog();
    }
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
        openFileName.filter = "�ļ�(*." + type + ")\0*." + type + "";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//Ĭ��·��
        openFileName.title = "�½�һ�����±�/�ı�txt�ĵ�������log����";
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
    /// �����־���·��� LOG & DETAILS ��
    /// </summary>
    /// <param name="log">Ҫ������ı����ݣ����Զ��ھ��׻��У�</param>
    public void PrintLog(string log)
    {
        logText.text = logText.text + "\n" + log;
        log = Regex.Replace(log, @"(<.*?>|</color>)", "");
        logSave = logSave + "\n" + log;
    }

    /// <summary>
    /// �����־
    /// </summary>
    public void ClearLog()
    {
        logText.text = "LOG:";
    }
    /// <summary>
    /// �������а����ݣ�����һ���Ѿ��ź����playerList
    /// </summary>
    public void UpdateRankingList()
    {
        List<Player> playerList = GameManager.instance.GetRankedPlayers();
        List<int> rank = GameManager.instance.GetPlayerRank(playerList);
        if (rank.Count != playerList.Count || playerList.Count > 8)
        {
            Debug.Log("List items Count Error");
            return;
        }
        int i ;
        for (i = 0; i < 8 && i < playerList.Count; i++)//Ҫ��ʾ������ΪActive
        {
            rankingList[i].text = rank[i].ToString() + ". " + playerList[i].coin.ToString() + ":" + playerList[i].playerName;
            rankingList[i].gameObject.SetActive(true);
        }
        rankingList[0].gameObject.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(340, i * 30);
        for (; i < 8; i++)//�����Ҫ����ΪunActive
        {
            rankingList[i].gameObject.SetActive(false);
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
    /// <summary>
    /// ��������ϵĹ������Լ���ҵ�����
    /// </summary>
    public void ClearAllCards()
    {
        Sprite emptyCard = Resources.Load<Sprite>("Cards/emptyPlace");
        for (int i = 0; i < communityCards.Count; i++)
        {
            communityCards[i].sprite = emptyCard;
            StartCoroutine(FlopAnim(communityCards[i].GetComponent<RectTransform>()));
        }
        for (int i = 0; i < playerObjects.Count; i++)
        {
            if (playerObjects[i].activeSelf)
            {
                playerObjects[i].GetComponent<PlayerObject>().card1Image.sprite = emptyCard;
                playerObjects[i].GetComponent<PlayerObject>().card2Image.sprite = emptyCard;
                StartCoroutine(FlopAnim(playerObjects[i].GetComponent<PlayerObject>().card1Image.GetComponent<RectTransform>()));
                StartCoroutine(FlopAnim(playerObjects[i].GetComponent<PlayerObject>().card1Image.GetComponent<RectTransform>()));
                cardsSetPanels[i].SetActive(false);//cardSetPanel��playerObjects�Ƕ�Ӧ�ģ���˿��Է���ͬһ��ѭ��������Ϊ��̫����Ļ������ֳ���Ҳ���ԣ�
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
            GlobalVar.pot = 0;
        else
            GlobalVar.pot += change;

        coinPoolText.text = GlobalVar.pot.ToString();
    }*/
    //������

    /// <summary>
    /// ���½���
    /// </summary>
    public void UpdateCoinsPool()
    {
        coinPoolText.text = GlobalVar.pot.ToString();
    }

    /// <summary>
    /// ֱ�Ӷ�ȡ���µ�ǰ����
    /// </summary>
    public void UpdateGameRounds()
    {
        gamesCountText.text = "GAMES: " + GlobalVar.curRoundNum + "/" + GlobalVar.totalRoundNum;
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
        p.playerObject.card1Image.sprite = p.playerCardList[0].GetSpriteSurface();
        p.playerObject.card2Image.sprite = p.playerCardList[1].GetSpriteSurface();
    }
}
