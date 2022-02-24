using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_pad : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "playerBlock")
        {
            var b = other.GetComponent<Block>();
            var dir = (transform.position - other.transform.position).normalized;
            b.ReachExitMove(dir);
        }


    }


}
