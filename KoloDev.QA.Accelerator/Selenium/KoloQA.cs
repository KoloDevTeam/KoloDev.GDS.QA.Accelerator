using BrowserStack;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using KoloDev.GDS.QA.Accelerator.Data;
using KoloDev.GDS.QA.Accelerator.Selenium;
using KoloDev.GDS.QA.Accelerator.Utility;
using Newtonsoft.Json;
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
using static KoloDev.GDS.QA.Accelerator.Data.KoloTestSuite;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace KoloDev.GDS.QA.Accelerator
{
    /// <summary>
    /// KoloQA is the main object for manipulating Selenium Sessions.
    /// </summary>
    public partial class KoloQA
    {
        private KoloTestSuite? testSuite;

        /// <summary>
        /// Kolo Test Suite Information
        /// </summary>
        public KoloTestSuite? TestSuite { get => testSuite; set => testSuite = value; }
        private Local local { get; set; } = new Local();

        /// <summary>
        /// Remote Web Driver Instance
        /// </summary>
        public IWebDriver Driver { get; set; } = null!;
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

        private bool LocalTest { get; set; } = false;

        /// <summary>
        /// BrowserStack Access Key
        /// </summary>
        private string AccessKey { get; set; } = "";

        /// <summary>
        /// Add Test Cases to Collection
        /// </summary>
        /// <param name="Id">The identifier for the Test Case</param>
        /// <param name="StepNumber">The Step Number of the Test Step</param>
        /// <param name="Name">The Name of the Test Step</param>
        /// <param name="Description">The Description of the Test Step</param>
        /// <param name="Passed">Whether the Test Step Passed or Not</param>
        /// <returns></returns>
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
        /// <param name="pageName"></param>
        /// <param name="wcagLevel"></param>
        /// <returns></returns>
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
                    string json = JsonSerializer.Serialize(axeResult);
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

        public KoloQA FindXPathThenClickFailFast(string XPath, BrowserStackBrowsers client)
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2.0);
                IWebElement element = Driver.FindElement(By.XPath(XPath));
                ScrollIntoViewAndClick(element);
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

        public KoloQA DropDownByCSSSelectorThenSelectValueWithoutScroll(string SelectListCSS, string ValueInList, BrowserStackBrowsers client)
        {
            string SelectListCSS2 = SelectListCSS;
            if (client == BrowserStackBrowsers.iPhonePortrait || client == BrowserStackBrowsers.iPhoneLandscape)
            {
                string xpath = GetXpathFromCSS(SelectListCSS2);
                try
                {
                    DefaultWait<IWebDriver> defaultWait = new DefaultWait<IWebDriver>(Driver);
                    defaultWait.Timeout = TimeSpan.FromSeconds(20.0);
                    defaultWait.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                    defaultWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement element = defaultWait.Until((IWebDriver x) => x.FindElement(By.XPath(xpath)));
                    SelectElement selectElement = new SelectElement(element);
                    selectElement.SelectByText(ValueInList);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> defaultWait2 = new DefaultWait<IWebDriver>(Driver);
                        defaultWait2.Timeout = TimeSpan.FromSeconds(20.0);
                        defaultWait2.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                        defaultWait2.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement element2 = defaultWait2.Until((IWebDriver x) => x.FindElement(By.XPath(xpath)));
                        SelectElement selectElement2 = new SelectElement(element2);
                        selectElement2.SelectByText(ValueInList);
                    }
                    catch
                    {
                        DefaultWait<IWebDriver> defaultWait3 = new DefaultWait<IWebDriver>(Driver);
                        defaultWait3.Timeout = TimeSpan.FromSeconds(20.0);
                        defaultWait3.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                        defaultWait3.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement element3 = defaultWait3.Until((IWebDriver x) => x.FindElement(By.XPath(xpath)));
                        SelectElement selectElement3 = new SelectElement(element3);
                        selectElement3.SelectByText(ValueInList);
                    }
                }
            }
            else
            {
                try
                {
                    DefaultWait<IWebDriver> defaultWait4 = new DefaultWait<IWebDriver>(Driver);
                    defaultWait4.Timeout = TimeSpan.FromSeconds(20.0);
                    defaultWait4.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                    defaultWait4.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement element4 = defaultWait4.Until((IWebDriver x) => x.FindElement(By.CssSelector(SelectListCSS2)));
                    SelectElement selectElement4 = new SelectElement(element4);
                    selectElement4.SelectByText(ValueInList);
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultWait<IWebDriver> defaultWait5 = new DefaultWait<IWebDriver>(Driver);
                        defaultWait5.Timeout = TimeSpan.FromSeconds(20.0);
                        defaultWait5.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                        defaultWait5.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement element5 = defaultWait5.Until((IWebDriver x) => x.FindElement(By.CssSelector(SelectListCSS2)));
                        SelectElement selectElement5 = new SelectElement(element5);
                        selectElement5.SelectByText(ValueInList);
                    }
                    catch
                    {
                        DefaultWait<IWebDriver> defaultWait6 = new DefaultWait<IWebDriver>(Driver);
                        defaultWait6.Timeout = TimeSpan.FromSeconds(20.0);
                        defaultWait6.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                        defaultWait6.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement element6 = defaultWait6.Until((IWebDriver x) => x.FindElement(By.CssSelector(SelectListCSS2)));
                        SelectElement selectElement6 = new SelectElement(element6);
                        selectElement6.SelectByText(ValueInList);
                    }
                }
            }

            return this;
        }


        public KoloQA UploadFileById(string FileName, string Id = "file")
        {
            IWebElement webElement = FluentWaitByIdReturnElement(Id);
            string text = "../../../FileUploads/" + FileName;
            LocalFileDetector fileDetector = new LocalFileDetector();
            IAllowsFileDetection allowsFileDetection = Driver as IAllowsFileDetection;
            if (allowsFileDetection != null)
            {
                allowsFileDetection.FileDetector = fileDetector;
            }

            webElement.SendKeys(text);
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

        public bool ValidateTextOfXPathElement(string XPath, string Value, int timeout = 10)
        {
            string text = "";
            try
            {
                WebDriverWait webDriverWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                webDriverWait.Until((IWebDriver c) => c.FindElement(By.XPath(XPath)));
                text = Driver.FindElement(By.XPath(XPath)).Text;
                if (text == Value)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                TestContext.Write("KoloQA: " + XPath + "' not found in current context page.");
                throw;
            }

            return false;
        }

        public bool ValidateTextOfCSSElement(string CSS, string Value, int timeout = 10)
        {
            string text = "";
            try
            {
                WebDriverWait webDriverWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                webDriverWait.Until((IWebDriver c) => c.FindElement(By.CssSelector(CSS)));
                text = Driver.FindElement(By.CssSelector(CSS)).Text;
                if (text == Value)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                TestContext.Write("KoloQA: " + CSS + "' not found in current context page.");
                throw;
            }

            return false;
        }

        public string GetTextOfXPathElement(string XPath, int timeout = 10)
        {
            try
            {
                WebDriverWait webDriverWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                webDriverWait.Until((IWebDriver c) => c.FindElement(By.XPath(XPath)));
                return Driver.FindElement(By.XPath(XPath)).Text;
            }
            catch (Exception)
            {
                TestContext.Write("KoloQA: " + XPath + "' not found in current context page.");
                throw;
            }
        }

        public IWebElement FluentWaitByNameReturnElement(string Name)
        {
            string Name2 = Name;
            try
            {
                DefaultWait<IWebDriver> defaultWait = new DefaultWait<IWebDriver>(Driver);
                defaultWait.Timeout = TimeSpan.FromSeconds(20.0);
                defaultWait.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                defaultWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement webElement = defaultWait.Until((IWebDriver x) => x.FindElement(By.Name(Name2)));
                ScrollIntoViewUsingJavaScript(webElement);
                return webElement;
            }
            catch (Exception)
            {
                try
                {
                    DefaultWait<IWebDriver> defaultWait2 = new DefaultWait<IWebDriver>(Driver);
                    defaultWait2.Timeout = TimeSpan.FromSeconds(20.0);
                    defaultWait2.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                    defaultWait2.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement webElement2 = defaultWait2.Until((IWebDriver x) => x.FindElement(By.Name(Name2)));
                    ScrollIntoViewUsingJavaScript(webElement2);
                    return webElement2;
                }
                catch
                {
                    DefaultWait<IWebDriver> defaultWait3 = new DefaultWait<IWebDriver>(Driver);
                    defaultWait3.Timeout = TimeSpan.FromSeconds(20.0);
                    defaultWait3.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                    defaultWait3.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement webElement3 = defaultWait3.Until((IWebDriver x) => x.FindElement(By.Name(Name2)));
                    ScrollIntoViewUsingJavaScript(webElement3);
                    return webElement3;
                }
            }
        }

        public List<IWebElement> FindListOfValuesByXPath(string XPath)
        {
            return Driver.FindElements(By.XPath(XPath)).ToList();
        }

        public KoloQA UploadFileByName(string FileName, string Id = "file")
        {
            IWebElement webElement = FluentWaitByNameReturnElement(Id);
            string text = "./FileUploads/" + FileName;
            LocalFileDetector fileDetector = new LocalFileDetector();
            IAllowsFileDetection allowsFileDetection = Driver as IAllowsFileDetection;
            if (allowsFileDetection != null)
            {
                allowsFileDetection.FileDetector = fileDetector;
            }

            webElement.SendKeys(text);
            return this;
        }

        public string GetTextOfCSSElement(string CSSSelector, int timeout = 10)
        {
            string CSSSelector2 = CSSSelector;
            string text = "";
            try
            {
                WebDriverWait webDriverWait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                webDriverWait.Until((IWebDriver c) => c.FindElement(By.CssSelector(CSSSelector2)));
                return Driver.FindElement(By.CssSelector(CSSSelector2)).Text;
            }
            catch (Exception)
            {
                TestContext.Write("KoloQA: " + CSSSelector2 + "' not found in current context page.");
                throw;
            }
        }

        public KoloQA FindXPathTypeThenTabThenType(string XPath, string FirstInput, string SecondInput, BrowserStackBrowsers client)
        {
            string XPath2 = XPath;
            if (FirstInput.ToLower() == "automated")
            {
                FirstInput = RandomString().ToUpper();
            }

            if (SecondInput.ToLower() == "automated")
            {
                FirstInput = RandomString().ToUpper();
            }

            try
            {
                DefaultWait<IWebDriver> defaultWait = new DefaultWait<IWebDriver>(Driver);
                defaultWait.Timeout = TimeSpan.FromSeconds(20.0);
                defaultWait.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                defaultWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                IWebElement webElement = defaultWait.Until((IWebDriver x) => x.FindElement(By.XPath(XPath2)));
                ScrollIntoViewAndClick(webElement);
                webElement.SendKeys(FirstInput + Keys.Tab + SecondInput);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + XPath2 + " and clicked");
            }
            catch
            {
                try
                {
                    DefaultWait<IWebDriver> defaultWait2 = new DefaultWait<IWebDriver>(Driver);
                    defaultWait2.Timeout = TimeSpan.FromSeconds(20.0);
                    defaultWait2.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                    defaultWait2.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                    IWebElement webElement2 = defaultWait2.Until((IWebDriver x) => x.FindElement(By.XPath(XPath2)));
                    ScrollIntoViewAndClick(webElement2);
                    webElement2.SendKeys(FirstInput + Keys.Tab + SecondInput);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + XPath2 + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> defaultWait3 = new DefaultWait<IWebDriver>(Driver);
                        defaultWait3.Timeout = TimeSpan.FromSeconds(20.0);
                        defaultWait3.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                        defaultWait3.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException));
                        IWebElement webElement3 = defaultWait3.Until((IWebDriver x) => x.FindElement(By.XPath(XPath2)));
                        ScrollIntoViewAndClick(webElement3);
                        webElement3.SendKeys(FirstInput + Keys.Tab + SecondInput);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + XPath2 + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA: Error FindByXPathThenClick, Selector " + XPath2);
                        throw;
                    }
                }
            }

            return this;
        }



        public KoloQA FindXPathThenClickWithoutScrollIntoView(string Xpath, BrowserStackBrowsers client)
        {
            try
            {
                DefaultWait<IWebDriver> defaultWait = new DefaultWait<IWebDriver>(Driver);
                defaultWait.Timeout = TimeSpan.FromSeconds(20.0);
                defaultWait.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                defaultWait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException), typeof(ElementNotVisibleException));
                IWebElement webElement = defaultWait.Until((IWebDriver x) => x.FindElement(By.XPath(Xpath)));
                webElement.Click();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KoloQA: Found " + Xpath + " and clicked");
            }
            catch
            {
                try
                {
                    DefaultWait<IWebDriver> defaultWait2 = new DefaultWait<IWebDriver>(Driver);
                    defaultWait2.Timeout = TimeSpan.FromSeconds(20.0);
                    defaultWait2.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                    defaultWait2.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException), typeof(ElementNotVisibleException));
                    IWebElement webElement2 = defaultWait2.Until((IWebDriver x) => x.FindElement(By.XPath(Xpath)));
                    webElement2.Click();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("KoloQA: Found " + Xpath + " and clicked");
                }
                catch
                {
                    try
                    {
                        DefaultWait<IWebDriver> defaultWait3 = new DefaultWait<IWebDriver>(Driver);
                        defaultWait3.Timeout = TimeSpan.FromSeconds(20.0);
                        defaultWait3.PollingInterval = TimeSpan.FromMilliseconds(250.0);
                        defaultWait3.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotInteractableException), typeof(ElementNotVisibleException));
                        IWebElement webElement3 = defaultWait3.Until((IWebDriver x) => x.FindElement(By.XPath(Xpath)));
                        webElement3.Click();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("KoloQA: Found " + Xpath + " and clicked");
                    }
                    catch
                    {
                        TestContext.WriteLine("KoloQA: Error FindXPathThenClickWithoutScrollIntoView, Selector " + Xpath);
                        throw;
                    }
                }
            }

            return this;
        }


        public KoloQA SaveScreenshotPNG(string filename)
        {
            ITakesScreenshot takesScreenshot = Driver as ITakesScreenshot;
            Screenshot screenshot = takesScreenshot.GetScreenshot();
            screenshot.SaveAsFile(filename, ScreenshotImageFormat.Png);
            return this;
        }

        public bool CheckIfFileExists(string FileName)
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)Driver;
            bool result = (bool)javaScriptExecutor.ExecuteScript("browserstack_executor: {\"action\": \"fileExists\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            TestContext.WriteLine("KoloQA: File Exists Check - " + FileName + " " + result);
            return result;
        }

        public BrowserStackModels.AutomationSession GetVideoUrl()
        {
            object obj = ((IJavaScriptExecutor)Driver).ExecuteScript("browserstack_executor: {\"action\": \"getSessionDetails\"}");
            string value = Convert.ToString(obj);
            BrowserStackModels.AutomationSession automationSession = JsonConvert.DeserializeObject<BrowserStackModels.AutomationSession>(value);
            TestContext.Write("KoloQA: Video Url: " + automationSession.VideoUrl);
            return automationSession;
        }


        public IDictionary<string, object> GetFileProperties(string FileName)
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)Driver;
            IDictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary = (IDictionary<string, object>)javaScriptExecutor.ExecuteScript("browserstack_executor: {\"action\": \"getFileProperties\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            TestContext.WriteLine("KoloQA: File Exists Check - " + FileName + " " + dictionary);
            return dictionary;
        }

        //
        // Summary:
        //     Writes the file to local.
        //
        // Parameters:
        //   FileName:
        //     Name of the file.
        public void WriteFileToLocal(string FileName)
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)Driver;
            javaScriptExecutor.ExecuteScript("browserstack_executor: {\"action\": \"getFileContent\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            string s = (string)javaScriptExecutor.ExecuteScript("browserstack_executor: {\"action\": \"getFileContent\", \"arguments\": {\"fileName\": \"" + FileName + "\"}}");
            byte[] bytes = Convert.FromBase64String(s);
            File.WriteAllBytes("./" + FileName, bytes);
        }



        public KoloQA StartSession(BrowserStackBrowsers client)
        {
            try
            {
                BrowserStackSession(client);
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("KoloQA: Retried Twice:  " + ex.Message);
                try
                {
                    BrowserStackSession(client);
                }
                catch (Exception ex2)
                {
                    TestContext.WriteLine("KoloQA: Retried Twice:  " + ex2.Message);
                }
            }

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

                    Driver = new RemoteWebDriver(
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

                    Driver = new RemoteWebDriver(
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

                    Driver = new RemoteWebDriver(
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

                    Driver = new RemoteWebDriver(
                    new Uri("https://hub-cloud.browserstack.com/wd/hub/"), capabilities.ToCapabilities(), TimeSpan.FromSeconds(120));
                    return this;
                }
                if (client.ToString() == "SamsungGalaxyAndroidLandscape")
                {
                    ChromeOptions capabilities = new ChromeOptions();
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
                if (client.ToString() == "SamsungGalaxyAndroidPortrait")
                {
                    ChromeOptions capabilities = new ChromeOptions();
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
                if (client.ToString() == "GooglePixel4XLAndroidLandscape")
                {
                    ChromeOptions capabilities = new ChromeOptions();
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
                if (client.ToString() == "GooglePixel4XLAndroidPortrait")
                {
                    ChromeOptions capabilities = new ChromeOptions();
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

                    Driver = new RemoteWebDriver(
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

                    Driver = new RemoteWebDriver(
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

                    Driver = new RemoteWebDriver(
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

                    Driver = new RemoteWebDriver(
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

                    Driver = new RemoteWebDriver(
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

        public IWebDriver GetDriver()
        {
            return Driver;
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// Waits until an element exists on page
        /// </summary>
        /// <param name="CSSSelector">The CSS Selector of the Element</param>
        /// <param name="timeout">The timeout before abandoning the wait</param>
        /// <returns></returns>
        public KoloQA WaitUntilElementExists(string CSSSelector, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeout));
                wait.Until(c => c.FindElement(By.CssSelector(CSSSelector)));
            }
            catch
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
        /// Uploads a file from the File Uploads Folder
        /// </summary>
        /// <param name="FileName">The filename to upload</param>
        /// <param name="Id">Optional Override for the Id if not marked as file</param>
        /// <returns></returns>
        public KoloQA UploadFile(string FileName, string Id = "file")
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
        /// <returns></returns>
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
            return xpath;
        }


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
        /// Find by CSS Selector then Click Without Scroll Into View
        /// </summary>
        /// <param name="CSSSelector">CSS Selector to find</param>
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
        /// Find by XPath Selector then Click
        /// </summary>
        /// <param name="XPath">XPath to find</param>
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
        /// Find Drop Down By Css Selector then Select the Value Provided
        /// </summary>
        /// <param name="SelectListCSS">CSS Selector of the List</param>
        /// <param name="ValueInList">The Value to select</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <returns></returns>
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
        /// <param name="ValueToType">The value to type into the element</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public KoloQA BrowserStackDownloadVideos()
        {
            KoloControl.GetTestVideos().Wait();
            return this;
        }

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
        /// <returns></returns>
        public KoloQA ScrollIntoViewAndTypeThenTab(IWebElement element, string input)
        {
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
        /// <returns></returns>
        public KoloQA ScrollIntoViewAndType(IWebElement element, string input)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            js.ExecuteScript("window.scroll(" + element.Location.X + "," + (element.Location.Y - 200) + ");");
            js.ExecuteScript("arguments[0].click(); ", element);
            element.SendKeys(input);
            return this;
        }

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
        /// <param name="CssSelector"></param>
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
        /// <param name="Id"></param>
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
        /// <returns></returns>
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

        #endregion

        #region GdsPageIndexing
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