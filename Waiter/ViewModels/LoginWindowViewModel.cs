﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using TuiHub.Protos.Librarian.Sephirah.V1;
using Waiter.Helpers;
using Waiter.Views.Windows;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Controls;
using static Waiter.GlobalContext;

namespace Waiter.ViewModels
{
    public partial class LoginWindowViewModel : ObservableObject
    {
        public LoginWindowViewModel() { }
        public LoginWindowViewModel(LoginWindow? window)
        {
            ParentWindow = window;
        }

        public LoginWindow? ParentWindow { get; set; }

        public bool IsCodeClosed { get; set; } = false;

        [ObservableProperty]
        private string _username = string.Empty;

        [RelayCommand]
        private async Task OnBtnLoginClickAsync(object passwordBox)
        {
            // clear GlobalContext state
            GlobalContext.UserConfig.InternalId = 0;
            await GlobalContextStateHelper.UpdateLoginState(null, null);

            // get password from passwordBox
            var password = (passwordBox as Wpf.Ui.Controls.PasswordBox)!.Password;

            var progressRingDialog = new ProgressRingWindow();
            progressRingDialog.Owner = ParentWindow;
            progressRingDialog.ViewModel.WorkText = "Logging in...";
            progressRingDialog.Show();
            ParentWindow!.IsEnabled = false;

            var loginWorker = new BackgroundWorker();
            loginWorker.WorkerReportsProgress = true;
            loginWorker.DoWork += loginWorker_DoWork;
            loginWorker.ProgressChanged += loginWorker_ProgressChanged;
            loginWorker.RunWorkerCompleted += loginWorker_RunWorkerCompleted;
            loginWorker.RunWorkerAsync(password);
        }

        private void loginWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            // close progressDialog
            foreach (Window w in this.ParentWindow!.OwnedWindows)
            {
                if (w is ProgressRingWindow)
                {
                    w.Close();
                }
            }
            if ((e.Result as EventResult).IsSucceed == false)
            {
                var uiMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Login error",
                    Content = (e.Result as EventResult).Message,
                    Owner = ParentWindow
                };
                uiMessageBox.ShowDialog();
                ParentWindow!.IsEnabled = true;
            }
            else
            {
                this.IsCodeClosed = true;
                ParentWindow!.Close();
            }
        }

        private void loginWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void loginWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            e.Result ??= new EventResult();
            var result = e.Result as EventResult;
            try
            {
                var client = new LibrarianSephirahService.LibrarianSephirahServiceClient(GrpcChannel);
                var password = e.Argument as string;
                var (accessToken, refreshToken) = LibrarianClientService.GetTokenAsync(client, Username, password).Result;
                // set GlobalContext
                await GlobalContextStateHelper.UpdateLoginState(accessToken, refreshToken);
                // TODO: Impl get user internal id func
                GlobalContext.UserConfig.InternalId = 2333;
                result.IsSucceed = true;
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Message = ex.Message;
            }
        }
        private class EventResult
        {
            public bool IsSucceed { get; set; }
            public string Message { get; set; } = string.Empty;
        }
        public class CodeClosingEventArgs : EventArgs
        {
        }
    }
}
