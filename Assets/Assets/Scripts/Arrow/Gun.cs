using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject enemy { get; set; }
    public float speed;
    public int dame { get; set; }

	void Update ()
    {
        FollowEnemy();
	}

    private void FollowEnemy() 
    {
        if (enemy != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, Time.deltaTime * speed);
            Vector3 dir = enemy.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if(Mathf.Abs( transform.position.x-enemy.transform.position.x)<=0.1f && Mathf.Abs(transform.position.y - enemy.transform.position.y)<=0.1f)
            {
                if (enemy.GetComponent<EnemyController>() != null)
                {
                    enemy.GetComponent<EnemyController>().TakeDamePhysic(dame);
                    Destroy(gameObject);
                }
                else if(enemy.GetComponent<FlyController>() != null)
                {
                    enemy.GetComponent<FlyController>().TakeDamePhysic(dame);
                    Destroy(gameObject);
                }
            }

        }
        else
            Destroy(gameObject);
    }
}
