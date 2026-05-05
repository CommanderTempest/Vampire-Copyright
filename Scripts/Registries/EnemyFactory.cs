using System.Collections.Generic;
using Godot;

public interface IEnemyFactory
{
    public Enemy createEnemy();
}

public class EnemyFactory : IEnemyFactory
{
    private static Dictionary<int,Enemy> enemy_list = new Dictionary<int, Enemy>();
    private static int generationNum = 0;

    public Enemy createEnemy()
    {
        Enemy newEnemy = new Enemy();

        // Attach EnemyDeath signal to the Defeat method
        newEnemy.EnemyDeath += Defeat;

        enemy_list.Add(generationNum++, newEnemy);
        return newEnemy;
    }

    public Enemy createEnemy(PackedScene scene)
    {
        Enemy newEnemy = scene.Instantiate() as Enemy;

        // Attach EnemyDeath signal to the Defeat method
        newEnemy.EnemyDeath += Defeat;

        enemy_list.Add(generationNum++, newEnemy);
        return newEnemy;
    }

    // Return a vector2 position to place the enemy at
    public void positionEnemy(Enemy enemy)
    {
        Vector2 playerPos = PlayerSingleton.GetPlayer().GlobalPosition;
        // Take player pos, add a static distance (400) to it, and then randomize a range on top of that
        enemy.GlobalPosition = playerPos + new Godot.Vector2(400,400) + new Godot.Vector2(
			(float)GD.RandRange(-1000, 1000),
			(float)GD.RandRange(-1000, 1000)
		);
    }

    private void Defeat(Enemy enemy)
    {
        enemy.QueueFree();
    }
}
