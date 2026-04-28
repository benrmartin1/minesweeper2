using Godot;
using System;

public partial class MainMenu : Node2D
{
	CanvasLayer ui;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ui = GetNode<CanvasLayer>("CanvasLayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void NewGameSmall()
	{
		// NewGame(5, 5);
		NewGame(8, 8, 10, 1.2f);
	}

	public void NewGameMedium()
	{
		// NewGame(6, 10);
		NewGame(16, 16, 40, 0.65f);
	}

	public void NewGameLarge()
	{
		NewGame(30, 16, 99, 0.35f);
	}

	public void NewGame(int width, int height, float zoom)
	{
		float ratio = 0.15f;
		int mines = (int)Math.Floor(width * height * ratio);
		// float ratio = GD.Randf();
		NewGame(width, height, mines, zoom);
	}

	public void NewGame(int width, int height, int mines, float zoom)
	{
		ui.Hide();

		var scene = GD.Load<PackedScene>("res://scenes/main.tscn");
		var instance = (Main)scene.Instantiate();
		// instance.Width = width;
		// instance.Height = height;
		AddChild(instance);
		instance.StartGame(width, height, mines);
		instance.SetZoomLevel(zoom);
		// instance.reset
	}
}
