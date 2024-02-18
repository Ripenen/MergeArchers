using System.Collections;
using DG.Tweening;
using RayFire;
using UnityEngine;

public class Balloon : MonoBehaviour, IDamageable
{
    [SerializeField] private int _health;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _scaler;
    [SerializeField] private Collider _collider;
    [SerializeField] private ArcherPlaceCell _cell;
    [SerializeField] private ParticleSystem _explosion;
    
    [HideInInspector] public TowerTemplate _tower;
    public bool Dyeing { get; private set; }

    public Archer PlacedArcher => _cell.PlacedArcher;

    private Vector3 _scale;

    private void Start()
    {
        _scale = _scaler.localScale;
    }

    public void TakeDamage()
    {
        _health -= 1;

        if (_health <= 0)
        {
            _rigidbody.isKinematic = false;

            _scaler.DOScale(Vector3.one * 0.25f, 2).OnComplete(() =>
            {
                _scaler.gameObject.SetActive(false);
                _collider.enabled = false;
            });

            _cell.PlacedArcher.Kill();

            Dyeing = true;
        }
    }

    public void Reload()
    {
        _scaler.DOKill();
        
        _rigidbody.isKinematic = true;
        _scaler.gameObject.SetActive(true);
        _collider.enabled = true;
        _health = 3;
        
        Dyeing = false;
        
        _scaler.transform.localScale = _scale;
        
        _explosion.gameObject.SetActive(true);
        gameObject.SetActive(true);
        
        StopAllCoroutines();
    }

    private IEnumerator OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Arrow>(out var a) 
           || (collision.gameObject.TryGetComponent<RagDollCollider>(out var rf) && rf.Owner.Killed))
            yield break;
        
        if(!_explosion.gameObject.activeSelf || _explosion.isPlaying)
            yield break;
        
        if(!_explosion.isPlaying)
            _explosion.Play();

        if (_tower.TryGetComponent<RayfireRigid>(out var rayfireRigid))
        {
            rayfireRigid.Demolish();
        }

        foreach (var cell in _tower.Cells)
        {
            if(cell.HasArcher && Vector3.Distance(cell.PlacedArcher.transform.position, transform.position) <= 5)
                cell.PlacedArcher.Kill();
        }

        yield return new WaitForSeconds(_explosion.main.duration);
        
        _explosion.gameObject.SetActive(false);
        gameObject.SetActive(false);
        Dyeing = false;
    }
}