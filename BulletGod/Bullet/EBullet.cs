using Godot;
using System;

public class EBullet : Area2D
{

	// bullet vars
	private int damage = 1;
	private float dirDelta = 0;
	public int dirMag = 0;
	public float dir = 0;
	public int speed = 1;

	private void BoundBullet() {

		if (Position.x <= 0) {
			QueueFree();
		} else if (Position.x >= 640) {
			QueueFree();
		}

		if (Position.y <= 0) {
			QueueFree();
		} else if (Position.y >= 360) {
			QueueFree();
		}


	}

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
		Random r = new Random();
		dirDelta = (float) (r.Next(-dirMag, dirMag) * Math.PI / 180);
	}

	public override void _Process(float delta) {
		MoveBullet();
		BoundBullet();
	}

}
