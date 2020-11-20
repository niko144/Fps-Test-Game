using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weaponselector : MonoBehaviour 
{
	public Transform[] Weapons;

	public bool[] HaveWeapons;
	public int[] WeaponsAmmo;
	public int grenade;
	public float lastGrenade;
	public float selectInterval = 2f;
	private float nextselect = 2f;
    public int currentWeapon = 0;
    private int previousWeapon = 0;
	public  AudioClip switchsound;
	public AudioSource myaudioSource;
	public bool canswitch;

	public bool hideweapons = false;
	bool oldhideweapons = false;
	public Text ammotext;
	public Text grenadetext;
	public int currentammo = 10;
    public GameObject weaponIcon;
    private Image weaponimage;
    public GameObject dynamiteIcon;
   

    public GameObject AIM;

	int oldAmmo=-1;
	int oldTotalAmmo =-1;


	public RectTransform mycanvas;
	private bool inventoryOn;

	
	public AudioSource myaudio;
	public Color selectColor;
	public Color deselectColor;
	public Color selectedrowColor;

	

    [SerializeField]
    WeaponSelectionBarUI weaponSelectionBar;

    [SerializeField]
	Sprite[]weaponsprites;

	//GameObject weaponspriteprefab;

    [SerializeField]
	int currentColumn ;

    [SerializeField]
	int currentrow ;

    int[][] weaponIndexMap = null;

    [SerializeField]
    int[] weaponIndexColumn1 =null;

    [SerializeField]
    int[] weaponIndexColumn2 = null;

    [SerializeField]
    int[] weaponIndexColumn3 = null;

    [SerializeField]
    int[] weaponIndexColumn4 = null;

    [SerializeField]
    int[] weaponIndexColumn5 = null;

    [SerializeField]
    int[] weaponIndexColumn6 = null;

    [SerializeField]
    int[] weaponIndexColumn7 = null;

    [SerializeField]
    int[] weaponIndexColumn8 = null;

    [SerializeField]
    int[] weaponIndexColumn9 = null;

    [SerializeField]
    int[] weaponIndexColumn0 = null;


    playercontroller playercontrol;

    int WeaponIndex
    {
        get
        {
            if (!validColumn(currentColumn) || !validRowIdex(currentColumn, currentrow))
            {
                print("Invalid column or row");
                return 0;
            }
            else
            {
                return weaponIndexMap[currentColumn][currentrow];
            }
        }
    }

	void Awake()
	{
		playercontrol = GetComponent<playercontroller>();

		for(int i = 0; i < Weapons.Length; i++){
			Weapons[i].gameObject.SetActive(false);
		}

		
		currentColumn = 0;
		canswitch = true;
		HaveWeapons=new bool[Weapons.Length];
		HaveWeapons[0]=true; // Knife
		if(WeaponsAmmo.Length==0) WeaponsAmmo=new int[Weapons.Length];

		
	}

	void Start()
	{
        AIM.gameObject.SetActive(false);
        StartCoroutine (selectWeapon (0));
		inventoryOn = false;
		//oldinventoryOn = inventoryOn;
        currentColumn = 0;
		currentrow = 0;
        
        weaponimage = weaponIcon.GetComponent<Image>();
        

        weaponIndexMap = new int[][]
        {
            weaponIndexColumn1,
            weaponIndexColumn2,
            weaponIndexColumn3,
            weaponIndexColumn4,
            weaponIndexColumn5,
            weaponIndexColumn6,
            weaponIndexColumn7,
            weaponIndexColumn8,
            weaponIndexColumn9,
            weaponIndexColumn0
        };

        weaponSelectionBar.Initialize();

        for (int column = 0; column < weaponIndexMap.Length; column++)
        {
            for (int row = 0; row < weaponIndexMap[column].Length; row++)
            {
                weaponSelectionBar.AddWeaponToColumn(column, weaponsprites[weaponIndexMap[column][row]]);
            }
        }

        weaponSelectionBar.SelectIcon(0, 0);

        weaponSelectionBar.gameObject.SetActive(false);

        
    }
	void Update()
	{
		
		bool changeAmmoText=false;
		if(oldAmmo!=currentammo){
			oldAmmo=currentammo;
			changeAmmoText=true;
		}
		if(oldTotalAmmo!=WeaponsAmmo[WeaponIndex]){
			oldTotalAmmo=currentammo;
			changeAmmoText=true;
		}
		if(changeAmmoText){
			if(currentColumn ==0)
            {
                ammotext.text = "";
                
            }
				
			else
            {
                ammotext.text = (currentammo + " / " + WeaponsAmmo[WeaponIndex]);
            }
				
		}
		grenadetext.text = grenade.ToString();

		if (Input.GetButtonDown("Inventory") && canswitch && !playercontrol.climbladder)
		{
			//inventoryOn = !inventoryOn;

            if (inventoryOn)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
		}
        
		if (inventoryOn)
		{
			
            ProcessInventoryInput();
            AIM.gameObject.SetActive(false);
           
        }
		else
		{
			
            //oldinventoryOn = inventoryOn;
            ProcessInventoryClosedInput();
        }
		if (hideweapons!= oldhideweapons)
		{
			if(hideweapons)
			{
				StartCoroutine(hidecurrentWeapon(WeaponIndex));
			}
			else
			{
				StartCoroutine(unhidecurrentWeapon(WeaponIndex));
			}
		}


        //oldinventoryOn = inventoryOn;
    }
   
    public void playSwithSound()
    {
		myaudioSource.PlayOneShot(switchsound, 1);
	}

    

    public void InitCurrentWeaponAmmo(int amountAmmo)
    {
		if(WeaponsAmmo.Length==0)WeaponsAmmo=new int[Weapons.Length];
		//Debug.Log("currentWeapon="+currentWeapon);
		WeaponsAmmo[WeaponIndex] +=amountAmmo;
       
    }
    public void UpdateCurrentWeaponAmmo(int amountAmmo)
    {
        WeaponsAmmo[WeaponIndex] = amountAmmo;
       
    }
    IEnumerator hidecurrentWeapon(int index)
    {
        ammotext.gameObject.SetActive(false);
        weaponIcon.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        weaponIcon.SetActive(false);

        Weapons[index].gameObject.BroadcastMessage("doRetract", SendMessageOptions.DontRequireReceiver);
        yield return new WaitForSeconds(0.15f);
        Weapons[index].gameObject.SetActive(false);
        oldhideweapons = hideweapons;



    }

    IEnumerator unhidecurrentWeapon(int index)
    {
        yield return new WaitForSeconds(0.15f);
        weaponimage.sprite = weaponsprites[index];
        weaponIcon.SetActive(true);
        ammotext.gameObject.SetActive(true);
        Weapons[index].gameObject.SetActive(true);
        Weapons[index].gameObject.BroadcastMessage("doNormal", SendMessageOptions.DontRequireReceiver);
        oldhideweapons = hideweapons;
    }

    IEnumerator selectWeapon(int index)
    {
        ammotext.gameObject.SetActive(false);
        weaponIcon.SetActive(false);
        Weapons[previousWeapon].gameObject.BroadcastMessage("doRetract", SendMessageOptions.DontRequireReceiver);

        yield return new WaitForSeconds(0.2f);
        Weapons[previousWeapon].gameObject.SetActive(false);
        Weapons[index].gameObject.SetActive(true);
        weaponimage.sprite = weaponsprites[index];
        weaponIcon.SetActive(true);
        ammotext.gameObject.SetActive(true);
        Weapons[index].gameObject.BroadcastMessage("doNormal", SendMessageOptions.DontRequireReceiver);
        
    }

    public void showAIM(bool show){
		if(show)
			AIM.SetActive(true);
		else
			AIM.SetActive(false);
	}

    void ProcessInventoryInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            CloseInventory();
            return;
        }

        if (canswitch && Time.time > nextselect && !hideweapons)
        {
           
           if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (weaponIndexMap.Length == 0 || weaponIndexMap[0].Length == 0)
                    return;

                PressedColumnButton(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (weaponIndexMap.Length < 2 || weaponIndexMap[1].Length == 0)
                    return;

                PressedColumnButton(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (weaponIndexMap.Length < 3 || weaponIndexMap[2].Length == 0)
                    return;

                PressedColumnButton(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (weaponIndexMap.Length < 4 || weaponIndexMap[3].Length == 0)
                    return;

                PressedColumnButton(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (weaponIndexMap.Length < 5 || weaponIndexMap[4].Length == 0)
                    return;

                PressedColumnButton(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (weaponIndexMap.Length < 6 || weaponIndexMap[5].Length == 0)
                    return;

                PressedColumnButton(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (weaponIndexMap.Length < 7 || weaponIndexMap[6].Length == 0)
                    return;

                PressedColumnButton(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (weaponIndexMap.Length < 8 || weaponIndexMap[7].Length == 0)
                    return;

                PressedColumnButton(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (weaponIndexMap.Length < 9 || weaponIndexMap[8].Length == 0)
                    return;

                PressedColumnButton(8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (weaponIndexMap.Length < 10 || weaponIndexMap[9].Length == 0)
                    return;

                PressedColumnButton(9);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                ScrollDecrement();
                UpdateSelectionUI();
                nextselect = Time.time + 0.1f;
                playSwithSound();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                ScrollIncrement();
                UpdateSelectionUI();
                nextselect = Time.time + 0.1f;
                playSwithSound();
            }
        }
    }

    void ProcessInventoryClosedInput()
    {
        if (canswitch && Time.time > nextselect && !hideweapons)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (weaponIndexMap.Length == 0 || weaponIndexMap[0].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 0)
                {
                    currentColumn = 0;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (weaponIndexMap.Length < 2 || weaponIndexMap[1].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 1)
                {
                    currentColumn = 1;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (weaponIndexMap.Length < 3 || weaponIndexMap[2].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 2)
                {
                    currentColumn = 2;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (weaponIndexMap.Length < 4 || weaponIndexMap[3].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 3)
                {
                    currentColumn = 3;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (weaponIndexMap.Length < 5 || weaponIndexMap[4].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 4)
                {
                    currentColumn = 4;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (weaponIndexMap.Length < 6 || weaponIndexMap[5].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 5)
                {
                    currentColumn = 5;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (weaponIndexMap.Length < 7 || weaponIndexMap[6].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 6)
                {
                    currentColumn = 6;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (weaponIndexMap.Length < 8 || weaponIndexMap[7].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 7)
                {
                    currentColumn = 7;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (weaponIndexMap.Length < 9 || weaponIndexMap[8].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 8)
                {
                    currentColumn = 8;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (weaponIndexMap.Length < 10 || weaponIndexMap[9].Length == 0)
                    return;

                OpenInventory();
                if (currentColumn != 9)
                {
                    currentColumn = 9;
                    currentrow = 0;
                }
                UpdateSelectionUI();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                OpenInventory();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                OpenInventory();
            }
        }
    }

    void IncrementWeaponColumn()
    {
        do
        {
            currentColumn++;

            if (currentColumn >= weaponIndexMap.Length)
            {
                currentColumn = 0;
            }
        } while (weaponIndexMap[currentColumn].Length == 0);

        if (currentrow >= weaponIndexMap[currentColumn].Length)
        {
            currentrow = weaponIndexMap[currentColumn].Length - 1;
        }
    }

    void DecrementWeaponColumn()
    {
        do
        {
            currentColumn--;

            if (currentColumn < 0)
            {
                currentColumn = weaponIndexMap.Length - 1;
            }
        } while (weaponIndexMap[currentColumn].Length == 0);

        if (currentrow >= weaponIndexMap[currentColumn].Length)
        {
            currentrow = weaponIndexMap[currentColumn].Length - 1;
        }
    }

    void IncrementWeaponRow()
    {
        currentrow++;

        if (currentrow >= weaponIndexMap[currentColumn].Length)
        {
            currentrow = 0;
        }
    }

    void DecrementWeaponRow()
    {
        currentrow--;

        if (currentrow < 0)
        {
            currentrow = weaponIndexMap[currentColumn].Length - 1;
        }
    }

    void ScrollIncrement()
    {
        currentrow++;

        if (currentrow >= weaponIndexMap[currentColumn].Length)
        {
            currentrow = 0;

            do
            {
                currentColumn++;

                if (currentColumn >= weaponIndexMap.Length)
                {
                    currentColumn = 0;
                }
            } while (weaponIndexMap[currentColumn].Length == 0);
        }
    }

    void ScrollDecrement()
    {
        currentrow--;

        if (currentrow < 0)
        {
            do
            {
                currentColumn--;

                if (currentColumn < 0)
                {
                    currentColumn = weaponIndexMap.Length - 1;
                }
            } while (weaponIndexMap[currentColumn].Length == 0);

            currentrow = weaponIndexMap[currentColumn].Length - 1;
        }
    }

    void OpenInventory()
    {
        StartCoroutine(hidecurrentWeapon(WeaponIndex));
        weaponSelectionBar.gameObject.SetActive(true);
        inventoryOn = true;
        playSwithSound();
    }

    void CloseInventory()
    {
        StartCoroutine(selectWeapon(WeaponIndex));
        weaponSelectionBar.gameObject.SetActive(false);
        inventoryOn = false;
        playSwithSound();
    }

    void PressedColumnButton(int columnIndex)
    {
        if (currentColumn != columnIndex)
        {
            currentColumn = columnIndex;
            currentrow = 0;
        }
        else
        {
            IncrementWeaponRow();
        }

        UpdateSelectionUI();
        playSwithSound();
    }

    void UpdateSelectionUI()
    {
        weaponSelectionBar.SelectIcon(currentColumn, currentrow);
    }

    bool validColumn(int index)
    {
        if (index < 0)
        {
            print("Column index less than 0");
            return false;
        }
        if (index >= weaponIndexMap.Length)
        {
            print("Column index too large");
            return false;
        }
        return true;
    }

    bool validRowIdex(int indexColumn, int indexRow)
    {
        if (indexRow < 0)
        {
            print("Row index less than 0");
            return false;
        }
        if (indexRow >= weaponIndexMap[indexColumn].Length)
        {
            print("Row index too large");
            return false;
        }
        return true;
    }
    public void disable()
    {
       
        grenadetext.gameObject.SetActive(false);
        ammotext.gameObject.SetActive(false);
       
        weaponSelectionBar.gameObject.SetActive(false);
        AIM.SetActive(false);
        weaponIcon.SetActive(false);
        dynamiteIcon.SetActive(false);
        
    }
}



	
