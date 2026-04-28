using Godot;

// Class containing the board, timer, and other UI outside the board
public partial class Main : Node
{
	// Node references
	private Label _timerLabel;
	private Label _flagLabel;
	private Board _board;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timerLabel = GetNode<Label>("CanvasLayer/TimerLabel");
		_flagLabel = GetNode<Label>("CanvasLayer/FlagLabel");
		_board = GetNode<Board>("CanvasLayer/BoardControl/Board");
		_board.UpdateTimer += OnTimerUpdated;
		_board.UpdateFlagCount += OnFlagCountUpdated;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartGame(int width, int height, int mines)
	{
		_board.SetupBoard(width, height, mines);
	}

	public void SetZoomLevel(float zoom)
	{

		// Control boardControl = GetNode<Control>("CanvasLayer/BoardControl");
		// boardControl.Scale = new Vector2(zoom, zoom);

		_board.Scale = new Vector2(zoom, zoom);

	}

	private void OnTimerUpdated(int newTime)
	{
		_timerLabel.Text = newTime.ToString();
	}

	private void OnFlagCountUpdated(int newFlagCount)
	{
		// Flag count is # of mines - # of flags
		_flagLabel.Text = newFlagCount.ToString();
	}
}
