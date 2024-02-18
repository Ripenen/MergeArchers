using UnityEngine;

public struct Trajectory
{
    private Vector3 _velocity;

    private static readonly Vector3 Gravity = Vector3.down;
    private float _speed;

    public Trajectory(Vector3 direction, float power, float speed)
    {
        _velocity = direction.normalized * power;
        _speed = speed;
    }

    public Vector3 GetNextPositionAdditionInFrame()
    {
        return GetNextPositionAdditionByTime(Time.deltaTime);
    }

    public Vector3 GetNextPositionAdditionByTime(float time)
    {
        var result = _velocity * (time * _speed);

        _velocity += Gravity * time;
        _speed -= 1.25f * time;
        
        return result;
    }

    public override string ToString()
    {
        return $"{_velocity} : {_speed}";
    }

    public bool EqualsByRange(float angleRange, float powerRange, Trajectory trajectory)
    {
        if (_velocity == Vector3.zero || trajectory._velocity == Vector3.zero)
            return false;
        
        return Vector3.Angle(_velocity, trajectory._velocity) <= angleRange 
               && Mathf.Abs(_velocity.magnitude - trajectory._velocity.magnitude) <= powerRange;
    }
}