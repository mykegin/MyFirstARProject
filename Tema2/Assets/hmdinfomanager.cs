using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HMDInfoManager : MonoBehaviour
{
    void Start()
    {
        // Verify for any active XR devices
        if (!XRSettings.isDeviceActive)
        {
            Debug.Log("No Headset plugged");
        }
        else if (XRSettings.isDeviceActive &&
                (XRSettings.loadedDeviceName == "Mock HMD" ||
                 XRSettings.loadedDeviceName == "MockHMDDisplay"))
        {
            Debug.Log("Using Mock HMD");
        }
        else
        {
            Debug.Log("We have a headset: " + XRSettings.loadedDeviceName);
        }
    }

    void Update()
    {
    }
}
