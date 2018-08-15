using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://unity3d.com/learn/tutorials/topics/scripting/object-pooling
/// <summary>
/// Generic object pooling script
/// </summary>
public class ObjectPoolScript : MonoBehaviour {

    public static ObjectPoolScript current;
    public GameObject pooledObj;
    public int pooledAmount = 10;
    public bool willGrow = true;

    private List<GameObject> _pooledObjList;

	private void Awake()
	{
        current = this;
	}

	// Use this for initialization
	void Start () 
    {
        _pooledObjList = new List<GameObject>();

        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObj);
            obj.SetActive(false);
            _pooledObjList.Add(obj);
            obj.transform.SetParent(this.transform);
        }
	}

    public GameObject GetPooledObject(){

        // Find an inactive object.
        for (int i = 0; i < _pooledObjList.Count; i++)
        {
            if(!_pooledObjList[i].activeInHierarchy)
            {
                return _pooledObjList[i];
            }
        }

        // If an object isn't found, make one.
        if(willGrow)
        {
            GameObject obj = (GameObject)Instantiate(pooledObj);
            _pooledObjList.Add(obj);
            obj.transform.SetParent(this.transform);
            return obj;
        }

        // Out of objects and cannot creat new ones.
        return null;
    }

    public void DeactivateAllObjects()
    {
        for (int i = 0; i < _pooledObjList.Count; i++)
        {
            if (_pooledObjList[i].activeInHierarchy)
            {
                _pooledObjList[i].SetActive(false);
            }
        }
    }
}
