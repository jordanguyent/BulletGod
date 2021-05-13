using Godot;
using System;

public class Boss : KinematicBody2D
{


	enum BossState {
		Idle_1,
		Move_1,
		IdleMove_2,
		Idle_2,
		MoveIdle_2,
		Idle_3,
		Vun_3,
		TwoToThree,
		Move_3,
		Death_4
	}

	private BossState state = BossState.Idle_1;

	// Player
	private Player player = null;

	// boss vars
	private AnimatedSprite bossAnimation = null;
	private Label label = null;
	private Area2D collision = null;
	private int bossHP = 800; // 800
	private int shootTimer1 = 20;
	private int shootTimer2 = 100;
	private int shootTimer3 = 100;
	private int shootTimer4 = 120;
	private int timer1 = 300;
	private float bossSpeed = 0.7f;
	private int hitFrame = 7;


	// Bullet
	private PackedScene proj = null;

	// Minion
	private bool init = false;
	public int count = 0;
	private PackedScene minion = null;

	private float GetDirectionBetweenBossAndPlayer() {
		float disX = player.GlobalPosition.x - GlobalPosition.x;
		float disY =  player.GlobalPosition.y - GlobalPosition.y;
		return (float) Math.Atan2(disY, disX);
	}

	private void SpawnBullet(Vector2 pos, float dir, int dm, int spd) {
		EBullet bullet = (EBullet) proj.Instance();
		bullet.dir = dir;
		bullet.dirMag = dm;
		bullet.Position = pos;
		bullet.speed = spd;
		GetParent().AddChild(bullet);
	}

	private void SpawnMinion() {
		Minion m = (Minion) minion.Instance();
		m.Position = Position + new Vector2(40, 40);
		count++;
		GetParent().AddChild(m);
		Minion n = (Minion) minion.Instance();
		n.Position = Position + new Vector2(-40, -40);
		count++;
		GetParent().AddChild(n);
		Minion o = (Minion) minion.Instance();
		o.Position = Position + new Vector2(-40, 40);
		count++;
		GetParent().AddChild(o);
		Minion p = (Minion) minion.Instance();
		p.Position = Position + new Vector2(40, -40);
		count++;
		GetParent().AddChild(p);
	}

	private void MoveBoss() {
		float posX = Position.x;
		float posY = Position.y;

		float speedX = (float) Math.Cos(GetDirectionBetweenBossAndPlayer()) * bossSpeed;
		float speedY = (float) Math.Sin(GetDirectionBetweenBossAndPlayer()) * bossSpeed;

		posX += speedX;
		posY += speedY;

		Position = new Vector2(posX, posY);
	}

	public override void _Ready() {
		bossAnimation = GetNode<AnimatedSprite>("AnimatedSprite");
		collision = GetNode<Area2D>("Area2D");
		label = GetNode<Label>("Label");
		label.SetAsToplevel(true);
		label.Text = "BossHP: " + bossHP.ToString();
		label.SetPosition(new Vector2(550, 10));
		player = GetNode<Player>("../Player");
		proj = GD.Load<PackedScene>("res://Bullet/EBullet.tscn");
		minion = GD.Load<PackedScene>("res://Minion/Minion.tscn");
	}

	public override void _Process(float delta)
	{
		label.Text = "BossHP: " + bossHP.ToString();
		switch (state) {
			case BossState.Idle_1:
				bossAnimation.Play("1_Idle");
				if (shootTimer1 <= 0) {
					SpawnBullet(Position, GetDirectionBetweenBossAndPlayer(), 5, 2);
					shootTimer1 = 20;
				} else {
					shootTimer1--;
				}

				if (timer1 <= 0) {
					state = BossState.Move_1;
					timer1 = 300;
				} else {
					timer1--;
				}

				if (bossHP < 700) {
					state = BossState.Move_1;
				}
				break;

			case BossState.Move_1:
				MoveBoss();
				bossAnimation.Play("1_Move");
				if (shootTimer2 <= 0) {
					for (int i = 0; i < 20; ++i) {
						SpawnBullet(Position, GetDirectionBetweenBossAndPlayer(), 20, 2);
					}
					shootTimer2 = 100;
				} else {
					shootTimer2--;
				}

				if (timer1 <= 0) {
					if (bossHP < 700) {
						state = BossState.IdleMove_2;
						timer1 = 300;
					} else {
						state = BossState.Idle_1;
						timer1 = 300;
					}
				} else {
					timer1--;
				}

				if (bossHP <= 400) {
					state = BossState.Idle_3;
					timer1 = 500;
				}
				break;

			case BossState.IdleMove_2:
				bossAnimation.Play("1_MoveToVun");
				if (bossAnimation.Frame > 3) {
					state = BossState.Idle_2;
				}

				break;

			case BossState.Idle_2:
				bossAnimation.Play("1_Vunerable");
				if (shootTimer3 <= 0) {
					for (int i = 0; i < 100; ++i) {
						SpawnBullet(Position, GetDirectionBetweenBossAndPlayer(), 360, 2);
					}
					shootTimer3 = 100;
				} else {
					shootTimer3--;
				}

				if (timer1 <= 0) {
					state = BossState.MoveIdle_2;
					timer1 = 300;
					
				} else {
					timer1--;
				}

				if (bossHP <= 400) {
					state = BossState.MoveIdle_2;
				}

				break;

			case BossState.MoveIdle_2:
				bossAnimation.Play("1_VunToMove");
				if (bossAnimation.Frame > 3) {
					state = BossState.Move_1;
				}

				
				break;

			case BossState.Idle_3:
				bossAnimation.Play("2_Idle");
				collision.SetCollisionLayerBit(2, false);
				if (!init) {
					SpawnMinion();
					init = true;
				}

				if (count <= 0) {
					state = BossState.Vun_3;
				}

				break;

			case BossState.Vun_3:
				init = false;
				bossAnimation.Play("2_Vunerable");
				collision.SetCollisionLayerBit(2, true);
				if (timer1 <= 0) {
					state = BossState.Idle_3;
					timer1 = 500;
				} else {
					timer1--;
				}

				if (bossHP <= 200) {
					state = BossState.TwoToThree;
				}
				break;

			case BossState.TwoToThree:
				bossAnimation.Play("2_GoTo3");
				if (bossAnimation.Frame > 3) {
					state = BossState.Move_3;
				}
				break;

			case BossState.Move_3:
				bossAnimation.Play("3_Move");
				MoveBoss();
				bossSpeed = 1.0f;
				if (bossHP <= 0) {
					state = BossState.Death_4;
				}

				if (shootTimer4 <= 0) {
					for (int i = 0; i < 180; ++i) {
						SpawnBullet(Position, GetDirectionBetweenBossAndPlayer(), 360, 2);
					}
					shootTimer4 = 120;
				} else {
					shootTimer4--;
				}
				break;

			case BossState.Death_4:
				bossAnimation.Play("4_Death");
				collision.SetCollisionLayerBit(2, false);
				if (bossAnimation.Frame > 13) {
					QueueFree();
				}
				break;
		}
		

		if (hitFrame <= 0) {
			bossAnimation.Modulate = new Color(1, 1, 1, 1);
			hitFrame = 7;
		} else {
			hitFrame--;
		}
		
	}
	
	private void _on_Area2D_area_entered(object area)
	{
		switch (state) {
			case BossState.Idle_1:
				bossHP -= 1;
				break;

			case BossState.Move_1:
				bossHP -= 1;
				break;

			case BossState.IdleMove_2:
				bossHP -= 1;
				break;

			case BossState.Idle_2:
				bossHP -= 2;
				break;

			case BossState.MoveIdle_2:
				bossHP -= 2;
				break;

			case BossState.Idle_3:
				bossHP -= 1;
				break;

			case BossState.Vun_3:
				bossHP -= 2;
				break;

			case BossState.TwoToThree:
				bossHP -= 1;
				break;

			case BossState.Move_3:
				bossHP -= 2;
				break;
		}
		bossAnimation.Modulate = new Color(1, 0, 0, 1);
		
	}
}



