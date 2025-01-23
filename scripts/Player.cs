using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public int Speed { get; set; } = 100; // How fast the player will move (pixels/sec).

	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero; // The player's movement vector.

		switch (true)
		{
			case bool _ when Input.IsActionPressed("move_right"):
				velocity.X = Speed;
				velocity.Y = 0;
				break;
			case bool _ when Input.IsActionPressed("move_left"):
				velocity.X = -Speed;
				velocity.Y = 0;
				break;
			case bool _ when Input.IsActionPressed("move_down"):
				velocity.X = 0;
				velocity.Y = Speed;
				break;
			case bool _ when Input.IsActionPressed("move_up"):
				velocity.X = 0;
				velocity.Y = -Speed;
				break;
			default:
				velocity.X = 0;
				velocity.Y = 0;
				break;
		}

		MoveAndSlide();
		
		 var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		 if (velocity.Length() > 0)
		 {
		 	velocity = velocity.Normalized() * Speed;
		 	animatedSprite2D.Play();
		 }
		 else
		 {
		 	animatedSprite2D.Stop();
		 }
		
		// // Clamp the player inside the screen.
		// Position += velocity * (float)delta;
		// Position = new Vector2(
		// 	x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
		// 	y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		// );
		//
		// if (velocity.X != 0)
		// {
		// 	animatedSprite2D.Animation = "walk";
		// 	animatedSprite2D.FlipV = false;
		// 	// See the note below about the following boolean assignment.
		// 	animatedSprite2D.FlipH = velocity.X < 0;
		// }
		// else if (velocity.Y != 0)
		// {
		// 	animatedSprite2D.Animation = "up";
		// 	animatedSprite2D.FlipV = velocity.Y > 0;
		// }
	}
}
