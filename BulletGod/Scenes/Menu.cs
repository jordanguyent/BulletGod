using Godot;
using System;

public class Menu : Node2D
{
	public override void _Process(float delta)
	{
		if (Input.IsActionPressed("ui_accept")) {
			GetTree().ChangeScene("res://Scenes/World.tscn");
		} else if (Input.IsActionPressed("ui_cancel")) {
			GetTree().Quit();
		}
	}
}
