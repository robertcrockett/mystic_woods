using Godot;
using System;

public enum PlayerAnimations
{
	BackAttack,
	BackIdle,
	BackWalk,
	Death,
	FrontAttack,
	FrontIdle,
	FrontWalk,
	SideAttack,
	SideIdle,
	SideWalk,
	Walk
}

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed { get; set; } = DefaultSpeed; // How fast the player will move (pixels/sec).

	private Vector2 _screenSize; // Size of the game window.
	private const int DefaultSpeed = 150;
	private const int ScreenPadding = 20;
	
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
	}
	
	public override void _PhysicsProcess (double delta)
	{
		Vector2 velocity = Vector2.Zero; // The player's movement vector.
		
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		switch (true)
		{
			case bool _ when Input.IsActionPressed("move_right"):
				PlayerAnimation(PlayerAnimations.SideWalk);
				animatedSprite2D.FlipH = false;
				velocity.X = Speed;
				velocity.Y = 0;
				break;
			case bool _ when Input.IsActionPressed("move_left"):
				PlayerAnimation(PlayerAnimations.SideWalk);
				animatedSprite2D.FlipH = true;
				velocity.X = -Speed;
				velocity.Y = 0;
				break;
			case bool _ when Input.IsActionPressed("move_down"):
				PlayerAnimation(PlayerAnimations.FrontWalk);
				velocity.X = 0;
				velocity.Y = Speed;
				break;
			case bool _ when Input.IsActionPressed("move_up"):
				PlayerAnimation(PlayerAnimations.BackWalk);
				velocity.X = 0;
				velocity.Y = -Speed;
				break;
			default:
				PlayerAnimation(PlayerAnimations.FrontIdle);
				velocity.X = 0;
				velocity.Y = 0;
				break;
		}
		
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		}
		else
		{
			animatedSprite2D.Stop();
		}
		
		// Move the player and slide along any colliding walls.
		Position += velocity * (float)delta;
		// Clamp the player inside the screen.
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, _screenSize.X),
			y: Mathf.Clamp(Position.Y, 0, _screenSize.Y - 20)
		);
		
		// TODO: Add when collisions are set up
		//MoveAndSlide();
	}
	
	
	
	private void PlayerAnimation(PlayerAnimations animation)
	{
		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		switch (animation)
		{
			case PlayerAnimations.FrontWalk:
				animatedSprite2D.Animation = "front_walk";
				break;
			case PlayerAnimations.FrontAttack:
				animatedSprite2D.Animation = "front_attack";
				break;
			case PlayerAnimations.SideIdle:
				animatedSprite2D.Animation = "side_idle";
				break;
			case PlayerAnimations.SideWalk:
				animatedSprite2D.Animation = "side_walk";
				break;
			case PlayerAnimations.SideAttack:
				animatedSprite2D.Animation = "side_attack";
				break;
			case PlayerAnimations.BackIdle:
				animatedSprite2D.Animation = "back_idle";
				break;
			case PlayerAnimations.BackWalk:
				animatedSprite2D.Animation = "back_walk";
				break;
			case PlayerAnimations.BackAttack:
				animatedSprite2D.Animation = "back_attack";
				break;
			case PlayerAnimations.Walk:
				animatedSprite2D.Animation = "walk";
				break;
			case PlayerAnimations.Death:
				animatedSprite2D.Animation = "death";
				break;
			default:
				animatedSprite2D.Animation = "front_idle";
				break;
		}
	}
}
