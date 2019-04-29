using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class WhalePath : MonoBehaviour
{

    public Animator anim;

    [SerializeField]
    public Transform player, whale, lookTarget;
    public float moveSpeed = 3.0f;
    public float lookSpeed = 2.0f;
    public float triggerDistance, playerTrigger, celebrationTime;
    public List<Transform> waypoints;
    public AudioSource wsrc;


    public int current, target;
    public bool reached, prev, playerReached, prevR, celebrating, end, start;
    public float celebrationStart = 0.0f;

    //temporyrary variable, used to remove turning(transform);
    private bool flag = true;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;
        target = 1;
        reached = false;
        prev = false;
        playerReached = false;
        prevR = false;
        lookTarget = player;
        end = false;
        start = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (start)
        {
            end = target == 6 && atTarget();
            if (target < 6)
            {   
                // to stop turning, replace true with flag
                if (flag)
                {
                    var targetRotation = Quaternion.LookRotation(lookTarget.transform.position - transform.position);
                    print(targetRotation);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
                }
            }
            else
            {
                triggerDistance = 0.05f;
                var targetRotation = Quaternion.LookRotation(new Vector3(player.position.x, 0, player.position.z) - new Vector3(transform.position.x, 0, transform.position.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
            }
            playerReached = player.gameObject.GetComponent<FollowPath>().atTarget();

            if (target > 3)
            {
                playerTrigger = 15;
            }

            if (atTarget())
            {
                if (reached == false)
                {
                    flag = false;
                }
                reached = true;
                lookTarget = player;
                if (reached != prev)
                {
                    anim.SetBool("Follow", true);
                    anim.SetBool("Swim", false);

                    prev = reached;
                }



                if (playerReached && playerReached != prevR)
                {
                    celebrating = true;
                    anim.SetBool("Celebrating", true);
                    celebrationStart = Time.time;
                    prevR = playerReached;
                    wsrc.Play();
                }
                else if (!playerReached)
                {
                    prevR = false;
                }
                if (Time.time - celebrationStart > celebrationTime && celebrating)
                {

                }
            }
            else
            {
                reached = false;
                prev = false;
                lookTarget = (waypoints[target]);
            }

            if (anim.GetBool("Swim"))
            {
                if (target < waypoints.Count)
                {
                    GetComponent<SplineWalker>().move();
                }
            }

            if (end)
            {
                moveToNext();
                end = false;
            }
        }
    }

    private bool atTarget()
    {
        return ((whale.position - waypoints[target].position).magnitude < triggerDistance);
    }

    // This method will be called when:
    //   1) end == true
    //   2) the happy_clap animation reaches the end (called by event).
    private void moveToNext()
    {
        celebrating = false;
        anim.SetBool("Celebrating", false);
        current = target;

        if (target < waypoints.Count - 1)
        {
            anim.SetBool("Swim", true);
            target++;
            anim.SetInteger("Target", target);
        }
        else
        {
            anim.SetBool("Swim", false);
            anim.SetBool("DoSpyHopping", true);
        }
        //anim.SetInteger("Target", target);
        anim.SetBool("Follow", false);
        player.GetComponent<FollowPath>().updateTarget();
    }

    public bool hasReached()
    {
        return reached;
    }

    public void StartMoves()
    {
        start = true;
    }

    public void SetFlag(int i)
    {
        print("setting flat");
        flag = true;
    }
}
