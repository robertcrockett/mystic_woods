using System;
using System.Collections.Generic;
using Godot;

public enum Direction
{
	Left,
	Right,
	Up,
	Down
}

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
	SideWalk
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
	private Direction _currentDirection;

	private readonly Dictionary<string, (PlayerAnimations animation, Vector2 velocity, Direction direction)> _inputMappings = new()
	{
		// Animations when facing right
		{ "move_right", (PlayerAnimations.SideWalk, new Vector2(1, 0), Direction.Right) },
		{ "face_right", (PlayerAnimations.SideIdle, new Vector2(0, 0), Direction.Right) },
		{ "attack_right", (PlayerAnimations.SideAttack, new Vector2(0, 0), Direction.Right) },
		// Animations when facing left
		{ "move_left", (PlayerAnimations.SideWalk, new Vector2(-1, 0), Direction.Left) },
		{ "face_left", (PlayerAnimations.SideIdle, new Vector2(0, 0), Direction.Left) },
		{ "attack_left", (PlayerAnimations.SideAttack, new Vector2(0, 0), Direction.Left) },
		// Animations when facing down
		{ "move_down", (PlayerAnimations.FrontWalk, new Vector2(0, 1), Direction.Down) },
		{ "face_down", (PlayerAnimations.FrontIdle, new Vector2(0, 0), Direction.Down) },
		{ "attack_down", (PlayerAnimations.FrontAttack, new Vector2(0, 0), Direction.Down) },
		// Animations when facing up
		{ "move_up", (PlayerAnimations.BackWalk, new Vector2(0, -1), Direction.Up) },
		{ "face_up", (PlayerAnimations.BackIdle, new Vector2(0, 0), Direction.Up) },
		{ "attack_up", (PlayerAnimations.BackAttack, new Vector2(0, 0), Direction.Up) },
		// Death Animation
		{ "death", (PlayerAnimations.Death, new Vector2(0, 0), Direction.Down) },
	};
	
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_currentDirection = Direction.Down;
		
		// Initialize the player's animation
		PlayerAnimation(PlayerAnimations.FrontIdle);
		_animatedSprite2D.Play();
	}
	
	public override void _PhysicsProcess (double delta)
	{
		Vector2 velocity = Vector2.Zero; // The player's movement vector.

		velocity = HandleInput();
		MovePlayer(velocity, delta);

		// TODO: Add when collisions are set up
		//MoveAndSlide();
	}
	
	private Vector2 HandleInput()
	{
		// Write out the action presseds to the console
		foreach (var inputMapping in _inputMappings)
		{
			if (!InputMap.HasAction(inputMapping.Key) || !Input.IsActionPressed(inputMapping.Key))
			{
				continue;
			}

			if (inputMapping.Key.StartsWith("attack"))
			{
				HandleAttackInput();
				return Vector2.Zero; // Attack does not change position
			}

			_currentDirection = inputMapping.Value.direction;
			PlayerAnimation(inputMapping.Value.animation);
			_animatedSprite2D.FlipH = inputMapping.Key == "move_left";
			return inputMapping.Value.velocity;
		}

		SetIdleAnimation();
		return Vector2.Zero;
	}

	private void HandleAttackInput()
	{
		string attackAction = _currentDirection switch
		{
			Direction.Right => "attack_right",
			Direction.Left => "attack_left",
			Direction.Down => "attack_down",
			Direction.Up => "attack_up",
			_ => "attack_down"
		};

		if (_inputMappings.TryGetValue(attackAction, out var attackMapping))
		{
			PlayerAnimation(attackMapping.animation);
		}
	}

	private void SetIdleAnimation()
	{
		string idleAction = _currentDirection switch
		{
			Direction.Right => "face_right",
			Direction.Left => "face_left",
			Direction.Down => "face_down",
			Direction.Up => "face_up",
			_ => "face_down"
		};

		if (_inputMappings.TryGetValue(idleAction, out var idleMapping))
		{
			PlayerAnimation(idleMapping.animation);
		}
	}
	
	private void MovePlayer(Vector2 velocity, double delta)
	{
		velocity = velocity.Normalized() * Speed;

		_animatedSprite2D.Play();
		Position += velocity * (float)delta;
		MoveAndSlide();
		
		// Clamp the player inside the screen.
		ClampPlayerToScreen();
	}

	/// <summary>
	/// Ensures that the player does not move off the screen. Updates the player's
	/// position if they do.
	/// </summary>
	private void ClampPlayerToScreen()
	{
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, _screenSize.X),
			y: Mathf.Clamp(Position.Y, 0, _screenSize.Y - ScreenPadding)
		);
	}
	
	private void PlayerAnimation(PlayerAnimations animation)
	{
		_animatedSprite2D.Animation = animation switch
		{
			PlayerAnimations.FrontIdle => "front_idle",
			PlayerAnimations.FrontWalk => "front_walk",
			PlayerAnimations.FrontAttack => "front_attack",
			PlayerAnimations.SideIdle => "side_idle",
			PlayerAnimations.SideWalk => "side_walk",
			PlayerAnimations.SideAttack => "side_attack",
			PlayerAnimations.BackIdle => "back_idle",
			PlayerAnimations.BackWalk => "back_walk",
			PlayerAnimations.BackAttack => "back_attack",
			PlayerAnimations.Death => "death",
			_ => "front_idle"
		};
	}
}
