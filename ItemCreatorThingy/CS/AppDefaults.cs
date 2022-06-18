namespace ItemCreatorThingy;

internal class AppDefaults
{
    internal static string GoogleDriveApiKey = "AIzaSyBtx6O9gGZh2cbxo8KbZK0lWoYDkKjJD6M";

    public static string LocalFolderFilesPath() => $"{Microsoft.Maui.Storage.FileSystem.AppDataDirectory}/ItemCreatorThingy/";
}