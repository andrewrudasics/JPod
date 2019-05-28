using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalfMovement : MonoBehaviour
{
    public BezierSpline spline;
    public GameObject parent;
    public GameObject damped;

    public float blend;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool parentSwim = parent.GetComponent<Animator>().GetBool("Swim");
        if (parentSwim)
        {
            blend = Mathf.Clamp01(blend + Time.deltaTime / 3);

        }
        else
        {
            blend = Mathf.Clamp01(blend - Time.deltaTime / 3);
        }
        transform.position = Vector3.Lerp(damped.transform.position, getPositionOnSpline(), blend);
        lookAtParent();
    }
    
    public Vector3 getPositionOnSpline()
    {
        float dt = parent.GetComponent<SplineWalker>().getProperTimeProgressive(-1);
        print(dt);
        return spline.GetPoint(parent.GetComponent<SplineWalker>().progress + dt);
        
    }
    
    public void lookAtParent()
    {
        float angularSpeed = 2;
        transform.forward = Vector3.RotateTowards(transform.forward, parent.transform.position - transform.position, angularSpeed * Time.deltaTime, 0);
    }
}
