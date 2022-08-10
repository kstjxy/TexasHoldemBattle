using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    Animator panelAnim;
    public bool panelCalled;

    [Header("UI Elements")]
    public Button callPanelButton;
    public InputField smallBlindInjection;
    public InputField maximumGames;
    public InputField raisisLimit;
    // Start is called before the first frame update
    void Start()
    {
        panelAnim = this.GetComponent<Animator>();
        panelAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        panelCalled = false;
        callPanelButton.onClick.AddListener(delegate { CallPanelButtonClicked(); });
    }

    void CallPanelButtonClicked()
    {
        if (panelCalled)
        {
            panelAnim.Play("Out");
            UIManager.instance.Continue_ButtonClicked();

            if (smallBlindInjection.text == "")
                UIManager.instance.PrintLog("���Сäעδ���룬����ʧ�ܣ�");
            else if (int.Parse(smallBlindInjection.text) <= 0)
                UIManager.instance.PrintLog("���Сäע����Ϊ����������ʧ�ܣ�");
            else if (GlobalVar.minBetCoin != int.Parse(smallBlindInjection.text))
            {
                UIManager.instance.PrintLog("���Сäע�����£�<color=#FFCAC9>" + smallBlindInjection.text + "</color>");
                GlobalVar.minBetCoin = int.Parse(smallBlindInjection.text);
            }

            if (maximumGames.text == "" )
                UIManager.instance.PrintLog("�����Ϸ����δ���룬����ʧ�ܣ�");
            else if (int.Parse(smallBlindInjection.text) <= GlobalVar.curRoundNum)
                UIManager.instance.PrintLog("�����Ϸ����С���ѽ�����Ϸ����������ʧ�ܣ�");
            else if (GlobalVar.totalRoundNum != int.Parse(maximumGames.text))
            {
                UIManager.instance.PrintLog("��Ϸ���������£�<color=#C9FFDD>" + maximumGames.text + "</color>");
                GlobalVar.totalRoundNum = int.Parse(maximumGames.text);
            }

            if (raisisLimit.text == "")
                UIManager.instance.PrintLog("ÿ�ּ�ע����δ���룬����ʧ�ܣ�");
            else if (int.Parse(raisisLimit.text) <= 0)
                UIManager.instance.PrintLog("ÿ�ּ�ע���Ʊ���Ϊ����������ʧ�ܣ�");
            else if (GlobalVar.maxBetCount != int.Parse(raisisLimit.text))
            {
                UIManager.instance.PrintLog("ÿ�ּ�ע���Ƹ��£�<color=#C9FFF9>" + raisisLimit.text + "</color>"); 
                GlobalVar.maxBetCount = int.Parse(raisisLimit.text);
            }
            
            UIManager.instance.UpdateGameRounds();
        }
        else
        {
            panelAnim.Play("In");
            smallBlindInjection.text = GlobalVar.minBetCoin.ToString();
            maximumGames.text = GlobalVar.totalRoundNum.ToString();
            raisisLimit.text = GlobalVar.maxBetCount.ToString();
            UIManager.instance.Pause_ButtonClicked();
        }
        panelCalled = !panelCalled;
    }
}
