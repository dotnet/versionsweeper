﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.GitHub
{
    public sealed class GitHubLabelService : IGitHubLabelService, IDisposable
    {
        readonly ResilientGitHubClientFactory _clientFactory;
        readonly ILogger<GitHubLabelService> _logger;
        readonly IMemoryCache _cache;

        private readonly SemaphoreSlim _labelCreationSemaphore = new(1, 1);
        private Label _label = null!;

        public GitHubLabelService(
            ResilientGitHubClientFactory clientFactory,
            ILogger<GitHubLabelService> logger,
            IMemoryCache cache) =>
            (_clientFactory, _logger, _cache) = (clientFactory, logger, cache);

        public async ValueTask<Label> GetOrCreateLabelAsync(
            string owner, string name, string token)
        {
            var labelsClient = GetLabelsClient(token);
            var labels = await _cache.GetOrCreateAsync(
                $"{owner}/{name}/labels",
                async _ => await labelsClient.GetAllForRepository(owner, name));

            static bool TryFindLabel(IReadOnlyList<Label> labels, out Label? label)
            {
                label = labels?.FirstOrDefault(l => l.Name == DefaultLabel.Name);
                return label is not null;
            }

            if (TryFindLabel(labels, out var label))
            {
                return label!;
            }
            else
            {
                await _labelCreationSemaphore.WaitAsync();
                try
                {
                    if (_label is null)
                    {
                        _logger.LogInformation($"Creating '{DefaultLabel.Name}' label.");
                    }

                    return _label ??= await CreateLabelAsync(labelsClient, owner, name);
                }
                finally
                {
                    _labelCreationSemaphore.Release();
                }
            }
        }

        IIssuesLabelsClient GetLabelsClient(string token)
        {
            var client = _clientFactory.Create(
                productHeaderValue: Product.Header,
                credentials: new(token));

            var labelsClient = client.Issue.Labels;
            return labelsClient;
        }

        static Task<Label> CreateLabelAsync(
            IIssuesLabelsClient labelsClient, string owner, string name) =>
            labelsClient.Create(owner, name, DefaultLabel.Value);

        public void Dispose() => _labelCreationSemaphore.Dispose();
    }
}