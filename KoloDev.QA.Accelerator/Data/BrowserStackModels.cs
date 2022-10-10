// ***********************************************************************
// Assembly         : KoloDev.GDS.QA.Accelerator
// Author           : KoloDev
// Created          : 02-07-2022
//
// Last Modified By : KoloDev
// Last Modified On : 02-08-2022
// ***********************************************************************
// <copyright file="BrowserStackModels.cs" company="KoloDev Ltd.">
//     Copyright © 2022 KoloDev Ltd. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Newtonsoft.Json;

namespace KoloDev.GDS.QA.Accelerator.Data
{
    /// <summary>
    /// BrowserStack Automation Build Model
    /// </summary>
    public class BrowserStackModels
    {
        /// <summary>
        /// Build Class Model
        /// </summary>
        public partial class Build
        {
            /// <summary>
            /// Automation Build Property
            /// </summary>
            /// <value>The automation build.</value>
            [JsonProperty("automation_build")]
            public AutomationBuild? AutomationBuild { get; set; }
        }

        /// <summary>
        /// Automation Build Class Model
        /// </summary>
        public partial class AutomationBuild
        {
            /// <summary>
            /// Automation build name
            /// </summary>
            /// <value>The name.</value>
            [JsonProperty("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Automation build duration
            /// </summary>
            /// <value>The duration.</value>
            [JsonProperty("duration")]
            public long? Duration { get; set; }

            /// <summary>
            /// Automation build status
            /// </summary>
            /// <value>The status.</value>
            [JsonProperty("status")]
            public string? Status { get; set; }

            /// <summary>
            /// Automation build hashed id
            /// </summary>
            /// <value>The hashed identifier.</value>
            [JsonProperty("hashed_id")]
            public string? HashedId { get; set; }

            /// <summary>
            /// Automation build tag
            /// </summary>
            /// <value>The build tag.</value>
            [JsonProperty("build_tag")]
            public object? BuildTag { get; set; }
        }

        /// <summary>
        /// Sessions within the build
        /// </summary>
        public partial class Sessions
        {
            /// <summary>
            /// An automation session
            /// </summary>
            /// <value>The automation session.</value>
            [JsonProperty("automation_session")]
            public AutomationSession? AutomationSession { get; set; }
        }

        /// <summary>
        /// Automation session model
        /// </summary>
        public partial class AutomationSession
        {
            /// <summary>
            /// The name of the automation session
            /// </summary>
            /// <value>The name.</value>
            [JsonProperty("name")]
            public string? Name { get; set; }

            /// <summary>
            /// The duration of the automation session
            /// </summary>
            /// <value>The duration.</value>
            [JsonProperty("duration")]
            public long? Duration { get; set; }

            /// <summary>
            /// The operating system used in the session
            /// </summary>
            /// <value>The os.</value>
            [JsonProperty("os")]
            public string? Os { get; set; }

            /// <summary>
            /// The version of the operating system used in the session
            /// </summary>
            /// <value>The os version.</value>
            [JsonProperty("os_version")]
            public string? OsVersion { get; set; }

            /// <summary>
            /// The browser version
            /// </summary>
            /// <value>The browser version.</value>
            [JsonProperty("browser_version")]
            public string? BrowserVersion { get; set; }

            /// <summary>
            /// The browser used
            /// </summary>
            /// <value>The browser.</value>
            [JsonProperty("browser")]
            public string? Browser { get; set; }

            /// <summary>
            /// The device used if relevant
            /// </summary>
            /// <value>The device.</value>
            [JsonProperty("device")]
            public string? Device { get; set; }

            /// <summary>
            /// The status of the session
            /// </summary>
            /// <value>The status.</value>
            [JsonProperty("status")]
            public string? Status { get; set; }

            /// <summary>
            /// The hashed id of the session
            /// </summary>
            /// <value>The hashed identifier.</value>
            [JsonProperty("hashed_id")]
            public string? HashedId { get; set; }

            /// <summary>
            /// The reason for the session
            /// </summary>
            /// <value>The reason.</value>
            [JsonProperty("reason")]
            public string? Reason { get; set; }

            /// <summary>
            /// The build name used for the session
            /// </summary>
            /// <value>The name of the build.</value>
            [JsonProperty("build_name")]
            public string? BuildName { get; set; }

            /// <summary>
            /// The project name used for the session
            /// </summary>
            /// <value>The name of the project.</value>
            [JsonProperty("project_name")]
            public string? ProjectName { get; set; }

            /// <summary>
            /// The test priority
            /// </summary>
            /// <value>The test priority.</value>
            [JsonProperty("test_priority")]
            public object? TestPriority { get; set; }

            /// <summary>
            /// Logs if available for the session
            /// </summary>
            /// <value>The logs.</value>
            [JsonProperty("logs")]
            public Uri? Logs { get; set; }

            /// <summary>
            /// The browserstack status for the session
            /// </summary>
            /// <value>The browserstack status.</value>
            [JsonProperty("browserstack_status")]
            public string? BrowserstackStatus { get; set; }

            /// <summary>
            /// Created At
            /// </summary>
            /// <value>The created at.</value>
            [JsonProperty("created_at")]
            public DateTimeOffset? CreatedAt { get; set; }

            /// <summary>
            /// The url for the browser
            /// </summary>
            /// <value>The browser URL.</value>
            [JsonProperty("browser_url")]
            public string? BrowserUrl { get; set; }

            /// <summary>
            /// The public url for the browser
            /// </summary>
            /// <value>The public URL.</value>
            [JsonProperty("public_url")]
            public string? PublicUrl { get; set; }

            /// <summary>
            /// Appium Logs URL
            /// </summary>
            /// <value>The appium logs URL.</value>
            [JsonProperty("appium_logs_url")]
            public string? AppiumLogsUrl { get; set; }

            /// <summary>
            /// The video of the sessions
            /// </summary>
            /// <value>The video URL.</value>
            [JsonProperty("video_url")]
            public Uri? VideoUrl { get; set; }

            /// <summary>
            /// The browser console logs url
            /// </summary>
            /// <value>The browser console logs URL.</value>
            [JsonProperty("browser_console_logs_url")]
            public Uri? BrowserConsoleLogsUrl { get; set; }

            /// <summary>
            /// The har file url
            /// </summary>
            /// <value>The har logs URL.</value>
            [JsonProperty("har_logs_url")]
            public Uri? HarLogsUrl { get; set; }

            /// <summary>
            /// The selenium logs url
            /// </summary>
            /// <value>The selenium logs URL.</value>
            [JsonProperty("selenium_logs_url")]
            public Uri? SeleniumLogsUrl { get; set; }
        }
    }
}