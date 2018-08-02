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
    public Color32 placeableColor = new Color32(0, 255, 0, 200);
    public Color32 notPlaceableColor = new Color32(255, 0, 0, 200);
    public Color32 defaultColor = new Color32(255, 255, 255, 255);
    public bool isPlaceable = false;

    private Renderer _renderer;


    [Header("Shooting Bullets")]
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    // point at where you want to fire bullet
    public Transform firePoint;
    public int enemyLayer = 10;
    private int layerMask;
    private float _fireCountdown = 0f;

    private Collider[] enemiesInRange;
    private Bullet _bulletScript;
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
        _renderer = GetComponent<Renderer>();
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

            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }

	}
	public void OnDestroy()
	{
        if (GameManager.Instance.activeEnemyList.Count > 0)
        {
            GameManager.Instance.StartMarching();
        }
	}
	public void SetUp(){
        _isPlaced = true;
        Center = transform.position;
        _renderer.material.color = defaultColor;
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
        _bulletScript = bullet.GetComponent<Bullet>();
        //_bulletScript.rangeRadius = rangeRadius;
        _fireCountdown = Time.deltaTime + fireRate;
    }

    private void OnTriggerEnter(Collider col)
	{
        Debug.Log("col.tag: " + col.tag);

        if (!_isPlaced)
        {
            if (col.tag == "Wall" || col.tag == "Building")
            {
                _renderer.material.color = notPlaceableColor;
                isPlaceable = false;
            }
        }
	}
	private void OnTriggerExit(Collider col)
	{
        if (!_isPlaced)
        {
            _renderer.material.color = placeableColor;
            isPlaceable = true;
        }
	}

}
