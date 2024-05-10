using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using urls;
using encryption;

/*
 *  TO-DO
 *      - Add another login "screen"/page
 *          - When user logs in, they have to click the screen to go to game screen
 *          - Give user option to log out of the account, bringing them back to the login "screen"/page
 */

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
    private GameObject lRememberMe;

    private GameObject rEmailField;
    private GameObject rPasswordField;
    private GameObject rRegisterButton;
    private GameObject rErrorMessage;

    private GameObject fEmailField;
    private GameObject fSendButton;
    private GameObject fErrorMessage;

    private Animator panelAnimator;

    private ServerManager server;
    private LevelManager levelManager;
    private Endpoints endpoints = new Endpoints();
    private Encryption encryption = new Encryption();

    void Awake()
    {
        lEmailField = login.transform.Find("Email").gameObject;
        lPasswordField = login.transform.Find("Password").gameObject;
        lLoginButton = login.transform.Find("Login Button").gameObject;
        lRememberMe = login.transform.Find("Remember Me").gameObject;

        rEmailField = register.transform.Find("Email").gameObject;
        rPasswordField = register.transform.Find("Password").gameObject;
        rRegisterButton = register.transform.Find("Register Button").gameObject;
        rErrorMessage = register.transform.Find("Error Message").gameObject;

        fEmailField = forgotPassword.transform.Find("Email").gameObject;
        fSendButton = forgotPassword.transform.Find("Reset Button").gameObject;
        fErrorMessage = forgotPassword.transform.Find("Error Message").gameObject;

        panelAnimator = panel.GetComponent<Animator>();
        server = GetComponent<ServerManager>();
        levelManager = GetComponent<LevelManager>();

        lEmailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(lEmailField, lPasswordField, lLoginButton); });
        lPasswordField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(lEmailField, lPasswordField, lLoginButton); });
        rEmailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(rEmailField, rPasswordField, rRegisterButton); });
        rPasswordField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(rEmailField, rPasswordField, rRegisterButton); });
        fEmailField.GetComponent<TMP_InputField>().onValueChanged.AddListener(delegate { onEdit(fEmailField, null, fSendButton); });

        if (PlayerPrefs.HasKey("rememberToken"))
        {
            //Debug.Log("Auto login");
            //Debug.Log(PlayerPrefs.GetString("rememberToken"));

            StartCoroutine(verifyRememberProcess(PlayerPrefs.GetString("rememberToken")));
        }
    }

    public void startLogin()
    {
        string email = lEmailField.GetComponent<TMP_InputField>().text;
        string password = lPasswordField.GetComponent<TMP_InputField>().text;
        bool rememberMe = lRememberMe.GetComponent<Toggle>().isOn;

        StartCoroutine(loginProcess(email, password, rememberMe));
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

        if (passwordField == null)
        {
            if (email.Length > 0)
            {
                button.GetComponent<Button>().interactable = true;
            }
            else
            {
                button.GetComponent<Button>().interactable = false;
            }
        } else
        {
            string password = passwordField.GetComponent<TMP_InputField>().text;

            if (email.Length > 0 && password.Length > 0)
            {
                button.GetComponent<Button>().interactable = true;
            }
            else
            {
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void disableButton()
    {
        startButton.SetActive(false);
    }

    public IEnumerator registerProcess(string email, string password)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("password", password),
        };

        IEnumerator registerCoroutine = server.sendRequest(endpoints.register, list);
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

    public IEnumerator loginProcess(string email, string password, bool rememberMe)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("password", password),
        };

        IEnumerator loginCoroutine = server.sendRequest(endpoints.login, list);
        yield return StartCoroutine(loginCoroutine);

        string status = loginCoroutine.Current as string;

        Debug.Log(status);

        if (status.Equals("not_verified") || status.Equals("invalid_fields"))
        {
            // Login failed (invalid inputs or user failed to verify)
            lEmailField.GetComponent<Outline>().enabled = true;
            lPasswordField.GetComponent<Outline>().enabled = true;
        } else
        {
            // Login success (refresh accessToken)
            lEmailField.GetComponent<Outline>().enabled = false;
            lPasswordField.GetComponent<Outline>().enabled = false;

            yield return StartCoroutine(authProcess(email));

            if (rememberMe)
            {
                yield return StartCoroutine(rememberProcess(email));
            }

            levelManager.loadLevel("Main");
        }
    }

    public IEnumerator rememberProcess(string email)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("email", email),
        };

        IEnumerator rememberTokenCoroutine = server.sendRequest(endpoints.rememberToken, list);
        yield return StartCoroutine(rememberTokenCoroutine);

        string status = rememberTokenCoroutine.Current as string;

        if (status.Equals("invalid_fields"))
        {
            lEmailField.GetComponent<Outline>().enabled = true;
            lPasswordField.GetComponent<Outline>().enabled = true;
        } else
        {
            lEmailField.GetComponent<Outline>().enabled = false;
            lPasswordField.GetComponent<Outline>().enabled = false;

            PlayerPrefs.SetString("rememberToken", encryption.encrypt(status));
            PlayerPrefs.Save();
        }
    }

    public IEnumerator authProcess(string email)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("email", email),
        };

        IEnumerator authTokenCoroutine = server.sendRequest(endpoints.authToken, list);
        yield return StartCoroutine(authTokenCoroutine);

        string status = authTokenCoroutine.Current as string;

        if (status.Equals("invalid_fields"))
        {
            lEmailField.GetComponent<Outline>().enabled = true;
            lPasswordField.GetComponent<Outline>().enabled = true;
        }
        else
        {
            lEmailField.GetComponent<Outline>().enabled = false;
            lPasswordField.GetComponent<Outline>().enabled = false;

            PlayerPrefs.SetString("authToken", encryption.encrypt(status));
            PlayerPrefs.Save();
        }
    }

    public IEnumerator resetProcess(string email)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("email", email),
        };

        IEnumerator resetCoroutine = server.sendRequest(endpoints.resetPassword, list);
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

    public IEnumerator verifyRememberProcess(string rememberToken)
    {
        var list = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("rememberToken", encryption.decrypt(rememberToken)),
        };

        IEnumerator verifyRememberCoroutine = server.sendRequest(endpoints.verifyRememberToken, list);
        yield return StartCoroutine(verifyRememberCoroutine);

        string status = verifyRememberCoroutine.Current as string;

        if (status.Equals("not_verified"))
        {
            // Remember Token is invalid or has expired
            PlayerPrefs.DeleteKey("rememberToken");
            PlayerPrefs.Save();
        } else
        {
            yield return StartCoroutine(authProcess(status));

            levelManager.loadLevel("Main");
        }
    }
}
