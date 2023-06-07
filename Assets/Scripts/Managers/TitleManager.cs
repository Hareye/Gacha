using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using urls;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    public GameObject login;

    [SerializeField]
    public GameObject startButton;

    private GameObject panelText;
    private GameObject emailField;
    private GameObject passwordField;
    private GameObject loginButton;
    private GameObject registerButton;
    private GameObject forgotPassword;
    private GameObject rememberMe;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    void Awake()
    {
        panelText = login.transform.Find("Panel Text").gameObject;
        emailField = login.transform.Find("Email").gameObject;
        passwordField = login.transform.Find("Password").gameObject;
        loginButton = login.transform.Find("Login Button").gameObject;
        registerButton = login.transform.Find("Register Button").gameObject;
        forgotPassword = login.transform.Find("Forgot Password").gameObject;
        rememberMe = login.transform.Find("Remember Me").gameObject;

        server = GetComponent<ServerManager>();

        emailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(); });
        passwordField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(); });
    }

    public void startLogin()
    {
        string email = emailField.GetComponent<TMP_InputField>().text;
        string password = passwordField.GetComponent<TMP_InputField>().text;

        StartCoroutine(server.authenticateUser(endpoints.authenticate, email, password));

        //startButton.SetActive(true);
    }

    public void startRegister()
    {

    }

    public void onEdit()
    {
        string email = emailField.GetComponent<TMP_InputField>().text;
        string password = passwordField.GetComponent<TMP_InputField>().text;

        if (email.Length > 0 && password.Length > 0)
        {
            loginButton.GetComponent<Image>().color = new Color32(148, 121, 173, 255);
        } else
        {
            loginButton.GetComponent<Image>().color = new Color32(106, 106, 106, 255);
        }
    }

    public void disableButton()
    {
        startButton.SetActive(false);
    }
}
