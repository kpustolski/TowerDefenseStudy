using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour{

    [Header("Attributes")]
    public string name;
    public int health = 10;
    //public GameObject prefab;
    public Vector3 worldLocation;
    [SerializeField]
    private float rangeRadius = 1f;
    private bool _isPlaced = false;
    // want gizmos on?
    [SerializeField]
    private bool isGizmosOn = false;

    [Header("Shooting Bullets")]
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    // point at where you want to fire bullet
    public Transform firePoint;
    public int enemyLayer = 10;
    private int layerMask;
    private float _fireCountdown = 0f;

    private Collider[] enemiesInRange;
    private Vector3 _center;
    public Vector3 Center
    {
        get
        {
            return _center;
        }
        set
        {
            this._center = value;
        }
    }

	private void Start()
	{
        layerMask = 1 << enemyLayer;
	}
	private void Update()
	{
        if (_isPlaced)
        {
            enemiesInRange = Physics.OverlapSphere(Center, rangeRadius, layerMask);

            if (enemiesInRange.Length >= 1)
            {
                if (_fireCountdown <= 0f)
                {
                    ShootBullet();
                    _fireCountdown = 1f / fireRate;
                }

                _fireCountdown -= Time.deltaTime;
            }
        }
	}

    public void SetUp(){
        _isPlaced = true;
        Center = transform.position;
    }
    void OnDrawGizmos()
    {
        if (isGizmosOn && _isPlaced)
        {
            //draw range of tower
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Center, rangeRadius);
        }

    }
    private void ShootBullet(){
        
        //Debug.Log("Shoot");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation) as GameObject;
        _fireCountdown = Time.deltaTime + fireRate;
    }
}
