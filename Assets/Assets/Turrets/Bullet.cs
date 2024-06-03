using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 5f;
    public string bulletID;
    public float damageValue;

    public float delay = 10f;

    public float SlowDown = 0;
    

    public void Seek(Transform seekTarget)
    {
        target = seekTarget;
    }
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, delay);
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            //Destroy(gameObject);
            return;
        }
        else
        {
            float step = speed * Time.deltaTime;

            // Move the GameObject towards the target position
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }

        //Vector3 dir = target.position - transform.position;
        /*
        float DistFrame = speed * Time.deltaTime;
        if(dir.magnitude <= DistFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * DistFrame, Space.World);
        */
    }
    /*
    void HitTarget()
    {
        Debug.Log("Boom");
        //Destroy(gameObject);

    }
    */
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Boom");
    }


}
