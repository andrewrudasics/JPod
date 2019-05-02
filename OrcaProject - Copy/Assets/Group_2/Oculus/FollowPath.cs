using System.Collections;
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

    public int current, target;
    private float initialVignette;
    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        target = 1;
        Vignette v = ScriptableObject.CreateInstance<Vignette>();
        Vignette VignetteLayer = null;
        post.profile.TryGetSettings(out VignetteLayer);
        initialVignette = VignetteLayer.intensity.value;
        print("iv" + initialVignette);
    }

    // Update is called once per frame
    void Update()
    {
        Vignette VignetteLayer = null;
        post.profile.TryGetSettings(out VignetteLayer);
        float amount = Mathf.Max(0, Input.GetAxis("Vertical"));
        print(initialVignette + amount / 5);
        VignetteLayer.intensity.value = initialVignette + amount / 8;

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
            if (!atTarget() && whale.GetComponent<WhalePath>().hasReached())
            {
                Vector2 xy = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);//SecondaryThumbstick);
                Debug.Log(xy);
                Vector3 move = Vector3.zero;

                move += Input.GetAxis("Horizontal") * transform.right;
                move += Input.GetAxis("Vertical") * transform.forward;

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
                player.position += move.normalized * playerSpeed * Time.deltaTime;
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
            player.position += move.normalized * playerSpeed * Time.deltaTime;
        }

        if (atTarget() && target == 6)
        {
            updateTarget();
        }
        
    }

    public bool atTarget() {
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
