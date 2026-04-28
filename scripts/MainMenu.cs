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
		NewGame(5, 5);
	}

	public void NewGameMedium()
	{
		NewGame(6, 10);
	}

	public void NewGameLarge()
	{
		NewGame(10, 12);
	}

	public void NewGame(int width, int height)
	{
		ui.Hide();

		var scene = GD.Load<PackedScene>("res://scenes/main.tscn");
		var instance = (Main)scene.Instantiate();
		// instance.Width = width;
		// instance.Height = height;
		AddChild(instance);
		instance.StartGame(width, height);
		// instance.reset
	}
}
