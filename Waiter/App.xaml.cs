﻿using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using TuiHub.ProcessTimeMonitorLibrary;
using TuiHub.SavedataManagerLibrary;
using Waiter.Helpers;
using Waiter.Models;
using Waiter.Models.Db;
using Waiter.Services;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace Waiter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                // App Host
                services.AddHostedService<ApplicationHostService>();

                // Add debug logging
                services.AddLogging(
                    builder =>
                    {
                        builder.AddDebug();
                    });

                // DbContext
                services.AddDbContext<ApplicationDbContext>();

                // process monitor service
                services.AddSingleton<ProcessTimeMonitor>(p => new ProcessTimeMonitor(null));

                // savedata manager service
                services.AddSingleton<SavedataManager>(p => new SavedataManager((ILogger<SavedataManager>)p.GetRequiredService(typeof(ILogger<SavedataManager>))));

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddScoped<INavigationWindow, Views.Windows.MainWindow>();
                services.AddScoped<ViewModels.MainWindowViewModel>();

                // Other windows
                services.AddScoped<Views.Windows.LoginWindow>();
                services.AddScoped<ViewModels.LoginWindowViewModel>();
                services.AddScoped<Views.Windows.ProgressRingWindow>();
                services.AddScoped<ViewModels.ProgressRingWindowViewModel>();
                services.AddScoped<Views.Windows.ProgressBarWindow>();
                services.AddScoped<ViewModels.ProgressBarWindowViewModel>();

                // Views and ViewModels
                services.AddScoped<Views.Pages.DashboardPage>();
                services.AddScoped<ViewModels.DashboardViewModel>();
                services.AddScoped<Views.Pages.DataPage>();
                services.AddScoped<ViewModels.DataViewModel>();
                services.AddScoped<Views.Pages.SettingsPage>();
                services.AddScoped<ViewModels.SettingsViewModel>();
                services.AddScoped<Views.Pages.AppsPage>();
                services.AddScoped<ViewModels.AppsViewModel>();
                services.AddScoped<Views.Pages.UserPage>();
                services.AddScoped<ViewModels.UserViewModel>();
                services.AddScoped<Views.Pages.AppStorePage>();
                services.AddScoped<ViewModels.AppStoreViewModel>();
                services.AddScoped<Views.Pages.AppCategoryPage>();
                services.AddScoped<ViewModels.AppCategoryViewModel>();

                // Configuration
                services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                              .SetBasePath(GlobalContext.AssemblyDir)
                              .AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            GlobalContext.SystemConfig = configuration.GetSection("SystemConfig").Get<SystemConfig>();
            GlobalContext.GrpcChannel = GrpcChannel.ForAddress(GlobalContext.SystemConfig.ServerURL);

            // ensure data, cache dir created
            Directory.CreateDirectory(GlobalContext.SystemConfig.GetRealDataDirPath());
            Directory.CreateDirectory(GlobalContext.SystemConfig.GetRealCacheDirPath());

            // ensure db created
            using var db = new ApplicationDbContext();
            db.Database.Migrate();

            // ensure user created
            if (db.Users.Any() == false)
                db.Users.Add(new User());
            await db.SaveChangesAsync();

            // set login state from db
            var user = db.Users.First();
            await GlobalContextStateHelper.UpdateLoginState(
                                              string.IsNullOrEmpty(user.AccessToken) ? null : user.AccessToken,
                                              string.IsNullOrEmpty(user.RefreshToken) ? null : user.RefreshToken);

            await _host.StartAsync();

            // set theme
            var isLightTheme = IsLightTheme();
            if (isLightTheme == false)
                Wpf.Ui.Appearance.Theme.Apply(
                    Wpf.Ui.Appearance.ThemeType.Dark,     // Theme type
                    Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                    true                                   // Whether to change accents automatically
                );
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
            var ex = e.Exception;
            var msg = ex.StackTrace ?? "Unknown Error";
            MessageBox.Show("Uncaughted error occured, error message：\n" + msg, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // get system theme
        // from https://www.meziantou.net/detecting-dark-and-light-themes-in-a-wpf-application.htm
        private static bool IsLightTheme()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var value = key?.GetValue("AppsUseLightTheme");
            return value is int i && i > 0;
        }
    }
}