using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class MergingState : State, ITickable
{
    private readonly PlayerTower _playerTower;
    private readonly Camera _camera;
    private readonly GameFactory _factory;
    private Transform _mergePointer;
    private readonly Transform _mergePointerTemplate;
    private readonly ParticleSystem _mergeEffectTemplate;
    private readonly GameObject _hand;

    private Archer _selectedArcher;
    private AudioSource _source;
    private ArcherPlaceCell _selectedPlaceCell;
    private ArcherPlaceCell _highlightedPlaceCell;

    public MergingState(PlayerTower playerTower, Camera camera, GameFactory factory, Transform mergePointer, ParticleSystem mergeEffectTemplate, GameObject hand)
    {
        _playerTower = playerTower;
        _camera = camera;
        _factory = factory;
        _mergePointerTemplate = mergePointer;
        _mergeEffectTemplate = mergeEffectTemplate;
        _hand = hand;
    }
    
    public override void Enter()
    {
        _mergePointer = Object.Instantiate(_mergePointerTemplate);
        
        Physics.queriesHitTriggers = true;
        
        foreach (var cell in _playerTower.Cells)
        {
            cell.CalculateScreenPosition(_camera);
            cell.MouseDownOnArcher += SelectArcher;
        }

        _mergePointer.gameObject.SetActive(false);
    }

    private void SelectArcher(ArcherPlaceCell archerPlaceCell)
    {
        _selectedArcher = archerPlaceCell.PlacedArcher;
        _selectedPlaceCell = archerPlaceCell;

        Sound.Play(Sound.Sounds.SelectUnit);
        
        _mergePointer.gameObject.SetActive(true);
        _mergePointer.position = archerPlaceCell.Position;
    }

    public void Tick()
    {
        if(!Input.GetMouseButton(0)) 
            MoveArchersToCells();

        if (_selectedArcher is not null)
        {
            _highlightedPlaceCell = GetSelectedPlaceCell(_playerTower, Input.mousePosition, _highlightedPlaceCell);
            
            MoveTowards(_selectedArcher.transform, _highlightedPlaceCell.Position, 10, Vector3.up * 2);
            MoveTowards(_mergePointer, _highlightedPlaceCell.Position, 10);
            
            if (Input.GetMouseButtonUp(0))
            {
                if (_highlightedPlaceCell.HasArcher == false || _highlightedPlaceCell.PlacedArcher == _selectedArcher)
                {
                    if (_hand.activeInHierarchy)
                    {
                        _selectedArcher = null;
                        _mergePointer.gameObject.SetActive(false);
                        return;
                    }
                    
                    _selectedPlaceCell.MakeFree();
                    _highlightedPlaceCell.MakeBusy(_selectedArcher);
                }
                else if (_highlightedPlaceCell.PlacedArcher.Level == _selectedArcher.Level && _highlightedPlaceCell != _selectedPlaceCell)
                {
                    UniteArchers(_highlightedPlaceCell);
                }
                else
                {
                    if (_hand.activeInHierarchy)
                    {
                        _selectedArcher = null;
                        _mergePointer.gameObject.SetActive(false);
                        return;
                    }
                    
                    _selectedPlaceCell.MakeBusy(_highlightedPlaceCell.PlacedArcher);
                    _highlightedPlaceCell.MakeBusy(_selectedArcher);
                }
                
                _selectedArcher = null;
                _mergePointer.gameObject.SetActive(false);
            }
        }
    }

    public void UniteArchers(ArcherPlaceCell unitePlaceCell)
    {
        if(!_factory.HasArcherLevel(unitePlaceCell.ArcherLevel + 1))
            return;
        
        if(_source && _source.isPlaying)
            Object.Destroy(_source);
            
        _source = Sound.Play(Sound.Sounds.ArcherUpgrade);

        var mergeEffect = Object.Instantiate(_mergeEffectTemplate, unitePlaceCell.transform);
        
        mergeEffect.Play();
        Object.Destroy(mergeEffect.gameObject, mergeEffect.main.duration);

        if (_selectedPlaceCell && _selectedArcher)
        {
            _selectedPlaceCell.MakeFree();

            _selectedArcher.Destroy();   
        }

        unitePlaceCell.PlacedArcher.Destroy();

        unitePlaceCell.UpLevel();

        _playerTower.Reload(_factory);
        
        _hand.gameObject.SetActive(false);
        _hand.transform.DOKill();
    }

    private void MoveArchersToCells()
    {
        foreach (var cell in _playerTower.Cells)
            if (cell.HasArcher)
                MoveTowards(cell.PlacedArcher.transform, cell.Position, 10);
    }

    public override void Exit()
    {
        Physics.queriesHitTriggers = false;

        foreach (var cell in _playerTower.Cells)
        {
            cell.MouseDownOnArcher -= SelectArcher;
        }
        
        Object.Destroy(_mergePointer.gameObject);
    }

    private static void MoveTowards(Transform movable, Vector3 target, float speed, Vector3 offset = default)
    {
        movable.position = Vector3.MoveTowards(movable.position,
            target + offset, speed * Time.deltaTime);
    }

    private ArcherPlaceCell GetSelectedPlaceCell(PlayerTower playerTower, Vector2 screenPoint, ArcherPlaceCell selected)
    {
        ArcherPlaceCell nearCell = playerTower.Cells.First();
         
        foreach (var cell in playerTower.Cells)
        {
            if ((nearCell.ScreenPosition - screenPoint).sqrMagnitude >=
                (cell.ScreenPosition - screenPoint).sqrMagnitude)
            {
                nearCell = cell;
            }
        }

        if (nearCell != selected)
            Sound.Play(Sound.Sounds.CreateUnit);

        return nearCell;
    }
}