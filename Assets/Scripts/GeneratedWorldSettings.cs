// Stores the settings that a world was last generated with, if one was generated
// during this game session. This is used to provide data to classes in the main
// game scene that use it to create a new save file.
public static class GeneratedWorldSettings
{
    public static string worldName;
    public static string worldSaveId;
}
