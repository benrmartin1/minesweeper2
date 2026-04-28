using Godot;

// Class containing the board, timer, and other UI outside the board
public partial class Main : Node
{
	// Node references
	private Label _timerLabel;
	private Label _flagLabel;
	private Board _board;

	public int Width; //todo make private/local?
	public int Height;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timerLabel = GetNode<Label>("TimerLabel");
		_flagLabel = GetNode<Label>("FlagLabel");
		_board = GetNode<Board>("Board");
		_board.UpdateTimer += OnTimerUpdated;
		_board.UpdateFlagCount += OnFlagCountUpdated;
		_board.SetupBoard(Width, Height);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartGame(int width, int height)
	{
		Width = width;
		Height = height;
		_board.SetupBoard(Width, Height);
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
