using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using urls;

public class ServerManager : MonoBehaviour
{
    private Endpoints endpoints = new Endpoints();

    public IEnumerator loginUser(string uri, string email, string password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("email", email));
        formData.Add(new MultipartFormDataSection("password", password));

        // UnityWebRequest automatically sets Content-Type
        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);
        uwr.SetRequestHeader("Authorization", PlayerPrefs.GetString("accessToken"));

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else if (uwr.responseCode == 200)
        {
            Debug.Log("loginUser: " + uwr.downloadHandler.text);
            yield return uwr.downloadHandler.text;
        }
    }

    public IEnumerator registerUser(string uri, string email, string password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("email", email));
        formData.Add(new MultipartFormDataSection("password", password));

        // UnityWebRequest automatically sets Content-Type
        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else if (uwr.responseCode == 200)
        {
            Debug.Log("registerUser: " + uwr.downloadHandler.text);
            yield return uwr.downloadHandler.text;
        }
    }

    public IEnumerator resetPassword(string uri, string email)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("email", email));

        // UnityWebRequest automatically sets Content-Type
        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else
        {
            Debug.Log("resetPassword: " + uwr.downloadHandler.text);
            yield return uwr.downloadHandler.text;
        }
    }
}
