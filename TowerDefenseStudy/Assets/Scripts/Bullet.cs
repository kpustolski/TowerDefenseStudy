﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float moveSpeed = 20f;
    public int damage = 1;
    //public float rangeRadius = 1f;
    private float _bulletMove;
    private Rigidbody _rb;
    private bool _isActive = true;
    private Transform _enemyPos;
    private GameObject[] _enemyArray;
    private int _randomEnemy;

	// Use this for initialization
	void OnEnable () {
        //chose a random enemy to shoot at
        _enemyArray = GameObject.FindGameObjectsWithTag("Enemy");

        if (_enemyArray.Length != 0)
        {
            _randomEnemy = Random.Range(0, _enemyArray.Length);
            _enemyPos = _enemyArray[_randomEnemy].transform;
        }

        _isActive = true;
	}
	
	//// Update is called once per frame
	void Update () {
        if (_isActive)
        {
            if (_enemyPos != null)
            {
                _bulletMove = moveSpeed * Time.deltaTime;
                gameObject.transform.position = Vector3.MoveTowards(transform.position, _enemyPos.position, _bulletMove);
            }
            else
            {
                //Destroy(gameObject);
                gameObject.SetActive(false);
            }

        }
	}

	private void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Enemy"){
           Enemy enemy = other.GetComponent<Enemy>();
            enemy.health -= damage;
            _isActive = false;
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
        if(other.tag == "Wall" || other.tag == "Floor"){
            //Debug.Log("Hit wall");
            _isActive = false;
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
	}

}
