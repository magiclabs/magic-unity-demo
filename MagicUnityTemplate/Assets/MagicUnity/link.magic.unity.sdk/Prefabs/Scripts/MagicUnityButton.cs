using System.Collections;
using System.Collections.Generic;
using link.magic.unity.sdk;
using UnityEngine;
using UnityEngine.UI;

public class MagicUnityButton : MonoBehaviour
{
    public GameObject windows_webVeiw;
    public Text result;
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
        // Debug.Log("logging in...");
        Magic magic = new Magic("pk_live_2C8DAF27FCBA05C9");
        var token = await magic.Auth.LoginWithEmailOtp("nicholasobri@gmail.com");
        Debug.Log("Sending Token...");
        // windows_webVeiw.GetComponent<Windows_Handler>().win_load_url("https://doubleunderscore.net");
        // result.text = $"token {token}";
        // Debug.Log("token: " + token);
    }

    public async void GetMetadata()
    {
        Magic magic = Magic.Instance;
        var metadata = await magic.User.GetMetadata();
        result.text = $"Metadata Email: {metadata.email} \n Public Address: {metadata.publicAddress}";
    }

    public async void Logout()
    {
        Magic magic = Magic.Instance;
        var isLogout = await magic.User.Logout();
        result.text = $"Logout: {isLogout.ToString()}";
    }
}
