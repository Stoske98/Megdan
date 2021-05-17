public class EndGame : GameState
{
    public override void Enter()
    {
        GameManager.Instance.canvasWin.SetActive(true);
    }

    public override void Execute()
    {
    }

    public override void Exit()
    {
    }
}

