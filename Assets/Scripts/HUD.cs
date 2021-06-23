using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Image PlayerDisplay;
    public Player Player;

    public Image XPBar;
    public Text LevelDisplay;
    public Text XPDisplay;
    public Text XPTNL;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        XPBar.fillAmount = Player.CurrentXP / (Player.CurrentLevel * 100);
        LevelDisplay.text = Player.CurrentLevel.ToString();
        XPDisplay.text = Player.CurrentXP.ToString(CultureInfo.CurrentCulture);
        XPTNL.text = (Player.CurrentLevel * 100).ToString();
    }

    public void Init()
    {
        PlayerDisplay.sprite = Player.sprite.sprite;

        PlayerDisplay.color = Player.sprite.color;
    }
}
