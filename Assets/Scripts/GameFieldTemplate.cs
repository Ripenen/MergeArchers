using System.Collections.Generic;
using UnityEngine;

public class GameFieldTemplate : MonoBehaviour
{
    [SerializeField] private Transform _playerTowerPosition;
    [SerializeField] private Transform _levelTowerPosition;
    [SerializeField] private int _id;
    
    public Transform PlayerTowerPosition => _playerTowerPosition;
    public Transform LevelTowerPosition => _levelTowerPosition;

    public int ID => _id;
}