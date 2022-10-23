namespace Services;


/// <summary>
/// Implements a service that loads/saves sequences of <see cref="Actions"/> from/to files.
/// </summary>
internal class ActionsServices
{
    #region Public static methods

    /// <summary>
    /// Loads a file with the actions to perform.
    /// </summary>
    /// <param name="path">Path of the file to load.</param>
    /// <returns>The actions to do in the game.</returns>
    /// <exception cref="System.ArgumentException">Invalid <paramref name="path"/>.</exception>
    /// <exception cref="System.ArgumentNullException">Null <paramref name="path"/> -or- empty file contents.</exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Could not open <paramref name="path"/>.</exception>
    /// <exception cref="System.IO.FileNotFoundException">The file specified in path was not found.</exception>
    /// <exception cref="System.NotSupportedException"><paramref name="path"/> is in an invalid format -or- Could not convert JSON.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="System.IO.InvalidDataException">The file data is not valid.</exception>
    public static Actions[] LoadFromFile(string path)
    {
        var result = new List<Actions>();

        // INFO: No try..catch as we want the caller to deal with any exceptions raised here.
        var lines = File.ReadAllLines(path, System.Text.Encoding.UTF8);

        for (int i = 0; i < lines.Length; i++)
        {
            // Parse the line.
            try
            {
                result.AddRange(ParseLine(lines[i]));
            }
            catch (NotSupportedException nsEx)
            {
                // Add the line number to the exception and rethrow it.
                throw new InvalidDataException(string.Format("Error on line {0}: {1}", i, nsEx.Message));
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// Saves the moves to file.
    /// </summary>
    /// <param name="path">Path of the file to save.</param>
    /// <param name="actions"><see cref="Actions"/> to save.</param>
    /// <exception cref="System.ArgumentException">Invalid <paramref name="path"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="actions"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Could not open <paramref name="path"/>.</exception>
    /// <exception cref="System.NotSupportedException"><paramref name="path"/> is in an invalid format -or- Could not convert JSON.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    public static void SaveToFile(string path, IEnumerable<Actions> actions)
    {
        // Null check.
        if (actions == null)
        {
            throw new ArgumentNullException(nameof(actions));
        }

        // Serialize to CSV.
        var actionsCSV = ActionsToCSV(actions);
        
        File.WriteAllText(path, actionsCSV, System.Text.Encoding.UTF8);
    }

    #endregion

    #region Private static methods

    /// <summary>
    /// Parses a line to a collection of <see cref="Actions"/>.
    /// </summary>
    /// <param name="line">The line with the actions in CSV format.</param>
    /// <returns>A collection of parsed <see cref="Action"/>.</returns>
    /// <exception cref="NotSupportedException"><paramref name="line"/> has an invalid action value.</exception>
    private static IEnumerable<Actions> ParseLine(string line)
    {
        var result = new List<Actions>();

        // Line should be in CVS format.
        foreach(var segment in line.Split(',', StringSplitOptions.TrimEntries))
        {
            // Empty check.
            if (!string.IsNullOrWhiteSpace(segment)
                && TryParseString(segment, out Actions action))
            {
                result.Add(action);
            }
            else
            {
                // Invalid/unkown move.
                throw new NotSupportedException(string.Format("Unsuported action: {0}", segment));
            }
        }

        return result;
    }

    /// <summary>
    /// Try parsing a string to an <see cref="Actions"/>.
    /// </summary>
    /// <param name="s">String to parse.</param>
    /// <param name="action">Parsed <see cref="Action"/>.</param>
    /// <returns><see langword="true"/> if parsing was completed; <see langword="false"/> otherwise.</returns>
    private static bool TryParseString(string s, out Actions action)
    {
        var result = false;

        // Try to convert the string to one of the moves.
        switch (s.ToLowerInvariant())
        {
            case nameof(Actions.Move):
            case "m":
                // String is a Move action.
                action = Actions.Move;
                result = true;
                break;

            case nameof(Actions.Rotate):
            case "r":
                // String is a Move action.
                action = Actions.Rotate;
                result = true;
                break;

            default:
                // Assign a invalid value to move as we could not convert the string to a Move.
                action = (Actions)100;
                break;
        }

        return result;
    }

    /// <summary>
    /// Converts a collections of <see cref="Actions"/> to a CSV string.
    /// </summary>
    /// <param name="actions"><see cref="Actions"/> to convert.</param>
    /// <returns>A string </returns>
    private static string ActionsToCSV(IEnumerable<Actions> actions)
    {
        // Parse the actions to string and strip all but the 1st char.
        // Move => m;
        // Rotate => r;
        var textActions = (from a in actions ?? Array.Empty<Actions>()
                           select a.ToString().ToLowerInvariant().First());

        // Joing the previous actions with a ",".
        return string.Join(",", textActions ?? Array.Empty<char>());
    }

    #endregion
}