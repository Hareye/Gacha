using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using urls;

public class ServerManager : MonoBehaviour
{
    private Endpoints endpoints = new Endpoints();

    public IEnumerator authenticateUser(string uri, string email, string password)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("email", email));
        formData.Add(new MultipartFormDataSection("password", password));

        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else if (uwr.responseCode == 200)
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }

    public IEnumerator registerUser(string uri, string email, string password)
    {
        IEnumerator checkUserCoroutine = checkUser(endpoints.checkUser, email);
        yield return StartCoroutine(checkUserCoroutine);

        string userExists = checkUserCoroutine.Current as string;

        if (userExists.Equals("false"))
        {
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("email", email));
            formData.Add(new MultipartFormDataSection("password", password));

            UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error while sending: " + uwr.error);
            }
            else if (uwr.responseCode == 200)
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        } else
        {
            // User already exists
            yield break;
        }
    }

    public IEnumerator checkUser(string uri, string email)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("email", email));

        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else if (uwr.responseCode == 200)
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            yield return uwr.downloadHandler.text;
        }
    }

    public IEnumerator getRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else if (uwr.responseCode == 200)
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }
}
