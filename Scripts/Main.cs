using Godot;

public partial class Main : Node2D
{
	[Export] public PackedScene EnemyScene;

	private int score = 0;
	private bool gameOver = false;

	private Label scoreLabel;
	private Control gameOverPanel;
	private Label finalScoreLabel;
	private Timer spawnTimer;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("UI/ScoreLabel");
		gameOverPanel = GetNode<Control>("UI/GameOverPanel");
		finalScoreLabel = GetNode<Label>("UI/GameOverPanel/VBoxContainer/FinalScoreLabel");
		spawnTimer = GetNode<Timer>("SpawnTimer");

		GD.Print("Main ready");
		GD.Print("EnemyScene assigned: ", EnemyScene != null);

		UpdateScore();
		gameOverPanel.Visible = false;
	}

	private void OnSpawnTimerTimeout()
	{
		GD.Print("Spawn timer fired");

		if (gameOver)
			return;

		if (EnemyScene == null)
		{
			GD.Print("EnemyScene is NULL");
			return;
		}

		Node2D enemy = EnemyScene.Instantiate<Node2D>();
		enemy.GlobalPosition = new Vector2(
			(float)GD.RandRange(50, 1100),
			(float)GD.RandRange(50, 600)
		);

		AddChild(enemy);
		GD.Print("Enemy spawned");
	}

	public void AddScore(int amount)
	{
		if (gameOver)
			return;

		score += amount;
		UpdateScore();
	}

	private void UpdateScore()
	{
		scoreLabel.Text = "Score: " + score;
	}

	public void ShowGameOver()
	{
		gameOver = true;
		spawnTimer.Stop();
		finalScoreLabel.Text = "Score: " + score;
		gameOverPanel.Visible = true;

		foreach (Node child in GetChildren())
		{
			if (child is Enemy enemy)
				enemy.SetPhysicsProcess(false);
		}
	}

	private void OnRestartButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	private void OnMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://environment/Scenes/mainmenu.tscn");
	}
}
