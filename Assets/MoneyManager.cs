using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public int moneyCount;


    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }


}
