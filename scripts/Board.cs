using System;
using Godot;

public partial class Board : Node2D
{
	[Signal]
	public delegate void UpdateTimerEventHandler(int newTime);
	[Signal]
	public delegate void UpdateFlagCountEventHandler(int newTime);

	// Local references
	[Export]
	public PackedScene TileScene { get; set; }
	private Control _gameOverPanel;
	private Tile[,] tilesArr;

	// private int xOffset = 64 * 1;
	// private int yOffset = 64 * 2;

	// Timer
	private bool gameRunning = false;
	private double gameTimer;
	private int gameTimerInt;

	// Save board size between rounds for quick reset
	private int _width;
	private int _height;
	private int _totalMineCount;

	// Current board state
	private int _tilesRevealed = 0;
	private int _tilesFlagged = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gameOverPanel = GetNode<Control>("CanvasLayer/GameOverPanel");
		_gameOverPanel.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (gameRunning)
		{
			gameTimer += delta;
			if ((int)gameTimer != gameTimerInt)
			{
				gameTimerInt = (int)gameTimer;
				EmitSignal(SignalName.UpdateTimer, gameTimerInt);
			}
		}
	}

	public void SetupBoard(int width, int height, int totalMineCount)
	{
		GetTree().CallGroup("tiles", Node.MethodName.QueueFree);
		gameRunning = false;
		gameTimer = gameTimerInt = 0;
		EmitSignal(SignalName.UpdateTimer, gameTimerInt);

		_tilesFlagged = _tilesRevealed = 0;
		_totalMineCount = totalMineCount;
		EmitSignal(SignalName.UpdateFlagCount, _totalMineCount);

		Generate(width, height, _totalMineCount);
		// Store current size for in-game resets
		_width = width;
		_height = height;
	}

	private void Generate(int width, int height, int mineCount)
	{
		GD.Print("Generating board with size ", width, "×", height, ", with ", mineCount, " mines");
		int minesRemaining = mineCount;
		int tilesRemaining = width * height;
		if (minesRemaining > tilesRemaining)
		{
			GD.PrintErr("Too many mines, setting minecount to tiles remaining");
			minesRemaining = tilesRemaining;
		}
		tilesArr = new Tile[width, height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Tile tile = TileScene.Instantiate<Tile>();
				tilesArr[x, y] = tile;
				// TODO: Separate bomb generation logic and tile drawing?
				// tile.Position = new Vector2(x * 64 + xOffset, y * 64 + yOffset);
				tile.Position = new Vector2(x * 64, y * 64);
				// GD.Print("created tile at ", tile.Position);

				float mineChance = (float)minesRemaining / tilesRemaining;
				bool isMine = mineChance >= GD.Randf();
				if (isMine)
				{
					tile.InitBomb();
					minesRemaining--;
				}
				tilesRemaining--;

				AddChild(tile);
				tile.AddToGroup("tiles");
				tile.TileRevealed += OnTileRevealed;
				tile.TileFlagged += OnTileFlagged;
				tile.TileUnflagged += OnTileUnflagged;
				tile.MineRevealed += OnMineRevealed;
			}
		}

		SetTileMineCounts(width, height);
	}

	private void SetTileMineCounts(int width, int height)
	{
		GD.Print("Setting tile mine counts for board with size ", width, "×", height);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				Tile tile = tilesArr[x, y];
				if (!tile.isMine)
				{
					int relativeMineCount = 0;
					for (int relX = Math.Max(0, x - 1); relX <= Math.Min(x + 1, width - 1); relX++)
					{
						for (int relY = Math.Max(0, y - 1); relY <= Math.Min(y + 1, height - 1); relY++)
						{
							// GD.Print("Checking mine:", relX, ", ", relY);
							if (tilesArr[relX, relY].isMine)
							{
								relativeMineCount++;
							}
							tile.AddNeighbor(tilesArr[relX, relY]);
						}
					}
					tile.InitEmpty(relativeMineCount);
				}
			}
		}
	}

	private void OnTileRevealed()
	{
		gameRunning = true;
		_tilesRevealed++;

		// Check game win conditions
		int numOfNonMines = _width * _height - _totalMineCount;
		if (_tilesRevealed == numOfNonMines)
		{
			gameRunning = false;
			GD.Print("you win");
			_gameOverPanel.Show();
			_gameOverPanel.GetNode<Label>("GameOverLabel").Text = "Winner!";
			string pluralSeconds = gameTimerInt == 1 ? "" : "s";
			_gameOverPanel.GetNode<Label>("ScoreLabel").Text =
				$"Finished game in {gameTimerInt} second{pluralSeconds}";
		}

		// for (int x = 0; x < _width; x++)
		// {
		// 	for (int y = 0; y < _height; y++)
		// 	{
		// 		Tile tile = tilesArr[x, y];
		// 		if (tile.isRevealed || tile.isMine)
		// 		{

					
		// 		}
		// 	}
		// }
	}
	private void OnMineRevealed()
	{
		GD.Print("Oh no it's a mine!");
		gameRunning = false;
		_gameOverPanel.Show();
		_gameOverPanel.GetNode<Label>("GameOverLabel").Text = "Kaboom :(";
		_gameOverPanel.GetNode<Label>("ScoreLabel").Text = "";

		// TODO Count flagged mines? probably not needed
		// string pluralSeconds = gameTimerInt == 1 ? "" : "s";
		// _gameOverPanel.GetNode<Label>("ScoreLabel").Text =
		// 	$"Flagged ?/{_totalMineCount} in {gameTimerInt} second{pluralSeconds}";
	}
	private void OnTileFlagged()
	{
		gameRunning = true; // Start timer even if user starts game with flag 
		_tilesFlagged++;
		EmitSignal(SignalName.UpdateFlagCount, _totalMineCount - _tilesFlagged);
	}

	private void OnTileUnflagged()
	{
		_tilesFlagged--;
		EmitSignal(SignalName.UpdateFlagCount, _totalMineCount - _tilesFlagged);
	}

	// Called when reseting from within the game (not main menu)
	private void OnResetButton()
	{
		SetupBoard(_width, _height, _totalMineCount);
		_gameOverPanel.Hide();
	}

	private void OnMainMenuButton()
	{
		GetTree().ChangeSceneToFile("res://scenes/main_menu.tscn");
	}
}
