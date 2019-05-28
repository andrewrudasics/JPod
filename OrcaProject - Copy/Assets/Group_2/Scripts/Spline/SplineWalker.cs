using UnityEngine;

public class SplineWalker : MonoBehaviour
{

    public BezierSpline spline;
    public bool lookForward;
    public float duration;
    public float undampedSpeed;
    public float maxSpeed;
    
    public float adjustmentTime;
    public float startProgress;
    public GameObject dummy;
    public float progress;
    private Vector3 rotationBeforeAdjustment;
    private float speed;

    private void Start()
    {
        transform.position = spline.GetPoint(startProgress);
        transform.forward = spline.GetDirection(0);
        adjustmentTime = 0;
        progress = startProgress;
    }


    public void move()
    {
        UpdateSpeed();

        if (dummy != null)
            dummy.transform.position = spline.GetPoint(progress + getProperTimeProgressive(1.25f));


        float dt = getProperTimeProgressive(Time.deltaTime);
        progress += dt;
        if (progress > 1f)
        {
            progress = 1f;
        }
        Vector3 position = spline.GetPoint(progress);

        if (GetComponent<WhalePath>().StartAdjustment())
        {
            if (adjustmentTime == 0)
            {
                rotationBeforeAdjustment = transform.forward;
            }
            adjustmentTime += Time.deltaTime;
            Vector3 adjustedPosition = Vector3.Lerp(transform.position + transform.forward.normalized  * Time.deltaTime,
                                                    position, 
                                                    Mathf.Pow(adjustmentTime / 2, 3));
            if (adjustmentTime >= 2)
            {
                GetComponent<WhalePath>().AdjustmentDone();
                adjustmentTime = 0;
            }
            GetComponent<Rigidbody>().MovePosition(adjustedPosition);

            if (lookForward)
            {
                Vector3 adjustedRotation = Vector3.Lerp(rotationBeforeAdjustment, spline.GetDirection(progress + 0.002f), adjustmentTime / 2);
               
                transform.forward = adjustedRotation;
            }

        } else
        {
            GetComponent<Rigidbody>().MovePosition(position);
            if (lookForward)
            {
                float angularSpeed = 3 * speed / maxSpeed;
                transform.forward = Vector3.RotateTowards(transform.forward, spline.GetDirection(progress), angularSpeed * Time.deltaTime, 0);
                Vector3 toDummy = dummy.transform.position - transform.position;
                // positive - counterclockwise, negative - clockwise
                float sign = Mathf.Sign(Vector3.Cross(transform.forward, toDummy).y);
                float cos = Vector3.Dot(transform.forward.normalized, toDummy.normalized);
                float prev = GetComponent<Animator>().GetFloat("Direction");
                float next = 0.5f + sign * (1 - cos);
                float v = prev + Mathf.Sign(next - prev) * Mathf.Min(Mathf.Abs(next - prev), 0.5f * Time.deltaTime); 
                GetComponent<Animator>().SetFloat("Direction", v);
            }
        }
        
    }

    
    // Progressive algorithm. Parameter could basically be anything.
    // Might be slightly more expensive.
    public float getProperTimeProgressive(float time)
    {
        // Search step
        float step = 0.001f * Mathf.Sign(time);
        // Keeps track the distance we've covered so far.
        float length = 0;
        // The increment on progress
        float dt = 0;
        Vector3 current = spline.GetPoint(progress);
        float c = 0;
        while (speed * Mathf.Abs(time) - length > 0.01 && c < 1000)
        {
            c++;
            // This could only happen when time < 0
            if (progress + dt + step < 0)
            {
                // dt can be at most as negative as -progress
                return -progress; 
            }
            Vector3 next = spline.GetPoint(progress + dt + step);
            if (length + (next - current).magnitude > speed * Mathf.Abs(time))
            {
                step /= 2;
            } else
            {
                length += (next - current).magnitude;
                current = next;
                dt += step;
            }
        }
        print("speed" + speed);
        return dt;
    }

    public void UpdateSpeed()
    {
        if (GetComponent<Animator>().GetBool("Swim"))
        {
            undampedSpeed = Mathf.Min(maxSpeed, undampedSpeed + 3 * Time.deltaTime);
        }
        
        Vector3 toDummy = dummy.transform.position - transform.position;
        float cos = Vector3.Dot(transform.forward.normalized, toDummy.normalized);
        speed =  undampedSpeed * (cos + 1) / 2;
        //return speed * Mathf.Max(0.5f, (1 - 2 * spline.GetCurvature(progress)));
    }


    // This fuction is called when the whale reached one of the stops.
    // Whales's orientation will be controlled by WhalePath while it stays there.
    public void whaleReached()
    {
        lookForward = false;
    }

    // This function is called when the whale leaves the stop.
    // Orientation is now controlled by SplineWalker.
    public void whaleDeparts()
    {
        lookForward = true;
    }
}