using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
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
