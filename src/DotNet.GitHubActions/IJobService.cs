using System;
using System.Threading.Tasks;

namespace DotNet.GitHubActions
{
    /// <summary>
    /// Based on https://github.com/actions/toolkit/blob/main/packages/core/src/core.ts
    /// </summary>
    public interface IJobService
    {
        void ExportVariable(string name, object? value = default);

        void SetSecret(string secret);

        void AddPath(string inputPath);

        string GetInput(string name, InputOptions? options = default);

        void SetOutput(string name, object? value = default);

        void SetCommandEcho(bool enabled);

        void SetFailed(string message);

        bool IsDebug();

        void Debug(string message);

        void Error(string message);

        void Warning(string message);

        void Info(string message);

        void StartGroup(string name);

        void EndGroup();

        async Task<T> GroupAsync<T>(string name, Func<Task<T>> func) where T : notnull
        {
            StartGroup(name);

            T result;
            try
            {
                result = await func();
            }
            finally
            {
                EndGroup();
            }

            return result;
        }

        void SaveState(string name, object? value = default);

        string GetState(string name);
    }
}
