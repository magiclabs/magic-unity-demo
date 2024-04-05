using Newtonsoft.Json;
using UnityEngine;
using Pinata.Client;
using System.Net.Http;
using System.Text;
using System.Net.Mime;
using Flurl.Http.Content;
using Flurl.Http.Configuration;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class NFTCreator : MonoBehaviour
{
    private const string ModelPinataHostURL = "https://gateway.pinata.cloud/ipfs/";
    private const string ImagePinataHostURL = "ipfs://";
    [SerializeField]
    private string _pinataApiKey;
    [SerializeField]
    private string _pinataApiSecret;
    [SerializeField]
    private GameObject _objectToExport;
    [SerializeField]
    private Texture2D _image;

    private NFTCreatorView _view;
    private PinataClient _client;
    private bool _isConverted;
    private ModelExporter _modelExporter;

    public void Awake()
    {
        _view = GetComponent<NFTCreatorView>();

        var config = new Config();
        config.ApiKey = _pinataApiKey;
        config.ApiSecret = _pinataApiSecret;

        _client = new PinataClient(config);
        _modelExporter = new ModelExporter();
        _view.ObjectStatus.text = "Not converted";
        _view.UploadStatus.text = "Not uploaded";
    }

    public async void CreateNFT()
    {
        _view.UploadStatus.text = "Uploading";
        var metadata = new NFTMetadataJSON();
        metadata.name = _view.NFTName.text;
        metadata.description = _view.NFTDescription.text;
        var modelIpfsHash = await LoadFileToPinataAsync("glb", _isConverted);
        metadata.animation_url = ModelPinataHostURL + modelIpfsHash;
        var imageIpfsHash = await LoadFileToPinataAsync("png", false);
        metadata.image = ImagePinataHostURL + imageIpfsHash;

        var json = JsonConvert.SerializeObject(metadata);
        var metadataIpfsHash = await LoadJsonToPinataAsync(json, "1.json");
        _view.UploadStatus.text = "Uploaded";
    }

    public async Task<string> LoadFileToPinataAsync(string extension, bool isGameObject)
    {
        var options = new PinataOptions();
        options.WrapWithDirectory = true;

        var path = Path.Combine(Application.persistentDataPath, "1." + extension);

        var bytes = isGameObject ? File.ReadAllBytes(path) : _image.EncodeToPNG();
        var file = new ByteArrayContent(bytes);

        var response = await _client.Pinning.PinFileToIpfsAsync(content => content.AddPinataFile(file, "1." + extension), null, options);

        if (response.IsSuccess)
        {
            Debug.Log(response.IpfsHash);
            return response.IpfsHash + "/1." + extension;
        }
        return "";
    }

    public async Task<string> LoadJsonToPinataAsync(string json, string fileName)
    {
        var file = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _client.Pinning.PinFileToIpfsAsync(content => content.AddPinataFile(file, fileName));

        if (response.IsSuccess)
        {
            Debug.Log(response.IpfsHash);
            return response.IpfsHash;
        }
        return "";
    }

    public async void ConvertGameObject()
    {
        var result = await _modelExporter.ConvertGameObjectToGLB(_objectToExport);

        _isConverted = result;
        _view.ObjectStatus.text = result ? "Converted" : "Conversion error";
    }
}
