﻿using System.Windows.Input;
using Wpf.Ui.Common;
using Wpf.Ui.Common.Interfaces;

namespace Waiter.Views.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class UserPage : INavigableView<ViewModels.UserViewModel>
    {
        public ViewModels.UserViewModel ViewModel
        {
            get;
        }

        public UserPage(ViewModels.UserViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }
    }
}