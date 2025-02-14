﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FollowPath : MonoBehaviour
{
    [SerializeField]
    public Transform player, whale;
    public float playerSpeed = 3.0f;
    public float lookSpeed = 0.5f;
    public float triggerDistance;
    public List<Transform> waypoints;
    public PostProcessVolume post;

    private int current, target;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        target = 1;
        Vignette v = ScriptableObject.CreateInstance<Vignette>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (target < 6)
        {
            var targetRotation = Quaternion.LookRotation(waypoints[target].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        } else
        {
            var targetRotation = Quaternion.LookRotation(new Vector3(0,0,0));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }*/
        if (target < 5)
        {
            if (!atTarget() && whale.GetComponent<WhalePath>().hasReached())
            {
                Vector2 xy = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);//SecondaryThumbstick);
                Debug.Log(xy);
                Vector3 move = Vector3.zero;
                if (Input.GetKey(KeyCode.UpArrow) || (Mathf.Sign(xy.y) > 0 && xy.y != 0.0f))
                {
                    move = waypoints[target].position - player.position;
                }
                else if (Input.GetKey(KeyCode.DownArrow) || (Mathf.Sign(xy.y) < 0 && xy.y != 0.0f))
                {
                    move = waypoints[current].position - player.position;
                }
               /* if (move.magnitude > 0) {
                    
                    v.enabled.Override(true);
                    v.intensity.Override(Mathf.Clamp(v.intensity + 0.05f, 0, 0.5f));
                } else {
                    v.enabled.Override(true);
                    v.intensity.Override(Mathf.Clamp(v.intensity - 0.05f, 0, 0.5f));
                }*/
                player.position += move.normalized * playerSpeed * Time.deltaTime;
            }
        } else
        {
            Vector2 xy = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);//SecondaryThumbstick);
            Debug.Log(xy);
            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow) || (Mathf.Sign(xy.y) > 0 && xy.y != 0.0f))
            {
                move = waypoints[6].position - player.position;
            }
            else if (Input.GetKey(KeyCode.DownArrow) || (Mathf.Sign(xy.y) < 0 && xy.y != 0.0f))
            {
                move = waypoints[5].position - player.position;
            }
            player.position += move.normalized * playerSpeed * Time.deltaTime;
        }

        if (atTarget() && target == 5)
        {
            updateTarget();
        }
        
    }

    private bool atTarget() {
        return ((player.position - waypoints[target].position).magnitude < triggerDistance);
    }

    public void updateTarget()
    {
        current = target;
        if (target < waypoints.Count - 1)
        {
            target++;
        }
        else if (target == waypoints.Count - 1)
        {
            target--;
        }
    }
    
}
