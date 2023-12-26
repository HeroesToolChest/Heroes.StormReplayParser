namespace Heroes.StormReplayParser.Player;

/// <summary>
/// Contains the properties for when a player has the left the game.
/// </summary>
public class PlayerDisconnect
{
    /// <summary>
    /// Gets or sets the reason why the player left. 0 is assumed to be the player left purposely or due to activity timeout, otherwise other numbers are disconnects.
    /// </summary>
    public int? LeaveReason { get; set; }

    /// <summary>
    /// Gets or sets the time when the player left the game.
    /// </summary>
    public TimeSpan From { get; set; }

    /// <summary>
    /// Gets or sets the time when the player rejoins the game. Returns <see langword="null"/> if the player did not rejoin.
    /// </summary>
    public TimeSpan? To { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (To is null)
        {
            return $"Player left at {From} (and did not rejoin)";
        }
        else
        {
            return $"Player left at {From} and rejoined at {To.Value}";
        }
    }
}
