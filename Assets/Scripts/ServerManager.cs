using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using urls;

public class ServerManager : MonoBehaviour
{
    private Endpoints endpoints = new Endpoints();

    public IEnumerator loginUser(string uri, string email, string password, string accessToken)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("email", email));
        formData.Add(new MultipartFormDataSection("password", password));
        formData.Add(new MultipartFormDataSection("accessToken", accessToken));

        UnityWebRequest uwr = UnityWebRequest.Post(uri, formData);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else if (uwr.responseCode == 200)
        {
            Debug.Log("loginUser: " + uwr.downloadHandler.text);

            if (!uwr.downloadHandler.text.Equals("failed") && !uwr.downloadHandler.text.Equals("expired"))
            {
                PlayerPrefs.SetString("accessToken", uwr.downloadHandler.text);
                PlayerPrefs.Save();
                yield return "true";
            } else if (uwr.downloadHandler.text.Equals("failed"))
            {
                yield return "invalid";
            } else
            {
                yield return "expired";
            }
        }
    }

    public IEnumerator registerUser(string uri, string email, string password)
    {
        IEnumerator checkUserCoroutine = checkUser(endpoints.checkUser, email);
        yield return StartCoroutine(checkUserCoroutine);

        string userExists = checkUserCoroutine.Current as string;

        // Check if inputs are valid
        if (!userExists.Equals("failed"))
        {
            // Check if user exists
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
                } else
                {
                    Debug.Log("registerUser: " + uwr.downloadHandler.text);
                    if (uwr.downloadHandler.text.Equals("failed"))
                    {
                        yield return "invalid";
                    } else
                    {
                        yield return "true";
                    }
                }
            } else
            {
                yield return "false";
            }
        } else
        {
            yield return "invalid";
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
            Debug.Log("checkUser: " + uwr.downloadHandler.text);
            yield return uwr.downloadHandler.text;
        }
    }
}
