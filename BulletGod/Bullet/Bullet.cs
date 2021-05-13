using Godot;
using System;

public class Bullet : Area2D
{

	[Signal] public delegate void BulletCollision(int damage);

	// bullet vars
	private AnimatedSprite bulletAnimation = null;
	private int damage = 1;
	private int lifeTime = 30;
	private float dirDelta = 0;
	public float dir = 0;
	public int speed = 2;


	enum BulletState {
		Move,
		Death
	}

	private BulletState state = BulletState.Move;


	private void MoveBullet() {
		float posX = Position.x;
		float posY = Position.y;

		float speedX = (float) Math.Cos(dir + dirDelta) * speed;
		float speedY = (float) Math.Sin(dir + dirDelta) * speed;

		posX += speedX;
		posY += speedY;

		Position = new Vector2(posX, posY);
	}

	public override void _Ready() {
		bulletAnimation = GetNode<AnimatedSprite>("AnimatedSprite");
		Random r = new Random();
		dirDelta = (float) (r.Next(-8, 8) * Math.PI / 180);
	}

	public override void _Process(float delta) {
		
		switch (state) {
			case BulletState.Move:
				lifeTime--;
				if (lifeTime < 0) {
					state = BulletState.Death;
				}

				bulletAnimation.Play("Idle");
				MoveBullet();
				break;

			case BulletState.Death:
				bulletAnimation.Play("Death");
				if (bulletAnimation.Frame > 4) {
					QueueFree();
				}
				MoveBullet();
				break;
		}
	}

}
