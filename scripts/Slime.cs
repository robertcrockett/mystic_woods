using Godot;
using System;

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
	private const float Speed = 35.0f;
	private bool _chasePlayer = false;
	private Player _player = null;
	
	private AnimatedSprite2D _animatedSprite2D;
	private Direction _currentDirection;

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
		Vector2 velocity = Velocity;

		// If the slime is chasing the player, move towards the player
		if (_chasePlayer && _player != null)
		{
			// Set the walk animation
			SlimeAnimation(SlimeAnimations.SideWalk);
			velocity = (_player.GlobalPosition - GlobalPosition).Normalized() * Speed;
		}
		else
		{
			// Set the idle animation
			SlimeAnimation(SlimeAnimations.SideIdle);
			velocity = Vector2.Zero;
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}
	
	public void OnDetectionAreaBodyEntered(Player body)
	{
		_chasePlayer = true;
		_player = body;
	}

	public void OnDetectionAreaBodyExited(Player body)
	{
		_chasePlayer = false;
		_player = null;
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
			_ => "side_idle"
		};
	}
}
