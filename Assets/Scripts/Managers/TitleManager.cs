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
    private GameObject lRegisterButton;
    private GameObject lForgotPassword;
    private GameObject lRememberMe;

    private GameObject rEmailField;
    private GameObject rPasswordField;
    private GameObject rRegisterButton;
    private GameObject rEmailVerification;

    private Animator panelAnimator;

    private ServerManager server;
    private Endpoints endpoints = new Endpoints();

    void Awake()
    {
        lEmailField = login.transform.Find("Email").gameObject;
        lPasswordField = login.transform.Find("Password").gameObject;
        lLoginButton = login.transform.Find("Login Button").gameObject;
        lRegisterButton = login.transform.Find("Register Button").gameObject;
        lForgotPassword = login.transform.Find("Forgot Password").gameObject;
        lRememberMe = login.transform.Find("Remember Me").gameObject;

        rEmailField = register.transform.Find("Email").gameObject;
        rPasswordField = register.transform.Find("Password").gameObject;
        rRegisterButton = register.transform.Find("Register Button").gameObject;
        rEmailVerification = register.transform.Find("Email Verification").gameObject;

        panelAnimator = panel.GetComponent<Animator>();

        server = GetComponent<ServerManager>();

        lEmailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(lEmailField, lPasswordField, lLoginButton); });
        lPasswordField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(lEmailField, lPasswordField, lLoginButton); });
        rEmailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(rEmailField, rPasswordField, rRegisterButton); });
        rPasswordField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(rEmailField, rPasswordField, rRegisterButton); });
    }

    public void startLogin()
    {
        string email = lEmailField.GetComponent<TMP_InputField>().text;
        string password = lPasswordField.GetComponent<TMP_InputField>().text;

        StartCoroutine(server.authenticateUser(endpoints.authenticate, email, password));

        //startButton.SetActive(true);
    }

    public void startRegister()
    {
        string email = rEmailField.GetComponent<TMP_InputField>().text;
        string password = rPasswordField.GetComponent<TMP_InputField>().text;

        rEmailVerification.SetActive(true);
        StartCoroutine(server.registerUser(endpoints.register, email, password));
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
}
