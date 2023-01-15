using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast;
using TMPro;

public class MainScript : MonoBehaviour
{
    [Serializable]
    public class WearableData
    {
        public List<Wearable> data;
    }

    private WearableData wearableData;
    [SerializeField] private GameObject parentForGLTFInstance;
    [SerializeField] private GltfAsset prefabGLTF;
    [SerializeField] private Vector3[] spawnLocation;

    public string assetBundleRefLink;

    public event Action OnDataFinishLoaded;

    [Space(10)]
    [Header("Debugging")]
    [SerializeField] TMP_Text debugText;
    private void Start()
    {
        StartCoroutine(RequestWearable());
    }

    private IEnumerator RequestWearable()
    {
        debugText.text = "Start calling the API";
        
        string endpoint = UrlHelper.apiUrl;
        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            www.SetRequestHeader("Access-Control-Allow-Origin", "*");

            yield return www.SendWebRequest();

            switch (www.result)
            {
                case UnityWebRequest.Result.Success:
                    var rawData = www.downloadHandler.text;
                    wearableData = JsonConvert.DeserializeObject<WearableData>(rawData);

                    if (OnDataFinishLoaded != null)
                    {
                        OnDataFinishLoaded();
                    }

                    break;
                case UnityWebRequest.Result.ConnectionError:

                    break;
                case UnityWebRequest.Result.ProtocolError:

                    break;
            }
        }
    }

    private GameObject InstantiateGLTF(string assetName, Vector3 worldPosition)
    {
        var uriBase = new Uri(UrlHelper.awsUrl);
        var endUri = new Uri(uriBase, assetName);

        var instance = Instantiate(prefabGLTF, parentForGLTFInstance.transform, true);
        instance.transform.position = worldPosition;
        GltfAsset GLTFCompRef = instance.GetComponent<GltfAsset>();
        GLTFCompRef.Url = endUri.ToString();
        return instance.gameObject;

        
    }

    private IEnumerator ActivateAnimation(GameObject instance)
    {
        var GLTFRef = instance.GetComponent<GltfAsset>();
        while (!GLTFRef.IsDone)
        {
            yield return new WaitForSeconds(2f);

            Animation anim = GLTFRef.GetComponent<Animation>();
            if (anim != null)
            {
                List<string> clipsName = new List<string>();
                foreach (AnimationState state in anim)
                {
                    clipsName.Add(state.name);
                }

                int animIndex = Int32.Parse(instance.name);
                anim.Play(clipsName[animIndex]);
            }
        }

        debugText.text = $"Clone number {Int32.Parse(instance.name) + 1} finish animation setup";

    }

    public string GetWearableByName(string name)
    {
        if (wearableData != null)
        {
            for (int i = 0; i < wearableData.data.Count; i++)
            {
                if (wearableData.data[i].wearableName.ToString().Contains(name))
                {
                    string value = wearableData.data[i].fileMeta.assetBundleUrl.ToString();
                    debugText.text = "Correctly get the bundle url for : test-orang-lari";
                    return value;
                }
            }
            return "";
        }
        return "";
    }

    public void Setupltf(string assetName)
    {
        var firstInstance = InstantiateGLTF(assetName, transform.position);

        if (spawnLocation.Length> 0)
        {
            for (int i = 0; i < spawnLocation.Length; i++)
            {
                Vector3 random = transform.position + spawnLocation[i];
                var instance = InstantiateGLTF(assetName, random);
                instance.name = i.ToString();

                StartCoroutine(ActivateAnimation(instance));
            }
            debugText.text = "Finish instantiating and place locations for all 8 clones, wait for them to fully load data...";
        }
    }


}
