using Godot;
using System;

public class Minion : KinematicBody2D
{
	enum MinionState {
		Spawn,
		Idle,
		Death
	}

	private MinionState state = MinionState.Spawn;
	private AnimatedSprite minionAnimation = null;
	private Boss boss = null;
	private int minionHP = 20;
	private float direction = 0;
	private int cd = 20;
	private int hitFrame = 7;

	// Bullet
	private PackedScene proj = null;

	private void SpawnBullet(Vector2 pos, float dir, int spd) {
		EBullet bullet = (EBullet) proj.Instance();
		bullet.dir = dir;
		bullet.Position = pos;
		bullet.speed = spd;
		GetParent().AddChild(bullet);
	}

	public override void _Ready()
	{
		minionAnimation = GetNode<AnimatedSprite>("AnimatedSprite");
		boss = GetNode<Boss>("../Boss");
		proj = GD.Load<PackedScene>("res://Bullet/EBullet.tscn");
	}

	public override void _Process(float delta)
	{
		switch (state) {
			case MinionState.Spawn:
				minionAnimation.Play("Spawn");
				if (minionAnimation.Frame > 3) {
					state = MinionState.Idle;
				}
				break;

			case MinionState.Idle:
				minionAnimation.Play("Idle");
				if (minionHP <= 0) {
					state = MinionState.Death;
				}

				if (cd <= 0) {
					direction += (float) ((double) 20 / 180 * Math.PI);
					SpawnBullet(Position, direction, 1);
					cd = 20;
				} else {
					cd--;
				}

				if (hitFrame <= 0) {
					minionAnimation.Modulate = new Color(1, 1, 1, 1);
					hitFrame = 7;
				} else {
					hitFrame--;
				}

				break;

			case MinionState.Death:
				minionAnimation.Play("Death");
				if (minionAnimation.Frame > 3) {
					boss.count--;
					QueueFree();
				}
				break;
		}
	}
	
	private void _on_Area2D_area_entered(object area)
	{
		minionAnimation.Modulate = new Color(1, 0, 0, 1);
		minionHP--;
	}
}



