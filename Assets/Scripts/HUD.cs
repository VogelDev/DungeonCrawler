using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Image PlayerDisplay;
    public Player Player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        PlayerDisplay.sprite = Player.sprite.sprite;

        PlayerDisplay.color = Player.sprite.color;
    }
}
