using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIconController : MonoBehaviour
{
    [SerializeField]
    Image iconBackGround = null;

    [SerializeField]
    Image iconWeapon = null;

	public Image GetIconBackground()
    {
        return iconBackGround;
    }

    public Image GetIconWeapon()
    {
        return iconWeapon;
    }
}
