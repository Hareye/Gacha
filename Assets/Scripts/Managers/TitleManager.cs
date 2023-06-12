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
    public GameObject forgotPassword;
    public GameObject startButton;

    private GameObject lEmailField;
    private GameObject lPasswordField;
    private GameObject lLoginButton;
    private GameObject lForgotPassword;
    private GameObject lRememberMe;

    private GameObject rEmailField;
    private GameObject rPasswordField;
    private GameObject rRegisterButton;
    private GameObject rErrorMessage;

    private GameObject fEmailField;
    private GameObject fErrorMessage;

    private Animator panelAnimator;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    /*
     *  Still need to implement forgot password
     */

    void Awake()
    {
        if (!PlayerPrefs.HasKey("rememberMe"))
        {
            PlayerPrefs.SetInt("rememberMe", 0);
            PlayerPrefs.Save();
        }
        if (!PlayerPrefs.HasKey("accessToken"))
        {
            PlayerPrefs.SetString("accessToken", null);
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
        rErrorMessage = register.transform.Find("Error Message").gameObject;

        fEmailField = forgotPassword.transform.Find("Email").gameObject;
        fErrorMessage = forgotPassword.transform.Find("Error Message").gameObject;

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
            StartCoroutine(loginProcess(PlayerPrefs.GetString("email"), "no password"));
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
            PlayerPrefs.SetString("accessToken", null);
            PlayerPrefs.DeleteKey("email");
            PlayerPrefs.Save();
        }

        StartCoroutine(loginProcess(email, password));
    }

    public void startRegister()
    {
        string email = rEmailField.GetComponent<TMP_InputField>().text;
        string password = rPasswordField.GetComponent<TMP_InputField>().text;

        StartCoroutine(registerProcess(email, password));
    }

    public void resetPassword()
    {
        string email = fEmailField.GetComponent<TMP_InputField>().text;

        StartCoroutine(resetProcess(email));
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
            button.GetComponent<Button>().interactable = true;
        } else
        {
            button.GetComponent<Button>().interactable = false;
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

        if (status.Equals("success"))
        {
            // Register success
            rEmailField.GetComponent<Outline>().enabled = false;
            rPasswordField.GetComponent<Outline>().enabled = false;
            rErrorMessage.SetActive(true);
            rErrorMessage.GetComponent<TextMeshProUGUI>().text = "A verification email has been sent";

        } else if (status.Equals("invalid_fields") || status.Equals("user_exists"))
        {
            // Register failed
            rEmailField.GetComponent<Outline>().enabled = true;
            rPasswordField.GetComponent<Outline>().enabled = true;
            rErrorMessage.SetActive(true);

            if (status.Equals("invalid_fields"))
            {
                rErrorMessage.GetComponent<TextMeshProUGUI>().text = "Invalid email or password";
            } else if (status.Equals("user_exists"))
            {
                rErrorMessage.GetComponent<TextMeshProUGUI>().text = "A user with that email already exists";
            }
        }
    }

    public IEnumerator loginProcess(string email, string password)
    {
        IEnumerator loginCoroutine = server.loginUser(endpoints.login, email, password);
        yield return StartCoroutine(loginCoroutine);

        string status = loginCoroutine.Current as string;

        if (status.Equals("expired"))
        {
            // Login failed (accessToken expired)
            PlayerPrefs.SetString("accessToken", null);
            PlayerPrefs.Save();
        } else if (status.Equals("not_verified") || status.Equals("invalid_fields"))
        {
            // Login failed (invalid inputs or user failed to verify)
            lEmailField.GetComponent<Outline>().enabled = true;
            lPasswordField.GetComponent<Outline>().enabled = true;
        } else
        {
            // Login success (refresh accessToken)
            PlayerPrefs.SetString("accessToken", status);
            PlayerPrefs.Save();

            lEmailField.GetComponent<Outline>().enabled = false;
            lPasswordField.GetComponent<Outline>().enabled = false;
        }
    }

    public IEnumerator resetProcess(string email)
    {
        IEnumerator resetCoroutine = server.resetPassword(endpoints.resetPassword, email);
        yield return StartCoroutine(resetCoroutine);

        string status = resetCoroutine.Current as string;

        if (status.Equals("invalid_fields"))
        {
            // Email is invalid
            fErrorMessage.SetActive(false);
        } else
        {
            // Email was sent successfully
            fErrorMessage.SetActive(true);
        }
    }
}
