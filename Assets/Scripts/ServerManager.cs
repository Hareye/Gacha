using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using urls;
using encryption;

public class ServerManager : MonoBehaviour
{
    private Endpoints endpoints = new Endpoints();
    private Encryption encryption = new Encryption();

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
        uwr.SetRequestHeader("authToken", encryption.decrypt(PlayerPrefs.GetString("authToken")));
        //Debug.Log(encryption.decrypt(PlayerPrefs.GetString("authToken")));

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
