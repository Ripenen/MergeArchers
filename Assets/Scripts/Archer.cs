using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum ArcherType
{
    Player,
    Enemy
}

public class Archer : MonoBehaviour, IPointerDownHandler, IDamageable
{
    [SerializeField] private ArcherAnimator _animator;
    [SerializeField] private Weapon _bow;
    [SerializeField] private Weapon _gun;
    [SerializeField] private ArcherSkeleton _skeleton;
    [SerializeField] private Collider _trigger;
    [SerializeField] private int _health;
    [SerializeField] private GameObject _shield;

    private Weapon _activeWeapon;

    private bool _killed;

    public IEnumerable<Projectile> Projectiles => _activeWeapon.Projectiles;
    public bool Killed => _killed;

    public Weapon ActiveWeapon => _activeWeapon;

    public event Action MouseDown;

    [HideInInspector] public ArcherType ArcherType;
    [HideInInspector] public int Level;

    private void Start()
    {
        _skeleton.SetRagDoll(false);

        _skeleton.Init(this);
        
        if(!_bow && !_gun)
            return;
        
        _activeWeapon = _bow;
        
        if (ArcherType == ArcherType.Player)
        {
            _bow.gameObject.SetActive(true);
            _gun.gameObject.SetActive(false);
        }
    }

    public void SetColliders(bool value)
    {
        _skeleton.SetColliders(value);
        _trigger.enabled = value;
    }

    public void ActivateGun()
    {
        if(!_bow && !_gun)
            return;
        
        _bow.gameObject.SetActive(false);
        _gun.gameObject.SetActive(true);

        _activeWeapon = _gun;
    }
    
    public void ActivateBow()
    {
        if(!_bow && !_gun)
            return;
        
        _bow.gameObject.SetActive(true);
        _gun.gameObject.SetActive(false);

        _activeWeapon = _bow;
    }

    public void Idle()
    {
        if(_shield)
            _shield.SetActive(true);
        
        _animator.PlayIdleAnimation();
        
        if(_activeWeapon)
            _activeWeapon.ResetWeapon();
    }
    
    public void Walk()
    {
        _animator.PlayWalkAnimation();
    }

    public void StartAim()
    {
        if(_shield)
            _shield.SetActive(false);

        _animator.PlayAimAnimation();

        _skeleton.PrepareToAim(_activeWeapon, ArcherType);
    }

    public void AimPower(float power)
    {
        _skeleton.MoveBowHand(power);
        _activeWeapon.PowerChangeAnimation(power);
    }

    public void AimAngle(Vector3 forward)
    {
        _skeleton.SetBodyForward(forward);

        _activeWeapon.SetForward(forward);
    }

    public void TakeDamage()
    {
        _health -= 1;
        
        if (_health <= 0)
        {
            Kill();  
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MouseDown?.Invoke();
    }

    public void Kill()
    {
        _animator.SetActive(false);
        _skeleton.SetRagDoll(true);

        SetColliders(true);
        _trigger.enabled = false;

        if(!_killed)
            Sound.Play(Sound.Sounds.Death);
        
        _skeleton.AddForce(-transform.forward, 5);

        _killed = true;
        _skeleton.SetColor(Color.white * 0.5f);
    }

    public void SetColor(Color color)
    {
        _skeleton.SetColor(color);
    }
}