using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerAimView : UiView
{
    [SerializeField] private TextMeshProUGUI _angleText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private List<RectTransform> _trajectoryPresentImages;
    [SerializeField] private List<RectTransform> _previousTrajectoryPresentImages;
    [SerializeField] private List<RectTransform> _previousTrajectoryImages;
    [SerializeField] private HighlightedPreviousPlayerTrajectory _previousPlayerTrajectory;

    private Vector3 _trajectoryStartPosition;
    private Trajectory _previousTrajectory;
    private int _level;
    
    public Tower Tower;
    private float _deg2Rad = 30 * Mathf.Deg2Rad;

    public void Setup(Vector3 trajectoryStartPosition, int level)
    {
        _trajectoryStartPosition = trajectoryStartPosition;
        _level = level;
    }
    
    public void Setup(Vector3 trajectoryStartPosition)
    {
        _trajectoryStartPosition = trajectoryStartPosition;
    }

    private void Start()
    {
        SetActivePreviousTrajectory(false);

        SetActiveTutorialTrajectory(_level == 1);

        PresentTrajectory(new Trajectory(new Vector2(Mathf.Cos(_deg2Rad), Mathf.Sin(_deg2Rad)), 1, 15f), _previousTrajectoryPresentImages);
    }

    public void Present(Vector2 forward, float power, float speed)
    {
        var angle = Vector2.Angle(Vector2.right, forward);
        
        _angleText.text = ((int)angle).ToString() + '°';
        _powerText.text = ((int)(power * 100) + 1).ToString() + '%';

        var trajectory = new Trajectory(forward, power, speed);
        
        PresentTrajectory(trajectory, _trajectoryPresentImages);
        
        if(_previousPlayerTrajectory.enabled)
            CheckEqualityPreviousAndNewTrajectory(trajectory);
    }

    public void SetActivePreviousTrajectory(bool value)
    {
        foreach (var presentImage in _previousTrajectoryImages)
        {
            presentImage.gameObject.SetActive(value);
        }
    }
    
    public void ResetPreviousTrajectory()
    {
        foreach (var presentImage in _previousTrajectoryImages)
        {
            presentImage.position = Vector3.zero;
        }
    }

    public void SetActiveTutorialTrajectory(bool value)
    {
        foreach (var presentImage in _previousTrajectoryPresentImages)
        {
            presentImage.gameObject.SetActive(value);
        }
    }

    public void PresentPreviousTrajectory(Trajectory trajectory)
    {
        PresentTrajectory(trajectory, _previousTrajectoryImages);
    }

    private void PresentTrajectory(Trajectory trajectory, IEnumerable<RectTransform> images)
    {
        var previousPosition = _trajectoryStartPosition + Vector3.up * 1.5f;

        var maxDistance = 0f;
        
        maxDistance = Tower.AliveArchers.Aggregate(maxDistance, (current, targetAliveArcher) => Mathf.Max(current, (_trajectoryStartPosition - targetAliveArcher.transform.position).magnitude));

        var distanceAdditionMultiplayer = maxDistance / 12;
        
        foreach (var trajectoryImage in images)
        {
            trajectoryImage.position = previousPosition + trajectory.GetNextPositionAdditionByTime(0.075f) * distanceAdditionMultiplayer;

            previousPosition = trajectoryImage.position;
        }
    }

    private void CheckEqualityPreviousAndNewTrajectory(Trajectory newTrajectory)
    {
        if (newTrajectory.EqualsByRange(2, 0.05f, new(new Vector2(Mathf.Cos(_deg2Rad), Mathf.Sin(_deg2Rad)), 1, 15)))
        {
            _previousPlayerTrajectory.Highlight();
        }
        else
        {
            _previousPlayerTrajectory.Unhighlight();
        }
    }

    public void EnablePreviousTrajectory()
    {
        foreach (var presentImage in _previousTrajectoryImages)
        {
            presentImage.gameObject.SetActive(true);
            presentImage.transform.SetParent(transform.parent);
        }
    }
    
    public void DisablePreviousTrajectory()
    {
        foreach (var presentImage in _previousTrajectoryImages)
        {
            presentImage.gameObject.SetActive(false);
            presentImage.transform.SetParent(transform);
        }
    }
}