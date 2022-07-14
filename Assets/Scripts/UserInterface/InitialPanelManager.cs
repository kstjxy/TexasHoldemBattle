using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialPanelManager : MonoBehaviour
{
    public Animator panelAnimator;

    [Header("RectTransform")]
    public RectTransform panelRect;

    [Header("InputFields")]
    public InputField InitialChips;
    public InputField smallBlindInjection;
    public InputField maximumGames;
    public InputField raisisLimit;

    public static InitialPanelManager instance;

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

            GolbalVar.initCoin = int.Parse(InitialChips.text);
            GolbalVar.minBetCoin = int.Parse(smallBlindInjection.text);
            GolbalVar.totalRoundNum = int.Parse(maximumGames.text);
            GolbalVar.maxBetCount = int.Parse(raisisLimit.text);

            for (int i = 0; i < PlayerManager.instance.seatedPlayers.Count; i++)
            {
                PlayerManager.instance.seatedPlayers[i].playerObject = UIManager.instance.SetPlayerOnSeat(PlayerManager.instance.seatedPlayers[i]);
            }
            GameManager.instance.Setting();
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

    public void CallInitialPanel()
    {
        ResetAllTheButtons();
        panelAnimator.Play("StartInitializing");
    }

    void ResetAllTheButtons()
    {
        for (int i = 0; i < panelRect.childCount; i++)
        {
            panelRect.GetChild(i).GetComponent<PlayerSelectButton>().RefreshButtonUI();
        }
    }

}
