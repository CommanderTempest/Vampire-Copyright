using Godot;

public partial class Main : Node2D
{
	[Export] public PackedScene EnemyScene;

	private static int score = 0;
	private static bool gameOver = false;

	private static Label scoreLabel;
	private static Control gameOverPanel;
	private static Label finalScoreLabel;
	private static Timer spawnTimer;

	private EnemyFactory enemyFactory;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("UI/ScoreLabel");
		gameOverPanel = GetNode<Control>("UI/GameOverPanel");
		finalScoreLabel = GetNode<Label>("UI/GameOverPanel/VBoxContainer/FinalScoreLabel");
		spawnTimer = GetNode<Timer>("SpawnTimer");
		enemyFactory = new EnemyFactory();

		GD.Print("Main ready");
		GD.Print("EnemyScene assigned: ", EnemyScene != null);
		GD.Print("GameOverPanel found: ", gameOverPanel != null);
		GD.Print("FinalScoreLabel found: ", finalScoreLabel != null);

		UpdateScore();

		// change this to true TEMPORARILY to test if the panel can be seen
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

		Enemy enemy = enemyFactory.createEnemy(EnemyScene);
		AddChild(enemy);
		enemyFactory.positionEnemy(enemy);
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

	public static void ShowGameOver()
	{
		GD.Print("SHOW GAME OVER CALLED");

		gameOver = true;
		spawnTimer.Stop();
		finalScoreLabel.Text = "Score: " + score;
		gameOverPanel.Visible = true;

		GD.Print("GameOverPanel visible: ", gameOverPanel.Visible);

		EnemyFactory.stopEnemies();
	}

	private void OnRestartButtonPressed()
	{
		GetTree().ReloadCurrentScene();
		gameOver = false;
	}

	private void OnMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://environment/Scenes/mainmenu.tscn");
	}
}
