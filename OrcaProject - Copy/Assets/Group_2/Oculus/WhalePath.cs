using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhalePath : MonoBehaviour {

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

    // Start is called before the first frame update
    void Start() {
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
    void Update() {

        if (start)
        {
            end = target == 5 && atTarget();
            if (target < 6)
            {
                var targetRotation = Quaternion.LookRotation(lookTarget.transform.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
            }
            else
            {
                triggerDistance = 0.05f;
                var targetRotation = Quaternion.LookRotation(new Vector3(player.position.x, 0, player.position.z) - new Vector3(transform.position.x, 0, transform.position.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
            }
            playerReached = (player.position - whale.position).magnitude < playerTrigger;


            if (target > 3)
            {
                playerTrigger = 15;
            }

            if (atTarget())
            {
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
                    Vector3 move = waypoints[target].position - whale.position;
                    whale.position += move.normalized * moveSpeed * Time.deltaTime;
                }
            }

            if (end)
            {
                moveToNext();
                end = false;
            }
        }
    }

    private bool atTarget() {
        return ((whale.position - waypoints[target].position).magnitude < triggerDistance);
    }

    public void moveToNext()
    {
        celebrating = false;
        anim.SetBool("Celebrating", false);
        current = target;
      
        if (target < waypoints.Count)
        {
            target++;
            anim.SetInteger("Target", target);
        }
        //anim.SetInteger("Target", target);
        anim.SetBool("Swim", true);
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
}
