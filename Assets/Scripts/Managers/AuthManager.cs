using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager instance;

    [SerializeField]
    public string authToken;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
