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
            [JsonProperty("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Automation build duration
            /// </summary>
            [JsonProperty("duration")]
            public long? Duration { get; set; }

            /// <summary>
            /// Automation build status
            /// </summary>
            [JsonProperty("status")]
            public string? Status { get; set; }

            /// <summary>
            /// Automation build hashed id
            /// </summary>
            [JsonProperty("hashed_id")]
            public string? HashedId { get; set; }

            /// <summary>
            /// Automation build tag
            /// </summary>
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
            [JsonProperty("name")]
            public string? Name { get; set; }

            /// <summary>
            /// The duration of the automation session
            /// </summary>
            [JsonProperty("duration")]
            public long? Duration { get; set; }

            /// <summary>
            /// The operating system used in the session
            /// </summary>
            [JsonProperty("os")]
            public string? Os { get; set; }

            /// <summary>
            /// The version of the operating system used in the session
            /// </summary>
            [JsonProperty("os_version")]
            public string? OsVersion { get; set; }

            /// <summary>
            /// The browser version
            /// </summary>
            [JsonProperty("browser_version")]
            public string? BrowserVersion { get; set; }

            /// <summary>
            /// The browser used
            /// </summary>
            [JsonProperty("browser")]
            public string? Browser { get; set; }

            /// <summary>
            /// The device used if relevant
            /// </summary>
            [JsonProperty("device")]
            public string? Device { get; set; }

            /// <summary>
            /// The status of the session
            /// </summary>
            [JsonProperty("status")]
            public string? Status { get; set; }

            /// <summary>
            /// The hashed id of the session
            /// </summary>
            [JsonProperty("hashed_id")]
            public string? HashedId { get; set; }

            /// <summary>
            /// The reason for the session
            /// </summary>
            [JsonProperty("reason")]
            public string? Reason { get; set; }

            /// <summary>
            /// The build name used for the session
            /// </summary>
            [JsonProperty("build_name")]
            public string? BuildName { get; set; }

            /// <summary>
            /// The project name used for the session
            /// </summary>
            [JsonProperty("project_name")]
            public string? ProjectName { get; set; }

            /// <summary>
            /// The test priority
            /// </summary>
            [JsonProperty("test_priority")]
            public object? TestPriority { get; set; }

            /// <summary>
            /// Logs if available for the session
            /// </summary>
            [JsonProperty("logs")]
            public Uri? Logs { get; set; }

            /// <summary>
            /// The browserstack status for the session
            /// </summary>
            [JsonProperty("browserstack_status")]
            public string? BrowserstackStatus { get; set; }

            /// <summary>
            /// Created At
            /// </summary>
            [JsonProperty("created_at")]
            public DateTimeOffset? CreatedAt { get; set; }

            /// <summary>
            /// The url for the browser
            /// </summary>
            [JsonProperty("browser_url")]
            public string? BrowserUrl { get; set; }

            /// <summary>
            /// The public url for the browser
            /// </summary>
            [JsonProperty("public_url")]
            public string? PublicUrl { get; set; }

            /// <summary>
            /// Appium Logs URL
            /// </summary>
            [JsonProperty("appium_logs_url")]
            public string? AppiumLogsUrl { get; set; }

            /// <summary>
            /// The video of the sessions
            /// </summary>
            [JsonProperty("video_url")]
            public Uri? VideoUrl { get; set; }

            /// <summary>
            /// The browser console logs url
            /// </summary>
            [JsonProperty("browser_console_logs_url")]
            public Uri? BrowserConsoleLogsUrl { get; set; }

            /// <summary>
            /// The har file url
            /// </summary>
            [JsonProperty("har_logs_url")]
            public Uri? HarLogsUrl { get; set; }

            /// <summary>
            /// The selenium logs url
            /// </summary>
            [JsonProperty("selenium_logs_url")]
            public Uri? SeleniumLogsUrl { get; set; }
        }
    }
}