using System.Collections;
using UnityEngine;


public class PhyItem : MonoBehaviour
{

    // Use this for initialization
    private void OnCollisionEnter(Collision collision)
    {
        //EventCenter.Instance.Test.Broadcast();
        EventCenter.Instance.OnCollisionEnter.Broadcast(transform.gameObject,collision);
        //collision.
    }

}
