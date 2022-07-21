using System.Collections;
using System.Collections.Generic;
using link.magic.unity.sdk;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagicUnityButton : MonoBehaviour
{
    public TMP_InputField result;
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
        var token = await magic.Auth.LoginWithMagicLink("d.batkulinov@knubisoft.com");
        result.text = $"token {token}";
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
