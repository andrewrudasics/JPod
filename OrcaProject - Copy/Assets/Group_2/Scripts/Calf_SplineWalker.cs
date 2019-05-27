using UnityEngine;

public class Calf_SplineWalker : MonoBehaviour
{

    public BezierSpline spline;
    public bool lookForward;
    public float duration;
    public float speed;
    public float startProgress;
    public float progress;
    public GameObject parent;

    private void Start()
    {
        transform.position = spline.GetPoint(startProgress);
        transform.forward = spline.GetDirection(0);
        progress = startProgress;
    }

    public void move()
    {
        float dt = getProperTimeProgressive(Time.deltaTime);
        progress += dt;
        if (progress > 1f)
        {
            progress = 1f;
        }
        Vector3 position = spline.GetPoint(progress);
        print(position);
        transform.position = position;

        /*
        if (lookForward)
        {
            transform.forward = spline.GetDirection(progress + 0.002f);
            float curvature = spline.GetCurvature(progress + 0.005f);
            Vector3 toDummy = dummy.transform.position - transform.position;
            // positive - counterclockwise, negative - clockwise
            float sign = Mathf.Sign(Vector3.Cross(transform.forward, toDummy).y);
            float cos = Vector3.Dot(transform.forward.normalized, toDummy.normalized);
            GetComponent<Animator>().SetFloat("Direction", 0.5f + sign * (1 - cos));
        }*/
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
            Vector3 next = spline.GetPoint(progress + dt + step);
            if (length + (next - current).magnitude > speed * time)
            {
                step /= 2;
            }
            else
            {
                length += (next - current).magnitude;
                current = next;
                dt += step;
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