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

    //����ģʽ
    public static UIManager instance;

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
        //����һЩ��ս��Ȼع��ʼ���Ĳ���
        if (Time.timeScale <= 0)
            return;
        InitialPanelManager.instance.CallInitialPanel();
        GameManager.instance.Restart();
        CardManager.instance.Restart();
        //ɾ�����������ϵ�AI
        for (int i = 0; i < PlayerManager.instance.seatedPlayers.Count; i++)
        {
            Destroy(PlayerManager.instance.seatedPlayers[i].playerObject.gameObject);
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
        GolbalVar.speedFactor = value + 0.01f;
        speedValueText.text = value.ToString();
    }

    /// <summary>
    /// �����־���·��� LOG & DETAILS ��
    /// </summary>
    /// <param name="log">Ҫ������ı����ݣ����Զ��ھ��׻��У�</param>
    public void PrintLog(string log)
    {
        logText.text = logText.text + "\n" + log;
    }

    //unfinished
    /// <summary>
    /// �������а����ݣ�ֱ�Ӷ�ȡ���� GameManager ������б�������
    /// </summary>
    public void UpdateRankingList()
    {
        //��ȡ����б��������
        Debug.Log("Ranking List Updated!!  ����������ʱ��ûд��");
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
    }

    /// <summary>
    /// �������Ϣ�����ʾ�ں��ʵ�����㣬������������ PlayerObject
    /// </summary>
    /// <param name="p">Player</param>
    /// <returns>PlayerObject ����</returns>
    public PlayerObject SetPlayerOnSeat(Player p)
    {
        GameObject po = Instantiate(Resources.Load<GameObject>("Prefabs/Player_Prefab"));

        if (p.seatNum < 0 || p.seatNum > 7)
        {
            PrintLog("����δ���������������λ�ţ�"+p.seatNum);
        }
        po.transform.SetParent(tableRect);
        RectTransform poRT = po.GetComponent<RectTransform>();
        poRT.anchoredPosition = positions[p.seatNum];
        poRT.localScale = new Vector3(1, 1, 1);
        po.GetComponent<PlayerObject>().InitializeThePlayer(p);
        return po.GetComponent<PlayerObject>();
    }

    /// <summary>
    /// ���½���
    /// </summary>
    /// <param name="change">���ر䶯�������Ϊ���������ȫ�������Ӯ��</param>
    public void UpdateCoinsPool(int change)
    {

        if (change < 0)
            GolbalVar.bot = 0;
        else
            GolbalVar.bot += change;

        coinPoolText.text = GolbalVar.bot.ToString();
    }

    /// <summary>
    /// ֱ�Ӷ�ȡ���µ�ǰ����
    /// </summary>
    public void UpdateGameRounds()
    {
        gamesCountText.text = "GAMES: " + GolbalVar.curRoundNum + "/" + GolbalVar.totalRoundNum;
    }
}
