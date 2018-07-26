using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{

    public float moveSpeed = 20f;
    public int damage = 1;
    //public float rangeRadius = 1f;
    private float _bulletMove;
    private Rigidbody _rb;
    private bool _isActive = true;
    private Transform _buildingPos;

    // Use this for initialization
    //void Start()
    //{
        //chose a random enemy to shoot at
        //_enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
        //if (_enemyArray.Length != 0)
        //{
        //    _randomEnemy = Random.Range(0, _enemyArray.Length);
        //    _enemyPos = _enemyArray[_randomEnemy].transform;
        //}
    //}

    //// Update is called once per frame
    private void Update()
    {
        if (_isActive)
        {
            if (_buildingPos != null)
            {
                _bulletMove = moveSpeed * Time.deltaTime;
                gameObject.transform.position = Vector3.MoveTowards(transform.position, _buildingPos.position, _bulletMove);
            }
            else
            {
                Destroy(gameObject);
            }

        }
    }
    public void SetUpBullet(Building b){
        _buildingPos = b.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Building")
        {
            Building building = other.GetComponent<Building>();
            building.health -= damage;
            _isActive = false;
            Destroy(gameObject);
        }
        //if (other.tag == "Wall" || other.tag == "Floor")
        //{
        //    //Debug.Log("Hit wall");
        //    _isActive = false;
        //    Destroy(gameObject);
        //}
    }

}
