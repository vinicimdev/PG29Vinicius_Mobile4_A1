using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Runtime.CompilerServices;
using System;

public class GPSFind : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _safeAreaContainer;
    [SerializeField] private TextMeshProUGUI _distanceText;
    [SerializeField] private TextMeshProUGUI _accuracyText;
    [SerializeField] private Image _temperatureIndicator;
    [SerializeField] private RectTransform _directionArrow;

    [Header("Variable Settings")]
    [SerializeField] private float _targetLatitude = 49.279293482940176f;
    [SerializeField] private float _targetLongitude = -123.10744620000001f;
    [SerializeField] private float _foundRadius = 5f;
    [SerializeField] private float _hotRadius = 20f;
    [SerializeField] private float _warmRadius = 50f;
    [SerializeField] private float _lukewarmRadius = 200f;
    [SerializeField] private float _coldRadius = 500f;

    private bool _wasFound;
    private Rect _lastSafeArea;

    private readonly Color _colorCold = new Color(0f, 0f, 1f);
    private readonly Color _colorLukewarm = new Color(0.5f, 1f, 0f);
    private readonly Color _colorWarm = new Color(1f, 1f, 0f);
    private readonly Color _colorHot = new Color(1f, 0f, 0f);

    private IEnumerator Start()
    {
        //if (Input.location.isEnabledByUser == false)
        //{
        //    yield break;
        //}

        yield return new WaitForSeconds(3f);

        Input.location.Start(5f, 2f);
        Debug.Log($"Is enabled by user: {Input.location.isEnabledByUser}, service status: {Input.location.status}");

        int timeout = 5;

        while (Input.location.status == LocationServiceStatus.Stopped && timeout > 0)
        {
            yield return new WaitForSeconds(1);
            timeout--;
        }
        
        if (Input.location.status != LocationServiceStatus.Running)
        {
            yield break;
        }

        StartCoroutine(TrackLocation());
    }

    private IEnumerator TrackLocation()
    {
        Debug.Log($"Is enabled by user: {Input.location.isEnabledByUser}, service status: {Input.location.status}");
        while (_wasFound == false)
        {
            var data = Input.location.lastData;
            float distance = DistanceMeters(data.latitude, data.longitude, 
                                            _targetLatitude, _targetLongitude);

            Debug.Log(distance);
            UpdateDistanceDisplay(distance, data.horizontalAccuracy);
            UpdateColorIndicator(distance);
            UpdateDirectionArrow(data.latitude, data.longitude);

            if (distance <= _foundRadius)
            {
                _wasFound = true;
                Input.location.Stop();
            }

            yield return new WaitForSeconds(1f);
        }
    }
    private void UpdateDistanceDisplay(float distance, float horizontalAccuracy)
    {
        _distanceText.text = distance >= 1000f ? $"{distance / 1000:F1} km away" : $"{distance:F0} m away";
        _accuracyText.text = $"GPS accuracy: ± {horizontalAccuracy:F0} m";
    }
    private void UpdateColorIndicator(float distance)
    {
        Color target;

        if (distance > _coldRadius)
        {
            target = _colorCold;
        }
        else if (distance > _lukewarmRadius)
        {
            target = _colorLukewarm;
        }
        else if (distance > _warmRadius)
        {
            target = _colorWarm;
        }
        else
        {
            target = _colorHot;
        }

        _temperatureIndicator.color = Color.Lerp(_temperatureIndicator.color, target, 0.1f);
    }

    private void UpdateDirectionArrow(float latitude, float longitude)
    {
        if (_directionArrow == null) return;
        float bearing = Bearing(latitude, longitude, _targetLatitude, _targetLongitude);
        _directionArrow.localEulerAngles = new Vector3(0, 0, -bearing);
    }

    private float DistanceMeters(float lat1, float lon1, float lat2, float lon2)
    {
        const float R = 6371000f;
        float dLat = (lat2 - lat1) * Mathf.Deg2Rad;
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float a = Mathf.Sin(dLat / 2f) * Mathf.Sin(dLat / 2f) +
                  Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Cos(lat2 * Mathf.Deg2Rad) *
                  Mathf.Sin(dLon / 2f) * Mathf.Sin(dLon / 2f);
        return R * 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f - a));
    }

    private float Bearing(float lat1, float lon1, float lat2, float lon2)
    {
        float dLon = (lon2 - lon1) * Mathf.Deg2Rad;
        float y = Mathf.Sin(dLon) * Mathf.Cos(lat2 * Mathf.Deg2Rad);
        float x = Mathf.Cos(lat1 * Mathf.Deg2Rad) * Mathf.Sin(lat2 * Mathf.Deg2Rad) -
                  Mathf.Sin(lat1 * Mathf.Deg2Rad) * Mathf.Cos(lat2 * Mathf.Deg2Rad) * Mathf.Cos(dLon);
        return ((Mathf.Atan2(y, x) * Mathf.Rad2Deg) + 360f) % 360f;
    }

    private void OnDestroy()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
    }
}
