using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ARCameraBackground : MonoBehaviour
{
    [SerializeField] private RawImage _backgroundImage;
    [SerializeField] private bool _useFrontCamera = false;

    private WebCamTexture _webCamTexture;

    /// <summary>Starts the camera feed. Called by GameManager when gameplay begins.</summary>
    public void StartFeed()
    {
        StartCoroutine(RequestAndInit());
    }

    /// <summary>Stops and releases the camera feed.</summary>
    public void StopFeed()
    {
        if (_webCamTexture != null && _webCamTexture.isPlaying)
        {
            _webCamTexture.Stop();
        }
    }

    private IEnumerator RequestAndInit()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogWarning("[ARCameraBackground] Camera permission denied.");
            yield break;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.LogWarning("[ARCameraBackground] No cameras found.");
            yield break;
        }

        string camName = null;
        foreach (var d in devices)
        {
            if (d.isFrontFacing == _useFrontCamera)
            {
                camName = d.name;
                break;
            }
        }
        camName ??= devices[0].name;

        _webCamTexture = new WebCamTexture(camName, Screen.width, Screen.height, 30);
        _backgroundImage.texture = _webCamTexture;
        _webCamTexture.Play();

        // Wait one frame so videoRotationAngle is populated
        yield return null;
        ApplyRotationAndMirror();
    }

    private void ApplyRotationAndMirror()
    {
        _backgroundImage.rectTransform.localEulerAngles =
            new Vector3(0f, 0f, -_webCamTexture.videoRotationAngle);

        if (_useFrontCamera)
            _backgroundImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
    }

    private void OnDestroy()
    {
        StopFeed();
    }
}
