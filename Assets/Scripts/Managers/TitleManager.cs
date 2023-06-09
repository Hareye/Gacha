using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using urls;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    public GameObject panel;
    public GameObject login;
    public GameObject register;
    public GameObject startButton;

    private GameObject lEmailField;
    private GameObject lPasswordField;
    private GameObject lLoginButton;
    private GameObject lForgotPassword;
    private GameObject lRememberMe;

    private GameObject rEmailField;
    private GameObject rPasswordField;
    private GameObject rRegisterButton;
    private GameObject rEmailVerification;
    private GameObject rUserExists;
    private GameObject rInvalid;

    private Animator panelAnimator;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    void Awake()
    {
        if (!PlayerPrefs.HasKey("rememberMe"))
        {
            PlayerPrefs.SetInt("rememberMe", 0);
            PlayerPrefs.Save();
        }

        lEmailField = login.transform.Find("Email").gameObject;
        lPasswordField = login.transform.Find("Password").gameObject;
        lLoginButton = login.transform.Find("Login Button").gameObject;
        lForgotPassword = login.transform.Find("Forgot Password").gameObject;
        lRememberMe = login.transform.Find("Remember Me").gameObject;

        rEmailField = register.transform.Find("Email").gameObject;
        rPasswordField = register.transform.Find("Password").gameObject;
        rRegisterButton = register.transform.Find("Register Button").gameObject;
        rEmailVerification = register.transform.Find("Email Verification").gameObject;
        rUserExists = register.transform.Find("User Exists").gameObject;
        rInvalid = register.transform.Find("Invalid").gameObject;

        panelAnimator = panel.GetComponent<Animator>();
        server = GetComponent<ServerManager>();

        lEmailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(lEmailField, lPasswordField, lLoginButton); });
        lPasswordField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(lEmailField, lPasswordField, lLoginButton); });
        rEmailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(rEmailField, rPasswordField, rRegisterButton); });
        rPasswordField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(rEmailField, rPasswordField, rRegisterButton); });

        if (PlayerPrefs.HasKey("email") && PlayerPrefs.GetInt("rememberMe") == 1)
        {
            lEmailField.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("email");
            lRememberMe.GetComponent<Toggle>().isOn = true;

            if (PlayerPrefs.HasKey("accessToken"))
            {
                Debug.Log("Auto Login");
                StartCoroutine(loginProcess(PlayerPrefs.GetString("email"), "empty", PlayerPrefs.GetString("accessToken")));
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (PlayerPrefs.GetInt("rememberMe") == 0 && PlayerPrefs.HasKey("accessToken"))
        {
            PlayerPrefs.DeleteKey("accessToken");
            PlayerPrefs.Save();
        }
    }

    public void startLogin()
    {
        string email = lEmailField.GetComponent<TMP_InputField>().text;
        string password = lPasswordField.GetComponent<TMP_InputField>().text;

        if (lRememberMe.GetComponent<Toggle>().isOn)
        {
            Debug.Log("Remember Me: On");
            PlayerPrefs.SetInt("rememberMe", 1);
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("Remember Me: Off");
            PlayerPrefs.SetInt("rememberMe", 0);
            PlayerPrefs.DeleteKey("email");
            PlayerPrefs.Save();
        }

        StartCoroutine(loginProcess(email, password, "empty"));
    }

    public void startRegister()
    {
        string email = rEmailField.GetComponent<TMP_InputField>().text;
        string password = rPasswordField.GetComponent<TMP_InputField>().text;

        StartCoroutine(registerProcess(email, password));
    }

    public void playPanelAnimation(string name)
    {
        panelAnimator.Play(name);
    }

    public void onEdit(GameObject emailField, GameObject passwordField, GameObject button)
    {
        string email = emailField.GetComponent<TMP_InputField>().text;
        string password = passwordField.GetComponent<TMP_InputField>().text;

        if (email.Length > 0 && password.Length > 0)
        {
            button.GetComponent<Image>().color = new Color32(148, 121, 173, 255);
        } else
        {
            button.GetComponent<Image>().color = new Color32(106, 106, 106, 255);
        }
    }

    public void disableButton()
    {
        startButton.SetActive(false);
    }

    public IEnumerator registerProcess(string email, string password)
    {
        IEnumerator registerCoroutine = server.registerUser(endpoints.register, email, password);
        yield return StartCoroutine(registerCoroutine);

        string status = registerCoroutine.Current as string;

        if (status.Equals("invalid"))
        {
            // Invalid email or password inputs
            rEmailField.GetComponent<Outline>().enabled = true;
            rPasswordField.GetComponent<Outline>().enabled = true;
            rEmailVerification.SetActive(false);
            rUserExists.SetActive(false);
            rInvalid.SetActive(true);
        } else if (status.Equals("true"))
        {
            // Register success
            rEmailField.GetComponent<Outline>().enabled = false;
            rPasswordField.GetComponent<Outline>().enabled = false;
            rEmailVerification.SetActive(true);
            rUserExists.SetActive(false);
            rInvalid.SetActive(false);
        } else
        {
            // Register failed (user already exists)
            rEmailField.GetComponent<Outline>().enabled = true;
            rPasswordField.GetComponent<Outline>().enabled = true;
            rEmailVerification.SetActive(false);
            rUserExists.SetActive(true);
            rInvalid.SetActive(false);
        }
    }

    public IEnumerator loginProcess(string email, string password, string accessToken)
    {
        IEnumerator loginCoroutine = server.loginUser(endpoints.login, email, password, accessToken);
        yield return StartCoroutine(loginCoroutine);

        string status = loginCoroutine.Current as string;

        if (status.Equals("true"))
        {
            // Login success
            lEmailField.GetComponent<Outline>().enabled = false;
            lPasswordField.GetComponent<Outline>().enabled = false;
        } else if (status.Equals("invalid"))
        {
            // Login failed (invalid inputs or user failed to verify)
            lEmailField.GetComponent<Outline>().enabled = true;
            lPasswordField.GetComponent<Outline>().enabled = true;
        } else
        {
            // Access token expired
        }
    }
}
