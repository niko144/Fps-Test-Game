using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectionBarUI : MonoBehaviour
{
    

    [SerializeField]
    GameObject weaponIconPrefab;

    [SerializeField]
    Sprite weaponBackgroundSpriteDefault;

    [SerializeField]
    Sprite weaponBackgroundSpriteSelected;

    [SerializeField]
    Color weaponBackgroundColorDefault;

    [SerializeField]
    Color weaponBackgroundColorSelected;

    [SerializeField]
    Color weaponIconColorDefault;

    [SerializeField]
    Color weaponIconColorSelected;

    [SerializeField]
    Transform[] columnTransforms;

    [SerializeField]
    List<WeaponIconController> weaponIconColumn1 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn2 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn3 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn4 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn5 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn6 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn7 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn8 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn9 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController> weaponIconColumn0 = new List<WeaponIconController>();

    [SerializeField]
    List<WeaponIconController>[] weaponIconColumns;

    [SerializeField]
    Vector2Int currentSelectedIndex = new Vector2Int(-1, -1);

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    /// <summary>
    /// Always run this method first when creating an instance
    /// </summary>
    public void Initialize()
    {
        weaponIconColumns = new List<WeaponIconController>[]
        {
            weaponIconColumn1,
            weaponIconColumn2,
            weaponIconColumn3,
            weaponIconColumn4,
            weaponIconColumn5,
            weaponIconColumn6,
            weaponIconColumn7,
            weaponIconColumn8,
            weaponIconColumn9,
            weaponIconColumn0
        };

        /*foreach (Image header in headerImages)
        {
            header.overrideSprite = headerSpriteDefault;
            header.color = headerColorDefault;
        }
        */
    }

    public void AddWeaponToColumn(int indexColumn, Sprite weaponSprite)
    {
        if (!validColumnIndex(indexColumn))
            return;

        WeaponIconController weaponIcon = Instantiate(weaponIconPrefab, columnTransforms[indexColumn]).GetComponent<WeaponIconController>();

        weaponIcon.GetIconWeapon().overrideSprite = weaponSprite;

        SetWeaponIconDefault(weaponIcon);

        weaponIconColumns[indexColumn].Add(weaponIcon);
    }

    public void SelectIcon(int indexColumn, int indexRow)
    {
        if (!validColumnIndex(indexColumn) || !validRowIdex(indexColumn, indexRow))
            return;

        /*for (int i = 0; i < headerImages.Length; i++)
        {
            if (i == indexColumn)
            {
                headerImages[i].overrideSprite = headerSpriteSelected;
                headerImages[i].color = headerColorSelected;
            }
            else
            {
                headerImages[i].overrideSprite = headerSpriteDefault;
                headerImages[i].color = headerColorDefault;
            }
        }*/

        if (currentSelectedIndex.x != indexColumn || currentSelectedIndex.y != indexRow)
        {
            if (validColumnIndex(currentSelectedIndex.x) && validRowIdex(currentSelectedIndex.x, currentSelectedIndex.y))
            {
                WeaponIconController icon = weaponIconColumns[currentSelectedIndex.x][currentSelectedIndex.y];
                SetWeaponIconDefault(icon);
            }

            WeaponIconController iconNew = weaponIconColumns[indexColumn][indexRow];
            SetWeaponIconSelected(iconNew);

            currentSelectedIndex = new Vector2Int(indexColumn, indexRow);
        }
    }

    /// <summary>
    /// Use reset funtion to remove all inventory data, so you can rebuild if player inventory changes
    /// </summary>
    public void ResetAll()
    {
        foreach (List<WeaponIconController> iconList in weaponIconColumns)
        {
            DestroyIconsFromList(iconList);
        }
    }

    public void ResetColumn(int index)
    {
        if (!validColumnIndex(index))
            return;

        DestroyIconsFromList(weaponIconColumns[index]);
    }

    void DestroyIconsFromList(List<WeaponIconController> weaponIcons)
    {
        foreach (WeaponIconController icon in weaponIcons)
        {
            Destroy(icon.gameObject);
        }

        weaponIcons.Clear();
    }

    bool validColumnIndex(int index)
    {
        if (index < 0)
            return false;
        if (index >= columnTransforms.Length)
            return false;
        return true;
    }

    bool validRowIdex(int indexColumn, int indexRow)
    {
        if (indexRow < 0)
            return false;
        if (indexRow >= weaponIconColumns[indexColumn].Count)
            return false;
        return true;
    }

    void SetWeaponIconDefault(WeaponIconController icon)
    {
        icon.GetIconBackground().overrideSprite = weaponBackgroundSpriteDefault;
        icon.GetIconBackground().color = weaponBackgroundColorDefault;
        icon.GetIconWeapon().color = weaponIconColorDefault;
    }

    void SetWeaponIconSelected(WeaponIconController icon)
    {
        icon.GetIconBackground().overrideSprite = weaponBackgroundSpriteSelected;
        icon.GetIconBackground().color = weaponBackgroundColorSelected;
        icon.GetIconWeapon().color = weaponIconColorSelected;
    }
}
