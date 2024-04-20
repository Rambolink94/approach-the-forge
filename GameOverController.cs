using Godot;

namespace ApproachTheForge;

public partial class GameOverController : CanvasLayer
{
	private PackedScene _mainMenu = ResourceLoader.Load<PackedScene>("res://MainMenu.tscn");
	
	private Button _restartButton;
	private Button _quitButton;
	private AudioStreamPlayer2D _failureMusicAudio;
	private AudioStreamPlayer2D _failureVoiceAudio;
	private ColorRect _transitionPanel;
	private Label _menuLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_transitionPanel = GetNode<ColorRect>("TransitionPanel");
		_restartButton = GetNode<Button>("Panel/MarginContainer/VBoxContainer/RestartButton");
		_quitButton = GetNode<Button>("Panel/MarginContainer/VBoxContainer/QuitButton");
		_menuLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/MenuLabel");
		_failureMusicAudio = GetNode<AudioStreamPlayer2D>("FailureMusicAudio");
		_failureVoiceAudio = GetNode<AudioStreamPlayer2D>("FailureVoiceAudio");

		var tree = GetTree();

		_restartButton.Pressed += () =>
		{
			tree.Paused = false;
			tree.ReloadCurrentScene();
		};
		
		_quitButton.Pressed += () =>
		{
			_failureVoiceAudio.Play();
			double clipLength = _failureVoiceAudio.Stream.GetLength();

			_transitionPanel.Visible = true;
			CreateTween()
				.TweenProperty(_transitionPanel, "color", Colors.Black, clipLength)
				.SetEase(Tween.EaseType.InOut)
				.Finished += () =>
			{
				tree.Paused = false;
				tree.ChangeSceneToPacked(_mainMenu);
				GD.Print("TRANSITION TO MAIN MENU");
			};
		};
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("menu_toggle"))
		{
			if (Visible)
			{
				Close();
			}
			else
			{
				Open("Game Paused");
			}
		}
	}

	public void Open(string menuMessage, bool isfailure = false)
	{
		if (isfailure)
		{
			_failureMusicAudio.Play();
		}
		_menuLabel.Text = menuMessage;
		GetTree().Paused = true;
		Visible = true;
	}

	public void Close()
	{
		GetTree().Paused = false;
		Visible = false;
	}
}
