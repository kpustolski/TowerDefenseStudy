using UnityEngine;
using System.Collections;
/// <summary>
/// https://github.com/SebLague/Pathfinding
/// https://www.youtube.com/watch?v=dn1XRIaROM4&t=232s
/// </summary>
public class Unit : MonoBehaviour
{

    public Transform target;
    public float speed = 20;
    Vector3[] path;
    int targetIndex;
    //katie code
    public int health = 5;
    public int goldGained = 5;

    public bool _isGizmosOn = false;
	//void Start()
	//{
	//    PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
	//}

	private void Update()
	{
        if(health <= 0 ){
            GameManager.Instance.IncreaseGold(goldGained);
            Destroy(gameObject);
        }
	}
	public void StartPath(){
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        //Debug.Log("Target Pos: " + target.position);
    }

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            //Debug.Log("Path Successful");

            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
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

    public void OnDrawGizmos()
    {
        if (_isGizmosOn)
        {
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
