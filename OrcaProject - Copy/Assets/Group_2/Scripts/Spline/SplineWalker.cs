using UnityEngine;

public class SplineWalker : MonoBehaviour
{

    public BezierSpline spline;
    public bool lookForward;
    public float duration;
    public float speed;
    private float progress;

    private void Start()
    {
        transform.position = spline.GetPoint(0);
        transform.forward = spline.GetDirection(0);
    }
    public void move()
    {
        
        progress += getProperTime();
        
        if (progress > 1f)
        {
            progress = 1f;
        }
        Vector3 position = spline.GetPoint(progress);
        transform.position = position;
        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }

    private float getProperTime()
    {
        float checker = 0;
        float prev = 0;
        float dt = Time.deltaTime;
        while (Mathf.Abs((transform.position - spline.GetPoint(progress + dt)).magnitude - Time.deltaTime * speed) > 0.01f)
        {
            float p = Mathf.Abs((transform.position - spline.GetPoint(progress + dt)).magnitude - Time.deltaTime * speed);
            checker += Time.deltaTime;
            if (checker > 3)
            {
                print("x");
                break;
            }
            if ((transform.position - spline.GetPoint(progress + dt)).magnitude > Time.deltaTime * speed)
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
}