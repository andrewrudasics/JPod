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
    private float progress;
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
        progress += getProperTime(Time.deltaTime);
        
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
                GetComponent<Animator>().SetFloat("Direction", 0.5f + curvature);
            }
        }
        
    }

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

    public float GetSpeed()
    {
        return speed;
        //return speed * Mathf.Max(0.5f, (1 - 2 * spline.GetCurvature(progress)));
    }
}