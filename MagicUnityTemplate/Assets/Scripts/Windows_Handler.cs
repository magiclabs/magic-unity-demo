using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using link.magic.unity.sdk;
using link.magic.unity.sdk.Provider;
using VoltstroStudios.UnityWebBrowser.Core;

public class Windows_Handler : MonoBehaviour
{
    [SerializeField] private BaseUwbClientManager clientmanager;
    private WebBrowserClient webClient;
    
    void Start()
    {
        webClient = clientmanager.browserClient;
        webClient.RegisterJsMethod("Test", _cb);
    }

    public void win_load_url(string url){
        if(webClient.ReadySignalReceived){
        
            webClient.ExecuteJs(
                "uwb.ExecuteJsMethod('Test')"  
            );

            var webObject = clientmanager.gameObject;
            // webClient.LoadUrl(url);
            webObject.GetComponent<RawImage>().enabled = true;
        }
    }

    public void _cb()
    {
        Debug.Log("Hello,World!");
    }
}
