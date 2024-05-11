using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GPSTrackDrawer : MonoBehaviour
{
    public const float Scale = 25000f;

    [SerializeField] private TextAsset _coordinates;

    private LineRenderer _drawer;
    private SortedList<DateTime, RMCNmeaData> _rmcData;
    private IEnumerator _updateTrackCoroutine;

    private void Awake()
    {
        _drawer = GetComponent<LineRenderer>();

        IEnumerable<RMCNmeaData> rmcData = NmeaUtility<RMCNmeaData>.ParseFromFile<RMCNmeaParser>(_coordinates);
        _rmcData = new SortedList<DateTime, RMCNmeaData>();

        foreach (var rmc in rmcData)
        {
            _rmcData[rmc.SendDate] = rmc;
        }
    }

    public void UpdateTrack()
    {
        if (_updateTrackCoroutine != null)
        {
            StopCoroutine(_updateTrackCoroutine);
        }

        _updateTrackCoroutine = UpdateTrackCoroutine();
        StartCoroutine(_updateTrackCoroutine);
    }

    private IEnumerator UpdateTrackCoroutine()
    {
        _drawer.positionCount = 0;
        Vector3? corrector = null;
        DateTime? previousTime = null;
        float delaySeconds = 0f;

        for (int i = 0; i < _rmcData.Count; i++)
        {
            RMCNmeaData rmc = _rmcData.Values[i];
            Vector3 point = new Vector3(rmc.Longitude, rmc.Latitude) * Scale;

            if (corrector.HasValue == false)
            {
                corrector = point - transform.position;
            }

            _drawer.positionCount = i + 1;
            _drawer.SetPosition(i, point - corrector.Value);

            if (previousTime.HasValue == true)
            {
                TimeSpan timeDifference = rmc.SendDate - previousTime.Value;
                delaySeconds = (float)timeDifference.TotalSeconds;
            }

            yield return new WaitForSeconds(delaySeconds);
            previousTime = rmc.SendDate;
        }

        _updateTrackCoroutine = null;
    }
}