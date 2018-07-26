using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// https://github.com/SebLague/Pathfinding
/// https://www.youtube.com/watch?v=dn1XRIaROM4&t=232s
/// </summary>
public class Enemy : MonoBehaviour {

    [Header("Attributes")]
    public Transform target;
    public float speed = 20;
    private float _enemyMove;
    Vector3[] path;
    int targetIndex;
    //katie code
    public int health = 5;
    public int goldGained = 5;
    public bool _isGizmosOn = false;

    [Header("Shooting Bullets")]
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    // point at where you want to fire bullet
    public Transform firePoint;
    public float rangeRadius = 5f;
    public float distanceFromTower = 5f;
    //public int enemyLayer = 10;
    //private int layerMask;
    private float _fireCountdown = 0f;
    private Collider[] buildingsInRange;
    private EnemyBullet _bulletScript;
    private Vector3 _center;
    private bool _isAttackingBuilding = false;
    private GameObject[] _buildingArray;
    private GameObject _randomBuilding;
    private Building _attackBuildingScript;
    private bool _isRandomBuildingPicked = false;
    private int _randomNum;
    private Vector3 pausePosition;


    private void Start()
    {
        //PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        //_center = transform.position;
    }

    private void Update()
    {

        if (health <= 0)
        {
            GameManager.Instance.IncreaseGold(goldGained);
            Destroy(gameObject);
        }
        if (_isAttackingBuilding)
        {
            if (!_isRandomBuildingPicked)
            {
                _buildingArray = GameObject.FindGameObjectsWithTag("Building");
                if (_buildingArray.Length != 0)
                {
                    _randomNum = Random.Range(0, _buildingArray.Length);
                    _randomBuilding = _buildingArray[_randomNum];
                    _attackBuildingScript = _randomBuilding.GetComponent<Building>();
                    pausePosition = new Vector3(_randomBuilding.transform.position.x, _randomBuilding.transform.position.y, _randomBuilding.transform.position.z + distanceFromTower);
                    _isRandomBuildingPicked = true;

                }
            }
            buildingsInRange = Physics.OverlapSphere(transform.position, rangeRadius);
            _enemyMove = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, pausePosition, _enemyMove);

            if (buildingsInRange.Length >= 1 && _randomBuilding != null)
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
    public void StartPath()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        //Debug.Log("Target Pos: " + target.position);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            _isAttackingBuilding = false;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else{
            _isAttackingBuilding = true;
            _isRandomBuildingPicked = false;
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    // Code where unit ends journey
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            yield return null;

        }
    }

    private void ShootBullet()
    {

        //Debug.Log("Shoot");
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation) as GameObject;
        _bulletScript = bullet.GetComponent<EnemyBullet>();
        _bulletScript.SetUpBullet(_attackBuildingScript);
        //_bulletScript.rangeRadius = rangeRadius;
        _fireCountdown = Time.deltaTime + fireRate;
    }

    public void OnDrawGizmos()
    {
        if (_isGizmosOn)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rangeRadius);

            if (path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(path[i], Vector3.one);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
        }
    }
}
