using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public enum WeaponTypes
    {
        None,
        ShortSword,
        BroadSword,
        Staff,
        Axe,
        ChainWhip,
        Hammer
    }

    public WeaponTypes WeaponType = WeaponTypes.ShortSword;

    public int StrengthModifier = 1;
    public int SpeedModifier = 1;
}
