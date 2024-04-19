using ApproachTheForge.Utility;
using Godot;
using System;
using System.Runtime.CompilerServices;

namespace ApproachTheForge.UI.Village
{
	public partial class UpgradePoint : Node2D
	{
		private Label InteractivePopup;
		private Area2D PlayerDetector;
		private CanvasLayer UpgradeScreen;
	
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			this.UpgradeScreen = GetTree().Root.GetNode("Game").GetNode<CanvasLayer>("Upgrade Screen Canvas");

			this.PlayerDetector = GetNode<Area2D>("Player Detection");
			this.InteractivePopup = GetNode<Label>("Label");

			this.PlayerDetector.BodyEntered += this.ToggleInteractivePopup;
			this.PlayerDetector.BodyExited += this.ToggleInteractivePopup;

			// Start the popup in the correct parity for ToggleInteractivePopup
			this.InteractivePopup.Visible = false;
		}

		public override void _Process(double delta)
		{
			base._Process(delta);

			this.HandleInput();
		}

		private void HandleInput()
		{
			if(Input.IsActionJustPressed("player_interact") && this.PlayerDetector.HasOverlappingBodies())
			{
				this.UpgradeScreen.Visible = !this.UpgradeScreen.Visible;
			}
			else if (Input.IsActionJustPressed("player_interact"))
			{
				this.UpgradeScreen.Visible = false;
			}
		}

		// Only the player can trigger this player detector. Turn on the popup when they enter, exit when they leave.
		private void ToggleInteractivePopup(Node _)
		{
			this.InteractivePopup.Visible = !this.InteractivePopup.Visible;
		}
	}
}

