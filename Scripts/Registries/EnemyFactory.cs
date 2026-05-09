using System.Collections.Generic;
using Godot;

public interface IEnemyFactory
{
    public Enemy createEnemy();
}

public class EnemyFactory : IEnemyFactory
{
    private static LinkedList<Enemy> enemy_list = new LinkedList<Enemy>();

    public Enemy createEnemy()
    {
        Enemy newEnemy = new Enemy();

        // Attach EnemyDeath signal to the Defeat method
        newEnemy.EnemyDeath += Defeat;

        enemy_list.AddLast(newEnemy);
        return newEnemy;
    }

    public Enemy createEnemy(PackedScene scene)
    {
        Enemy newEnemy = scene.Instantiate() as Enemy;

        // Attach EnemyDeath signal to the Defeat method
        newEnemy.EnemyDeath += Defeat;

        enemy_list.AddLast(newEnemy);
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

    public static void stopEnemies()
    {
        foreach (Enemy enemy in enemy_list)
        {
            enemy.SetPhysicsProcess(false);
        }
    }

    public static void reloadScene()
    {
        foreach (Enemy enemy in enemy_list)
        {
            enemy.SetPhysicsProcess(true);
        }
    }

    private void Defeat(Enemy enemy)
    {
        // play an animation and then:

        enemy_list.Remove(enemy);
        enemy.QueueFree();
    }
}
