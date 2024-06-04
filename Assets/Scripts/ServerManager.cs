using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance;

    private void Awake()
    {
        instance = this;
        // Don't destroy when implementing persistency
        //DontDestroyOnLoad(gameObject);
    }

    public IEnumerator sendRequest(string uri, List<KeyValuePair<string, string>> parameters)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (KeyValuePair<string, string> param in parameters)
        {
            formData.Add(new MultipartFormDataSection(param.Key, param.Value));
        }

        // UnityWebRequest automatically sets Content-Type
        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else if (uwr.responseCode == 200)
        {
            yield return uwr.downloadHandler.text;
        }
    }

    public IEnumerator sendRequestWithAuth(string uri, List<KeyValuePair<string, string>> parameters)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (KeyValuePair<string, string> param in parameters)
        {
            formData.Add(new MultipartFormDataSection(param.Key, param.Value));
        }

        // UnityWebRequest automatically sets Content-Type
        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);
        //uwr.SetRequestHeader("authToken", AuthManager.instance.authToken);

        // Temporary - for dev work to test individual screens
        uwr.SetRequestHeader("authToken", PlayerPrefs.GetString("authToken"));

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else
        {
            //Debug.Log("gachaPull: " + uwr.downloadHandler.text);
            yield return uwr.downloadHandler.text;
        }
    }

    public IEnumerator sendRequestWithAuth(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        //uwr.SetRequestHeader("authToken", AuthManager.instance.authToken);

        uwr.SetRequestHeader("authToken", PlayerPrefs.GetString("authToken"));

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else
        {
            //Debug.Log("gachaPull: " + uwr.downloadHandler.text);
            yield return uwr.downloadHandler.text;
        }
    }
}
