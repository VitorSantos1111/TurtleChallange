using System.Text.Json;

namespace Services;

/// <summary>
/// Implements a service that loads/saves <see cref="Models.Settings"/> from/to files.
/// </summary>
internal class SettingsService
{

    #region Public static methods

    /// <summary>
    /// Loads a settings file.
    /// </summary>
    /// <param name="path">Path of the file to load.</param>
    /// <returns>The settings within the file.</returns>
    /// <exception cref="System.ArgumentException">Invalid <paramref name="path"/>.</exception>
    /// <exception cref="System.ArgumentNullException">Null <paramref name="path"/> -or- empty file contents.</exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Could not open <paramref name="path"/>.</exception>
    /// <exception cref="System.IO.FileNotFoundException">The file specified in path was not found.</exception>
    /// <exception cref="System.NotSupportedException"><paramref name="path"/> is in an invalid format -or- Could not convert JSON.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="System.Text.Json.JsonException">The JSON is invalid.</exception>
    /// <exception cref="System.IO.InvalidDataException">The serialized <see cref="Models.Settings"/> is not valid.</exception>
    public static Models.Settings LoadFromFile(string path)
    {
        // INFO: No try..catch as we want the caller to deal with any exceptions raised here.
        var jsonContents = File.ReadAllText(path, System.Text.Encoding.UTF8);
        // INFO: Throw ArgumentNullException is the desirialized JSON is null.
        Models.Settings settings = JsonSerializer.Deserialize<Models.Settings>(jsonContents, GetSerializerOptions()) ?? throw new ArgumentNullException(nameof(path));

        // Validate the settings.
        var isValid = settings.Validate();
        if (!string.IsNullOrEmpty(isValid?.ErrorMessage ?? ""))
        {
            // Settings are not valid!
            throw new InvalidDataException(isValid?.ErrorMessage);
        }

        return settings;
    }

    /// <summary>
    /// Saves the settings to file.
    /// </summary>
    /// <param name="path">Path of the file to save.</param>
    /// <param name="settings"><see cref="Models.Settings"/> to save.</param>
    /// <exception cref="System.ArgumentException">Invalid <paramref name="path"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> or <paramref name="settings"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="System.IO.IOException">An I/O error occurred while opening the file.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Could not open <paramref name="path"/>.</exception>
    /// <exception cref="System.NotSupportedException"><paramref name="path"/> is in an invalid format -or- Could not convert JSON.</exception>
    /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
    public static void SaveToFile(string path, Models.Settings settings)
    {
        // Null check.
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }
        
        // Serialize to JSON.
        // Save enums as strings.
        // Adapted from: https://www.techrepository.in/blog/posts/serializing-enums-as-strings-using-system-text-json-library-in-net-core-3-0
        var sJson = System.Text.Json.JsonSerializer.Serialize(settings, GetSerializerOptions());
        
        File.WriteAllText(path, sJson, System.Text.Encoding.UTF8);
    }

    #endregion

    #region Private static methods
    
    /// <summary>
    /// Gets the common <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="JsonSerializerOptions"/>.</returns>
    private static JsonSerializerOptions GetSerializerOptions()
    {
        return new JsonSerializerOptions{
            Converters = {
                new System.Text.Json.Serialization.JsonStringEnumConverter()
            }
        };
    }
    #endregion
}