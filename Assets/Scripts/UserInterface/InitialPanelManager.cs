using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitialPanelManager : MonoBehaviour
{
    public Animator panelAnimator;

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
    //��Ϸ��ʼʱִ�и÷���
    public void StartButtonClicked()
    {
        //�����û������������Ϸȫ�ֵĳ�ʼ��������ѡ���AI���������ʵ����������ز������ö�ȡ���ύ��GameManager
        panelAnimator.Play("GameStarted");//ʹ�ö�����Panel�˳�����
    }

    public void CallInitialPanel()
    {
        panelAnimator.Play("StartInitializing");//ʹ�ö�����Panel�˳�����
    }
}
