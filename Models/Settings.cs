using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Models;

/// <summary>
/// Represents the settings for the game.
/// </summary>
internal class Settings
{
    #region Properties

    /// <summary>
    /// Gets or sets the number of rows in the game.
    /// </summary>            
    public uint Rows { get; set; }

    /// <summary>
    /// Gets or sets the number of columns in the game.
    /// </summary>
    public uint Columns { get; set; }

    /// <summary>
    /// Gets or sets the tile that the turtle starts in.
    /// </summary>
    public Tile StartingTile { get; set; } = new Tile();

    /// <summary>
    /// Gets or sets the initial turtle's direction.
    /// </summary>
    public Direction InitialDirection { get; set; } = Direction.North;

    /// <summary>
    /// Gets or sets the exit's title.
    /// </summary>
    public Tile ExitTile { get; set; }  = new Tile();

    /// <summary>
    /// Gets or sets the collection of mines.
    /// </summary>
    public List<Tile> Mines { get; set; } = new List<Tile>();

    #endregion

    #region Public methods

    /// <summary>
    /// Validates if the current values are valid for the game.
    /// </summary>
    /// <returns>A <see cref="ValidationResult"/> indicating if the settings are valid and the error message is not.</returns>
    public ValidationResult Validate()
    {
        // Setting are only valid if:
        // Starting position has to be within the board.
        if (!IsTileWithinBoard(StartingTile))
        {
            return new ValidationResult("Invalid starting tile.");
        }
        // Exit position has to be within the board.
        else if (!IsTileWithinBoard(ExitTile))
        {
            return new ValidationResult("Invalid exit tile.");
        }
        // Initial direction must be a valid enum value.
        else if (!Enum.IsDefined<Direction>(InitialDirection))
        {
            return new ValidationResult("Invalid exit position.");
        }
        else
        {
            return new ValidationResult(null);
        }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Determine if a given tile is within the board.
    /// </summary>
    /// <param name="t"><see cref="Tile"/> to validate.</param>
    /// <returns><see langword="true"/> if the tile is within the board; <see langword="false"/> otherwise.</returns>
    private bool IsTileWithinBoard(Tile? t)
    {
        return t != null
            && t.X < Columns
            && t.Y < Rows;
    }

    #endregion
}