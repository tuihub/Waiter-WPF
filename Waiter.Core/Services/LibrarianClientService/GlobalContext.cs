﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waiter.Core.Services
{
    public static class GlobalContext
    {
        public static string AccessToken { get; set; } = string.Empty;
        public static string RefreshToken { get; set; } = string.Empty;
    }
}
