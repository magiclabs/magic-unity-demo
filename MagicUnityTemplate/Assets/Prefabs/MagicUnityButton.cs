using System.Collections;
using System.Collections.Generic;
using MagicSDK;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagicUnityButton : MonoBehaviour
{
    public Magic magic;
    [SerializeField] public TextMeshProUGUI result;

    [SerializeField] internal string OTPEmail;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void Login()
    {
        Magic magic = Magic.Instance;
        var token = await magic.Auth.LoginWithEmailOtp(OTPEmail);
        Debug.Log("token: " + token);
        result.text = $"token {token}";
    }

    public async void GetMetadata()
    {
        Magic magic = Magic.Instance;
        var metadata = await magic.User.GetMetadata();
        Debug.Log("metadata: " + metadata);
        result.text = $"Metadata Email: {metadata.email} \n Public Address: {metadata.publicAddress}";
    }

    public async void Logout()
    {
        Magic magic = Magic.Instance;
        var isLogout = await magic.User.Logout();
        Debug.Log("isLogout: " + isLogout);
        result.text = $"Logout: {isLogout.ToString()}";
    }
}
