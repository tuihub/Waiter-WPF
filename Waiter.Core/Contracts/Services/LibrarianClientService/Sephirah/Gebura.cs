﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;
using Waiter.Core.Models;

namespace Waiter.Core.Contracts.Services
{
    public partial interface ILibrarianClientService
    {
        // get user purchased apps
        Task<IEnumerable<App>> GetPurchasedAppsAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client);
        Task<App> GetAppAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appId);
        // get packages for specific app
        Task<IEnumerable<AppPackage>> GetAppPackagesAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appId);
        Task AddAppPackageRunTimeAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appPackageId, DateTime startDT, TimeSpan duration);
        Task<TimeSpan> GetAppPackageRunTimesAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appPackageId);
        // return UploadToken
        Task<string> UploadGameSaveFileAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appPackageId, FileMetadata fileMetadata);
        Task<string> DownloadGameSaveFileAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long fileMetadataId);
        Task<IEnumerable<GameSave>> GetAppPackageGameSavesAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appPackageId);
        Task RemoveGameSaveFileAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long gameSaveFileId);
        Task<IEnumerable<AppCategory>> ListAppCategoriesAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client);
        Task UpdateAppAppCategoriesAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appId, IEnumerable<long> appCategoryIds);
        Task<IEnumerable<App>> SearchAppsAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, string keyword);
        Task CreateAppCategoryAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, AppCategory appCategory);
        Task UpdateAppCategoryAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, AppCategory appCategory);
        Task RemoveAppCategoryAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appCategoryId);
        Task PurchaseAppAsync(LibrarianSephirahService.LibrarianSephirahServiceClient client, long appId);
    }
}
