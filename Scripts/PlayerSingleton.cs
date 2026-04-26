/*
This singleton exists to store a reference to the player to use as needed
*/

using Godot;

public partial class PlayerSingleton : Node
{
    private static player player;

    public static player GetPlayer()
    {
        return player;
    }

    public static void SetPlayer(player newPlayer)
    {
        player = newPlayer;
    }
}
