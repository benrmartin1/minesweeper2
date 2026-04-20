using System;
using Godot;

public partial class Board : Node2D
{
	[Export]
	public PackedScene TileScene { get; set; }

	private int xOffset = 64*1;
	private int yOffset = 64*2;
	private Tile[,] tilesArr;

	// Timer
	[Signal]
	public delegate void UpdateTimerEventHandler(int newTime);
	private bool gameRunning = false;
	private double gameTimer;
	private int gameTimerInt;

	// Save board size between rounds for quick reset
	private int _width;
	private int _height;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Generate(3, 4, 3);
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

	public void Reset(int width, int height)
	{
		GetTree().CallGroup("tiles", Node.MethodName.QueueFree);
		gameRunning = false;
		gameTimer = gameTimerInt = 0;
		EmitSignal(SignalName.UpdateTimer, gameTimerInt);

		float ratio = 0.15f;
		// float ratio = GD.Randf();
		int mineCount = (int)Math.Floor(width * height * ratio);
		
		Generate(width, height, mineCount);
	}

	private void Generate(int width, int height, int mineCount)
	{
		GD.Print("Generating board with size ", width, "×", height, ", with ", mineCount, "mines");
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
				tile.Position = new Vector2(x * 64 + xOffset, y * 64 + yOffset);
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
					for (int relX = Math.Max(0, x-1); relX <= Math.Min(x+1, width-1); relX++)
					{
						for (int relY = Math.Max(0, y-1); relY <= Math.Min(y+1, height-1); relY++)
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
	}

	// Called when reseting from within the game (not main menu)
	private void OnResetButton()
	{
		Reset(_width, _height);
	}
}
