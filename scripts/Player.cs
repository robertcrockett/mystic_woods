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

	// Private Constants
	private const int DefaultSpeed = 150;
	private const int ScreenPadding = 20;
	
	// Private Variables
	private Vector2 _screenSize; // Size of the game window.
	private AnimatedSprite2D _animatedSprite2D;

	
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}
	
	public override void _PhysicsProcess (double delta)
	{
		Vector2 velocity = Vector2.Zero; // The player's movement vector.
		
		switch (true)
		{
			case bool _ when Input.IsActionPressed("move_right"):
				PlayerAnimation(PlayerAnimations.SideWalk);
				_animatedSprite2D.FlipH = false;
				velocity.X = Speed;
				velocity.Y = 0;
				break;
			case bool _ when Input.IsActionPressed("move_left"):
				PlayerAnimation(PlayerAnimations.SideWalk);
				_animatedSprite2D.FlipH = true;
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
			_animatedSprite2D.Play();
		}
		else
		{
			_animatedSprite2D.Stop();
		}
		
		// Move the player and slide along any colliding walls.
		Position += velocity * (float)delta;
		// Clamp the player inside the screen.
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, _screenSize.X),
			y: Mathf.Clamp(Position.Y, 0, _screenSize.Y - ScreenPadding)
		);
		
		// TODO: Add when collisions are set up
		//MoveAndSlide();
	}
	
	
	
	private void PlayerAnimation(PlayerAnimations animation)
	{
		switch (animation)
		{
			case PlayerAnimations.FrontWalk:
				_animatedSprite2D.Animation = "front_walk";
				break;
			case PlayerAnimations.FrontAttack:
				_animatedSprite2D.Animation = "front_attack";
				break;
			case PlayerAnimations.SideIdle:
				_animatedSprite2D.Animation = "side_idle";
				break;
			case PlayerAnimations.SideWalk:
				_animatedSprite2D.Animation = "side_walk";
				break;
			case PlayerAnimations.SideAttack:
				_animatedSprite2D.Animation = "side_attack";
				break;
			case PlayerAnimations.BackIdle:
				_animatedSprite2D.Animation = "back_idle";
				break;
			case PlayerAnimations.BackWalk:
				_animatedSprite2D.Animation = "back_walk";
				break;
			case PlayerAnimations.BackAttack:
				_animatedSprite2D.Animation = "back_attack";
				break;
			case PlayerAnimations.Walk:
				_animatedSprite2D.Animation = "walk";
				break;
			case PlayerAnimations.Death:
				_animatedSprite2D.Animation = "death";
				break;
			default:
				_animatedSprite2D.Animation = "front_idle";
				break;
		}
	}
}
