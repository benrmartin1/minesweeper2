using Godot;

public partial class Main : Node
{
	// Node references
	private Label _timerLabel;

	public int Width;
	public int Height;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timerLabel = GetNode<Label>("TimerLabel");
		Board board = GetNode<Board>("Board");
		board.UpdateTimer += OnTimerUpdated;
		OnResetButtonPressed();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnResetButtonPressed()
	{
		GetNode<Board>("Board").Reset(Width, Height);
	}

	private void OnTimerUpdated(int newTime)
	{
		_timerLabel.Text = newTime.ToString();
	}
}
