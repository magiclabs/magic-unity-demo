using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using link.magic.unity.sdk.Provider;

using VoltstroStudios.UnityWebBrowser;
using VoltstroStudios.UnityWebBrowser.Core;
using VoltstroStudios.UnityWebBrowser.Shared.Core;
using VoltstroStudios.UnityWebBrowser.Core.Engines;
using VoltstroStudios.UnityWebBrowser.Communication;
using VoltstroStudios.UnityWebBrowser.Input;
using WindowsHandler;
using static VoltstroStudios.UnityWebBrowser.Core.Engines.Engine;



namespace link.magic.unity.sdk.Relayer
{
    public class WebviewController
    {
        // Switch variable depending on OS
#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
        private readonly WebViewObject _webViewObject;

#else
        private GameObject windows_webView;
        private GameObject webviewContainer;
        public BaseUwbClientManager clientmanager;
        private WebBrowserClient webClient;
        RectTransform uwbRectTransform;
#endif
        private readonly Dictionary<int, Func<string, bool>> _messageHandlers = new();


        private readonly Queue<string> _queue = new();
        private bool _relayerLoaded;
        private bool _relayerReady;


#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
        public WebviewController()
        {
            // instantiate webview 
            _webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
            _webViewObject.Init(
                cb: _cb,
                ld: (msg) =>
                {
                    _relayerLoaded = true;
                },
                httpErr: (msg) =>
                {
                    Debug.Log(string.Format("MagicUnity, LoadRelayerHttpError[{0}]", msg));
                },
                err: (msg) =>
                {
                    Debug.Log(string.Format("MagicUnity, LoadRelayerError[{0}]", msg));
                }
            );
        }
#else
        public WebviewController()
        {
            webviewContainer = new GameObject("WebViewObjectContainer");

            //Create canvas
            Canvas canvas = webviewContainer.AddComponent<Canvas>();
            canvas.sortingOrder = 10;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            // canvas.worldCamera = Camera.main;

            CanvasScaler canvasScaler = webviewContainer.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0;

            webviewContainer.AddComponent<GraphicRaycaster>();
            
            //Child object, where raw image and UWB itself will live
            GameObject uwbGameObject = new("UWBContainer");
            uwbGameObject.transform.SetParent(webviewContainer.transform);
            
            //Configure rect transform
            uwbRectTransform = uwbGameObject.AddComponent<RectTransform>();
            uwbRectTransform.anchorMin = Vector2.zero;
            uwbRectTransform.anchorMax = Vector2.one;
            uwbRectTransform.pivot = new Vector2(0.5f, 0.5f);
            uwbRectTransform.localScale = Vector3.one;
            uwbRectTransform.offsetMin = Vector2.zero;
            uwbRectTransform.offsetMax = Vector2.zero;
            
            //Add raw image
            uwbGameObject.AddComponent<RawImage>();
            
            //UWB Pre-Setup
            
            //Create engine dynamically
            EngineConfiguration engineConfig = ScriptableObject.CreateInstance<EngineConfiguration>();
            engineConfig.engineAppName = "UnityWebBrowser.Engine.Cef";
            
#if UNITY_EDITOR
            engineConfig.engineFiles = new[] { 
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.Windows64,
                    engineFileLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.win.x64/Engine~/"
                },
                new Engine.EnginePlatformFiles
                {
                    platform = Platform.Linux64,
                    engineFileLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.linux.x64/Engine~/"
                }
            };
#endif
            
            //Create coms layer dynamically
            CommunicationLayer comsLayer = ScriptableObject.CreateInstance<TCPCommunicationLayer>();
            
            //Create input handler dynamically
            WebBrowserInputHandler inputHandler = ScriptableObject.CreateInstance<WebBrowserOldInputHandler>();
            
            //UWB Object Setup
            WebBrowserUIBasic webBrowser = uwbGameObject.AddComponent<WebBrowserUIBasic>();
            webBrowser.browserClient.engine = engineConfig;
            webBrowser.browserClient.communicationLayer = comsLayer;
            webBrowser.inputHandler = inputHandler;

            clientmanager = uwbGameObject.GetComponent<BaseUwbClientManager>();
            webClient = clientmanager.browserClient;
            webClient.jsMethodManager.jsMethodsEnable = true;
            webClient.initialUrl = "https://box.magic.link";
            webClient.RegisterJsMethod<string>("_cb", _cb);

            uwbRectTransform.localScale = Vector3.zero;
        }


         // GameObject.FindWithTag("Windows_Browser").GetComponent<BaseUwbClientManager>();
        public void WebviewController2()
        {
            // WebBrowserClient browserClient = clientManager.GetComponent<WebBrowserClient>();
            // browserClient.javascript = true;
            // browserClient.initialUrl = "https://google.com";

            // ----
            // windows_webView_container = new GameObject("WebViewObjectContainer");
            // Canvas canvas = windows_webView_container.AddComponent<Canvas>();
            // CanvasScaler canvasScaler = windows_webView_container.AddComponent<CanvasScaler>();

            // windows_webView = new GameObject("WebViewObject");
            // windows_webView.transform.parent = windows_webView_container.transform;

            // WebBrowserUIBasic browser = windows_webView.AddComponent<WebBrowserUIBasic>();
            // clientmanager = windows_webView.GetComponent<BaseUwbClientManager>();

            // browser.inputHandler = ScriptableObject.CreateInstance<WebBrowserOldInputHandler>();
            // webClient = clientmanager.browserClient;
            // webClient.jsMethodManager.jsMethodsEnable = true;
            // webClient.javascript = true;
            // EngineConfiguration config = ScriptableObject.CreateInstance<EngineConfiguration>();
            // config.engineAppName = "UnityWebBrowser.Engine.Cef";
            // EnginePlatformFiles file = new Engine.EnginePlatformFiles();
            // file.platform = Platform.Windows64;
            // file.engineFileLocation = "Packages/dev.voltstro.unitywebbrowser.engine.cef.win.x64/Engine~/";
            // config.engineFiles = new EnginePlatformFiles[] { file };
            // webClient.engine = config;
            // webClient.communicationLayer = ScriptableObject.CreateInstance<TCPCommunicationLayer>();
            // webClient.RegisterJsMethod<string>("_cb", _cb);

            // ----     
            clientmanager = GameObject.FindWithTag("Windows_Browser").GetComponent<BaseUwbClientManager>();
            webClient = clientmanager.browserClient;
            webClient.javascript = true;
            webClient.jsMethodManager.jsMethodsEnable = true;
            webClient.RegisterJsMethod<string>("_cb", _cb);

            Debug.Log(clientmanager);
            
            // windows_webView = new GameObject("Windows_Webview");
            // windows_webView.AddComponent<Windows_Handler>();
            // windows_webView.tag = "win_web";

            Debug.Log("launching Webview");

            //  DelayAction(5.0f); 
            //StartCoroutine(DelayAction());
        }
#endif

        internal void Load(string url)
        {
            Debug.Log("Load.url: " + url);
#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
            _webViewObject.LoadURL(url);
#else
            Debug.Log("browser.Load()" + url);
            DelayLoadUrl(url);
           // TestAsync(url); // url "http://google.com"
             //Invoke("Test", 5.0f);
#endif
        }

        // void Test() {
        //     Debug.Log("Test");
        //     // _relayerLoaded = true;
        //     _relayerReady = true;
        //     _dequeue();
        // }

        async void DelayLoadUrl(string url) {
            await Task.Delay(1500);
            _relayerLoaded = true;
            webClient.LoadUrl(url);
            // await Task.Delay(2000);
            // Test();
        }


        // callback js hooks
        private void _cb(string msg)
        {
            Debug.Log($"MagicUnity Received Message from Relayer: {msg}");
            // Do SimRle Relayer JSON Deserialization just to fetch ids for handlers

#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
            var res = JsonUtility.FromJson<RelayerResponse<object>>(msg);
            var msgType = res.msgType;

            var method = msgType.Split("-")[0];

            switch (method)
            {
                case nameof(InboundMessageType.MAGIC_OVERLAY_READY):
                    _relayerReady = true;
                    _dequeue();
                    break;
                case nameof(InboundMessageType.MAGIC_SHOW_OVERLAY):
                    _webViewObject.SetVisibility(true);
                    break;
                case nameof(InboundMessageType.MAGIC_HIDE_OVERLAY):
                    _webViewObject.SetVisibility(false);
                    break;
                case nameof(InboundMessageType.MAGIC_HANDLE_EVENT):
                    //Todo Unsupported for now
                    break;
                case nameof(InboundMessageType.MAGIC_HANDLE_RESPONSE):
                    _handleResponse(msg, res);
                    break;
            }
#else
            Debug.Log("CALLBACK");
            Debug.Log(msg);

            var res = JsonUtility.FromJson<RelayerResponse<object>>(msg);
            var msgType = res.msgType;

            var method = msgType.Split("-")[0];

            Debug.Log("method: " + method);
            switch (method)
            {
                case nameof(InboundMessageType.MAGIC_OVERLAY_READY):
                    _relayerReady = true;
                    _dequeue();
                    break;
                case nameof(InboundMessageType.MAGIC_SHOW_OVERLAY):
                    //_webViewObject.SetVisibility(true);
                    Debug.Log("SHOW BROWSER");
                    uwbRectTransform.localScale = Vector3.one;
                    break;
                case nameof(InboundMessageType.MAGIC_HIDE_OVERLAY):
                    //_webViewObject.SetVisibility(false);
                    uwbRectTransform.localScale = Vector3.zero;
                    Debug.Log("HIDE BROWSER");
                    break;
                case nameof(InboundMessageType.MAGIC_HANDLE_EVENT):
                    //Todo Unsupported for now
                    break;
                case nameof(InboundMessageType.MAGIC_HANDLE_RESPONSE):
                    _handleResponse(msg, res);
                    break;
            }
#endif
        }


        /// <summary>
        ///     Queue
        /// </summary>
        internal void Enqueue(string message, int id, Func<string, bool> callback)
        {
            Debug.Log("Enqueue: " + message);
            _queue.Enqueue(message);
            _messageHandlers.Add(id, callback);
            _dequeue();
        }

        private void _dequeue()
        {
            Debug.Log("_dequeue " + (_relayerLoaded ? "true" : "false") + " " + (_relayerReady ? "true" : "false") + " " + _queue.Count.ToString());
            if (_queue.Count != 0 && _relayerReady && _relayerLoaded)
            {
                var message = _queue.Dequeue();

                Debug.Log($"MagicUnity Send Message to Relayer: {message}");
#if !UNITY_EDITOR_WIN || !UNITY_STANDALONE_WIN
                _webViewObject.EvaluateJS(
                    $"window.dispatchEvent(new MessageEvent('message', {{ 'data': {message} }}));");
#else
                //windows_webView.GetComponent<Windows_Handler>().launch($"window.dispatchEvent(new MessageEvent('message', {{ 'data': {message} }}));");
                Debug.Log("ExecuteJs: " + $"window.dispatchEvent(new MessageEvent('message', {{ 'data': {message} }}));");
                webClient.ExecuteJs($"window.dispatchEvent(new MessageEvent('message', {{ 'data': {message} }}));");
#endif
                _dequeue();
            }
        }

        private void _handleResponse(string originalMsg, RelayerResponse<object> relayerResponse)
        {
            Debug.Log("_handleResponse: " + originalMsg);
            var payloadId = relayerResponse.response.id;
            var handler = _messageHandlers[payloadId];
            handler(originalMsg);
            _messageHandlers.Remove(payloadId);
        }
    }
}