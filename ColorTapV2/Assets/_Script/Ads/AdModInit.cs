using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;  


public class AdModInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }

}
