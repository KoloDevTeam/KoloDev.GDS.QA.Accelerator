using BrowserStack;
using KoloDev.GDS.QA.Accelerator.Data;
using KoloDev.GDS.QA.Accelerator.Selenium;
using KoloDev.GDS.QA.Accelerator.Utility;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using Selenium.Axe;
using static KoloDev.GDS.QA.Accelerator.Data.KoloTestSuite;

namespace KoloDev.GDS.QA.Accelerator;

/// <summary>
/// KoloQA is the main object for manipulating Selenium Sessions.
/// </summary>
public class KoloQA
{
    private KoloTestSuite? _testSuite;

    /// <summary>
    /// Kolo Test Suite Information
    /// </summary>
    public KoloTestSuite? TestSuite { get => _testSuite; set => _testSuite = value; }

    private Local Local { get; set; } = new();

    /// <summary>
    /// Remote Web Driver Instance
    /// </summary>
    private IWebDriver Driver { get; set; } = null!;

    /// <summary>
    /// BrowserStack Project Name
    /// </summary>
    private string ProjectName { get; set; } = "";

    /// <summary>
    /// BrowserStack Build Name
    /// </summary>
    private string BuildName { get; set; } = "";

    /// <summary>
    /// BrowserStack Test Name
    /// </summary>
    private string TestName { get; set; } = "";

    /// <summary>
    /// Browserstack User Name
    /// </summary>
    private string UserName { get; set; } = "";

    private bool LocalTest { get; set; }

    /// <summary>
    /// BrowserStack Access Key
    /// </summary>
    private string AccessKey { get; set; } = "";

    /// <summary>
    /// Add Test Cases to Collection
    /// </summary>
    /// <param name="id">The identifier for the Test Case</param>
    /// <param name="stepNumber">The Step Number of the Test Step</param>
    /// <param name="name">The Name of the Test Step</param>
    /// <param name="description">The Description of the Test Step</param>
    /// <param name="passed">Whether the Test Step Passed or Not</param>
    /// <returns></returns>
    public KoloQA AddStepToTestCase(string id, int stepNumber, string name, string description, bool passed)
    {
        try
        {
            if (_testSuite == null)
            {
                _testSuite = new KoloTestSuite();
                _testSuite.TestCases ??= new List<KoloTestCase>();
            }
            if (_testSuite.TestCases != null && _testSuite.TestCases.Any(i => i.Id == id))
            {
                var testCase = _testSuite.TestCases.Single(i => i.Id == id);
                var step = new KoloTestSteps
                {
                    Name = name,
                    Description = description,
                    Passed = passed,
                    StepNumber = stepNumber
                };
                testCase.TestSteps.Add(step);
            }
            else
            {
                var testCase = new KoloTestCase
                {
                    Id = id
                };

                var step = new KoloTestSteps
                {
                    StepNumber = stepNumber,
                    Name = name,
                    Description = description,
                    Passed = passed
                };
                testCase.TestSteps.Add(step);
                if (_testSuite.TestCases is not null) _testSuite.TestCases.Add(testCase);
            }
        }
        catch
        {
            throw;
        }
        return this;
    }

    /// <summary>
    /// Check the Accessibility of the Page
    /// </summary>
    /// <param name="pageName"></param>
    /// <param name="wcagLevel"></param>
    /// <returns></returns>
    public KoloQA AccessibilityOnPage(string pageName, WcagLevel wcagLevel = WcagLevel.wcag2aa)
    {
        try
        {
            const string folderName = @"../../../AccessibilityReports";
            // If directory does not exist, create it
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            var axeResult = new AxeBuilder(Driver)
                .WithTags(wcagLevel.ToString())
                .Analyze();
            Driver.CreateAxeHtmlReport(axeResult, "../../../AccessibilityReports/" + pageName + ".html");
            var accessibility = File.ReadAllText("../../../AccessibilityReports/" + pageName + ".html");

            accessibility = GdsHtmlPage.ApplyGdsStylingToAccessibilityReport(accessibility, pageName);
            File.WriteAllText("../../../AccessibilityReports/" + pageName + ".html", accessibility);
        }
        catch (Exception e)
        {
            Console.Write(e.Message);
        }
        return this;
    }

    /// <summary>
    /// Outputs the Completed Steps to this Point
    /// </summary>
    /// <param name="Id">The Id given to the test</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA OutputCompletedStepsToThisPoint(string Id)
    {
        if (_testSuite?.TestCases == null) return this;
        var testCase = _testSuite.TestCases.Single(i => i.Id == Id);
        TestContext.WriteLine("Test Case: " + testCase.Id);
        foreach (KoloTestSteps step in testCase.TestSteps)
        {
            TestContext.WriteLine("Test Step: " + step.StepNumber + " - " + step.Name + " " + step.Description + " - Step Passed: " + step.Passed.ToString());
        }

        return this;
    }

    /// <summary>
    /// Sets The BrowserStack Test Name
    /// </summary>
    /// <param name="testName">Name of the Test</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA BrowserstackTestName(string testName)
    {
        TestName = testName;
        return this;
    }

    /// <summary>
    /// Sets the Routing of the traffic to the local execution environment for access to services not published on the internet
    /// </summary>
    /// <param name="localTest">If True will route traffic locally</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA LocalTestRun(bool localTest)
    {
        LocalTest = localTest;
        return this;
    }

    /// <summary>
    /// Sets the BrowserStack User Name
    /// </summary>
    /// <param name="userName">BrowserStack User Name</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA BrowserstackUserName(string userName)
    {
        UserName = userName;
        return this;
    }

    /// <summary>
    /// Sets the BrowserStack Access Key
    /// </summary>
    /// <param name="accessKey">BrowserStack Access Key</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA BrowserstackAccessKey(string accessKey)
    {
        AccessKey = accessKey;
        return this;
    }

    /// <summary>
    /// Sets the BrowserStack Project Name
    /// </summary>
    /// <param name="projectName">BrowserStack Project Name</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA BrowserstackProjectName(string projectName)
    {
        ProjectName = projectName;
        return this;
    }

    /// <summary>
    /// Sets the BrowserStack Build Name
    /// </summary>
    /// <param name="buildName">BrowserStack Build Name</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA BrowserstackBuildName(string buildName)
    {
        BuildName = buildName;
        return this;
    }

    /// <summary>
    /// Starts a Local Chrome Instance
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA LaunchChrome()
    {
        TestContext.WriteLine(@"
NNN/  :dNNd- .ohNMMNds:   -NNN.       :sdNMMNdo.
MMM/.yMMMo  sMMMmhydMMMd. -MMM.     .dMMMdyymMMMy`
MMMhNMMh.  yMMm-    `yMMN`-MMM.    `mMMh`    -mMMh
MMMMMMM/   NMM+      .MMM:-MMM.    -MMM.      +MMM
MMMMyNMMh. yMMm-    `yMMN`-MMM.    `mMMh.    -mMMy
MMM+ `hMMN+`sMMMmhydMMMd. -MMMhhhhh/.dMMMdyhmMMMy`
mmm:   /mmmy`.ohmMMNds:   -mmmmmmmm+  -sdNMMNho.");
        TestContext.WriteLine("--------------------------------------------------------");
        TestContext.WriteLine("----------Launching Browser Session for Tests-----------");
        TestContext.WriteLine("--------------------------------------------------------");
        TestContext.WriteLine("KoloQA: In Web Driver Starter Local Chrome Launching");
        Driver = new ChromeDriver();
        return this;
    }

    /// <summary>
    /// Maximises the Browser Window
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA MaximiseBrowser()
    {
        Driver.Manage().Window.Maximize();
        return this;
    }

    /// <summary>
    /// Opens a Local Connection Proxy for Browserstack to route traffic to protected services
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA OpenBrowserStackLocalConnection()
    {
        try
        {
            var bsLocalArgs = new List<KeyValuePair<string, string>> {
                new("key", AccessKey) };
            Local.start(bsLocalArgs);
            TestContext.WriteLine("KoloQA: Initiating Local Connection to Route traffic to protected services");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return this;
    }

    /// <summary>
    /// Closes the Local Connection proxy for Browserstack to route traffic to protected services
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA CloseBrowserStackLocalConnection()
    {
        Local.stop();
        TestContext.WriteLine("KoloQA: Closing Local Connection to Route traffic to protected services");
        return this;
    }

    /// <summary>
    /// Starts a BrowserStack Session with the desired Client
    /// </summary>
    /// <param name="client">A client from the predesignated list of browsers and operating systems</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA BrowserStackSession(Browser client)
    {
        TestContext.WriteLine(@"
NNN/  :dNNd- .ohNMMNds:   -NNN.       :sdNMMNdo.
MMM/.yMMMo  sMMMmhydMMMd. -MMM.     .dMMMdyymMMMy`
MMMhNMMh.  yMMm-    `yMMN`-MMM.    `mMMh`    -mMMh
MMMMMMM/   NMM+      .MMM:-MMM.    -MMM.      +MMM
MMMMyNMMh. yMMm-    `yMMN`-MMM.    `mMMh.    -mMMy
MMM+ `hMMN+`sMMMmhydMMMd. -MMMhhhhh/.dMMMdyhmMMMy`
mmm:   /mmmy`.ohmMMNds:   -mmmmmmmm+  -sdNMMNho.");
        Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
        browserstackOptions.Add("projectName", ProjectName);
        browserstackOptions.Add("buildName", BuildName);
        browserstackOptions.Add("sessionName", TestName + " - " + DateTime.Now.ToString("dddd, dd MMMM yyyy"));
        browserstackOptions.Add("userName", UserName);
        browserstackOptions.Add("accessKey", AccessKey);

        if (LocalTest)
        {
            browserstackOptions.Add("local", "true");
        }

        TestContext.WriteLine("--------------------------------------------------------");
        TestContext.WriteLine("----------Launching Browser Session for Tests-----------");
        TestContext.WriteLine("--------------------------------------------------------");
        TestContext.WriteLine("KoloQA: In Web Driver Starter " + client.ToString() + " Requested on BrowserStack");
        try
        {
            if (client.ToString() == "LocalChrome")
            {
                LaunchChrome();
            }
            if (client.ToString() == "iPhoneLandscape")
            {
                var capabilities = new SafariOptions();
                browserstackOptions.Add("osVersion", "15");
                browserstackOptions.Add("device", "iPhone 13");
                browserstackOptions.Add("realMobile", "true");
                browserstackOptions.Add("deviceOrientation", "landscape");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "iPhonePortrait")
            {
                var capabilities = new SafariOptions();
                browserstackOptions.Add("osVersion", "15");
                browserstackOptions.Add("deviceName", "iPhone 13");
                browserstackOptions.Add("realMobile", "true");
                browserstackOptions.Add("deviceOrientation", "portrait");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "iPadPortrait")
            {
                var capabilities = new SafariOptions();
                browserstackOptions.Add("osVersion", "14");
                browserstackOptions.Add("deviceName", "iPad Air 4");
                browserstackOptions.Add("realMobile", "true");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "iPadLandscape")
            {
                var capabilities = new SafariOptions();
                browserstackOptions.Add("osVersion", "14");
                browserstackOptions.Add("deviceName", "iPad Air 4");
                browserstackOptions.Add("realMobile", "true");
                browserstackOptions.Add("deviceOrientation", "landscape");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "SamsungGalaxyAndroidLandscape")
            {
                var capabilities = new ChromeOptions();
                browserstackOptions.Add("osVersion", "11.0");
                browserstackOptions.Add("deviceName", "Samsung Galaxy S21");
                browserstackOptions.Add("realMobile", "true");
                browserstackOptions.Add("deviceOrientation", "landscape");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "SamsungGalaxyAndroidPortrait")
            {
                var capabilities = new ChromeOptions();
                browserstackOptions.Add("osVersion", "11.0");
                browserstackOptions.Add("deviceName", "Samsung Galaxy S21");
                browserstackOptions.Add("realMobile", "true");
                browserstackOptions.Add("deviceOrientation", "portrait");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "GooglePixel4XLAndroidLandscape")
            {
                var capabilities = new ChromeOptions();
                browserstackOptions.Add("osVersion", "10.0");
                browserstackOptions.Add("deviceName", "Google Pixel 4 XL");
                browserstackOptions.Add("realMobile", "true");
                browserstackOptions.Add("deviceOrientation", "landscape");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "GooglePixel4XLAndroidPortrait")
            {
                var capabilities = new ChromeOptions();
                browserstackOptions.Add("osVersion", "10.0");
                browserstackOptions.Add("deviceName", "Google Pixel 4 XL");
                browserstackOptions.Add("realMobile", "true");
                browserstackOptions.Add("deviceOrientation", "portrait");
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                return this;
            }
            if (client.ToString() == "OSXSafari")
            {
                var capabilities = new SafariOptions();
                capabilities.BrowserVersion = "14.0";
                browserstackOptions.Add("os", "OS X");
                browserstackOptions.Add("osVersion", "Big Sur");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "OSXChrome")
            {
                var capabilities = new ChromeOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "OS X");
                browserstackOptions.Add("osVersion", "Big Sur");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "OSXFireFox")
            {
                var capabilities = new FirefoxOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "OS X");
                browserstackOptions.Add("osVersion", "Big Sur");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "OSXEdge")
            {
                var capabilities = new EdgeOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "OS X");
                browserstackOptions.Add("osVersion", "Big Sur");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "Win11Edge")
            {
                var capabilities = new EdgeOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "Windows");
                browserstackOptions.Add("osVersion", "11");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "Win11Chrome")
            {
                var capabilities = new ChromeOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "Windows");
                browserstackOptions.Add("osVersion", "11");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "Win11FireFox")
            {
                var capabilities = new FirefoxOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "Windows");
                browserstackOptions.Add("osVersion", "11");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "Win10Edge")
            {
                var capabilities = new EdgeOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "Windows");
                browserstackOptions.Add("osVersion", "10");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "Win10Chrome")
            {
                var capabilities = new ChromeOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "Windows");
                browserstackOptions.Add("osVersion", "10");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            if (client.ToString() == "Win10FireFox")
            {
                var capabilities = new FirefoxOptions();
                capabilities.BrowserVersion = "latest";
                browserstackOptions.Add("os", "Windows");
                browserstackOptions.Add("osVersion", "10");
                var safariOptions = new Dictionary<string, object>
                {
                    {"enablePopups", "true"},
                    {"allowAllCookies", "true"}
                };
                browserstackOptions.Add("safari", safariOptions);
                capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                Driver.Manage().Window.Maximize();
                return this;
            }
            return this;
        }
        catch (Exception e)
        {
            TestContext.WriteLine(e.Message);
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
#pragma warning restore CS0618 // Type or member is obsolete
        //Driver = null;
        return this;
    }

    /// <summary>
    /// Write Output to the Test Log
    /// </summary>
    /// <param name="output">The content to be written to the Test Log</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA WriteOutput(string output)
    {
        TestContext.Write(output);
        return this;
    }

    #region Timeouts

    /// <summary>
    /// Set Page Load Timeout
    /// </summary>
    /// <param name="timeout">Timeout in seconds</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA SetPageLoadTimeout(int timeout)
    {
        Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
        return this;
    }

    /// <summary>
    /// Set Implicit Wait
    /// </summary>
    /// <param name="timeout">Timeout in seconds</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA SetImplicitWait(int timeout)
    {
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
        return this;
    }

    /// <summary>
    /// Set Async JavaScript Timeout
    /// </summary>
    /// <param name="timeout">Timeout in seconds</param>
    /// <returns></returns>
    public KoloQA SetAsyncExecuteScriptTimeout(int timeout)
    {
        Driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout);
        return this;
    }

    #endregion Timeouts

    #region BrowserControls

    /// <summary>
    /// Open a Url
    /// </summary>
    /// <param name="url">Url to Open</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA OpenUrl(string url)
    {
        try
        {
            Driver.Navigate().GoToUrl(url);
            return this;
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Raises a Defect in your Azure Devops Project
    /// </summary>
    /// <param name="id">Test Case Id or Name</param>
    /// <param name="browser">The Browser/OS/Device Combination being used</param>
    /// <param name="priority">The Priority of the defect</param>
    /// <param name="severity">The Severity of the defect 1-4 with 1 being Highest</param>
    /// <param name="assignTo">Assign the defect to someone, will default to empty</param>
    /// <returns></returns>
    public KoloQA AzureDevopsRaiseDefect(string id, Browser browser, int priority, int severity, string assignTo = "")
    {
        var testCase = TestSuite.TestCases.Single(i => i.Id == id);
        KoloControl.TakeScreenshot(Driver, testCase + browser.ToString() + ".png");
        var bugRaiser = new BugRaiser();
        bugRaiser.CreateBugUsingClientLib(testCase, browser, priority, severity, testCase + browser.ToString() + ".png", assignTo);
        return this;
    }

    /// <summary>
    /// Close the Browser
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA CloseBrowser()
    {
        Driver.Close();
        return this;
    }

    /// <summary>
    /// Quit the active session and dispose of the driver
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA QuitSession()
    {
        Driver.Quit();
        return this;
    }

    /// <summary>
    /// Dispose of Browser
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA DisposeBrowser()
    {
        Driver.Dispose();
        return this;
    }

    /// <summary>
    /// Navigate Forward
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA GoForward()
    {
        Driver.Navigate().Forward();
        return this;
    }

    /// <summary>
    /// Navigate Back
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA GoBack()
    {
        Driver.Navigate().Back();
        return this;
    }

    /// <summary>
    /// Refresh Page
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA RefreshPage()
    {
        Driver.Navigate().Refresh();
        return this;
    }

    #endregion BrowserControls

    #region Waits

    /// <summary>
    /// Wait Until Page Fully Loaded
    /// </summary>
    /// <param name="timeoutSec">Timeout in Seconds</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA WaitUntilPageFullyLoaded(int timeoutSec = 15)
    {
        Thread.Sleep(300);
        IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
        WebDriverWait wait = new WebDriverWait(Driver, new TimeSpan(0, 0, timeoutSec));
        wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        Thread.Sleep(300);
        return this;
    }

    /// <summary>
    /// Wait for Text to be present
    /// </summary>
    /// <param name="text">Text to search for</param>
    /// <param name="timeout">The timeout to wait for the text to be displayed</param>
    /// <returns></returns>
    public KoloQA WaitForText(string text, int timeout = 10)
    {
        try
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            wait.Until(c => c.FindElement(By.XPath("//*[contains(text(), '" + text + "')]")));
        }
        catch
        {
            throw;
        }
        return this;
    }

    /// <summary>
    /// Waits until an element exists on page
    /// </summary>
    /// <param name="cssSelector">The CSS Selector of the Element</param>
    /// <param name="timeout">The timeout before abandoning the wait</param>
    /// <returns></returns>
    public KoloQA WaitUntilElementExists(string cssSelector, int timeout = 10)
    {
        try
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            wait.Until(c => c.FindElement(By.CssSelector(cssSelector)));
        }
        catch
        {
            TestContext.Write("KoloQA: " + cssSelector + "' not found in current context page.");
            throw;
        }
        return this;
    }

    /// <summary>
    /// Wait in Milliseconds
    /// </summary>
    /// <param name="milliseconds">Milliseconds</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA WaitInMilliseconds(int milliseconds)
    {
        Thread.Sleep(milliseconds);
        return this;
    }

    #endregion Waits

    #region Fluents

    /// <summary>
    /// Wait by ID and then return element
    /// </summary>
    /// <param name="linkText"></param>
    /// <returns></returns>
    public IWebElement FluentWaitByIdReturnElement(string linkText)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.Id(linkText)));
            ScrollIntoViewUsingJavaScript(link);
            return link;
        }
        catch (Exception)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.Id(linkText)));
                ScrollIntoViewUsingJavaScript(link);
                return link;
            }
            catch
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.Id(linkText)));
                ScrollIntoViewUsingJavaScript(link);
                return link;
            }
        }
    }

    /// <summary>
    /// Uploads a file from the File Uploads Folder
    /// </summary>
    /// <param name="FileName">The filename to upload</param>
    /// <param name="Id">Optional Override for the Id if not marked as file</param>
    /// <returns></returns>
    public KoloQA UploadFile(string FileName, string Id = "file")
    {
        var upload = FluentWaitByIdReturnElement(Id);
        var path = "../../../FileUploads/" + FileName;
        var detector = new LocalFileDetector();
        var allowsDetection = Driver as IAllowsFileDetection;
        if (allowsDetection != null)
        {
            allowsDetection.FileDetector = detector;
        }
        upload.SendKeys(path);
        return this;
    }

    /// <summary>
    /// Find by link text then click
    /// </summary>
    /// <param name="LinkText">Text of Link</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindLinkTextThenClick(string LinkText)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(20);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.LinkText(LinkText)));
            ScrollIntoViewAndClick(link);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("KoloQA: Found " + LinkText + " and clicked");
        }
        catch (Exception)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.LinkText(LinkText)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + LinkText + " and clicked");
            }
            catch
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.LinkText(LinkText)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + LinkText + " and clicked");
            }
        }
        return this;
    }

    /// <summary>
    /// Find by CSS Selector then Click
    /// </summary>
    /// <param name="cssSelector">CSS Selector to find</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindCssSelectorThenClick(string cssSelector)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
            ScrollIntoViewAndClick(link);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("KoloQA: Found " + cssSelector + " and clicked");
        }
        catch
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + cssSelector + " and clicked");
            }
            catch
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + cssSelector + " and clicked");
            }
        }
        return this;
    }

    /// <summary>
    /// Find Drop Down By Css Selector then Select the Value Provided
    /// </summary>
    /// <param name="selectListCss">CSS Selector of the List</param>
    /// <param name="valueInList">The Value to select</param>
    /// <returns></returns>
    public KoloQA DropDownByCssSelectorThenSelectValue(string selectListCss, string valueInList)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.CssSelector(selectListCss)));
            ScrollIntoViewUsingJavaScript(link);
            var select = new SelectElement(link);
            select.SelectByText(valueInList);
        }
        catch (Exception)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(selectListCss)));
                ScrollIntoViewUsingJavaScript(link);
                var select = new SelectElement(link);
                select.SelectByText(valueInList);
            }
            catch
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(selectListCss)));
                ScrollIntoViewUsingJavaScript(link);
                var select = new SelectElement(link);
                select.SelectByText(valueInList);
            }
        }
        return this;
    }

    /// <summary>
    /// Select a value from a drop down list selected by Id
    /// </summary>
    /// <param name="selectListId">The Id of the select list</param>
    /// <param name="valueInList">The Value in the list</param>
    /// <returns>KoloQA instance</returns>
    public KoloQA DropDownByIdThenSelectValue(string selectListId, string valueInList)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.Id(selectListId)));
            ScrollIntoViewUsingJavaScript(link);
            var select = new SelectElement(link);
            select.SelectByText(valueInList);
        }
        catch (Exception)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.Id(selectListId)));
                ScrollIntoViewUsingJavaScript(link);
                var select = new SelectElement(link);
                select.SelectByText(valueInList);
            }
            catch
            {
                throw;
            }
        }
        return this;
    }

    /// <summary>
    /// Select a value from a drop down list selected by name attribute
    /// </summary>
    /// <param name="SelectListName">The select list name attribute</param>
    /// <param name="ValueInList">The value to select in the list</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA DropDownByNameThenSelectValue(string SelectListName, string ValueInList)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.Name(SelectListName)));
            ScrollIntoViewUsingJavaScript(link);
            var select = new SelectElement(link);
            select.SelectByText(ValueInList);
        }
        catch (Exception)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.Name(SelectListName)));
                ScrollIntoViewUsingJavaScript(link);
                var select = new SelectElement(link);
                select.SelectByText(ValueInList);
            }
            catch
            {
                throw;
            }
        }
        return this;
    }

    /// <summary>
    /// Find an Element by CSS Selector and then type the value provided into it
    /// </summary>
    /// <param name="cssSelector">The CSS Selector of the element</param>
    /// <param name="valueToType">The value to type into the element</param>
    /// <returns></returns>
    public KoloQA FindByCssSelectorThenType(string cssSelector, string valueToType)
    {
        valueToType = KoloControl.StringTranslater(valueToType);
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
            ScrollIntoViewAndType(link, valueToType);
        }
        catch (Exception)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                ScrollIntoViewAndType(link, valueToType);
            }
            catch
            {
                throw;
            }
        }
        return this;
    }

    #region Controls

    /// <summary>
    /// Download Videos
    /// </summary>
    /// <returns></returns>
    public KoloQA BrowserStackDownloadVideos()
    {
        KoloControl.GetTestVideos().Wait();
        return this;
    }

    /// <summary>
    /// Set NUnit test context
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public KoloQA SetNunitTestContext(string key, string value)
    {
        SetNunitTestContext(key, value);
        return this;
    }

    #endregion Controls

    #region Scrollers

    /// <summary>
    /// Scroll Element into View
    /// </summary>
    /// <param name="element">The Element to Scroll</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA ScrollIntoView(IWebElement element)
    {
        Actions actions = new Actions(Driver);
        actions.MoveToElement(element);
        actions.Perform();
        return this;
    }

    /// <summary>
    /// Use JavaScript to Scroll the Element into View
    /// </summary>
    /// <param name="element">An Element</param>
    /// <returns>KoloQA instance</returns>
    public KoloQA ScrollIntoViewUsingJavaScript(IWebElement element)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
        return this;
    }

    /// <summary>
    /// Scroll into View and Click
    /// </summary>
    /// <param name="element">Element to Scroll and Click</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA ScrollIntoViewAndClick(IWebElement element)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
        js.ExecuteScript("arguments[0].click(); ", element);
        return this;
    }

    /// <summary>
    /// Scroll into view and the type
    /// </summary>
    /// <param name="element">The Element to be scrolled in to view</param>
    /// <param name="input">The Value to be typed into the element</param>
    /// <returns></returns>
    public KoloQA ScrollIntoViewAndType(IWebElement element, string input)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
        js.ExecuteScript("arguments[0].click(); ", element);
        element.SendKeys(input + Keys.Tab);
        return this;
    }

    /// <summary>
    /// Scroll down a specific pixel amount
    /// </summary>
    /// <param name="pixel">Pixel count</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA ScrollDownSpecificPixel(int pixel)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scrollBy(0," + pixel + ")", "");
        return this;
    }

    #endregion Scrollers

    #region Validation

    /// <summary>
    /// Page title is equal to
    /// </summary>
    /// <param name="title">The Page Title to check</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA PageTitleEquals(string title)
    {
        Assert.Equals(title, Driver.Title);
        return this;
    }

    /// <summary>
    /// Check the Page contains certain text
    /// </summary>
    /// <param name="text">Text to check in page</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA PageContainsText(string text)
    {
        var body = Driver.FindElement(By.TagName("body"));
        Assert.IsTrue(body.Text.Contains(text));
        return this;
    }

    #endregion Validation

    /// <summary>
    /// Find an Input by its CssSelector and Clear its Contents
    /// </summary>
    /// <param name="CssSelector"></param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindByCssSelectorClearInputField(string CssSelector)
    {
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var link = fluentWait.Until(x => x.FindElement(By.CssSelector(CssSelector)));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
        link.Click();
        link.Clear();
        return this;
    }

    /// <summary>
    /// Clicks an element by its Id
    /// </summary>
    /// <param name="Id">The Id of the element to Click</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA ClickById(string Id)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            var link = fluentWait.Until(x => x.FindElement(By.Id(Id)));
            ScrollIntoViewUsingJavaScript(link);
            link.Click();
        }
        catch (Exception)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                var link = fluentWait.Until(x => x.FindElement(By.Id(Id)));
                ScrollIntoViewUsingJavaScript(link);
            }
            catch
            {
                throw;
            }
        }
        return this;
    }

    /// <summary>
    /// Click a link by the link text
    /// </summary>
    /// <param name="linkText">The Text in the link</param>
    /// <returns>KolOQA Instance</returns>
    public KoloQA ClickLinkByLinkText(string linkText)
    {
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var link = fluentWait.Until(x => x.FindElement(By.XPath("//*[contains(text(), '" + linkText + "')]")));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
        try
        {
            js.ExecuteScript("arguments[0].click();", link);
        }
        catch (ElementNotInteractableException)
        {
            var link2 = fluentWait.Until(x => x.FindElement(By.XPath("//*[contains(text(), '" + linkText + "')]/..")));
            js.ExecuteScript("arguments[0].click();", link);
        }

        return this;
    }

    /// <summary>
    /// Click a link by partial link text
    /// </summary>
    /// <param name="linkText">The text contained in the link</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA ClickLinkByPartialLinkText(string linkText)
    {
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var link = fluentWait.Until(x => x.FindElement(By.PartialLinkText(linkText)));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
        link.Click();
        return this;
    }

    /// <summary>
    /// Find an Input by its Id and Clear its Contents
    /// </summary>
    /// <param name="Id"></param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindByIdClearInputField(string Id)
    {
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var link = fluentWait.Until(x => x.FindElement(By.Id(Id)));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
        link.Click();
        link.Clear();
        return this;
    }

    /// <summary>
    /// Find by Id and then Type into the Input Box
    /// </summary>
    /// <param name="id">The Id of the input box</param>
    /// <param name="inputText">The text to type into the input box</param>
    /// <returns></returns>
    public KoloQA FindByIdThenType(string id, string inputText)
    {
        inputText = KoloControl.StringTranslater(inputText);
        var fluentWait = new DefaultWait<IWebDriver>(Driver);
        fluentWait.Timeout = TimeSpan.FromSeconds(20);
        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var link = fluentWait.Until(x => x.FindElement(By.Id(id)));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
        link.Click();
        link.SendKeys(inputText);
        return this;
    }

    #region BrowserStack Marker

    /// <summary>
    /// Marks the Test as Passed on BrowserStack
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA TestPassed(Browser client)
    {
        try
        {
            if (client != Browser.LocalChrome)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Test Passed \"}}");
            }
        }
        catch
        {
            throw;
        }

        return this;
    }

    /// <summary>
    /// Marks the Test as Failed on BrowserStack
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA TestFailed(Browser client)
    {
        if (client != Browser.LocalChrome)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Test Failed \"}}");
        }
        return this;
    }

    #endregion BrowserStack Marker

    #region GdsPageIndexing

    /// <summary>
    /// Index GDS Page model (Async)
    /// </summary>
    /// <param name="pageModelName"></param>
    /// <param name="element"></param>
    /// <param name="ignores"></param>
    /// <returns></returns>
    public async Task<KoloQA> IndexGdsPageModelAsync(string pageModelName, string element = "", List<string>? ignores = null)
    {
        var pageModelGenerator = new GdsPageModelGenerator();
        var model = await pageModelGenerator.GDSPageGeneratorAsync(Driver, element);
        KoloControl.GeneratePageClass(pageModelName, model);
        KoloControl.GenerateSimplePageClass(pageModelName, model);
        return this;
    }

    #endregion GdsPageIndexing

    #endregion Fluents
}