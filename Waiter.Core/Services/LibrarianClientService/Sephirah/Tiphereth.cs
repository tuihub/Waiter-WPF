﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;
using Waiter.Core.Contracts.Services;
using Waiter.Core.Helpers;

namespace Waiter.Core.Services
{
    public partial class LibrarianClientService : ILibrarianClientService
    {
        public async Task<(string, string)> GetTokenAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, string username, string password)
        {
            var request = new GetTokenRequest
            {
                Username = username,
                Password = password
            };
            var response = await client.GetTokenAsync(request);
            return (response.AccessToken, response.RefreshToken);
        }

        public async Task<(string, string)> GetTokenAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client)
        {
            var response = await client.RefreshTokenAsync(
                                            new RefreshTokenRequest(),
                                            headers: JwtHelper.GetMetadataWithRefreshToken());
            return (response.AccessToken, response.RefreshToken);
        }
    }
}
