using System;
using Godot;
using System.Collections.Generic;

public enum SlimeAnimations
{
	BackIdle,
	BackWalk,
	Death,
	FrontIdle,
	FrontWalk,
	SideIdle,
	SideWalk
}

public partial class Slime : CharacterBody2D
{
	private const float Speed = 40.0f;
	private bool _chasePlayer;
	private Player _player;
	
	private AnimatedSprite2D _animatedSprite2D;
	private Direction _currentDirection;
	
	private readonly Dictionary<string, (SlimeAnimations animation, Vector2 velocity, Direction direction)> _slimeMappings = new()
	{
		// Animations when facing right
		{ "move_right", (SlimeAnimations.SideWalk, new Vector2(1, 0), Direction.Right) },
		{ "face_right", (SlimeAnimations.SideIdle, new Vector2(0, 0), Direction.Right) },
		// Animations when facing left
		{ "move_left", (SlimeAnimations.SideWalk, new Vector2(-1, 0), Direction.Left) },
		{ "face_left", (SlimeAnimations.SideIdle, new Vector2(0, 0), Direction.Left) },
		// Animations when facing down
		{ "move_down", (SlimeAnimations.FrontWalk, new Vector2(0, 1), Direction.Down) },
		{ "face_down", (SlimeAnimations.FrontIdle, new Vector2(0, 0), Direction.Down) },
		// Animations when facing up
		{ "move_up", (SlimeAnimations.BackWalk, new Vector2(0, -1), Direction.Up) },
		{ "face_up", (SlimeAnimations.BackIdle, new Vector2(0, 0), Direction.Up) },
		// Death Animation
		{ "death", (SlimeAnimations.Death, new Vector2(0, 0), Direction.Down) },
	};

	public override void _Ready()
	{
		// Initialize the slime's variables
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_currentDirection = Direction.Down;
		
		// Initialize the slime's animation
		SlimeAnimation(SlimeAnimations.FrontIdle);
		_animatedSprite2D.Play();
	}
	public override void _PhysicsProcess(double delta)
	{
		// If _chasePlayer is true and _player is not null, set the velocity to chase the player, otherwise set the velocity to zero
		Vector2 velocity = _chasePlayer && _player != null ? GetChaseVelocity() : Vector2.Zero;
		Console.WriteLine("Velocity: " + velocity);
		
		// Set the animation based on the velocity
		SetAnimationBasedOnDirection(velocity);
		
		// Handle collisions to prevent getting stuck
		Velocity = velocity;
		
		bool hasCollidedWithPlayer = false;
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			KinematicCollision2D collision = GetSlideCollision(i);
			if (collision.GetCollider() is Player)
			{
				hasCollidedWithPlayer = true;
				break;
			}
		}

		if (!hasCollidedWithPlayer && velocity != Vector2.Zero && _player != null)
		{
			Position += (_player.GlobalPosition - GlobalPosition) / Speed;
		}
	}
	
	private void OnDetectionAreaBodyEntered(Player body)
	{
		_chasePlayer = true;
		_player = body;
	}

	private void OnDetectionAreaBodyExited(Player body)
	{
		_chasePlayer = false;
		_player = null;
	}
	
	private Vector2 GetChaseVelocity()
	{
		return (_player.GlobalPosition - GlobalPosition).Normalized() * Speed;
	}
	
	private void DetermineDirection(Vector2 velocity)
	{
		if (velocity == Vector2.Zero)
		{
			_currentDirection = Direction.Down;
		}
		else if (Mathf.Abs(velocity.X) > Mathf.Abs(velocity.Y))
		{
			_currentDirection = velocity.X > 0 ? Direction.Right : Direction.Left;
		}
		else
		{
			_currentDirection = velocity.Y > 0 ? Direction.Down : Direction.Up;
		}
	}
	
	private void SetAnimationBasedOnDirection(Vector2 velocity)
	{
		DetermineDirection(velocity);
		string action = velocity == Vector2.Zero ? GetIdleAction() : GetMoveAction();
		if (_slimeMappings.TryGetValue(action, out var mapping))
		{
			Console.WriteLine("Action: " + action);
			SlimeAnimation(mapping.animation);
			_animatedSprite2D.FlipH = action == "move_left" || action == "face_left";
		}
	}
	
	private string GetMoveAction()
	{
		return _currentDirection switch
		{
			Direction.Right => "move_right",
			Direction.Left => "move_left",
			Direction.Down => "move_down",
			Direction.Up => "move_up",
			_ => "move_down"
		};
	}
	
	private string GetIdleAction()
	{
		return _currentDirection switch
		{
			Direction.Right => "face_right",
			Direction.Left => "face_left",
			Direction.Down => "face_down",
			Direction.Up => "face_up",
			_ => "face_down"
		};
	}
	
	private void SlimeAnimation(SlimeAnimations animation)
	{
		_animatedSprite2D.Animation = animation switch
		{
			SlimeAnimations.FrontIdle => "front_idle",
			SlimeAnimations.FrontWalk => "front_walk",
			SlimeAnimations.SideIdle => "side_idle",
			SlimeAnimations.SideWalk => "side_walk",
			SlimeAnimations.BackIdle => "back_idle",
			SlimeAnimations.BackWalk => "back_walk",
			SlimeAnimations.Death => "death",
			_ => "front_idle"
		};
	}
}
