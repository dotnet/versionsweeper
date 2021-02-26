// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Extensions;

namespace DotNet.GitHub
{
    public sealed class GitHubLabelService : IGitHubLabelService, IDisposable
    {
        readonly IResilientGitHubClientFactory _clientFactory;
        readonly ILogger<GitHubLabelService> _logger;
        readonly IMemoryCache _cache;

        private readonly SemaphoreSlim _labelCreationSemaphore = new(1, 1);
        private Label _label = null!;

        public GitHubLabelService(
            IResilientGitHubClientFactory clientFactory,
            ILogger<GitHubLabelService> logger,
            IMemoryCache cache) =>
            (_clientFactory, _logger, _cache) = (clientFactory, logger, cache);

        public async ValueTask<Label> GetOrCreateLabelAsync(
            string owner, string name, string token)
        {
            var labelsClient = GetLabelsClient(token);
            var cacheKey = $"{owner}/{name}/labels";
            var labels = await _cache.GetOrCreateAsync(
                cacheKey,
                async _ => await labelsClient.GetAllForRepository(owner, name));

            var label = labels?.FirstOrDefault(l => l.Name == DefaultLabel.Name);
            if (label is not null)
            {
                return _label = label;
            }
            else
            {
                await _labelCreationSemaphore.WaitAsync();
                try
                {
                    if (_label is null)
                    {
                        _logger.LogInformation($"Creating '{DefaultLabel.Name}' label.");
                        _label = await CreateLabelAsync(labelsClient, owner, name);
                        _cache.Remove(cacheKey);
                    }

                    return _label;
                }
                finally
                {
                    _labelCreationSemaphore.Release();
                }
            }
        }

        IIssuesLabelsClient GetLabelsClient(string token)
        {
            var client = _clientFactory.Create(token);
            var labelsClient = client.Issue.Labels;
            return labelsClient;
        }

        static Task<Label> CreateLabelAsync(
            IIssuesLabelsClient labelsClient, string owner, string name) =>
            labelsClient.Create(owner, name, DefaultLabel.Value);

        public void Dispose() => _labelCreationSemaphore.Dispose();
    }
}
