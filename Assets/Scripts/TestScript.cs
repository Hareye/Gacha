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
        //StartCoroutine(getRequest(endpoints.testServer));
        //StartCoroutine(authenticateUser(endpoints.testForm));
    }

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
        } else if (uwr.responseCode == 200)
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
        } else if (uwr.responseCode == 200)
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }
}
