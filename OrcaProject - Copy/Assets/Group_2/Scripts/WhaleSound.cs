using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleSound : MonoBehaviour {
    public AudioClip[] clips;
    public float maxGap;
    public float minGap;

   
	// Use this for initialization
	void Start () {
        StartCoroutine(PlaySound());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(Random.Range(minGap, maxGap));
        while (true)
        {
            print("playering");
            GetComponent<AudioSource>().PlayOneShot(clips[(int)Random.Range(0, clips.Length)]);
            yield return new WaitForSeconds(Random.Range(minGap, maxGap));
        }
    }
}
