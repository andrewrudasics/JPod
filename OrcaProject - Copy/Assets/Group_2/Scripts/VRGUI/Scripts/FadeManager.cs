using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace VRStandardAssets.Utils
{
    public class FadeManager : MonoBehaviour
    {
        public GameObject whale;
        // Start is called before the first frame update
        void Start()
        {
            transform.Find("SelectionSlider").GetComponent<SelectionSlider>().OnBarFilled += fade;
        }

        public void fade()
        {
            whale.GetComponent<Animator>().SetBool("Start", true);
            GetComponent<UIFader>().StartCoroutine("FadeOut");
        }
    }

}