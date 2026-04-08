using Godot;

public partial class MainMenu : Control
{
	private void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://environment/Scenes/main.tscn");
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
