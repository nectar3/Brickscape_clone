using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    private void Start()
    {
        Invoke("ShotRay", 0.5f);
    }


    void ShotRay()
    {
        Debug.Log("shot = " );
        //if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 20f))
        //{
        //    Debug.Log("hit.name = " + hit.collider.gameObject.name);
        //}


        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit2, 10f))
        {
            Debug.Log("hit2.name = " + hit2.collider.gameObject.name);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(transform.position, Vector3.up));
    }

}
