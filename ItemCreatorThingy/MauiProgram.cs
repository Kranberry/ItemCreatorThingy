using Microsoft.AspNetCore.Components.WebView.Maui;
using MudBlazor.Services;


namespace ItemCreatorThingy;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		//GoogleDriveApi api = new();
  //      api.DownloadJsonFilesFromDrive().Wait();


        var builder = MauiApp.CreateBuilder();
        builder.Services.AddMudServices();

        builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif


        CreateLocalAppFolder();

		return builder.Build();
	}

	private static void CreateLocalAppFolder()
    {
		string f = AppDefaults.LocalFolderFilesPath();
        System.IO.Directory.CreateDirectory(f);
    }
}
