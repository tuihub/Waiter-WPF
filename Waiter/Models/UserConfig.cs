﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waiter.Models
{
    public partial class UserConfig : ObservableObject
    {
        [ObservableProperty]
        private bool _isLoggedIn;
        [ObservableProperty]
        private int _internalId;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
