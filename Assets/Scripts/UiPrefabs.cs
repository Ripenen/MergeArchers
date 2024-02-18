using UnityEngine;

[CreateAssetMenu(menuName = "Create UiPrefabs", fileName = "UiPrefabs", order = 0)]
public class UiPrefabs : ScriptableObject
{
    public Canvas CanvasTemplate;
    public PlayerAimView PlayerAimView;
    public InGameView InGameView;
    public LoseView LoseView;
    public WinView WinView;
    public TutorialView TutorialView;
    public MergeView MergeView;
    public SkinsView SkinsView;
    public ChestsView ChestsView;
    public ShopView ShopView;
    public MagicShop MagicShop;
}