using ApproachTheForge.Utility;
using ApproachTheForge.Utility.Upgrade;
using Godot;
using System.Collections.Generic;

namespace ApproachTheForge.UI.Village
{
	public partial class UpgradeScreenCanvas : CanvasLayer
	{

		private GameManager GameManager;

		private UpgradeWrapper<PlayerUpgrade> PlayerUpgradeWrapper;

		private Button PlayerButton;

		private UpgradeWrapper<GolemUpgrade> GolemUpgradeWrapper;

		private Button GolemButton;

		private UpgradeWrapper<TowerUpgrade> TowerUpgradeWrapper;

		private Button TowerButton;

		private Dictionary<ResourceType, string> ResourceIcons = new()
		{
			{ ResourceType.Common, "res://Art/ResourceImages/commonDrop_32x32.png"},
			{ ResourceType.Rare, "res://Art/ResourceImages/rareDrop_32x32.png"},
			{ ResourceType.Super, "res://Art/ResourceImages/superDrop_32x32.png"},
		};

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Get the reference to the game manager
			this.GameManager = GetTree().Root.GetNode("Game").GetNode<GameManager>("GameManager");

			Node upgradeContainer = GetNode<Node>("Upgrade Screen").GetNode<Node>("UpgradesContainer");

			// Set up player upgrade button
			this.PlayerButton = upgradeContainer.GetNode<Button>("PlayerUpgrade");
			this.PlayerButton.ButtonUp += () => 
			{
				this.GameManager.UpgradeManager.AddUpgrade(this.PlayerUpgradeWrapper.Upgrade);
				this.Visible = false;
				this.GenerateNewRandomUpgradeWrappers();

			};

			// Set up golem upgrade button
			this.GolemButton = upgradeContainer.GetNode<Button>("GolemUpgrade");
			this.GolemButton.ButtonUp += () =>
			{
				this.GameManager.UpgradeManager.AddUpgrade(this.GolemUpgradeWrapper.Upgrade);
				this.Visible = false;
				this.GenerateNewRandomUpgradeWrappers();

			};

			// Set up tower upgrade button
			this.TowerButton = upgradeContainer.GetNode<Button>("TowerUpgrade");
			this.TowerButton.ButtonUp += () =>
			{
				this.GameManager.UpgradeManager.AddUpgrade(this.TowerUpgradeWrapper.Upgrade);
				this.Visible = false;
				this.GenerateNewRandomUpgradeWrappers();

			};

			// Populate the buttons whenever the visibility changes
			this.VisibilityChanged += this.PopulateButtons;
			this.GenerateNewRandomUpgradeWrappers();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			
		}

		private void GenerateNewRandomUpgradeWrappers()
		{
			var randomWrapperGenerator = new RandomUpgradeGenerator();

			this.PlayerUpgradeWrapper = randomWrapperGenerator.
				GenerateRandomUpgradeWrapper<PlayerUpgrade, RandomPlayerStat>(new RandomPlayerStat());

			this.TowerUpgradeWrapper = randomWrapperGenerator.
				GenerateRandomUpgradeWrapper<TowerUpgrade, RandomTowerStat>(new RandomTowerStat());

			this.GolemUpgradeWrapper = randomWrapperGenerator.
				GenerateRandomUpgradeWrapper<GolemUpgrade, RandomGolemStat>(new RandomGolemStat());
		}

		private void PopulateButtons()
		{
			this.PopulateButton(this.PlayerButton, this.PlayerUpgradeWrapper, "Player");
			this.PopulateButton(this.TowerButton, this.GolemUpgradeWrapper, "Tower");
			this.PopulateButton(this.GolemButton, this.TowerUpgradeWrapper, "Golem");
		}

		private void PopulateButton<T>(Button button, UpgradeWrapper<T> upgrade, string entityName)
			where T : IUpgrade
		{
			// Set the text upgrade description and upgrade value 
			var description = button.GetNode<Label>("Upgrade Description");

			description.Text = entityName + " " + upgrade.Upgrade.UpgradedStat.ToString().Replace("_", " ");

			var value = button.GetNode<Label>("Stat Description");

			value.Text = upgrade.Upgrade.UpgradeValue.ToString() + "% " + upgrade.Upgrade.UpgradedStat.ToString().Replace("_", " ");

			// Set the upgrade requirements
			var costDescription = button.GetNode<HBoxContainer>("Cost Container");

			var cost = costDescription.GetNode<Label>("Resource Cost");

			cost.Text = upgrade.Cost.ToString();

			var resourceType = costDescription.GetNode<TextureRect>("Resource Icon");

			resourceType.Texture = Godot.ResourceLoader.Load<Texture2D>(this.ResourceIcons[upgrade.ResourceType]);
		}
	}
}

