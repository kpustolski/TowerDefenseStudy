using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private int _totalGold;
    public int TotalGold{
        get{
            return _totalGold;
        }
        set{
            this._totalGold = value;
        }
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void DecreaseGold(int amount){
        TotalGold -= amount;
    }
    public void IncreaseGold(int amount){
        TotalGold += amount;
    }
}
