using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPanelManager : MonoBehaviour
{
    //��Ϸ��ʼʱִ�и÷���
    public void StartButtonClicked(Animator panelAnimator)
    {
        //�����û������������Ϸȫ�ֵĳ�ʼ��������ѡ���AI���������ʵ����������ز������ö�ȡ���ύ��GameManager
        panelAnimator.Play("GameStarted");//ʹ�ö�����Panel�˳�����
    }
}
