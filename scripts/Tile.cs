using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Tile : Node2D
{
	[Signal]
	public delegate void TileRevealedEventHandler();
	[Signal]
	public delegate void MineRevealedEventHandler();
	[Signal]
	public delegate void TileFlaggedEventHandler();
	[Signal]
	public delegate void TileUnflaggedEventHandler();

	private int _neighborMineCount = 0;
	public bool isMine = false;
	public bool isFlagged = false;
	public bool isRevealed = false;

	private List<Tile> neighbors = [];
	private bool isPressing = false;
	private double pressTimeMs = 0;
	private const double longPressTimeMs = 0.5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// isBomb = GD.Randi() % 2 == 0;

		// if (isBomb)
		// {
		// 	var image = Image.LoadFromFile("res://sprites/bomb.png");
		// 	GetNode<Sprite2D>("Sprite2D").Texture = ImageTexture.CreateFromImage(image);
		// }

		// if (isBomb)
		// {

		// }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isPressing)
		{
			pressTimeMs += delta;
			// GD.Print("is presssing", pressTimeMs);
			if (pressTimeMs >= longPressTimeMs)
			{
				GD.Print("long press activated");
				Input.VibrateHandheld(200, 0.05f); // 1000 or lower creates no vibration on my phone :(
				isPressing = false;
				Flag();
			}
		}
	}

	public void InitBomb()
	{
		isMine = true;
	}

	public void InitEmpty(int mineCount)
	{
		isMine = false;
		// GD.Print("mine count:", mineCount);

		if (mineCount < 0 || mineCount > 8)
		{
			GD.PrintErr("Bad mines number");
			return;
		}
		_neighborMineCount = mineCount;
	}

	public void OnControlGuiInput(InputEvent inputEvent)
	{
		if (isRevealed)
		{
			return;
		}

		// if (inputEvent is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right)
		// {

		// }
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			GD.Print(mouseEvent, mouseEvent.Pressed, mouseEvent.ButtonIndex);
			if (mouseEvent.Pressed)
			{
				if (mouseEvent.ButtonIndex == MouseButton.Left)
				{
					isPressing = true;
					pressTimeMs = 0;
					GD.Print("pressing...");
				}
				else if (mouseEvent.ButtonIndex == MouseButton.Right)
				{
					GD.Print("right click...");
					Flag();
				}
			}
			else if (mouseEvent.ButtonIndex == MouseButton.Left)
			{
				// if >= long press was already activated in _Process
				if (pressTimeMs < longPressTimeMs)
				{
					Reveal(false);
				}
				isPressing = false;
				pressTimeMs = 0;
			}
		}

		// else if (inputEvent is InputEventScreenTouch touchEvent)
		// {
		// 	GD.Print(touchEvent);
		// 	if (touchEvent.Pressed)
		// 	{
		// 		Reveal();

		// 	}
		// 	else if (touchEvent.DoubleTap)
		// 	{
		// 		Flag();
		// 	}
		// }
	}

	public void AddNeighbor(Tile tile)
	{
		neighbors.Add(tile);
	}

	public void Reveal(bool forced)
	{
		if (isRevealed || (isFlagged && !forced))
		{
			// ignore reveals for revealed or flagged tiles
			return;
		}
		isRevealed = true;
		if (isFlagged)
		{
			isFlagged = false;
			EmitSignal(SignalName.TileUnflagged);
		}

		if (isMine)
		{
			var texture = ResourceLoader.Load<Texture2D>("res://sprites/bomb.png");
			GetNode<Sprite2D>("Sprite2D").Texture = texture;
			EmitSignal(SignalName.MineRevealed);
		}
		else
		{
			var texture = ResourceLoader.Load<Texture2D>($"res://sprites/{_neighborMineCount}.png");
			GetNode<Sprite2D>("Sprite2D").Texture = texture;

			if (_neighborMineCount == 0)
			{
				foreach (Tile neighbor in neighbors)
				{
					neighbor.Reveal(true);
				}
			}
			EmitSignal(SignalName.TileRevealed);
		}
	}

	public void Flag()
	{
		if (isFlagged)
		{
			var texture = ResourceLoader.Load<Texture2D>("res://sprites/empty.png");
			GetNode<Sprite2D>("Sprite2D").Texture = texture;
			EmitSignal(SignalName.TileUnflagged); 
		}
		else
		{
			var texture = ResourceLoader.Load<Texture2D>("res://sprites/flag.png");
			GetNode<Sprite2D>("Sprite2D").Texture = texture;
			EmitSignal(SignalName.TileFlagged); 
		}
		isFlagged = !isFlagged;
	}
}
