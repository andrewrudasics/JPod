using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace VRStandardAssets.Utils
{
    public class FadeManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            transform.Find("SelectionSlider").GetComponent<SelectionSlider>().OnBarFilled += fade;
        }

        public void fade()
        {
            GetComponent<UIFader>().StartCoroutine("FadeOut");
        }
    }

}