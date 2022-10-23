namespace Models;

/// <summary>
/// Represents a board tile.
/// </summary>
internal class Tile
{
    #region Properties

    /// <summary>
    /// Gets or sets the X tile position.
    /// </summary>
    public uint X { get; set; }

    /// <summary>
    /// Gets or sets the Y title position.
    /// </summary>
    public uint Y { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new instance of <see cref="Tile"/> in the default position.
    /// </summary>
    public Tile() : this(0, 0)
    { }

    /// <summary>
    /// Creates a new instance of <see cref="Tile"/> with a pre-defined position.
    /// </summary>
    /// <param name="x">Initial <see cref="X"/> value.</param>
    /// <param name="y">Initial <see cref="Y"/> value.</param>
    public Tile(uint x, uint y)
    {
        X = x;
        Y = y;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Builds a string representation of this instance.
    /// </summary>
    /// <returns>A string representation of this instance.</returns>
    public override string ToString()
    {
        return string.Format("{0}, {1}", X, Y);
    }

    public override bool Equals(object? obj)
    {
        return (obj != null
            && obj is Tile t
            && t.X == this.X
            && t.Y == this.Y);
    }

    public override int GetHashCode()
    {
        return this.X.GetHashCode() ^ this.Y.GetHashCode();
    }

    #endregion
}