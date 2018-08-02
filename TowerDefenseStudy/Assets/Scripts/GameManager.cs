using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct Level
{
    public string levelName;
    public GameObject enemyPrefab;
    public int spawnNumber;
    public ShopItem unlockedShopItem;
    public bool isLastLevel;
}

public class GameManager : Singleton<GameManager> {

    // State of game. Is the player building buildings or are the enemies attacking?
    public enum GameState {BuildState,AttackState}
    private GameState _gameState;

    [Header("UI Components")][Space(5)]
    public Text goldText;
    public Text timerText;
    public Text livesLeftText;
    public Text gameStateText;
    public Text levelText;


    [Header("Attributes")][Space(5)]
    [SerializeField]
    private int _livesLeft = 10;
    public int LivesLeft
    {
        get
        {
            return _livesLeft;
        }
        set
        {
            _livesLeft = value;
            livesLeftText.text = "Lives: " + _livesLeft.ToString();

        }
    }
    [SerializeField]
    private int _totalGold = 5;
    public int TotalGold{
        get{
            return _totalGold;
        }
        set{
            _totalGold = value;
            goldText.text = "Gold: " + _totalGold.ToString();
        }
    }

    // Enemy spawn and end locations
    public Transform enemySpawnLocation;
    public Transform enemyEndPosition;

    [Header("Lists")][Space(5)]
    public ShopItem[] shopItemsArray;
    public Level[] levelList;
    public List<Enemy> activeEnemyList = new List<Enemy>();

    [Header("Timer")][Space(5)]
    public float initWaitTime = 10f;
    private float _waitTime;

    // Selecting shop items
    private GameObject currentShopItemPrefab;
    private ShopItem _currentShopItemScript;
    private Building _currentBuildingScript;
    private bool _isItemPreviewActive = false;

    //Grid Script
    private Grid _gridScript;

    //For item preview
    private Vector3 _mousePos;
    private float _actualDistance;
    private float _customDistance = 50f;
    private bool _useInitCamDistance = false;

    //temp
    private RtsCamera _camScript;
    private bool _isCamScriptOn;

    //Levels
    private int _currentLevel = 0;
    private Level _currentLvlScript;
    private int _numOfActiveEnemies = 0;

	// Use this for initialization
	void Start () {
        //assign an ID to the shop items and check prices.
        for (int i = 0; i < shopItemsArray.Length; i++){
            shopItemsArray[i].ID = i;
            CheckShopPrices(shopItemsArray[i]);
        }

        _gridScript = GetComponent<Grid>();
        goldText.text = "Gold: " + TotalGold.ToString();
        livesLeftText.text = "Lives Left: " + _livesLeft;
        _camScript = Camera.main.GetComponent<RtsCamera>();
        _waitTime = initWaitTime;

        _gameState = GameState.BuildState;
        gameStateText.text = "Build State";
        levelText.text = "Level: " + (1+_currentLevel).ToString();
        LoadLevel(_currentLevel);

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
            if (Input.GetMouseButtonDown(0) && _currentBuildingScript.isPlaceable)
            {
                _isItemPreviewActive = false;
                _currentBuildingScript.SetUp();
                DecreaseGold(_currentShopItemScript.itemCost);
                CheckShopPrices();
                //currentShopItemPrefab = null;
                //_currentShopItemScript = null;
                _actualDistance = 0f;

            }
        }
        // Cheats
        if(Input.GetKeyDown(KeyCode.R)){
            ResetScene();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(_isCamScriptOn){
                _camScript.enabled = false;
                _isCamScriptOn = false;
            }
            else{
                _camScript.enabled = true;
                _isCamScriptOn = true;
            }
        }

        // As long as we have lives left, continue game.
        if (_livesLeft > 0)
        {
            if (_gameState == GameState.BuildState)
            {
                _waitTime -= Time.deltaTime;
                timerText.text = Mathf.Round(_waitTime).ToString();
                if (_waitTime <= 0)
                {
                    _waitTime = 0;
                    StartMarching();
                    for (int i = 0; i < shopItemsArray.Length; i++)
                    {
                        shopItemsArray[i].Deactivate();
                    }
                    gameStateText.text = "Attack State";
                    _gameState = GameState.AttackState;

                }
            }
            else if (_gameState == GameState.AttackState)
            {
                // if there are no more active enemies on the map.
                if (_numOfActiveEnemies == 0)
                {
                    // Did we hit the last level? If so end game.
                    // Otherwise, continue game.
                    if (_currentLvlScript.isLastLevel)
                    {
                        WinGame();
                    }
                    else
                    {
                        //reset array
                        activeEnemyList.Clear();
                        _currentLevel += 1;
                        LoadLevel(_currentLevel);
                        _waitTime = initWaitTime;
                        for (int i = 0; i < shopItemsArray.Length; i++)
                        {
                            shopItemsArray[i].Activate();
                        }
                        gameStateText.text = "Build State";
                        _gameState = GameState.BuildState;
                    }
                }
            }
        // If there are no more lives left, end game. 
        }else{
            GameOver();
        }

	}

    public void StartMarching(){
        _gridScript.CreateGrid();

        for (int i = 0; i < activeEnemyList.Count; i++){
            activeEnemyList[i].StartPath();
        }
    }

    public void DecreaseGold(int amount){
        TotalGold -= amount;
    }
    // Enemy has died. Increase Gold and decrease number of active enemies
    public void IncreaseGold(int amount){
        TotalGold += amount;
        _numOfActiveEnemies--;
    }

    // for selecting shop item
    public void SelectShopItem(ShopItem item)
    {

        GameObject obj = Instantiate(item.itemPrefab) as GameObject;

        currentShopItemPrefab = obj;
        _currentShopItemScript = item;
        _currentBuildingScript = obj.GetComponent<Building>();

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
    // What if the enemy reaches its destination? Decrease lives.
    public void EnemyReachedBoat()
    {
        _numOfActiveEnemies--;
        LivesLeft--;
    }
    // Attached to reset button "R"
    private void ResetScene(){
        SceneManager.LoadScene(0);
    }

	private void CheckShopPrices(ShopItem item){
        if(item.isUnlocked){
            if(item.itemCost <= TotalGold){
                //Debug.Log("item.name: " + item.name);

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

    private void LoadLevel(int lvl){
        _currentLvlScript = levelList[lvl];
        levelText.text = "Level: "+ (1+lvl).ToString();

        Vector3 randomStartPos;
        float randX;
        float randZ;
        Enemy enemyScript;
        GameObject obj;

        // spawn enemies in a random spot around specified location.
        for (int i = 0; i < _currentLvlScript.spawnNumber; i++){
            randX = Random.Range(-5, 5);
            randZ = Random.Range(-5, 5);

            randomStartPos = new Vector3(enemySpawnLocation.position.x + randX, enemySpawnLocation.position.y, enemySpawnLocation.position.z + randZ);
            obj = Instantiate(_currentLvlScript.enemyPrefab,randomStartPos, _currentLvlScript.enemyPrefab.transform.rotation) as GameObject;
            enemyScript = obj.GetComponent<Enemy>();
            enemyScript.target = enemyEndPosition;

            Debug.Log(enemyScript);
            Debug.Log(activeEnemyList);

            activeEnemyList.Add(enemyScript);
            _numOfActiveEnemies++;
        }
    }
    /// <summary>
    /// Called when the game has ended.
    /// </summary>
    private void GameOver(){
        Debug.Log("Game Over");
    }

    private void WinGame(){
        Debug.Log("You won the game. Victory Screatch!");
    }
}
