using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;

public class FollowPath : MonoBehaviour
{
    [SerializeField]
    public Transform player, whale;
    // The numerical speed
    public float speed = 3.0f;
    public float lookSpeed = 0.5f;
    public float triggerDistance;
    public List<Transform> waypoints;
    public PostProcessVolume post;
    public GameObject VRCam;
    public int current, target;
    private float initialVignette;
    // The reference speed used to calculated movement.
    private Transform refTransform;
    // Start is called before the first frame update
    void Start()
    {
        // no movement on y axis.
        //refSpeed = new Vector3(1, 0, 1);
        var device = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if(device != null)
        {
            //device.TryGetFeatureValue()
        }

        current = 0;
        target = 1;
        Vignette v = ScriptableObject.CreateInstance<Vignette>();
        Vignette VignetteLayer = null;
        post.profile.TryGetSettings(out VignetteLayer);
        initialVignette = VignetteLayer.intensity.value;
    
    }

    // Update is called once per frame
    void Update()
    {
        Vignette VignetteLayer = null;
        post.profile.TryGetSettings(out VignetteLayer);
        float amount = Mathf.Max(0, OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y);
        VignetteLayer.intensity.value = initialVignette + amount / 2;

        /*if (target < 6)
        {
            var targetRotation = Quaternion.LookRotation(waypoints[target].position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        } else
        {
            var targetRotation = Quaternion.LookRotation(new Vector3(0,0,0));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }*/

        // used to be "if target < 5".
        if (true)
        {
            if (true || target == 6 || (!atTarget() && whale.GetComponent<WhalePath>().hasReached()))
            {
                Vector2 xy = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);//SecondaryThumbstick);
               // Debug.Log(xy);
                Vector3 move = Vector3.zero;

                xy += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                //move += xy.x * transform.Find("OVRCameraRig").right;

                //move += xy.y * Vector3.Scale(refSpeed, transform.forward);

                move += xy.y * VRCam.transform.forward;

                //print(transform.Find("OVRCameraRig").Find("CenterEyeAnchor").forward);
                //Debug.DrawRay(transform.position, transform.Find("OVRCameraRig").forward * 1000, Color.red);

                /*if (Input.GetKey(KeyCode.UpArrow) || (Mathf.Sign(xy.y) > 0 && xy.y != 0.0f))
                {
                    move = waypoints[target].position - player.position;
                }
                else if (Input.GetKey(KeyCode.DownArrow) || (Mathf.Sign(xy.y) < 0 && xy.y != 0.0f))
                {
                    move = waypoints[current].position - player.position;
                } */
                /* if (move.magnitude > 0) {

                     v.enabled.Override(true);
                     v.intensity.Override(Mathf.Clamp(v.intensity + 0.05f, 0, 0.5f));
                 } else {
                     v.enabled.Override(true);
                     v.intensity.Override(Mathf.Clamp(v.intensity - 0.05f, 0, 0.5f));
                 }*/
                player.position += move.normalized * speed * Time.deltaTime;
            }
        } else
        {
            Vector2 xy = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick); //SecondaryThumbstick);
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
            player.position += move.normalized * speed * Time.deltaTime;
        }

        if (atTarget() && target == 6)
        {
            updateTarget();
        }
     
        

    }

    public bool atTarget() {
        if (target == 6)
        {
            return false;
        }
        return ((transform.position - whale.position).magnitude < triggerDistance);
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
    /*
    // Sets the player speed based on the given ratio and transform
    public void SetPlayerSpeed(float ratio, Transform t)
    {
        Vector3 dir = Vector3.Scale(refSpeed, transform.forward).normalized;
        Vector3 localDir = t.InverseTransformDirection(dir);
        localDir.x *= ratio;        
    }
    */
}
