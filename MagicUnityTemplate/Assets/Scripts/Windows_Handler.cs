using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using link.magic.unity.sdk;
using link.magic.unity.sdk.Provider;
using VoltstroStudios.UnityWebBrowser.Core;

public class Windows_Handler : MonoBehaviour
{
    public GameObject UWB_prefab;
    [SerializeField] private BaseUwbClientManager clientmanager;
    private WebBrowserClient webClient;

    void Awake()
    {
        UWB_prefab = Instantiate(Resources.Load("UnityWebBrowser") as GameObject);
        clientmanager = GameObject.FindWithTag("Windows_Browser").GetComponent<BaseUwbClientManager>();
        
        webClient = clientmanager.browserClient;
        webClient.RegisterJsMethod("Test", _cb);
        webClient.RegisterJsMethod("Login", _login);
    }

    public void Update(){
    }

    private bool can_start(){
        var start = webClient.ReadySignalReceived;
        return start;
    }

    public void start_process(string function){
        StartCoroutine(launch(function));
    }

    public IEnumerator launch(string function){
        //Latency issue here that needs testing
        //UWB will not launch/pass ExecuteJS until ReadySignalReceived is true
        //But still needs a hard-wired delay for UWB to be completely loaded

        yield return new WaitUntil(can_start);
        yield return new WaitForSeconds(0.1f);

        webClient.ExecuteJs(
            "uwb.ExecuteJsMethod('"+ function + "')"  
        ); 

        var webObject = clientmanager.gameObject;

        // // webClient.LoadUrl(url);
        
        webObject.GetComponent<RawImage>().enabled = true;
     
    }

    

    public void _cb()
    {
        Debug.Log("Test");
    }

    public void _login()
    {
        Debug.Log("Logged In.");
    }
}
