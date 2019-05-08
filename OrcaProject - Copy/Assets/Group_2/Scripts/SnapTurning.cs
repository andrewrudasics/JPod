using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapTurning : MonoBehaviour
{
    private bool prev;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 xy = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        if (!prev && (xy.y == 1 || xy.y == -1))
        {
            prev = true;
            StartCoroutine(Snap(0.2f, 30f));   
        }
        if (!(xy.y == 1 || xy.y == -1))
        {
            prev = false;
        }
    }

    IEnumerator Snap(float duration, float degree)
    {
        float t = 0;
        while (t < duration)
        {
            transform.Rotate(new Vector3(0, degree * Time.deltaTime / duration, 0), Space.Self);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
