using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Boarder : MonoBehaviour
{
    // How far the player can get into the collider before the movement is stopped, 
    public float penetrationDistance;

    private BoxCollider bc;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        // The x-axis value of the box plane whose normal is the same as the local x axis
        float xPlane = bc.center.x + bc.size.x;
        print(other);
        if (other.CompareTag("Player"))
        {
            
            Vector3 otherInLocal = transform.InverseTransformPoint(other.transform.position);
            Vector3 forwardInLocal = transform.InverseTransformDirection(other.transform.forward);
            Vector3

            print(otherInLocal.x);
            float ratio = Mathf.Clamp01(1 - Mathf.Abs(otherInLocal.x - (bc.size.x) / 2) / penetrationDistance);
            
            other.gameObject.GetComponent<FollowPath>().SetPlayerSpeed(ratio);
        }
    }
}
