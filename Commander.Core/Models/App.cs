﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Commander.Core.Models
{
    public class App
    {
        public long InternalId { get; set; }
        public AppSource Source { get; set; }
        public string? SourceAppId { get; set; }
        public string? SourceUrl { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<string> AltNames { get; set; } = new List<string>();
        public AppType Type { get; set; }
        public string? ShortDescription { get; set; }
        public string? IconImageUrl { get; set; }
        public string? HeroImageUrl { get; set; }
        public AppDetails? AppDetails { get; set; }
        public IEnumerable<long> AppCategoryIds { get; set; } = new List<long>();
        public IEnumerable<string> Tags { get; set; } = new List<string>();
        public App(TuiHub.Protos.Librarian.V1.App app)
        {
            InternalId = app.Id.Id;
            Source = app.Source;
            SourceAppId = string.IsNullOrEmpty(app.SourceAppId) ? null : app.SourceAppId;
            SourceUrl = string.IsNullOrEmpty(app.SourceUrl) ? null : app.SourceUrl;
            Name = app.Name;
            AltNames = app.AltNames;
            Type = app.Type;
            ShortDescription = string.IsNullOrEmpty(app.ShortDescription) ? null : app.ShortDescription;
            IconImageUrl = string.IsNullOrEmpty(app.IconImageUrl) ? null : app.IconImageUrl;
            HeroImageUrl = string.IsNullOrEmpty(app.HeroImageUrl) ? null : app.HeroImageUrl;
            AppDetails = new AppDetails(app.Details);
            AppCategoryIds = app.AppCategoryIds.Select(x => x.Id);
            Tags = app.Tags;
        }
        public App() { }

        public TuiHub.Protos.Librarian.V1.App ToProtoApp()
        {
            var app = new TuiHub.Protos.Librarian.V1.App
            {
                Id = new InternalID { Id = this.InternalId },
                Source = this.Source,
                SourceAppId = this.SourceAppId ?? string.Empty,
                SourceUrl = this.SourceUrl ?? string.Empty,
                Name = this.Name,
                Type = this.Type,
                ShortDescription = this.ShortDescription ?? string.Empty,
                IconImageUrl = this.IconImageUrl ?? string.Empty,
                HeroImageUrl = this.HeroImageUrl ?? string.Empty,
                Details = (this.AppDetails ?? new AppDetails()).ToProtoAppDetails(),
            };
            app.AltNames.AddRange(this.AltNames);
            app.Tags.AddRange(this.Tags);

            return app;
        }
    }
}
