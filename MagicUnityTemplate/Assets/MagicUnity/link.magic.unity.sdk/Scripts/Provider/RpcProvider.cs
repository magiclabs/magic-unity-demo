using System;
using System.IO;
using System.Threading.Tasks;
using link.magic.unity.sdk.Relayer;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;

namespace link.magic.unity.sdk.Provider

{
    public class RpcProvider : ClientBase
    {
        // Nethereum
        private readonly JsonSerializerSettings _jsonSerializerSettings =
            DefaultJsonSerializerSettingsFactory.BuildDefaultJsonSerializerSettings();

        private readonly WebviewController _relayer;

        protected internal RpcProvider(UrlBuilder urlBuilder, GameObject canvas)
        {
            var url = _generateBoxUrl(urlBuilder);

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            _relayer = new(canvas);
#else
            _relayer = new();
#endif 
            // init relayer
            _relayer.Load(url);
        }

        //WHERE THE JS GETS BUILT
        private string _generateBoxUrl(UrlBuilder urlBuilder)
        {
            // encode options params to base 64
            var optionsJsonString = JsonUtility.ToJson(urlBuilder);
            Debug.Log(optionsJsonString);

            var url = $"{UrlBuilder.Host}/send/?params={urlBuilder.EncodedParams}";

            return url;
        }

        // 
        protected override async Task<RpcResponseMessage> SendAsync(RpcRequestMessage request, string route = null)
        {
            var msgType = $"{nameof(OutboundMessageType.MAGIC_HANDLE_REQUEST)}-{UrlBuilder.Instance.EncodedParams}";
            var relayerRequest = new RelayerRequestNethereum(msgType, request);
            var requestMsg = JsonConvert.SerializeObject(relayerRequest, _jsonSerializerSettings);
            Debug.Log($" MagicUnity 1{requestMsg}");

            var promise = new TaskCompletionSource<RpcResponseMessage>();

            // handle Response in the callback, so that webview is type free
            _relayer.Enqueue(requestMsg, (int)request.Id, responseMsg =>
            {
                var reader = new JsonTextReader(new StringReader(responseMsg));
                var serializer = JsonSerializer.Create(_jsonSerializerSettings);
                var relayerResponseNethereum = serializer.Deserialize<RelayerResponseNethereum>(reader);
                var result = relayerResponseNethereum?.Response;
                return promise.TrySetResult(result);
            });

            return await promise.Task;
        }


        protected internal async Task<TResult> MagicSendAsync<TParams, TResult>(MagicRpcRequest<TParams> magicRequest)
        {
            // Wrap with Relayer params and send to relayer
            var msgType = $"{nameof(OutboundMessageType.MAGIC_HANDLE_REQUEST)}-{UrlBuilder.Instance.EncodedParams}";
            var relayerRequest = new RelayerRequest<TParams>(msgType, magicRequest);
            var msgStr = JsonUtility.ToJson(relayerRequest);

            var promise = new TaskCompletionSource<TResult>();

            // handle Response in the callback, so that webview is type free
            _relayer.Enqueue(msgStr, magicRequest.id, msg =>
            {
                var relayerResponse = JsonUtility.FromJson<RelayerResponse<TResult>>(msg);

                var error = relayerResponse.response.error;
                if ((error != null) & (error?.message != null))
                    return promise.TrySetException(new Exception(error.message));

                var result = relayerResponse.response.result;

                return promise.TrySetResult(result);
            });

            return await promise.Task;
        }
    }
}