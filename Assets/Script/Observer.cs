using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    [SerializeField] MainScript mainScript;

    private void OnEnable()
    {
        if (mainScript != null)
        {
            mainScript.OnDataFinishLoaded += ActionForDataFinishLoad;
        }
    }

    private void OnDisable()
    {
        if (mainScript != null)
        {
            mainScript.OnDataFinishLoaded -= ActionForDataFinishLoad;
        }
    }
    private void ActionForDataFinishLoad()
    {
        mainScript.assetBundleRefLink = mainScript.GetWearableByName("test-orang-lari");
        mainScript.Setupltf(mainScript.assetBundleRefLink);
    }
}
