using BrowserStack;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
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
/// Kolo QA is the main object for manipulating Selenium Sessions.
/// </summary>
public class KoloQA
{
    /// <summary>
    /// Kolo Test Suite Information
    /// </summary>
    public KoloTestSuite? TestSuite { get; set; }

    private Local Local { get; } = new();

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
            TestSuite ??= new KoloTestSuite();
            TestSuite.TestCases ??= new List<KoloTestCase>();

            if (TestSuite.TestCases.Any(i => i.Id == id))
            {
                var testCase = TestSuite.TestCases.Single(i => i.Id == id);
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
                TestSuite.TestCases.Add(testCase);
            }
        }
        catch
        {
            // TODO Create custom exceptions
            throw;
        }
        return this;
    }

    /// <summary>
    /// Check the Accessibility of the Page
    /// </summary>
    /// <param name="pageName"></param>
    /// <param name="client"></param>
    /// <param name="wcagLevel"></param>
    /// <returns></returns>
    public KoloQA AccessibilityOnPage(string pageName, BrowserStackBrowsers client, WcagLevel wcagLevel = WcagLevel.wcag2aa)
    {
        if (TestContext.Parameters["Accessibility"] != "true" || client.ToString().Contains("iP")) return this;
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
    /// <param name="id">The Id given to the test</param>
    /// <returns>Kolo QA Instance</returns>
    public KoloQA OutputCompletedStepsToThisPoint(string id)
    {
        if (TestSuite?.TestCases is null)
        {
            throw new NullReferenceException("Cannot add completed steps to a null case");
        }

        var testCase = TestSuite.TestCases.Single(i => i.Id == id);
        TestContext.WriteLine("Test Case: " + testCase.Id);
        foreach (var step in testCase.TestSteps)
            TestContext.WriteLine("Test Step: " + step.StepNumber + " - " + step.Name + " " + step.Description +
                                  " - Step Passed: " + step.Passed);
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
    /// Return a generated random string
    /// </summary>
    /// <returns></returns>
    public string RandomString()
    {
        var randomString = KoloControl.RandomStringGenerator();
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
        var device = deviceSim switch
        {
            ChromeDeviceSim.iPadAir => "iPad Air",
            ChromeDeviceSim.GalaxyFold => "Galaxy Fold",
            ChromeDeviceSim.iPadMini => "iPad Mini",
            ChromeDeviceSim.iPhone12Pro => "iPhone 12 Pro",
            ChromeDeviceSim.iPhoneSE => "iPhone SE",
            ChromeDeviceSim.iPhoneXr => "iPhone XR",
            ChromeDeviceSim.NestHub => "Nest Hub",
            ChromeDeviceSim.NestHubMax => "Nest Hub Max",
            ChromeDeviceSim.Pixel5 => "Pixel 5",
            ChromeDeviceSim.SamsungGalaxyA5171 => "Samsung Galaxy A51/71",
            ChromeDeviceSim.SamsungGalaxyS20Ultra => "Samsung Galaxy S20 Ultra",
            ChromeDeviceSim.SamsungGalaxyS8Plus => "Samsung Galaxy S8 Plus",
            ChromeDeviceSim.SurfaceDuo => "Surface Duo",
            ChromeDeviceSim.SurfacePro7 => "Surface Pro 7",
            _ => ""
        };

        var chromeOptions = new ChromeOptions();
        chromeOptions.EnableMobileEmulation(device);
        Driver = new ChromeDriver(chromeOptions);
        return this;
    }

    /// <summary>
    /// Maximises the Browser Window
    /// </summary>
    /// <returns>KoloQA Instance</returns>
    public KoloQA MaximiseBrowser(BrowserStackBrowsers client)
    {
        if (client is BrowserStackBrowsers.iPhonePortrait 
                   or BrowserStackBrowsers.iPhoneLandscape 
                   or BrowserStackBrowsers.iPadLandscape 
                   or BrowserStackBrowsers.iPadPortrait)
        {
            return this;
        }

        Driver.Manage().Window.Maximize();
        return this;
    }

    /// <summary>
    /// Set Implicit Timeout
    /// </summary>
    /// <param name="timeOut">Timeout in seconds</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA SetImplicitTimeout(int timeOut)
    {
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeOut);
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
            var bsLocalArgs = new List<KeyValuePair<string, string>>
            {
                new("key", AccessKey)
            };
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
    public KoloQA BrowserStackSession(BrowserStackBrowsers client)
    {
        TestContext.WriteLine(@"
                                NNN/  :dNNd- .ohNMMNds:   -NNN.       :sdNMMNdo.
                                MMM/.yMMMo  sMMMmhydMMMd. -MMM.     .dMMMdyymMMMy`
                                MMMhNMMh.  yMMm-    `yMMN`-MMM.    `mMMh`    -mMMh
                                MMMMMMM/   NMM+      .MMM:-MMM.    -MMM.      +MMM
                                MMMMyNMMh. yMMm-    `yMMN`-MMM.    `mMMh.    -mMMy
                                MMM+ `hMMN+`sMMMmhydMMMd. -MMMhhhhh/.dMMMdyhmMMMy`
                                mmm:   /mmmy`.ohmMMNds:   -mmmmmmmm+  -sdNMMNho.");
        var browserstackOptions = new Dictionary<string, object>
        {
            { "projectName", ProjectName },
            { "buildName", BuildName },
            { "sessionName", TestName + " - " + DateTime.Now.ToString("dddd, dd MMMM yyyy") },
            { "userName", UserName },
            { "accessKey", AccessKey }
        };

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
            switch (client.ToString())
            {
                case "LocalChrome":
                    LaunchChrome();
                    break;
                case "iPhoneLandscape":
                {
                    var capabilities = new SafariOptions();
                    browserstackOptions.Add("osVersion", "15");
                    browserstackOptions.Add("deviceName", "iPhone 13");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                case "iPhonePortrait":
                {
                    var capabilities = new SafariOptions();
                    browserstackOptions.Add("osVersion", "15");
                    browserstackOptions.Add("deviceName", "iPhone 13");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "portrait");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                case "iPadPortrait":
                {
                    var capabilities = new SafariOptions();
                    browserstackOptions.Add("osVersion", "14");
                    browserstackOptions.Add("deviceName", "iPad Air 4");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                case "iPadLandscape":
                {
                    var capabilities = new SafariOptions();
                    browserstackOptions.Add("osVersion", "14");
                    browserstackOptions.Add("deviceName", "iPad Air 4");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                case "SamsungGalaxyAndroidLandscape":
                {
                    var capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "11.0");
                    browserstackOptions.Add("deviceName", "Samsung Galaxy S21");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    return this;
                }
                case "SamsungGalaxyAndroidPortrait":
                {
                    var capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "11.0");
                    browserstackOptions.Add("deviceName", "Samsung Galaxy S21");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "portrait");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    return this;
                }
                case "GooglePixel4XLAndroidLandscape":
                {
                    var capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "10.0");
                    browserstackOptions.Add("deviceName", "Google Pixel 4 XL");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "landscape");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    return this;
                }
                case "GooglePixel4XLAndroidPortrait":
                {
                    var capabilities = new ChromeOptions();
                    browserstackOptions.Add("osVersion", "10.0");
                    browserstackOptions.Add("deviceName", "Google Pixel 4 XL");
                    browserstackOptions.Add("realMobile", "true");
                    browserstackOptions.Add("deviceOrientation", "portrait");
                    browserstackOptions.Add("appiumVersion", "2.0.0");
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    return this;
                }
                case "OSXSafari":
                {
                    var capabilities = new SafariOptions
                    {
                        BrowserVersion = "14.0"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "OSXChrome":
                {
                    var capabilities = new ChromeOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "OSXFireFox":
                {
                    var capabilities = new FirefoxOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);
                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "OSXEdge":
                {
                    var capabilities = new EdgeOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "OS X");
                    browserstackOptions.Add("osVersion", "Big Sur");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "Win11Edge":
                {
                    var capabilities = new EdgeOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "11");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "Win11Chrome":
                {
                    var capabilities = new ChromeOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "11");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "Win11FireFox":
                {
                    var capabilities = new FirefoxOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "11");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "Win10Edge":
                {
                    var capabilities = new EdgeOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "10");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "Win10Chrome":
                {
                    var capabilities = new ChromeOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "10");
                        browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
                case "Win10FireFox":
                {
                    var capabilities = new FirefoxOptions
                    {
                        BrowserVersion = "latest"
                    };
                    var safariOptions = new Dictionary<string, object>
                    {
                        { "enablePopups", "true" },
                        { "allowAllCookies", "true" }
                    };
                    browserstackOptions.Add("os", "Windows");
                    browserstackOptions.Add("osVersion", "10");
                    browserstackOptions.Add("safari", safariOptions);
                    capabilities.AddAdditionalOption("bstack:options", browserstackOptions);

                    Driver = new RemoteWebDriver(
                        new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities);
                    Driver.Manage().Window.Maximize();
                    return this;
                }
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
    public KoloQA OpenURL(string url)
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
    /// <param name="Id">Test Case Id or Name</param>
    /// <param name="browser">The Browser/OS/Device Combination being used</param>
    /// <param name="priority">The Priority of the defect</param>
    /// <param name="severity">The Severity of the defect 1-4 with 1 being Highest</param>
    /// <param name="assignTo">Assign the defect to someone, will default to empty</param>
    /// <returns></returns>
    public KoloQA AzureDevopsRaiseDefect(string Id, BrowserStackBrowsers browser, int priority, int severity, string assignTo = "")
    {
        if (TestSuite?.TestCases is null) return this;

        var testCase = TestSuite.TestCases.Single(i => i.Id == Id);
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
        var js = (IJavaScriptExecutor)Driver;
        var wait = new WebDriverWait(Driver, new TimeSpan(0, 0, timeoutSec));
        wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        Thread.Sleep(300);
        return this;
    }

    /// <summary>
    /// Wait for Text to be present
    /// </summary>
    /// <param name="Text">Text to search for</param>
    /// <param name="timeout">The timeout to wait for the text to be displayed</param>
    /// <returns></returns>
    public KoloQA WaitForText(string Text, int timeout = 10)
    {
        try
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
            wait.Until(c => c.FindElement(By.XPath("//*[contains(text(), '" + Text + "')]")));
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
    /// 
    /// </summary>
    /// <param name="LinkText"></param>
    /// <returns></returns>
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
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.Id(LinkText)));
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
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.Id(LinkText)));
                ScrollIntoViewUsingJavaScript(link);
                return link;
            }
        }
    }

    /// <summary>
    /// Uploads a file from the File Uploads Folder
    /// </summary>
    /// <param name="fileName">The filename to upload</param>
    /// <param name="id">Optional Override for the Id if not marked as file</param>
    /// <returns></returns>
    public KoloQA UploadFile(string fileName, string id = "file")
    {
        var upload = FluentWaitByIdReturnElement(id);
        var path = "../../../FileUploads/" + fileName;
        var detector = new LocalFileDetector();
        if (Driver is IAllowsFileDetection allowsDetection)
        {
            allowsDetection.FileDetector = detector;
        }
        upload.SendKeys(path);
        return this;
    }

    /// <summary>
    /// Find by link text then click
    /// </summary>
    /// <param name="linkText">Text of Link</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindLinkTextThenClick(string linkText)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            var link = fluentWait.Until(x => x.FindElement(By.LinkText(linkText)));
            ScrollIntoViewAndClick(link);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("KoloQA: Found " + linkText + " and clicked");
        }
        catch (Exception e)
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.LinkText(linkText)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + linkText + " and clicked");
            }
            catch
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.LinkText(linkText)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + linkText + " and clicked");
            }
        }
        return this;
    }

    /// <summary>
    /// Retrieves XPath from CSS Selector provided
    /// </summary>
    /// <param name="cssSelector">CSS Selector for the element</param>
    /// <returns></returns>
    public string GetXpathFromCss(string cssSelector)
    {
        var xpath = "";

        try
        {
            WaitUntilPageFullyLoaded();
            
            var pageHtml = Driver.PageSource;
            var pageMaster = new HtmlDocument();
            pageMaster.LoadHtml(pageHtml);
            var document = pageMaster.DocumentNode;
            if (cssSelector.Contains("nth-of-type"))
            {
                var nodenth = document.NthOfTypeQuerySelector(cssSelector);
                xpath = nodenth.XPath;
            }
            else
            {
                var node = document.QuerySelector(cssSelector);
                xpath = node.XPath;
                TestContext.WriteLine("KoloQA: Calculated XPath: " + xpath);
            }
        }
        catch (Exception e)
        {
            TestContext.WriteLine("KoloQA: Selector Was: " + cssSelector);
            TestContext.WriteLine("KoloQA: Can not find or generate " + e.Message);
        }
        TestContext.WriteLine(xpath);
        return xpath;
    }

    /// <summary>
    /// Find by CSS Selector then Click
    /// </summary>
    /// <param name="cssSelector">CSS Selector to find</param>
    /// <param name="client"></param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindCSSSelectorThenClick(string cssSelector, BrowserStackBrowsers client)
    {
        if (client is BrowserStackBrowsers.iPhonePortrait or BrowserStackBrowsers.iPhoneLandscape)
        {
            var xpath = GetXpathFromCss(cssSelector);

            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
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
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        ScrollIntoViewAndClick(link);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + cssSelector + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA: Error FindByCSSSelectorThenType, Selector " + cssSelector);
                        throw;
                    }
                }
            }
        }
        else
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
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
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                        ScrollIntoViewAndClick(link);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + cssSelector + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + cssSelector);
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
    /// <param name="cssSelector">CSS Selector to find</param>
    /// <param name="client"></param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindCSSSelectorThenClickWithoutScrollIntoView(string cssSelector, BrowserStackBrowsers client)
    {
        if (client is BrowserStackBrowsers.iPhonePortrait or BrowserStackBrowsers.iPhoneLandscape)
        {
            var xpath = GetXpathFromCss(cssSelector);

            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                link.Click();
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    link.Click();
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
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                        link.Click();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + cssSelector + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA: Error FindByCSSSelectorThenType, Selector " + cssSelector);
                        throw;
                    }
                }
            }
        }
        else
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                link.Click();
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                    link.Click();
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
                        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                        link.Click();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + cssSelector + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + cssSelector);
                        throw;
                    }
                }
            }
        }
        return this;
    }

    /// <summary>
    /// Find by XPath Selector then Click
    /// </summary>
    /// <param name="xPath">XPath to find</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindXPathThenClick(string xPath)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            var link = fluentWait.Until(x => x.FindElement(By.XPath(xPath)));
            ScrollIntoViewAndClick(link);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("KoloQA: Found " + xPath + " and clicked");
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
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(xPath)));
                ScrollIntoViewAndClick(link);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + xPath + " and clicked");
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.XPath(xPath)));
                    ScrollIntoViewAndClick(link);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + xPath + " and clicked");
                }
                catch
                {
                    TestContext.WriteLine("KoloQA: Error FindByXPathThenClick, Selector " + xPath);
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
    /// <returns></returns>
    public KoloQA DropDownByCSSSelectorThenSelectValue(string SelectListCSS, string ValueInList, BrowserStackBrowsers client)
    {
        if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
        {
            var xpath = GetXpathFromCss(SelectListCSS);

            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewUsingJavaScript(link);
                    var select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
                catch
                {
                    var fluentWait = new DefaultWait<IWebDriver>(Driver)
                    {
                        Timeout = TimeSpan.FromSeconds(20),
                        PollingInterval = TimeSpan.FromMilliseconds(250)
                    };
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewUsingJavaScript(link);
                    var select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
            }
        }
        else
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                    ScrollIntoViewUsingJavaScript(link);
                    var select = new SelectElement(link);
                    select.SelectByText(ValueInList);
                }
                catch
                {
                    var fluentWait = new DefaultWait<IWebDriver>(Driver)
                    {
                        Timeout = TimeSpan.FromSeconds(20),
                        PollingInterval = TimeSpan.FromMilliseconds(250)
                    };
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.CssSelector(SelectListCSS)));
                    ScrollIntoViewUsingJavaScript(link);
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
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
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
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
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
    /// <param name="selectListName">The select list name attribute</param>
    /// <param name="valueInList">The value to select in the list</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA DropDownByNameThenSelectValue(string selectListName, string valueInList)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            var link = fluentWait.Until(x => x.FindElement(By.Name(selectListName)));
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
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.Name(selectListName)));
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
    /// Select a Drop Down Value by Xpath
    /// </summary>
    /// <param name="selectListName">The Select List XPath</param>
    /// <param name="valueInList">The Value to Select</param>
    /// <returns></returns>
    public KoloQA DropDownByXpathThenSelectValue(string selectListName, string valueInList)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            var link = fluentWait.Until(x => x.FindElement(By.XPath(selectListName)));
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
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(selectListName)));
                ScrollIntoViewUsingJavaScript(link);
                var select = new SelectElement(link);
                select.SelectByText(valueInList);
            }
            catch
            {
                TestContext.WriteLine("KoloQA Error: DropDownByXpathThenSelectValue, Selector " + selectListName);
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
    /// <param name="client"></param>
    /// <returns></returns>
    public KoloQA FindByCSSSelectorThenType(string cssSelector, string valueToType, BrowserStackBrowsers client)
    {
        valueToType = KoloControl.StringTranslater(valueToType);
        if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape || client == BrowserStackBrowsers.iPadLandscape || client == BrowserStackBrowsers.iPadPortrait)
        {
            var xpath = GetXpathFromCss(cssSelector);

            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewAndType(link, valueToType);
                }
                catch
                {
                    TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + cssSelector + " Value to Type Was: " + valueToType);
                    throw;
                }
            }
        }
        else
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                    ScrollIntoViewAndType(link, valueToType);
                }
                catch
                {
                    TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + cssSelector + " Value to Type Was: " + valueToType);
                    throw;
                }
            }
        }

        return this;
    }

    /// <summary>
    /// Find an Element by Xpath Selector and then type the value provided into it
    /// </summary>
    /// <param name="xPath">The XPath of the element</param>
    /// <param name="valueToType">The value to type into the element</param>
    /// <param name="client"></param>
    /// <returns></returns>
    public KoloQA FindByXPathThenType(string xPath, string valueToType, BrowserStackBrowsers client)
    {
        valueToType = KoloControl.StringTranslater(valueToType);

        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            var link = fluentWait.Until(x => x.FindElement(By.XPath(xPath)));
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
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(xPath)));
                ScrollIntoViewAndType(link, valueToType);
            }
            catch
            {
                TestContext.WriteLine("KoloQA Error: FindByXPathThenType, Selector " + xPath + " Value to Type Was: " + valueToType);
                throw;
            }
        }
        return this;
    }

    /// <summary>
    /// Find an Element by CSS Selector and then type the value provided into it
    /// </summary>
    /// <param name="cssSelector">The CSS Selector of the element</param>
    /// <param name="client"></param>
    /// <returns></returns>
    public KoloQA FindByCSSSelectorThenPressEnter(string cssSelector, BrowserStackBrowsers client)
    {
        if (client is BrowserStackBrowsers.iPhonePortrait or BrowserStackBrowsers.iPhoneLandscape or BrowserStackBrowsers.iPadLandscape or BrowserStackBrowsers.iPadPortrait)
        {
            var xpath = GetXpathFromCss(cssSelector);

            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                ScrollIntoViewAndTypeThePressEnter(link);
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.XPath(xpath)));
                    ScrollIntoViewAndTypeThePressEnter(link);
                }
                catch
                {
                    TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenPressEnter, Selector " + cssSelector);
                    throw;
                }
            }
        }
        else
        {
            try
            {
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                ScrollIntoViewAndTypeThePressEnter(link);
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
                    fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
                    ScrollIntoViewAndTypeThePressEnter(link);
                }
                catch
                {
                    TestContext.WriteLine("KoloQA Error: FindByCSSSelectorThenType, Selector " + cssSelector);
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
        var actions = new Actions(Driver);
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
        var js = (IJavaScriptExecutor)Driver;
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
        var js = (IJavaScriptExecutor)Driver;
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
    public KoloQA ScrollIntoViewAndTypeThenTab(IWebElement element, string input)
    {
        var js = (IJavaScriptExecutor)Driver;
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
    /// <returns></returns>
    public KoloQA ScrollIntoViewAndType(IWebElement element, string input)
    {
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
        js.ExecuteScript("arguments[0].click(); ", element);
        element.SendKeys(input);
        return this;
    }

    /// <summary>
    /// Scroll element into view and press enter key
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public KoloQA ScrollIntoViewAndTypeThePressEnter(IWebElement element)
    {
        var js = (IJavaScriptExecutor)Driver;
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
        var js = (IJavaScriptExecutor)Driver;
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
    /// <param name="cssSelector"></param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindByCssSelectorClearInputField(string cssSelector)
    {
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
        var link = fluentWait.Until(x => x.FindElement(By.CssSelector(cssSelector)));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
        link.Click();
        link.Clear();
        return this;
    }

    /// <summary>
    /// Clicks an element by its Id
    /// </summary>
    /// <param name="id">The Id of the element to Click</param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA ClickById(string id)
    {
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            var link = fluentWait.Until(x => x.FindElement(By.Id(id)));
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
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.Id(id)));
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
    /// <returns>KoloQA Instance</returns>
    public KoloQA ClickLinkByLinkText(string linkText)
    {
        if (linkText == null) throw new ArgumentNullException(nameof(linkText));
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
        var link = fluentWait.Until(x => x.FindElement(By.XPath("//*[contains(text(), '" + linkText + "')]")));
        var js = (IJavaScriptExecutor)Driver;
        js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
        try
        {
            js.ExecuteScript("arguments[0].click();", link);
        }
        catch (Exception)
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
        try
        {
            var fluentWait = new DefaultWait<IWebDriver>(Driver)
            {
                Timeout = TimeSpan.FromSeconds(20),
                PollingInterval = TimeSpan.FromMilliseconds(250)
            };
            fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
            var link = fluentWait.Until(x => x.FindElement(By.PartialLinkText(linkText)));
            var js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + link.Location.X + "," + (link.Location.Y - 200) + ");");
            link.Click();
        }
        catch
        {
            try
            {
                RefreshPage();
                var fluentWait = new DefaultWait<IWebDriver>(Driver)
                {
                    Timeout = TimeSpan.FromSeconds(20),
                    PollingInterval = TimeSpan.FromMilliseconds(250)
                };
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                var link = fluentWait.Until(x => x.FindElement(By.PartialLinkText(linkText)));
                link.Click();
            }
            catch
            {
                TestContext.WriteLine("KoloQA Error: ClickByPartialLinkText, Link " + linkText);
                throw;
            }
        }
        return this;
    }

    /// <summary>
    /// Find an Input by its Id and Clear its Contents
    /// </summary>
    /// <param name="id"></param>
    /// <returns>KoloQA Instance</returns>
    public KoloQA FindByIdClearInputField(string id)
    {
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
        var link = fluentWait.Until(x => x.FindElement(By.Id(id)));
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
        var fluentWait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = TimeSpan.FromSeconds(20),
            PollingInterval = TimeSpan.FromMilliseconds(250)
        };
        fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
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
    public KoloQA BrowserStackMarkTestPassed(BrowserStackBrowsers client)
    {
        try
        {
            if (client != BrowserStackBrowsers.LocalChrome)
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
    public KoloQA BrowserStackMarkTestFailed(BrowserStackBrowsers client)
    {
        if (client != BrowserStackBrowsers.LocalChrome)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Test Failed \"}}");
        }
        return this;
    }

    #endregion BrowserStack Marker

    #region GdsPageIndexing

    /// <summary>
    /// Index GDS page model async
    /// </summary>
    /// <param name="pageModelName"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public async Task<KoloQA> IndexGdsPageModelAsync(string pageModelName, string element = "")
    {
        if (TestContext.Parameters["GeneratePages"] != "true") return this;
        var pageModelGenerator = new GdsPageModelGenerator();
        var model = await pageModelGenerator.GDSPageGeneratorAsync(Driver, element);
        KoloControl.GeneratePageClass(pageModelName, model);
        KoloControl.GenerateSimplePageClass(pageModelName, model);
        return this;
    }

    #endregion GdsPageIndexing

    #endregion Fluents
}