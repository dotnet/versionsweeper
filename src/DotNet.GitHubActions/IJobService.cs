using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.GitHubActions
{
    /// <summary>
    /// Based on https://github.com/actions/toolkit/blob/main/packages/core/src/core.ts
    /// </summary>
    public interface IJobService
    {
        bool IsDebug();

        void ExportVariable<T>(string name, T? value = default);

        void SetSecret(string secret);

        void AddPath(string inputPath);

        string GetInput(string name, InputOptions? options = default);

        /// <summary>
        /// Writes the given <paramref name="properties"/> and <paramref name="message"/> arguments
        /// in the following format: "::set-output {properties}::{message}", for example:
        /// "::set-output key-1=some initial value, key-2=anthor value::Hi friends!" when
        /// <paramref name="message"/> is "Hi friends!" and <paramref name="properties"/> is 
        /// new Dictionary{string, string} {  ["key-1"] = "some initial value", ["key-2"] = "another value" });
        /// </summary>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// IJobService job = new DefaultJobService();
        ///
        /// // ::set-output::Hi friends!
        /// job.SetOutput("Hi friends");
        /// 
        /// // ::set-output prop1=Value 1, prop2=Value 2::
        /// job.SetOutput(
        ///      properties: new Dictionary<string, string>
        ///      {
        ///          ["prop1"] = "Value 1",
        ///          ["prop2"] = "Value 2"
        ///      });
        ///
        /// // ::set-output prop1=Value 1, prop2=Value 2::Hi friends!
        /// job.SetOutput(
        ///      message: "Hi friends",
        ///      properties: new Dictionary<string, string>
        ///      {
        ///          ["prop1] = "Value 1",
        ///          ["prop2"] = "Value 2"
        ///      });
        /// ]]>
        /// </code>
        /// </example>
        void SetOutput(string? message, Dictionary<string, string>? properties = default);

        void SetCommandEcho(bool enabled);

        void SetFailed(string message, int? exitCode = null);

        void Debug(string? message);

        void Error(string? message);

        void Warning(string? message);

        void Info(string? message);

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

        /// <summary>
        /// Writes the given <paramref name="stateName"/> and <paramref name="state"/> arguments
        /// in the following format: "::save-state name={stateName}::{state}", for example:
        /// "::save-state name=state-7::The number seven" when
        /// <paramref name="stateName"/> is "state-7" and <paramref name="state"/> is "The number seven"
        /// </summary>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// IJobService job = new DefaultJobService();
        ///
        /// // ::save-state name=state_1::Hi friends!
        /// job.SaveState("state_1", "Hi friends");
        ///
        /// // ::save-state name=state_2::2
        /// job.SaveState("state_2", 2);
        ///
        /// // ::save-state name=state_3::false
        /// job.SaveState("state_3", false);
        /// ]]>
        /// </code>
        /// </example>
        void SaveState<T>(string stateName, T? state = default);

        string GetState(string name);
    }
}
