using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NFTCreatorView : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _nftName;
    [SerializeField]
    private TMP_InputField _nftDescription;
    [SerializeField]
    private TextMeshProUGUI _uploadStatus;
    [SerializeField]
    private TextMeshProUGUI _objectStatus;

    public TMP_InputField NFTName => _nftName;
    public TMP_InputField NFTDescription => _nftDescription;
    public TextMeshProUGUI UploadStatus => _uploadStatus;
    public TextMeshProUGUI ObjectStatus => _objectStatus;
}
