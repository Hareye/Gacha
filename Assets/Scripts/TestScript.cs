using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using urls;

public class TestScript : MonoBehaviour
{
    Endpoints endpoints = new Endpoints();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getRequest(endpoints.testRoute));
    }

    IEnumerator getRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        } else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }
}
