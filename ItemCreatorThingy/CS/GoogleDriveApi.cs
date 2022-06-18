using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.Diagnostics;
using static Google.Apis.Drive.v3.AboutResource;

namespace ItemCreatorThingy;
internal class GoogleDriveApi
{
    private const string BaseUrl = "https://www.googleapis.com/drive/v3";
    private const string ApplicationName = "ItemCreatorThingy";
    private static HttpClient HttpClient { get; set; }

    private async Task<UserCredential> GetUserCredentials()
    {
        string credentialsPath = @"C:\Users\Jesus Kvistus\source\repos\ItemCreatorThingy\ItemCreatorThingy\Credentials\credentials.json";
        string credPath = AppDefaults.LocalFolderFilesPath() + "token.json";
        string[] Scopes = { DriveService.Scope.DriveReadonly };

        using FileStream stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read);
        UserCredential credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(credPath, true));

        return credentials;
    }

    private async Task<DriveService> GetDriveService()
    {
        //return new(new BaseClientService.Initializer { ApiKey = AppDefaults.GoogleDriveApiKey });
        return new(new BaseClientService.Initializer { HttpClientInitializer = await GetUserCredentials(), ApplicationName = ApplicationName });
    }

    public async Task DownloadJsonFilesFromDrive()
    {
        // List folders with name
        IList<Google.Apis.Drive.v3.Data.File> folders = await ListFolders("Items");
        if (folders == null || folders.Count == 0)
        {
            return;
        }

        // Start downloading the json files
        foreach (Google.Apis.Drive.v3.Data.File folder in folders)
        {
            if (folders == null || folders.Count == 0)
            {
                continue;
            }

            // List all files in folder
            IList<Google.Apis.Drive.v3.Data.File> files = await ListFilesInFolder(folder.Id, ".json");
            foreach (Google.Apis.Drive.v3.Data.File file in files)
            {
                // Download file
                string path = AppDefaults.LocalFolderFilesPath() + folder.Name + "/" + file.Name;
                await DownloadFile(file.Id, path);
            }
        }
    }

    public async Task DownloadFile(string fileId, string downloadToPath)
    {
        DriveService service = await GetDriveService();
        Google.Apis.Drive.v3.FilesResource.GetRequest fileToDownload = service.Files.Get(fileId);
        using MemoryStream stream = new();

        fileToDownload.MediaDownloader.ProgressChanged += progress =>
        {
            switch (progress.Status)
            {
                case DownloadStatus.Completed:
                {
                    using FileStream fileStream = new(downloadToPath, FileMode.Create);
                    stream.Position = 0;
                    stream.CopyTo(fileStream);
                    break;
                }
            }
        };
        fileToDownload.Download(stream);
    }

    private async Task<IList<Google.Apis.Drive.v3.Data.File>> ListFolders(string name)
    {
        DriveService service = await GetDriveService();
        FilesResource.ListRequest folderRequest = service.Files.List();
        folderRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{name}'";
        folderRequest.Spaces = "drive";
        folderRequest.PageSize = 5;
        folderRequest.Fields = "nextPageToken, files(id, name)";

        IList<Google.Apis.Drive.v3.Data.File> folders = folderRequest.Execute().Files;
        return folders;
    } 

    private async Task<IList<Google.Apis.Drive.v3.Data.File>> ListFilesInFolder(string folderId, string fileEnd)
    {
        DriveService service = await GetDriveService();
        FilesResource.ListRequest fileRequest = service.Files.List();
        fileRequest.Q = $"parents in '{folderId}' and name contains '{fileEnd}'";
        fileRequest.Spaces = "drive";
        fileRequest.PageSize = 50;
        fileRequest.Fields = "nextPageToken, files(id, name)";

        IList<Google.Apis.Drive.v3.Data.File> files = fileRequest.Execute().Files;
        return files;
    }

    //private string[] GetFileIds()
    //{

    //}
}