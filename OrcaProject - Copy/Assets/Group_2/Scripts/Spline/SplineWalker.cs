using UnityEngine;

public class SplineWalker : MonoBehaviour
{

    public BezierSpline spline;
    public bool lookForward;
    public float duration;
    public float speed;
    public float adjustmentTime;
    public float startProgress;
    public GameObject dummy;
    public float progress;
    private Vector3 rotationBeforeAdjustment;

    private void Start()
    {
        transform.position = spline.GetPoint(startProgress);
        transform.forward = spline.GetDirection(0);
        adjustmentTime = 0;
        progress = startProgress;
    }
    public void move()
    {
        if (dummy != null)
            dummy.transform.position = spline.GetPoint(progress + getProperTimeProgressive(2));


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
                transform.forward = spline.GetDirection(progress + 0.002f);
                float curvature = spline.GetCurvature(progress + 0.005f);
                Vector3 toDummy = dummy.transform.position - transform.position;
                // positive - counterclockwise, negative - clockwise
                float sign = Mathf.Sign(Vector3.Cross(transform.forward, toDummy).y);
                float cos = Vector3.Dot(transform.forward.normalized, toDummy.normalized);
                GetComponent<Animator>().SetFloat("Direction", 0.5f + sign * (1 - cos));
            }
        }
        
    }

    // Doesn't support variable time.
    private float getProperTime(float delta)
    {
        float checker = 0;
        float prev = 0;
        float deltaTime = delta;
        float dt = delta;
        float s = GetSpeed();
        while (Mathf.Abs((transform.position - spline.GetPoint(progress + dt)).magnitude - deltaTime * s) > 0.01f)
        {
            float p = Mathf.Abs((transform.position - spline.GetPoint(progress + dt)).magnitude - deltaTime * s);
            checker += Time.deltaTime;
            if (checker > 3)
            {
                print("x");
                break;
            }
            if ((transform.position - spline.GetPoint(progress + dt)).magnitude > deltaTime * s)
            {
                dt = (dt + prev) / 2;
            } else
            {
                prev = dt;
                dt = dt * 2;
            }
            
        }
        return dt;
    }

    // Progressive algorithm. Parameter could basically be anything.
    // Might be slightly more expensive.
    private float getProperTimeProgressive(float time)
    {
        // Search step
        float step = 0.001f;
        // Keeps track the distance we've covered so far.
        float length = 0;
        // Intended speed
        float speed = GetSpeed();
        // The increment on progress
        float dt = 0;
        Vector3 current = spline.GetPoint(progress);
        float c = 0;
        while (speed * time - length > 0.01 && c < 1000)
        {
            c++;
            Vector3 next = spline.GetPoint(progress + dt + step);
            if (length + (next - current).magnitude > speed * time)
            {
                step /= 2;
            } else
            {
                length += (next - current).magnitude;
                current = next;
                dt += step;
            }
        }
        return dt;
    }

    private float getProperTimeAccumulate(float time)
    {
        int itrs = (int)Mathf.Round(time / Time.deltaTime);
        itrs = 100;
        // Save progress. Restores after calculation is done.
        float savedProgress = progress;
        for (int i = 0; i < itrs; i++)
        {
            float t = getProperTime(Time.deltaTime);
            progress += t;
        }
        float dt = progress - savedProgress;
        progress = savedProgress;
        return dt;
    }

    public float GetSpeed()
    {
        return speed;
        //return speed * Mathf.Max(0.5f, (1 - 2 * spline.GetCurvature(progress)));
    }
}