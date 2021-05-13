using Godot;
using System;

public class Player : KinematicBody2D
{
	enum PlayerState {
		Move,
		Roll,
		Death
	};

	// Player Vars
	private AnimatedSprite playerAnimation = null;
	private Area2D collision = null;
	private Label label = null;
	private PlayerState state = PlayerState.Move;
	private Vector2 velocity = new Vector2();
	private bool isShooting = false;
	private int cd = 10;
	private int shootTimer = 10;
	private int hitCD = 60;
	[Export] int playerHP = 6;
	[Export] int speed = 100;
	[Export] int acceleration = 30;


	// Bullet
	private PackedScene proj = null;

	// Returns magnitude of direction X
	private int GetInputX() {
		return (int) (Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"));
	}

	// Returns magnitude of direction Y
	private int GetInputY() {
		return (int) (Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up"));
	}

	// Determines velocity of player
	private void PlayerMove(float delta) {
		velocity = velocity.MoveToward(new Vector2(GetInputX(), GetInputY()).Normalized() * speed, acceleration);
	}

	// Prevents player from going out of bounds
	private void BoundPlayer() {
		float posX = Position.x;
		float posY = Position.y;

		if (Position.x <= 0) {
			posX = 0;
		} else if (Position.x >= 640) {
			posX = 640;
		}

		if (Position.y <= 0) {
			posY = 0;
		} else if (Position.y >= 360) {
			posY = 360;
		}

		Position = new Vector2(posX, posY);
	}

	private float GetDirectionBetweenMouseAndPlayer() {
		float disX = GetGlobalMousePosition().x - GlobalPosition.x;
		float disY =  GetGlobalMousePosition().y - GlobalPosition.y;
		return (float) Math.Atan2(disY, disX);
	}

	private void SpawnBullet() {
		Bullet bullet = (Bullet) proj.Instance();
		bullet.dir = GetDirectionBetweenMouseAndPlayer();
		bullet.Position = Position;
		GetParent().AddChild(bullet);
	}

	public override void _Ready() {
		playerAnimation = GetNode<AnimatedSprite>("AnimatedSprite");
		collision = GetNode<Area2D>("Area2D");
		label = GetNode<Label>("Label");
		label.SetAsToplevel(true);
		label.Text = "HP: " + playerHP.ToString();
		label.SetPosition(new Vector2(10, 10));
		proj = GD.Load<PackedScene>("res://Bullet/Bullet.tscn");
	}

	public override void _Process(float delta) {
		label.Text = "HP: " + playerHP.ToString();
		// Transition State
		switch (state) {
			case PlayerState.Move:
				if (playerHP <= 0) {
					state = PlayerState.Death;
				}

				PlayerMove(delta);
				velocity = MoveAndSlide(velocity);
				if (Input.IsActionJustPressed("ui_space")) {
					state = PlayerState.Roll;
				} 

				if (Input.IsActionPressed("ui_shoot")) {
					isShooting = true;
				} else {
					isShooting = false;
				}

				if (isShooting) {
					shootTimer++;
					if (shootTimer >= cd) {
						shootTimer = 0;
						SpawnBullet();
					}
				} else {
					shootTimer = 10;
				}

				if (hitCD <= 0) {
					playerAnimation.Modulate = new Color(1, 1, 1, 1);
					collision.SetCollisionLayerBit(1, true);
				} else {
					hitCD--;
				}
				
				break;
			
			case PlayerState.Roll:
				velocity = MoveAndSlide(velocity);
				collision.SetCollisionLayerBit(1, false);
				
				if (playerAnimation.Frame > 5) {
					state = PlayerState.Move;
				}
				break;

			case PlayerState.Death:
				if (playerAnimation.Frame > 4) {
					QueueFree();
					GetTree().ReloadCurrentScene();
				}
				break;
		}

		// Animation State
		switch (state) {
			case PlayerState.Move:
				if (velocity == Vector2.Zero) {
					if (isShooting) {
						playerAnimation.Play("ShootIdle");
					} else {
						playerAnimation.Play("Idle");
					}
					
				} else {
					if (isShooting) {
						playerAnimation.Play("ShootRun");
					} else {
						playerAnimation.Play("Run");
					}
				}

				if (velocity.x > 0) {
					playerAnimation.FlipH = false;
				} else if (velocity.x < 0) {
					playerAnimation.FlipH = true;
				}
				break;
			
			case PlayerState.Roll:
				playerAnimation.Play("Roll");
				break;
			
			case PlayerState.Death:
				playerAnimation.Play("Death");
				break;
		}

		BoundPlayer();
		if (Input.IsActionPressed("ui_cancel")) {
			GetTree().Quit();
		}
	}

	private void _on_Area2D_area_entered(object area)
	{
		hitCD = 60;
		collision.SetCollisionLayerBit(1, false);
		playerHP--;
		playerAnimation.Modulate = new Color(1, 0, 0, 0.5f);
		GD.Print(playerHP);
		
	}
}
