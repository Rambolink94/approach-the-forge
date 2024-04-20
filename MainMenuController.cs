using Godot;

namespace ApproachTheForge;

public partial class MainMenuController : Control
{
	private PackedScene _mainScene = ResourceLoader.Load<PackedScene>("res://main_scene.tscn");
	private Button _playButton;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_playButton = GetNode<Button>("Panel/Button");
		_playButton.Pressed += () =>
		{
			GetTree().ChangeSceneToPacked(_mainScene);
		};
	}
}