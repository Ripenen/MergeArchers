using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ShootState : State, ITickable
{
    private readonly Tower _shooter;
    private readonly Transform _camera;
    private readonly Camera _camera1;
    private readonly IDataProvider<Trajectory> _trajectory;
    private readonly Tower _target;
    private readonly KeysView _view;

    private float _minCameraHeight;
    private float _maxX;
    private float _minX;
    private float _distanceAdditionMultiplayer;

    private Trajectory _shootTrajectory;

    public ShootState(Tower shooter, Camera camera, IDataProvider<Trajectory> trajectory, Tower target, KeysView view)
    {
        _shooter = shooter;
        _camera = camera.transform;
        _camera1 = camera;
        _trajectory = trajectory;
        _target = target;
        _view = view;
    }

    public override void Enter()
    {
        _shootTrajectory = _trajectory.Data;
        
        if(!_shooter.AliveArchers.Any())
            StateMachine.TransitToNext();

        var transformParent = _shooter.AliveArchers.First().transform;
        var maxDistance = 0f;
        var position = _shooter.Center;
       
        foreach (var archer in _shooter.AliveArchers)
        {
            var zOffset = 0.25f;

            foreach (var arrow in archer.Projectiles)
            {
                var transform = arrow.transform;

                arrow.KeysView = _view;

                var newPosition = transform.position;
                newPosition += new Vector3(0, 0.5f, zOffset);

                arrow.Camera = _camera1;

                transform.position = newPosition;

                arrow.SetColliderActive(true);

                arrow.transform.parent = transformParent;

                zOffset = Random.Range(-3f, 3f);
            }
        }
        
        maxDistance = _target.AliveArchers.Aggregate(maxDistance, (current, targetAliveArcher) => Mathf.Max(current, (position - targetAliveArcher.transform.position).magnitude));

        _distanceAdditionMultiplayer = maxDistance / 12;

        var positionWithOffset = position + new Vector3(-8f, 5f, -8f);
        
        _camera.position = positionWithOffset;

        _minCameraHeight = 10;

        _maxX = Mathf.Max(_shooter.Position.x - 5, _target.Position.x - 5);
        _minX = Mathf.Min(_shooter.Position.x - 5, _target.Position.x - 5);

        _camera.DOLookAt(position, 0.5f);
    }

    public override void Exit()
    {
        foreach (var archer in _shooter.AliveArchers)
        {
            if(!archer)
                continue;
            
            archer.SetColliders(true);
        }
    }

    public void Tick()
    {
        var d = _distanceAdditionMultiplayer - 1.25f * Time.deltaTime;
        var addition = _shootTrajectory.GetNextPositionAdditionInFrame() * d;
        
        int collidedArrows = 0;
        int allArrows = 0;

        addition.z = 0;

        foreach (var archer in _shooter.AliveArchers)
        {
            if(!archer)
                continue;
            
            var scale = 1f;
            
            foreach (var projectile in archer.Projectiles)
            {
                allArrows++;
                
                SetupArrowZOffset(projectile);

                if (projectile.Collided)
                    collidedArrows++;
                else
                    projectile.Translate(addition * scale);
                
                scale += Random.Range(-0.25f, 0.25f);
            }
        }
        
        addition = ClampAddition(addition);

        _camera.position += addition;

        if(collidedArrows == allArrows && !_target.AwaitShoot)
            StateMachine.TransitToNext();
    }

    private void SetupArrowZOffset(Projectile arrow)
    {
        var archer1 = _target.AliveArchers.FirstOrDefault();

        if (!archer1) 
            return;
        
        foreach (var targetAliveArcher in _target.AliveArchers)
        {
            if ((targetAliveArcher.transform.position - arrow.Position).sqrMagnitude <
                (archer1.transform.position - arrow.Position).sqrMagnitude)
                archer1 = targetAliveArcher;
        }

        arrow.ZOffset = (archer1.transform.position - arrow.Position).normalized.z * 0.25f;
    }

    private Vector3 ClampAddition(Vector3 addition)
    {
        if (_camera.position.y <= _minCameraHeight && addition.y < 0)
            addition.y = 0;

        if ((addition.x > 0 && _camera.position.x >= _maxX) || (addition.x < 0 && _camera.position.x <= _minX))
            addition.x = 0;
        
        return addition;
    }
}