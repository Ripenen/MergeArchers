using UnityEngine;
using Zenject;


public class PlayerAimState : State, ITickable, IDataProvider<Trajectory>
{
    private readonly PlayerTower _playerTower;
    private readonly PlayerAimView _view;
    private readonly InGameView _inGameView;
    private readonly Camera _camera;
    
    private Vector3 _cameraStartPosition;
    private Vector2 _previousForwardAim = Vector2.right;
    private float _previousPower;

    private Vector2 _viewportOffsetPoint;
    private const float Speed = 15;

    public PlayerAimState(PlayerTower playerTower, PlayerAimView view, Camera camera, InGameView inGameView)
    {
        _playerTower = playerTower;
        _view = view;
        _inGameView = inGameView;
        _camera = camera;
    }

    public Trajectory Data { get; private set; }

    public override void Enter()
    {
        _view.Disable();

        _view.EnablePreviousTrajectory();
        
        _inGameView.SetBombActive(true);
        
        _cameraStartPosition = _camera.transform.position;
        
        _playerTower.DeactivateBombGun();
        
        _view.Setup(_playerTower.Center);

        foreach (var archer in _playerTower.AliveArchers)
        {
            archer.SetColliders(false);
        }
        
        MonoCached.LateTick += LateTick;
        
        TryStartAim();
    }

    private void LateTick()
    {
        if (Input.GetMouseButton(0) && _view.Active)
        {
            var offset = CalculateOffset(_viewportOffsetPoint);
            _previousPower = CalculatePower(offset, _viewportOffsetPoint, _previousPower);
            _previousForwardAim = CalculateForwardAim(offset, _previousForwardAim);

            var transform = _camera.transform;
            
            transform.position = Vector3.MoveTowards(transform.position, _cameraStartPosition + transform.forward * (1 * (1 - _previousPower)),
                1 * Time.deltaTime);

            _camera.fieldOfView = Mathf.Lerp(90, 100, _previousPower);

            foreach (var playerArcher in _playerTower.AliveArchers)
            {
                playerArcher.AimPower(_previousPower);
                playerArcher.AimAngle(_previousForwardAim);
            }
            
            _view.Present(_previousForwardAim, _previousPower, Speed);
        }
    }

    public void Tick()
    {
        TryStartAim();

        if (Input.GetMouseButtonUp(0) && _view.Active)
        {
            Data = CalculateTrajectory();
            
            foreach (var playerArcher in _playerTower.AliveArchers)
            {
                playerArcher.Idle();
            }

            _view.Disable();
            _view.PresentPreviousTrajectory(Data);
            _view.SetActivePreviousTrajectory(true);

            Sound.Play(_playerTower.IsBow ? Sound.Sounds.Shoot : Sound.Sounds.GunShoot);

            StateMachine.TransitToNext();
        }
    }

    private void TryStartAim()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckValidAimArea() && !_view.Active)
            {
                _viewportOffsetPoint = ConvertScreenToViewportPoint(Input.mousePosition);
            
                foreach (var playerArcher in _playerTower.AliveArchers)
                {
                    playerArcher.StartAim();
                }
            
                _inGameView.SetBombActive(false);
                
                if(_playerTower.IsBow)
                    Sound.Play(Sound.Sounds.BowCharge);

                _view.Enable();
            }
        }
    }

    public override void Exit()
    {
        MonoCached.LateTick -= LateTick;
        
        _inGameView.SetBombActive(false);
        
        _camera.fieldOfView = 90;
        
        _view.DisablePreviousTrajectory();
    }

    private static Vector2 CalculateOffset(Vector2 offset)
    {
        var mouseViewportPoint = ConvertScreenToViewportPoint(Input.mousePosition);

        return mouseViewportPoint - offset;
    }

    private static Vector2 ConvertScreenToViewportPoint(Vector2 screenPoint)
    {
        if(Screen.width > Screen.height)
            return new Vector2(screenPoint.x / Screen.width,
                screenPoint.y / Screen.height);
        
        return new Vector2(screenPoint.x / Screen.width,
            screenPoint.y / Screen.height);
    }

    private static bool CheckValidAimArea()
    {
        var viewportPoint = ConvertScreenToViewportPoint(Input.mousePosition);
        
        return viewportPoint.x is >= 0 and < 0.8f && viewportPoint.y is >= 0 and < 0.9f;
    }

    private static float CalculatePower(Vector2 offset, Vector2 offsetPoint, float previousPower)
    {
        var range = 0.15f;

        range = Mathf.Clamp(range, 0, offsetPoint.x);
        range = Mathf.Clamp(range, 0, offsetPoint.y);

        var power = Mathf.Clamp(offset.magnitude, 0, range) / range;

        var t = 0.075f;

        if (Screen.width > Screen.height)
            t = 0.15f;
        
        return Mathf.Lerp(previousPower, power, t);
    }

    private static Vector2 CalculateForwardAim(Vector2 offset, Vector2 previous)
    {
        var forward = -offset.normalized;
            
        if(forward.x < 0)
            forward = Vector2.Reflect(-forward, Vector2.down);
        
        forward.y *= 1.5f;
        forward.x /= 1.5f;
        
        return Vector2.Lerp(previous, forward, 0.1f);
    }
    
    private Trajectory CalculateTrajectory()
    {
        var offset = CalculateOffset(_viewportOffsetPoint);
        var power = CalculatePower(offset, _viewportOffsetPoint, _previousPower);
        var forward = CalculateForwardAim(offset, _previousForwardAim);

        return new Trajectory(forward, power, Speed);
    }
}