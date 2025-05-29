using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMenuScrollChoose : MonoBehaviour
{
    public static ButtonMenuScrollChoose instance { get; private set; }
    public Button correctButton;
    public Button notCorrectButton;
}
