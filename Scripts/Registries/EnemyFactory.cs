using System.Collections.Generic;
using System.Numerics;

public interface IEnemyFactory
{
    public void createEnemy();
}

public class EnemyFactory : IEnemyFactory
{
    private Dictionary<int,Enemy> enemy_list;
    private int generationNum = 0;

    public void createEnemy()
    {
        throw new System.NotImplementedException();
        Enemy newEnemy = new Enemy();

        // Attach EnemyDeath signal to the Defeat method
        newEnemy.EnemyDeath += Defeat;

        enemy_list.Add(generationNum++, newEnemy);
    }

    // Return a vector2 position to place the enemy at
    private Vector2 positionEnemy()
    {
        return new Vector2(0,0);
    }

    private void Defeat()
    {
        
    }
}
