using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatDriver : MonoBehaviour
{
    public Transform engine;
    public float force;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 xyz = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        print(xyz);
        rb.AddForceAtPosition(xyz, engine.position);
    }
}
