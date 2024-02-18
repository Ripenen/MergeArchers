public class GameUI
{
    public readonly InGameView InGameView;
    public readonly PlayerAimView PlayerAimView;

    public GameUI(InGameView inGameView, PlayerAimView playerAimView)
    {
        InGameView = inGameView;
        PlayerAimView = playerAimView;
    }
}