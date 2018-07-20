using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Level {

    public string Name;
    public int iD;
    public Unit unitType;
}
public class GameManager : MonoBehaviour {

    public Text goldText;

    [SerializeField]
    private int _totalGold = 5;
    public int TotalGold{
        get{
            return _totalGold;
        }
        set{
            this._totalGold = value;
            goldText.text = "Gold: " + TotalGold.ToString();
        }
    }
    public ShopItem[] shopItemsArray;
    public Level[] levelList;

    // Selecting shop item
    private GameObject currentShopItemPrefab;
    private ShopItem _currentShopItemScript;
    private bool _isItemPreviewActive = false;

    //Grid Script
    private Grid _gridScript;

    //For item preview
    private Vector3 _mousePos;
    private float _actualDistance;
    private float _customDistance = 50f;
    private bool _useInitCamDistance = false;

    //temp
    public Unit[] units;

	// Use this for initialization
	void Start () {
        //assign an ID to the shop items
        for (int i = 0; i < shopItemsArray.Length; i++){
            shopItemsArray[i].ID = i;
            CheckShopPrices(shopItemsArray[i]);
        }
        _gridScript = GetComponent<Grid>();
        goldText.text = "Gold: " + TotalGold.ToString();
	}
	
	// Update is called once per frame
	void Update () {

        // check to see what towers are affordable
        //assign an ID to the shop items
        for (int i = 0; i < shopItemsArray.Length; i++)
        {
            shopItemsArray[i].ID = i;
        }

        if(_isItemPreviewActive){

            _mousePos = Input.mousePosition;
            _mousePos.z = _actualDistance;
            currentShopItemPrefab.transform.position = Camera.main.ScreenToWorldPoint(_mousePos);
            if (Input.GetMouseButtonDown(0))
            {
                _isItemPreviewActive = false;
                DecreaseGold(_currentShopItemScript.itemCost);
                CheckShopPrices();
                //currentShopItemPrefab = null;
                //_currentShopItemScript = null;
                _actualDistance = 0f;

            }
        }
	}

    public void StartMarching(){
        _gridScript.CreateGrid();
        for (int i = 0; i < units.Length; i++){
            units[i].StartPath();
        }
    }

    public void DecreaseGold(int amount){
        TotalGold -= amount;
    }
    public void IncreaseGold(int amount){
        TotalGold += amount;
    }

    // for selecting shop item
    public void SelectShopItem(ShopItem item)
    {

        GameObject obj = Instantiate(item.itemPrefab) as GameObject;
        currentShopItemPrefab = obj;
        _currentShopItemScript = item;

        if (_useInitCamDistance)
        {
            _actualDistance = (currentShopItemPrefab.transform.position - Camera.main.transform.position).magnitude;
        }
        else
        {
            _actualDistance = _customDistance;
        }
        _isItemPreviewActive = true;

    }

    private void CheckShopPrices(ShopItem item){
        if(item.isUnlocked){
            if(item.itemCost <= TotalGold){
                item.Activate();
            }
            else{
                item.Deactivate();
            }
        }
    }
    private void CheckShopPrices()
    {
        for (int i = 0; i < shopItemsArray.Length; i++)
        {
            ShopItem item = shopItemsArray[i];
            if (item.isUnlocked)
            {
                if (item.itemCost <= TotalGold)
                {
                    item.Activate();
                }
                else
                {
                    item.Deactivate();
                }
            }
        }
    }

}
