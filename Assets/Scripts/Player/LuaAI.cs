using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaAI : MonoBehaviour
{
    public string name = "my Name";
    public GameStat stats;


    public delegate void OnInit(int n);
    public delegate int Bet(); 
    public virtual List<Card> FinalSelection()
    {
        return new List<Card>();
    }
}
