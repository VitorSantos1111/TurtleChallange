namespace Services;

/// <summary>
/// Represents the turtle game engine.
/// </summary>
internal class GameEngine
{
    #region Constants

    private const string SEQUENCE_SUCCESS = "Success!";
    private const string SEQUENCE_HIT_MINE = "Mine hit!";
    private const string SEQUENCE_CANT_MOVE = "Can't move to the {0} direction!";
    private const string SEQUENCE_END_FAILED = "Still in danger!";
    private const string SEQUENCE_END_SUCCESS = "Exited the board safely!";

    #endregion

    #region Fields

    private readonly Models.Settings settings;
    private readonly List<Actions> actions;

    private readonly Models.Tile currentPosition;
    private Direction currentDirection;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new instance of <see cref="GameEngine"/>.
    /// </summary>
    /// <param name="settings">The game's settings.</param>
    /// <param name="actions">The player's actions.</param>
    public GameEngine(Models.Settings settings, IEnumerable<Actions> actions)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        this.actions = new List<Actions>(actions);
        this.currentPosition = new Models.Tile(settings.StartingTile.X, settings.StartingTile.Y);
        this.currentDirection = settings.InitialDirection;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Run the game with the current settings and actions.
    /// </summary>
    public void Run()
    {
        // Run the actions.
        // INFO: The request is to run ALL actions. So if a mine is hit, we must continue to the next move.
        for (int i = 0; i < actions.Count; i++)
        {
            // Perform the action and record it's status.
            var status = "";

            switch (actions[i])
            {
                case Actions.Move:
                    // Move the position.
                    status = (Move()
                        ? SEQUENCE_SUCCESS
                        : SEQUENCE_CANT_MOVE);
                    break;
                case Actions.Rotate:
                    Rotate();
                    status = SEQUENCE_SUCCESS;
                    break;
            }

            // Last action?
            if (i == (actions.Count -1))
            {
                // Check if the player reached the exit.
                status = HasReachedTheExit()
                    ? SEQUENCE_END_SUCCESS
                    : SEQUENCE_END_FAILED;
            }
            else 
            {
                // Has hit a mine?
                if (HasHitMine())
                {
                    status = SEQUENCE_HIT_MINE;
                }
            }

            // Write status.
            Console.WriteLine(string.Format("Sequence {0}: {1}", i + 1, status));
        }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Rotate the current direction.
    /// </summary>
    private void Rotate()
    {
        // INFO: We can play arround Enums by treating them as an int where the next Enum value is +1 of the current one.
        currentDirection = (Direction)(currentDirection + 1);

        // Did we overshoot the enum value?
        if (!Enum.IsDefined<Direction>(currentDirection))
        {
            currentDirection = Direction.North;
        }
    }

    /// <summary>
    /// Moves the current position 1 tile in the current direction.
    /// </summary>
    /// <returns><see langword="true"/> if the move was completed; <see langword="false"/> otherwise.</returns>
    private bool Move()
    {
        var status = false;

        switch (currentDirection)
        {
            case Direction.North:
                // Validate new position.
                if ((long)(currentPosition.Y - 1) >= 0)
                {
                    currentPosition.Y--;
                    status = true;
                }
                break;
            case Direction.South:
                // Validate new position.
                if ((long)(currentPosition.Y + 1) < settings.Rows)
                {
                    currentPosition.Y++;
                    status = true;
                }
                break;
            case Direction.West:
                // Validate new position.
                if ((long)(currentPosition.X - 1) >= 0)
                {
                    currentPosition.X--;
                    status = true;
                }
                break;
            case Direction.East:
                // Validate new position.
                if ((long)(currentPosition.X + 1) < settings.Columns)
                {
                    currentPosition.X++;
                    status = true;
                }
                break;
        }

        return status;
    }

    /// <summary>
    /// Determines if there's a mine in the current position.
    /// </summary>
    /// <returns><see langword="true"/> if there's a mine in the current position; <see langword="false"/> otherwise.</returns>
    private bool HasHitMine()
    {
        return settings.Mines?.Any(x => x.Equals(currentPosition)) ?? false;
    }

    /// <summary>
    /// Determines if the turtle reached the exit position.
    /// </summary>
    /// <returns><see langword="true"/> if the exit is in the current position; <see langword="false"/> otherwise.</returns>
    private bool HasReachedTheExit()
    {
        return settings.ExitTile.Equals(currentPosition);
    }

    #endregion
}