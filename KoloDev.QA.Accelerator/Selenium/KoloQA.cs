// ***********************************************************************
// Assembly         : KoloDev.GDS.QA.Accelerator
// Author           : KoloDev
// Created          : 08-18-2022
//
// Last Modified By : KoloDev
// Last Modified On : 10-06-2022
// ***********************************************************************
// <copyright file="KoloQA.cs" company="KoloDev Ltd.">
//     Copyright © 2022 KoloDev Ltd. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using BrowserStack;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using KoloDev.GDS.QA.Accelerator.Data;
using KoloDev.GDS.QA.Accelerator.Selenium;
using KoloDev.GDS.QA.Accelerator.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Text.Json;
using System.Text.RegularExpressions;
using static KoloDev.GDS.QA.Accelerator.Data.BrowserStackModels;
using static KoloDev.GDS.QA.Accelerator.Data.KoloTestSuite;
using static System.Collections.Specialized.BitVector32;


namespace KoloDev.GDS.QA.Accelerator
{
    /// <summary>
    /// Class ScreenShotRemoteWebDriver.
    /// Implements the <see cref="RemoteWebDriver" />
    /// Implements the <see cref="ITakesScreenshot" />
    /// </summary>
    /// <seealso cref="RemoteWebDriver" />
    /// <seealso cref="ITakesScreenshot" />
    public class ScreenShotRemoteWebDriver : RemoteWebDriver, ITakesScreenshot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenShotRemoteWebDriver"/> class.
        /// </summary>
        /// <param name="remoteAddress">The remote address.</param>
        /// <param name="options">The options.</param>
        public ScreenShotRemoteWebDriver(Uri remoteAddress, DriverOptions options) : base(remoteAddress, options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenShotRemoteWebDriver"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="dc">The dc.</param>
        /// <param name="timeSpan">The time span.</param>
        public ScreenShotRemoteWebDriver(Uri uri, ICapabilities dc, TimeSpan timeSpan)
          : base(uri, dc)
        {
        }

        /// <summary>
        /// Gets the screenshot.
        /// </summary>
        /// <returns>Screenshot.</returns>
        public new Screenshot GetScreenshot()
        {
            Response screenshotResponse = this.Execute(DriverCommand.Screenshot, null);
            string base64 = screenshotResponse.Value.ToString();
            return new Screenshot(base64);
        }
    }

    /// <summary>
    /// KoloQA is the main object for manipulating Selenium Sessions.
    /// </summary>
    public partial class KoloQA
    {
        /// <summary>
        /// The test suite
        /// </summary>
        private KoloTestSuite? testSuite;

        /// <summary>
        /// Kolo Test Suite Information
        /// </summary>
        /// <value>The test suite.</value>
        public KoloTestSuite? TestSuite { get => testSuite; set => testSuite = value; }
        /// <summary>
        /// Gets or sets the local.
        /// </summary>
        /// <value>The local.</value>
        private Local local { get; set; } = new Local();

        /// <summary>
        /// Remote Web Driver Instance
        /// </summary>
        /// <value>The driver.</value>
        private IWebDriver Driver { get; set; } = null!;
        /// <summary>
        /// BrowserStack Project Name
        /// </summary>
        /// <value>The name of the project.</value>
        private string ProjectName { get; set; } = "";

        /// <summary>
        /// BrowserStack Build Name
        /// </summary>
        /// <value>The name of the build.</value>
        private string BuildName { get; set; } = "";

        /// <summary>
        /// BrowserStack Test Name
        /// </summary>
        /// <value>The name of the test.</value>
        private string TestName { get; set; } = "";

        /// <summary>
        /// Browserstack User Name
        /// </summary>
        /// <value>The name of the user.</value>
        private string UserName { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether [local test].
        /// </summary>
        /// <value><c>true</c> if [local test]; otherwise, <c>false</c>.</value>
        private bool LocalTest { get; set; } = false;

        /// <summary>
        /// BrowserStack Access Key
        /// </summary>
        /// <value>The access key.</value>
        private string AccessKey { get; set; } = "";

        /// <summary>
        /// Add Test Cases to Collection
        /// </summary>
        /// <param name="Id">The identifier for the Test Case</param>
        /// <param name="StepNumber">The Step Number of the Test Step</param>
        /// <param name="Name">The Name of the Test Step</param>
        /// <param name="Description">The Description of the Test Step</param>
        /// <param name="Passed">Whether the Test Step Passed or Not</param>
        /// <returns>KoloQA.</returns>
        public KoloQA AddStepToTestCase(string Id, int StepNumber, string Name, string Description, bool Passed)
        {
            try
            {
                if (testSuite == null)
                {
                    testSuite = new KoloTestSuite();
                    if (testSuite.TestCases == null)
                    {
                        testSuite.TestCases = new List<KoloTestCase>();
                    }
                }
                if (testSuite.TestCases.Any(i => i.Id == Id))
                {
                    KoloTestCase testCase;
                    testCase = testSuite.TestCases.Single(i => i.Id == Id);
                    KoloTestSteps step = new KoloTestSteps();
                    step.Name = Name;
                    step.Description = Description;
                    step.Passed = Passed;
                    step.StepNumber = StepNumber;
                    testCase.TestSteps.Add(step);
                }
                else
                {
                    KoloTestCase testCase = new KoloTestCase();
                    testCase.Id = Id;

                    KoloTestSteps step = new KoloTestSteps();
                    step.StepNumber = StepNumber;
                    step.Name = Name;
                    step.Description = Description;
                    step.Passed = Passed;
                    if (testCase.TestSteps == null)
                    {
                        testCase.TestSteps = new List<KoloTestSteps>();
                    }
                    testCase.TestSteps.Add(step);
                    testSuite.TestCases.Add(testCase);
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
        /// <param name="pageName">Name of the page.</param>
        /// <param name="client">The client.</param>
        /// <param name="wcagLevel">The wcag level.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA AccessibilityOnPage(string pageName, BrowserStackBrowsers client, WcagLevel wcagLevel = WcagLevel.wcag2aa)
        {
            if (TestContext.Parameters["Accessibility"] == "true" && !client.ToString().Contains("iP"))
            {
                try
                {
                    var folderName = @"TestResults";
                    // If directory does not exist, create it
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    var axeResult = new AxeBuilder(Driver)
                    .WithTags(wcagLevel.ToString())
                    .Analyze();
                    Driver.CreateAxeHtmlReport(axeResult, "TestResults/" + pageName + ".html");
                    var accessibility = File.ReadAllText("TestResults/" + pageName + ".html");

                    accessibility = GdsHtmlPage.ApplyGdsStylingToAccessibilityReport(accessibility, pageName);
                    File.WriteAllText("TestResults/" + pageName + ".html", accessibility);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            return this;
        }


        /// <summary>
        /// Check the Accessibility of the Page
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="client">The client.</param>
        /// <param name="wcagLevel">The wcag level.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA AccessibilityOnPageAxeReport(string pageName, BrowserStackBrowsers client, WcagLevel wcagLevel = WcagLevel.wcag2aa)
        {
            if (TestContext.Parameters["Accessibility"] == "true" && !client.ToString().Contains("iP"))
            {
                try
                {
                    var folderName = @"TestResults";
                    // If directory does not exist, create it
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    var axeResult = new AxeBuilder(Driver)
                    .WithTags(wcagLevel.ToString())
                    .Analyze();
                    Driver.CreateAxeHtmlReport(axeResult, "TestResults/" + pageName + ".html");
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            return this;
        }


        /// <summary>
        /// Accessibilities the on page json.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="client">The client.</param>
        /// <param name="wcagLevel">The wcag level.</param>
        /// <returns><br /></returns>
        public KoloQA AccessibilityOnPageJson(string pageName, BrowserStackBrowsers client, WcagLevel wcagLevel = WcagLevel.wcag2aa)
        {
            if (TestContext.Parameters["Accessibility"] == "true" && !client.ToString().Contains("iP"))
            {
                try
                {
                    var folderName = @"TestResults";
                    // If directory does not exist, create it
                    if (!Directory.Exists(folderName))
                    {
                        Directory.CreateDirectory(folderName);
                    }

                    AxeResult axeResult = new AxeBuilder(Driver)
                    .WithTags(wcagLevel.ToString())
                    .Analyze();
                    string json = System.Text.Json.JsonSerializer.Serialize(axeResult);
                    File.WriteAllText("TestResults/" + pageName + ".json", json);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
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
            KoloTestCase testCase;
            testCase = testSuite.TestCases.Single(i => i.Id == Id);
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
        /// Randoms the string.
        /// </summary>
        /// <returns>System.String.</returns>
        public string RandomString()
        {
            string randomString = KoloControl.RandomStringGenerator();
            return randomString;

        }

        /// <summary>
        /// Starts a Local Chrome Instance
        /// </summary>
        /// <returns>KoloQA Instance</returns>
        public KoloQA LaunchChrome()
        {
            Driver = new ChromeDriver();
            return this;
        }

        /// <summary>
        /// Simulate devices when testing in chrome
        /// </summary>
        /// <param name="deviceSim">The device you wish to simulate</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA LaunchChromeDeviceMode(ChromeDeviceSim deviceSim)
        {
            string device = "";
            if (deviceSim == ChromeDeviceSim.iPadAir)
            {
                device = "iPad Air";
            }
            if (deviceSim == ChromeDeviceSim.GalaxyFold)
            {
                device = "Galaxy Fold";
            }
            if (deviceSim == ChromeDeviceSim.iPadMini)
            {
                device = "iPad Mini";
            }
            if (deviceSim == ChromeDeviceSim.iPhone12Pro)
            {
                device = "iPhone 12 Pro";
            }
            if (deviceSim == ChromeDeviceSim.iPhoneSE)
            {
                device = "iPhone SE";
            }
            if (deviceSim == ChromeDeviceSim.iPhoneXr)
            {
                device = "iPhone XR";
            }
            if (deviceSim == ChromeDeviceSim.NestHub)
            {
                device = "Nest Hub";
            }
            if (deviceSim == ChromeDeviceSim.NestHubMax)
            {
                device = "Nest Hub Max";
            }
            if (deviceSim == ChromeDeviceSim.Pixel5)
            {
                device = "Pixel 5";
            }
            if (deviceSim == ChromeDeviceSim.SamsungGalaxyA5171)
            {
                device = "Samsung Galaxy A51/71";
            }
            if (deviceSim == ChromeDeviceSim.SamsungGalaxyS20Ultra)
            {
                device = "Samsung Galaxy S20 Ultra";
            }
            if (deviceSim == ChromeDeviceSim.SamsungGalaxyS8Plus)
            {
                device = "Samsung Galaxy S8 Plus";
            }
            if (deviceSim == ChromeDeviceSim.SurfaceDuo)
            {
                device = "Surface Duo";
            }
            if (deviceSim == ChromeDeviceSim.SurfacePro7)
            {
                device = "Surface Pro 7";
            }

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.EnableMobileEmulation(device);
            Driver = new ChromeDriver(chromeOptions);
            return this;
        }

        /// <summary>
        /// Maximises the Browser Window
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA MaximiseBrowser(BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape || client == BrowserStackBrowsers.iPadLandscape || client == BrowserStackBrowsers.iPadPortrait)
            {
                return this;
            }
            else
            {
                Driver.Manage().Window.Maximize();
            }
            return this;
        }

        /// <summary>
        /// Set Implicit Timeout
        /// </summary>
        /// <param name="TimeOut">Timeout in seconds</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA SetImplicitTimeout(int TimeOut)
        {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TimeOut);
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
                List<KeyValuePair<string, string>> bsLocalArgs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("key", AccessKey) };
                local.start(bsLocalArgs);
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
            local.stop();
            TestContext.WriteLine("KoloQA: Closing Local Connection to Route traffic to protected services");
            return this;
        }

        public KoloQA StartSession(BrowserStackBrowsers client)
        {
            try
            {
                BrowserStackSession(client);
            }
            catch(Exception ex)
            {
                TestContext.WriteLine("KoloQA: Retried Twice:  " + ex.Message);
                try
                {
                    BrowserStackSession(client);
                }
                catch(Exception e)
                {
                    TestContext.WriteLine("KoloQA: Retried Twice:  " + e.Message);
                }
            }
            return this;
        }

        /// <summary>
        /// Starts a BrowserStack Session with the desired Client
        /// </summary>
        /// <param name="client">A client from the predesignated list of browsers and operating systems</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA BrowserStackSession(BrowserStackBrowsers client)
        {
            TestContext.WriteLine(@"
  _  __    _      ___     _     _____       _     ___ _            _          _ 
 | |/ /___| |___ / _ \   /_\   |_   _|__ __| |_  / __| |_ __ _ _ _| |_ ___ __| |
 | ' </ _ \ / _ \ (_) | / _ \    | |/ -_|_-<  _| \__ \  _/ _` | '_|  _/ -_) _` |
 |_|\_\___/_\___/\__\_\/_/ \_\   |_|\___/__/\__| |___/\__\__,_|_|  \__\___\__,_|");
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
                    browserstackOptions.Add("deviceName", "iPhone 13");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "iPhonePortrait")
                {
                    var capabilities = new SafariOptions();
                    browserstackOptions.Add("osVersion", "15");
                    browserstackOptions.Add("deviceName", "iPhone 13");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "portrait");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "iPadPortrait")
                {
                    var capabilities = new SafariOptions();
                    browserstackOptions.Add("osVersion", "14");
                    browserstackOptions.Add("deviceName", "iPad Air 4");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "iPadLandscape")
                {
                    var capabilities = new SafariOptions();
                    browserstackOptions.Add("osVersion", "14");
                    browserstackOptions.Add("deviceName", "iPad Air 4");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "SamsungGalaxyAndroidLandscape")
                {
                    ChromeOptions capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "latest");
                    browserstackOptions.Add("deviceName", "Samsung Galaxy S21");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "SamsungGalaxyAndroidPortrait")
                {
                    ChromeOptions capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "latest");
                    browserstackOptions.Add("deviceName", "Samsung Galaxy S21");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "portrait");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "GooglePixel4XLAndroidLandscape")
                {
                    ChromeOptions capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "10.0");
                    browserstackOptions.Add("deviceName", "Google Pixel 4 XL");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "GooglePixel4XLAndroidPortrait")
                {
                    ChromeOptions capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "10.0");
                    browserstackOptions.Add("deviceName", "Google Pixel 4 XL");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "portrait");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "OSXSafari")
                {
                    SafariOptions capabilities = new SafariOptions();
                    capabilities.BrowserVersion = "14.0";
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "OSXChrome")
                {
                    ChromeOptions capabilities = new ChromeOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "OSXFireFox")
                {
                    FirefoxOptions capabilities = new FirefoxOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "OSXEdge")
                {
                    EdgeOptions capabilities = new EdgeOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "Win11Edge")
                {
                    EdgeOptions capabilities = new EdgeOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "11");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new ScreenShotRemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "Win11Chrome")
                {
                    ChromeOptions capabilities = new ChromeOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "11");
                    browserstackOptions.Add("local", "false");
                    browserstackOptions.Add("seleniumVersion", "4.5.0");
                    browserstackOptions.Add("browserName", "Chrome");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "Win11FireFox")
                {
                    FirefoxOptions capabilities = new FirefoxOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "11");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "Win10Edge")
                {
                    EdgeOptions capabilities = new EdgeOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "10");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "Win10Chrome")
                {
                    ChromeOptions capabilities = new ChromeOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "10");
                    browserstackOptions.Add("local", "false");
                    browserstackOptions.Add("seleniumVersion", "4.5.0");
                    browserstackOptions.Add("browserName", "Chrome");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);


                    Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                if (client.ToString() == "Win10FireFox")
                {
                    FirefoxOptions capabilities = new FirefoxOptions();
                    capabilities.BrowserVersion = "latest";
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "10");
                    Dictionary<string, object> safariOptions = new Dictionary<string, object>();
                    safariOptions.Add("enablePopups", "true");
                    safariOptions.Add("allowAllCookies", "true");
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
                TestContext.WriteLine("KoloQA: Erro Starting " + e.Message);
                throw e;
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

        /// <summary>
        /// Checks if file exists.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns><br /></returns>
        public bool CheckIfFileExists(string FileName)
        {
            IJavaScriptExecutor jse = (IJavaScriptExecutor)Driver;
            // Check if file exists
            bool val1 = (bool)jse.ExecuteScript("browserstack_executor: {\"action\": \"fileExists\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            TestContext.WriteLine("KoloQA: File Exists Check - " + FileName + " " + val1);
            return val1;
        }

        public Sessions GetVideoUrl()
        {
            // get details of the session
            object sessionObject = ((IJavaScriptExecutor)Driver).ExecuteScript("browserstack_executor: {\"action\": \"getSessionDetails\"}");
            // convert Object to String for parsing
            string json_resp = Convert.ToString(sessionObject);
            // parse the data
            Sessions session_details = JsonConvert.DeserializeObject<Sessions>(json_resp);
            return session_details;

        }

        /// <summary>
        /// Gets the file properties.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <returns>IDictionary&lt;System.String, System.Object&gt;.</returns>
        public IDictionary<string, object> GetFileProperties(string FileName)
        {
            IJavaScriptExecutor jse = (IJavaScriptExecutor)Driver;
            // Check if file exists
            IDictionary<string, object> fileprops = new Dictionary<string, object>(); 
            fileprops = (IDictionary<string, object>)jse.ExecuteScript("browserstack_executor: {\"action\": \"getFileProperties\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            TestContext.WriteLine("KoloQA: File Exists Check - " + FileName + " " + fileprops);
            return fileprops;
        }

        /// <summary>
        /// Writes the file to local.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        public void WriteFileToLocal(string FileName)
        {
            IJavaScriptExecutor jse = (IJavaScriptExecutor)Driver;
            jse.ExecuteScript("browserstack_executor: {\"action\": \"getFileContent\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            string base64encode = (string)jse.ExecuteScript("browserstack_executor: {\"action\": \"getFileContent\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            byte[] b = Convert.FromBase64String(base64encode);
            File.WriteAllBytes("./" + FileName, b);
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
        /// <returns>KoloQA.</returns>
        public KoloQA SetAsyncExecuteScriptTimeout(int timeout)
        {
            Driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout);
            return this;
        }
        #endregion

        #region BrowserControls
        /// <summary>
        /// Open a Url
        /// </summary>
        /// <param name="URL">Url to Open</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA OpenURL(string URL)
        {
            try
            {
                Driver.Navigate().GoToUrl(URL);
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
        /// <param name="Id">Test Case Id or Name</param>
        /// <param name="Browser">The Browser/OS/Device Combination being used</param>
        /// <param name="Priority">The Priority of the defect</param>
        /// <param name="Severity">The Severity of the defect 1-4 with 1 being Highest</param>
        /// <param name="AssignTo">Assign the defect to someone, will default to empty</param>
        /// <returns>KoloQA.</returns>
        public KoloQA AzureDevopsRaiseDefect(string Id, BrowserStackBrowsers Browser, int Priority, int Severity, string AssignTo = "")
        {
            KoloTestCase testCase;
            testCase = TestSuite.TestCases.Single(i => i.Id == Id);
            KoloControl.TakeScreenshot(Driver, testCase + Browser.ToString() + ".png");
            BugRaiser bugRaiser = new BugRaiser();
            bugRaiser.CreateBugUsingClientLib(testCase, Browser, Priority, Severity, testCase + Browser.ToString() + ".png", AssignTo);
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
        /// Saves the screenshot PNG.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA SaveScreenshotPNG(string filename)
        {
            ITakesScreenshot screenshotDriver = Driver as ITakesScreenshot;
            Screenshot screenshot = screenshotDriver.GetScreenshot();
            screenshot.SaveAsFile(filename, ScreenshotImageFormat.Png);
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
        #endregion

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
        /// <param name="Text">Text to search for</param>
        /// <param name="timeout">The timeout to wait for the text to be displayed</param>
        /// <returns>KoloQA.</returns>
        public KoloQA WaitForText(string Text, int timeout = 10)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                wait.Until(c => c.FindElement(By.XPath("//*[contains(text(), '" + Text + "')]")));
            }
            catch
            {
                throw;
            }
            return this;
        }

        /// <summary>
        /// Finds the list of values by CSS.
        /// </summary>
        /// <param name="CSSSelector">The CSS selector.</param>
        /// <returns>List&lt;IWebElement&gt;.</returns>
        public List<IWebElement> FindListOfValuesByCSS(string CSSSelector)
        {
            List<IWebElement> Values = Driver.FindElements(By.CssSelector(CSSSelector)).ToList();
            return Values;
        }

        /// <summary>
        /// Gets the list of elements beneath x path.
        /// </summary>
        /// <param name="XPath">The x path.</param>
        /// <returns>List&lt;IWebElement&gt;.</returns>
        public List<IWebElement> GetListOfElementsBeneathXPath(string XPath)
        {
            IWebElement Root = Driver.FindElement(By.XPath(XPath));
            List<IWebElement> Elements = Root.FindElements(By.XPath("//*")).ToList();
            return Elements;
        }

        /// <summary>
        /// Gets the list of elements by x path.
        /// </summary>
        /// <param name="XPath">The x path.</param>
        /// <returns>List&lt;IWebElement&gt;.</returns>
        public List<IWebElement> GetListOfElementsByXPath(string XPath)
        {
            List<IWebElement> Elements = Driver.FindElements(By.XPath(XPath)).ToList();
            return Elements;
        }

        /// <summary>
        /// Finds the list of values by x path.
        /// </summary>
        /// <param name="XPath">The x path.</param>
        /// <returns>List&lt;IWebElement&gt;.</returns>
        public List<IWebElement> FindListOfValuesByXPath(string XPath)
        {
            List<IWebElement> Values = Driver.FindElements(By.XPath(XPath)).ToList();
            return Values;
        }

        /// <summary>
        /// Gets the text of CSS element.
        /// </summary>
        /// <param name="CSSSelector">The CSS selector.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.String.</returns>
        public string GetTextOfCSSElement(string CSSSelector, int timeout = 10)
        {
            string TxtOfElement = "";
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                wait.Until(c => c.FindElement(By.CssSelector(CSSSelector)));
                TxtOfElement = Driver.FindElement(By.CssSelector(CSSSelector)).Text;
            }
            catch (Exception e)
            {
                TestContext.Write("KoloQA: " + CSSSelector + "' not found in current context page.");
                throw;
            }
            return TxtOfElement;
        }

        /// <summary>
        /// Gets the text of x path element.
        /// </summary>
        /// <param name="XPath">The x path.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.String.</returns>
        public string GetTextOfXPathElement(string XPath, int timeout = 10)
        {
            string TxtOfElement = "";
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                wait.Until(c => c.FindElement(By.XPath(XPath)));
                TxtOfElement = Driver.FindElement(By.XPath(XPath)).Text;
            }
            catch (Exception e)
            {
                TestContext.Write("KoloQA: " + XPath + "' not found in current context page.");
                throw;
            }
            return TxtOfElement;
        }

        /// <summary>
        /// Validates the text of CSS element.
        /// </summary>
        /// <param name="CSSSelector">The CSS selector.</param>
        /// <param name="Value">The value.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ValidateTextOfCSSElement(string CSSSelector, string Value, int timeout = 10)
        {
            string TxtOfElement = "";
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                wait.Until(c => c.FindElement(By.CssSelector(CSSSelector)));
                TxtOfElement = Driver.FindElement(By.CssSelector(CSSSelector)).Text;

                if(TxtOfElement == Value)
                { 
                    return true; 
                }
            }
            catch (Exception e)
            {
                TestContext.Write("KoloQA: " + CSSSelector + "' not found in current context page.");
                throw;
            }
            return false;
        }

        /// <summary>
        /// Validates the text of x path element.
        /// </summary>
        /// <param name="XPath">The x path.</param>
        /// <param name="Value">The value.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ValidateTextOfXPathElement(string XPath, string Value, int timeout = 10)
        {
            string TxtOfElement = "";
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                wait.Until(c => c.FindElement(By.XPath(XPath)));
                TxtOfElement = Driver.FindElement(By.XPath(XPath)).Text;

                if (TxtOfElement == Value)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                TestContext.Write("KoloQA: " + XPath + "' not found in current context page.");
                throw;
            }
            return false;
        }

        /// <summary>
        /// Waits until an element exists on page
        /// </summary>
        /// <param name="CSSSelector">The CSS Selector of the Element</param>
        /// <param name="timeout">The timeout before abandoning the wait</param>
        /// <returns>KoloQA.</returns>
        public KoloQA WaitUntilElementExists(string CSSSelector, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                wait.Until(c => c.FindElement(By.CssSelector(CSSSelector)));
            }
            catch(Exception e)
            {
                TestContext.Write("KoloQA: " + CSSSelector + "' not found in current context page.");
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
        #endregion

        #region Fluents
        /// <summary>
        /// Fluents the wait by identifier return element.
        /// </summary>
        /// <param name="LinkText">The link text.</param>
        /// <returns>IWebElement.</returns>
        public IWebElement FluentWaitByIdReturnElement(string LinkText)
        {
            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(LinkText)));
                ScrollIntoViewUsingJavaScript(link);
                return link;
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(LinkText)));
                    ScrollIntoViewUsingJavaScript(link);
                    return link;
                }
                catch
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(LinkText)));
                    ScrollIntoViewUsingJavaScript(link);
                    return link;
                }
            }
        }

        /// <summary>
        /// Fluents the wait by name return element.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <returns>IWebElement.</returns>
        public IWebElement FluentWaitByNameReturnElement(string Name)
        {
            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.Name(Name)));
                ScrollIntoViewUsingJavaScript(link);
                return link;
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.Name(Name)));
                    ScrollIntoViewUsingJavaScript(link);
                    return link;
                }
                catch
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.Name(Name)));
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
        /// <returns>KoloQA.</returns>
        public KoloQA UploadFileById(string FileName, string Id = "file")
        {
            IWebElement upload = FluentWaitByIdReturnElement(Id);
            string path = "../../../FileUploads/" + FileName;
            LocalFileDetector detector = new LocalFileDetector();
            var allowsDetection = Driver as IAllowsFileDetection;
            if (allowsDetection != null)
            {
                allowsDetection.FileDetector = detector;
            }
            upload.SendKeys(path);
            return this;
        }

        /// <summary>
        /// Uploads the name of the file by.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="Id">The identifier.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA UploadFileByName(string FileName, string Id = "file")
        {
            IWebElement upload = FluentWaitByNameReturnElement(Id);
            string path = "./FileUploads/" + FileName;
            LocalFileDetector detector = new LocalFileDetector();
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
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.LinkText(LinkText)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + LinkText + " and clicked");
            }
            catch (Exception e)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.LinkText(LinkText)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + LinkText + " and clicked");
                }
                catch
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.LinkText(LinkText)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + LinkText + " and clicked");
                }
            }
            return this;
        }

        /// <summary>
        /// Retrieves XPath from CSS Selector provided
        /// </summary>
        /// <param name="CSSSelector">CSS Selector for the element</param>
        /// <returns>System.String.</returns>
        public string GetXpathFromCSS(string CSSSelector)
        {
            string xpath = "";

            try
            {
                WaitUntilPageFullyLoaded();
                string PageHtml = Driver.PageSource;
                var pagemaster = new HtmlDocument();
                pagemaster.LoadHtml(PageHtml);
                var document = pagemaster.DocumentNode;
                if (CSSSelector.Contains("nth-of-type"))
                {
                    HtmlNode nodenth = document.NthOfTypeQuerySelector(CSSSelector);
                    xpath = nodenth.XPath;
                }
                else
                {
                    HtmlNode node = document.QuerySelector(CSSSelector);
                    xpath = node.XPath;
                    TestContext.WriteLine("KoloQA: Calculated XPath: " + xpath);
                }
            }
            catch (Exception e)
            {
                TestContext.WriteLine("KoloQA: Selector Was: " + CSSSelector);
                TestContext.WriteLine("KoloQA: Can not find or generate " + e.Message);
            }
            if(xpath.Length > 1)
            {
                try
                {
                    WaitUntilPageFullyLoaded();
                    string PageHtml = Driver.PageSource;
                    var pagemaster = new HtmlDocument();
                    pagemaster.LoadHtml(PageHtml);
                    var document = pagemaster.DocumentNode;
                    if (CSSSelector.Contains("nth-of-type"))
                    {
                        HtmlNode nodenth = document.NthOfTypeQuerySelector(CSSSelector);
                        xpath = nodenth.XPath;
                    }
                    else
                    {
                        HtmlNode node = document.QuerySelector(CSSSelector);
                        xpath = node.XPath;
                        TestContext.WriteLine("KoloQA: Calculated XPath: " + xpath);
                    }
                }
                catch (Exception e)
                {
                    TestContext.WriteLine("KoloQA: Selector Was: " + CSSSelector);
                    TestContext.WriteLine("KoloQA: Can not find or generate " + e.Message);
                }
            }
            if (xpath.Length > 1)
            {
                try
                {
                    WaitUntilPageFullyLoaded();
                    string PageHtml = Driver.PageSource;
                    var pagemaster = new HtmlDocument();
                    pagemaster.LoadHtml(PageHtml);
                    var document = pagemaster.DocumentNode;
                    if (CSSSelector.Contains("nth-of-type"))
                    {
                        HtmlNode nodenth = document.NthOfTypeQuerySelector(CSSSelector);
                        xpath = nodenth.XPath;
                    }
                    else
                    {
                        HtmlNode node = document.QuerySelector(CSSSelector);
                        xpath = node.XPath;
                        TestContext.WriteLine("KoloQA: Calculated XPath: " + xpath);
                    }
                }
                catch (Exception e)
                {
                    TestContext.WriteLine("KoloQA: Selector Was: " + CSSSelector);
                    TestContext.WriteLine("KoloQA: Can not find or generate " + e.Message);
                }
            }
            return xpath;
        }


        /// <summary>
        /// Finds the CSS selector then click with offset.
        /// </summary>
        /// <param name="CSSSelector">The CSS selector.</param>
        /// <param name="X">The x.</param>
        /// <param name="Y">The y.</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindCSSSelectorThenClickWithOffset(string CSSSelector, int X, int Y, BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
            {
                string xpath = GetXpathFromCSS(CSSSelector);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    Actions move = new Actions(Driver);
                    move.MoveToElement(link).MoveByOffset(X, Y).Click().Perform();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        Actions move = new Actions(Driver);
                        move.MoveToElement(link).MoveByOffset(X, Y).Click().Perform();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                            Actions move = new Actions(Driver);
                            move.MoveToElement(link).MoveByOffset(X, Y).Click().Perform();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: Error FindByCSSSelectorThenType, Selector " + CSSSelector);
                            SaveScreenshotPNG( TestContext.CurrentContext.Test.Name.Trim() + "_FAILED.png");
                            throw;
                        }

                    }
                }

            }
            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                    Actions move = new Actions(Driver);
                    move.MoveToElement(link).MoveByOffset(X, Y).Click().Perform();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                        Actions move = new Actions(Driver);
                        move.MoveToElement(link).MoveByOffset(X, Y).Click().Perform();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                            Actions move = new Actions(Driver);
                            move.MoveToElement(link).MoveByOffset(X, Y).Click().Perform();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + CSSSelector);
                            SaveScreenshotPNG( TestContext.CurrentContext.Test.Name.Trim() + "_FAILED.png");
                            throw;
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Find by CSS Selector then Click
        /// </summary>
        /// <param name="CSSSelector">CSS Selector to find</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA FindCSSSelectorThenClick(string CSSSelector, BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
            {
                string xpath = GetXpathFromCSS(CSSSelector);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        ScrollIntoViewAndClick(link);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                            ScrollIntoViewAndClick(link);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: Error FindByCSSSelectorThenType, Selector " + CSSSelector);
                            throw;
                        }

                    }
                }

            }
            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                        ScrollIntoViewAndClick(link);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                            ScrollIntoViewAndClick(link);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + CSSSelector);
                            throw;
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Finds the CSS selector near then click.
        /// </summary>
        /// <param name="CSSSelector">The CSS selector.</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindCSSSelectorNearThenClick(string CSSSelector, BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
            {
                string xpath = GetXpathFromCSS(CSSSelector);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        ScrollIntoViewAndClick(link);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                            ScrollIntoViewAndClick(link);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: Error FindByCSSSelectorThenType, Selector " + CSSSelector);
                            throw;
                        }

                    }
                }

            }
            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                        ScrollIntoViewAndClick(link);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                            ScrollIntoViewAndClick(link);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + CSSSelector);
                            throw;
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Find by CSS Selector then Click Without Scroll Into View
        /// </summary>
        /// <param name="CSSSelector">CSS Selector to find</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA FindCSSSelectorThenClickWithoutScrollIntoView(string CSSSelector, BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
            {
                string xpath = GetXpathFromCSS(CSSSelector);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    link.Click();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        link.Click();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                            link.Click();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: Error FindByCSSSelectorThenType, Selector " + CSSSelector);
                            throw;
                        }

                    }
                }

            }
            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                    link.Click();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                        link.Click();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                            link.Click();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + CSSSelector + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + CSSSelector);
                            throw;
                        }
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Finds the x path then click without scroll into view.
        /// </summary>
        /// <param name="Xpath">The xpath.</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindXPathThenClickWithoutScrollIntoView(string Xpath, BrowserStackBrowsers client)
        {
            try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException), typeof(ElementNotVisibleException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(Xpath)));
                    link.Click();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + Xpath + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException), typeof(ElementNotVisibleException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(Xpath)));
                        link.Click();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + Xpath + " and clicked");
                    }
                    catch
                    {
                        try
                        {
                            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                            fluentWait.Timeout = TimeSpan.FromSeconds(20);
                            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException), typeof(ElementNotVisibleException));
                            IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(Xpath)));
                            link.Click();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("KoloQA: Found " + Xpath + " and clicked");
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: Error FindByCSSSelectorThenType, Selector " + Xpath);
                            throw;
                        }

                    }
                }
            return this;
        }

        /// <summary>
        /// Find by XPath Selector then Click
        /// </summary>
        /// <param name="XPath">XPath to find</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA FindXPathThenClick(string XPath, BrowserStackBrowsers client)
        {

            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + XPath + " and clicked");
            }
            catch
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + XPath + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                        ScrollIntoViewAndClick(link);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + XPath + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA: Error FindByXPathThenClick, Selector " + XPath);
                        throw;
                    }

                }
            }

            return this;
        }

        /// <summary>
        /// Finds the x path then click fail fast.
        /// </summary>
        /// <param name="XPath">The x path.</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindXPathThenClickFailFast(string XPath, BrowserStackBrowsers client)
        {

            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                IWebElement link = Driver.FindElement(By.XPath(XPath));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + XPath + " and clicked");
            }
            catch
            {
                TestContext.WriteLine("KoloQA: Error FindByXPathThenClick, Selector " + XPath);
                throw;
            }

            return this;
        }

        /// <summary>
        /// Finds the type of the x path type then tab then.
        /// </summary>
        /// <param name="XPath">The x path.</param>
        /// <param name="FirstInput">The first input.</param>
        /// <param name="SecondInput">The second input.</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindXPathTypeThenTabThenType(string XPath, string FirstInput, string SecondInput, BrowserStackBrowsers client)
        {
            if(FirstInput.ToLower() == "automated")
            {
                FirstInput = RandomString().ToUpper();
            }
            if (SecondInput.ToLower() == "automated")
            {
                FirstInput = RandomString().ToUpper();
            }

            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                ScrollIntoViewAndClick(link);
                link.SendKeys(FirstInput + Keys.Tab + SecondInput);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + XPath + " and clicked");
            }
            catch
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                    ScrollIntoViewAndClick(link);
                    link.SendKeys(FirstInput + Keys.Tab + SecondInput);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + XPath + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                        ScrollIntoViewAndClick(link);
                        link.SendKeys(FirstInput + Keys.Tab + SecondInput);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + XPath + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA: Error FindByXPathThenClick, Selector " + XPath);
                        throw;
                    }

                }
            }

            return this;
        }

        /// <summary>
        /// Find Drop Down By Css Selector then Select the Value Provided
        /// </summary>
        /// <param name="SelectListCSS">CSS Selector of the List</param>
        /// <param name="ValueInList">The Value to select</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA DropDownByCSSSelectorThenSelectValue(string SelectListCSS, string ValueInList, BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
            {
                string xpath = GetXpathFromCSS(SelectListCSS);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewUsingJavaScript(link);
                    SelectElement select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        ScrollIntoViewUsingJavaScript(link);
                        SelectElement select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                    catch
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        ScrollIntoViewUsingJavaScript(link);
                        SelectElement select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                }
            }
            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                    ScrollIntoViewUsingJavaScript(link);
                    SelectElement select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                        ScrollIntoViewUsingJavaScript(link);
                        SelectElement select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                    catch
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                        ScrollIntoViewUsingJavaScript(link);
                        SelectElement select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Drops down by CSS selector then select value without scroll.
        /// </summary>
        /// <param name="SelectListCSS">The select list CSS.</param>
        /// <param name="ValueInList">The value in list.</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA DropDownByCSSSelectorThenSelectValueWithoutScroll(string SelectListCSS, string ValueInList, BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
            {
                string xpath = GetXpathFromCSS(SelectListCSS);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    var select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        var select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                    catch
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        var select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                }
            }
            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                    var select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                        var select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                    catch
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                        var select = new SelectElement(link);
                        select.SelectByText(ValueInList);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Select a value from a drop down list selected by Id
        /// </summary>
        /// <param name="SelectListId">The Id of the select list</param>
        /// <param name="ValueInList">The Value in the list</param>
        /// <returns>KoloQA instance</returns>
        public KoloQA DropDownByIdThenSelectValue(string SelectListId, string ValueInList)
        {
            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(SelectListId)));
                ScrollIntoViewUsingJavaScript(link);
                SelectElement select = new SelectElement(link);
                select.SelectByText(ValueInList);
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(SelectListId)));
                    ScrollIntoViewUsingJavaScript(link);
                    SelectElement select = new SelectElement(link);
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
        /// Select a value from a drop down list selected by name attribute
        /// </summary>
        /// <param name="SelectListName">The select list name attribute</param>
        /// <param name="ValueInList">The value to select in the list</param>
        /// <param name="appium">if set to <c>true</c> [appium].</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA DropDownByNameThenSelectValue(string SelectListName, string ValueInList, bool appium = false)
        {
            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.Name(SelectListName)));
                ScrollIntoViewUsingJavaScript(link);
                SelectElement select = new SelectElement(link);
                select.SelectByText(ValueInList);
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.Name(SelectListName)));
                    ScrollIntoViewUsingJavaScript(link);
                    SelectElement select = new SelectElement(link);
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
        /// Select a Drop Down Value by Xpath
        /// </summary>
        /// <param name="SelectListName">The Select List XPath</param>
        /// <param name="ValueInList">The Value to Select</param>
        /// <returns>KoloQA.</returns>
        public KoloQA DropDownByXpathThenSelectValue(string SelectListName, string ValueInList)
        {
            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(SelectListName)));
                ScrollIntoViewUsingJavaScript(link);
                SelectElement select = new SelectElement(link);
                select.SelectByText(ValueInList);
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(SelectListName)));
                    ScrollIntoViewUsingJavaScript(link);
                    SelectElement select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
                catch
                {
                    TestContext.WriteLine("KoloQA Error: DropDownByXpathThenSelectValue, Selector " + SelectListName);
                    throw;
                }
            }
            return this;
        }

        /// <summary>
        /// Find an Element by CSS Selector and then type the value provided into it
        /// </summary>
        /// <param name="CSSSelector">The CSS Selector of the element</param>
        /// <param name="ValueToType">The value to type into the element</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindByCSSSelectorThenType(string CSSSelector, string ValueToType, BrowserStackBrowsers client)
        {
            ValueToType = KoloControl.StringTranslater(ValueToType);
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape || client == BrowserStackBrowsers.iPadLandscape || client == BrowserStackBrowsers.iPadPortrait)
            {
                string xpath = GetXpathFromCSS(CSSSelector);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewAndType(link, ValueToType);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        ScrollIntoViewAndType(link, ValueToType);
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + CSSSelector + " Value to Type Was: " + ValueToType);
                        throw; throw;
                    }
                }
            }

            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                    ScrollIntoViewAndType(link, ValueToType);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                        ScrollIntoViewAndType(link, ValueToType);
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + CSSSelector + " Value to Type Was: " + ValueToType);
                        throw;
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Find an Element by Xpath Selector and then type the value provided into it
        /// </summary>
        /// <param name="XPath">The XPath of the element</param>
        /// <param name="ValueToType">The value to type into the element</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindByXPathThenType(string XPath, string ValueToType, BrowserStackBrowsers client)
        {
            ValueToType = KoloControl.StringTranslater(ValueToType);

            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                ScrollIntoViewAndType(link, ValueToType);
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(XPath)));
                    ScrollIntoViewAndType(link, ValueToType);
                }
                catch
                {
                    TestContext.WriteLine("KoloQA Error: FindByXPathThenType, Selector " + XPath + " Value to Type Was: " + ValueToType);
                    throw; throw;
                }
            }
            return this;
        }

        /// <summary>
        /// Find an Element by CSS Selector and then type the value provided into it
        /// </summary>
        /// <param name="CSSSelector">The CSS Selector of the element</param>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindByCSSSelectorThenPressEnter(string CSSSelector, BrowserStackBrowsers client)
        {
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape || client == BrowserStackBrowsers.iPadLandscape || client == BrowserStackBrowsers.iPadPortrait)
            {
                string xpath = GetXpathFromCSS(CSSSelector);

                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewAndTypeThePressEnter(link);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        ScrollIntoViewAndTypeThePressEnter(link);
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenPressEnter, Selector " + CSSSelector);
                        throw; throw;
                    }
                }
            }

            else
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                    ScrollIntoViewAndTypeThePressEnter(link);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                        fluentWait.Timeout = TimeSpan.FromSeconds(20);
                        fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CSSSelector)));
                        ScrollIntoViewAndTypeThePressEnter(link);
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + CSSSelector);
                        throw;
                    }
                }
            }

            return this;
        }

        #region Controls
        /// <summary>
        /// Download Videos
        /// </summary>
        /// <returns>KoloQA.</returns>
        public KoloQA BrowserStackDownloadVideos()
        {
            KoloControl.GetTestVideos().Wait();
            return this;
        }

        /// <summary>
        /// Sets the nunit test context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA SetNunitTestContext(string key, string value)
        {
            SetNunitTestContext(key, value);
            return this;
        }

        #endregion

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
        /// <returns>KoloQA.</returns>
        public KoloQA ScrollIntoViewAndTypeThenTab(IWebElement element, string input)
        {
            if (input.ToLower() == "automated")
            {
                input = RandomString().ToUpper();
            }

            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
            js.ExecuteScript("arguments[0].click(); ", element);
            element.SendKeys(input + Keys.Tab);
            return this;
        }

        /// <summary>
        /// Scroll into view and the type
        /// </summary>
        /// <param name="element">The Element to be scrolled in to view</param>
        /// <param name="input">The Value to be typed into the element</param>
        /// <returns>KoloQA.</returns>
        public KoloQA ScrollIntoViewAndType(IWebElement element, string input)
        {
            if (input.ToLower() == "automated")
            {
                input = RandomString().ToUpper();
            }
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
            js.ExecuteScript("arguments[0].click(); ", element);
            element.SendKeys(input);
            return this;
        }

        /// <summary>
        /// Scrolls the into view and type the press enter.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>KoloQA.</returns>
        public KoloQA ScrollIntoViewAndTypeThePressEnter(IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
            js.ExecuteScript("arguments[0].click(); ", element);
            element.SendKeys(Keys.Enter);
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
        #endregion

        #region Validation

        /// <summary>
        /// Page title is equal to
        /// </summary>
        /// <param name="Title">The Page Title to check</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA PageTitleEquals(string Title)
        {
            Assert.Equals(Title, Driver.Title);
            return this;
        }

        /// <summary>
        /// Check the Page contains certain text
        /// </summary>
        /// <param name="Text">Text to check in page</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA PageContainsText(string Text)
        {
            IWebElement body = Driver.FindElement(By.TagName("body"));
            Assert.IsTrue(body.Text.Contains(Text));
            return this;
        }
        #endregion

        /// <summary>
        /// Find an Input by its CssSelector and Clear its Contents
        /// </summary>
        /// <param name="CssSelector">The CSS selector.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA FindByCssSelectorClearInputField(string CssSelector)
        {
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(20);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            IWebElement link = fluentWait.Until(x => x.FindElement(By.CssSelector(CssSelector)));
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
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
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(Id)));
                ScrollIntoViewUsingJavaScript(link);
                link.Click();
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(Id)));
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
        /// <param name="LinkText">The Text in the link</param>
        /// <returns>KolOQA Instance</returns>
        public KoloQA ClickLinkByLinkText(string LinkText)
        {
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(20);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            IWebElement link = fluentWait.Until(x => x.FindElement(By.XPath("//*[contains(text(), '" + LinkText + "')]")));
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
            try
            {
                js.ExecuteScript("arguments[0].click();", link);
            }
            catch (Exception)
            {
                IWebElement link2 = fluentWait.Until(x => x.FindElement(By.XPath("//*[contains(text(), '" + LinkText + "')]/..")));
                js.ExecuteScript("arguments[0].click();", link);
            }

            return this;
        }

        /// <summary>
        /// Click a link by partial link text
        /// </summary>
        /// <param name="LinkText">The text contained in the link</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA ClickLinkByPartialLinkText(string LinkText)
        {
            try
            {
                DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                fluentWait.Timeout = TimeSpan.FromSeconds(20);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement link = fluentWait.Until(x => x.FindElement(By.PartialLinkText(LinkText)));
                IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
                js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
                link.Click();
            }
            catch
            {
                try
                {
                    DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
                    fluentWait.Timeout = TimeSpan.FromSeconds(20);
                    fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement link = fluentWait.Until(x => x.FindElement(By.PartialLinkText(LinkText)));
                    link.Click();
                }
                catch
                {
                    TestContext.WriteLine("KoloQA Error: ClickByPartialLinkText, Link " + LinkText);
                    throw;
                }
            }
            return this;
        }

        /// <summary>
        /// Find an Input by its Id and Clear its Contents
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA FindByIdClearInputField(string Id)
        {
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(20);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(Id)));
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
            link.Click();
            link.Clear();
            return this;
        }

        /// <summary>
        /// Find by Id and then Type into the Input Box
        /// </summary>
        /// <param name="Id">The Id of the input box</param>
        /// <param name="InputText">The text to type into the input box</param>
        /// <returns>KoloQA.</returns>
        public KoloQA FindByIdThenType(string Id, string InputText)
        {
            InputText = KoloControl.StringTranslater(InputText);
            DefaultWait<IWebDriver> fluentWait = new DefaultWait<IWebDriver>(Driver);
            fluentWait.Timeout = TimeSpan.FromSeconds(20);
            fluentWait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            IWebElement link = fluentWait.Until(x => x.FindElement(By.Id(Id)));
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
            link.Click();
            link.SendKeys(InputText);
            return this;
        }

        #region BrowserStack Marker
        /// <summary>
        /// Marks the Test as Passed on BrowserStack
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA BrowserStackMarkTestPassed(BrowserStackBrowsers client)
        {
            try
            {
                if (client != BrowserStackBrowsers.LocalChrome)
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Test Passed \"}}");
                }
                TestContext.WriteLine(@"
-----------------------------------------------------------------
  _______          _     _____                            _ 
 |__   __|        | |   |  __ \                          | |
    | |  ___  ___ | |_  | |__) |__ _  ___  ___   ___   __| |
    | | / _ \/ __|| __| |  ___// _` |/ __|/ __| / _ \ / _` |
    | ||  __/\__ \| |_  | |   | (_| |\__ \\__ \|  __/| (_| |
    |_| \___||___/ \__| |_|    \__,_||___/|___/ \___| \__,_|
                                                            
-----------------------------------------------------------------                                                            
");
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
        /// <param name="client">The client.</param>
        /// <returns>KoloQA Instance</returns>
        public KoloQA BrowserStackMarkTestFailed(BrowserStackBrowsers client)
        {
            if (client != BrowserStackBrowsers.LocalChrome)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Test Failed \"}}");
            }
            TestContext.WriteLine(@"
-----------------------------------------------------------------
  _______          _     ______      _  _            _ 
 |__   __|        | |   |  ____|    (_)| |          | |
    | |  ___  ___ | |_  | |__  __ _  _ | |  ___   __| |
    | | / _ \/ __|| __| |  __|/ _` || || | / _ \ / _` |
    | ||  __/\__ \| |_  | |  | (_| || || ||  __/| (_| |
    |_| \___||___/ \__| |_|   \__,_||_||_| \___| \__,_|          

-----------------------------------------------------------------");
            return this;
        }

        #endregion

        #region GdsPageIndexing
        /// <summary>
        /// Index GDS page model as an asynchronous operation.
        /// </summary>
        /// <param name="PageModelName">Name of the page model.</param>
        /// <param name="Element">The element.</param>
        /// <param name="Ignores">The ignores.</param>
        /// <returns>A Task&lt;KoloQA&gt; representing the asynchronous operation.</returns>
        public async Task<KoloQA> IndexGdsPageModelAsync(string PageModelName, string Element = "", List<string> Ignores = null)
        {
            if (TestContext.Parameters["GeneratePages"] == "true")
            {
                GdsPageModelGenerator pageModelGenerator = new GdsPageModelGenerator();
                GdsPageModel model = await pageModelGenerator.GDSPageGeneratorAsync(Driver, Element);
                KoloControl.GeneratePageClass(PageModelName, model);
                KoloControl.GenerateSimplePageClass(PageModelName, model);
            }
            return this;
        }
        #endregion
        #endregion
    }
}