// ***********************************************************************
// Assembly         : KoloDev.GDS.QA.Accelerator
// Author           : KoloDev
// Created          : 02-07-2022
//
// Last Modified By : KoloDev
// Last Modified On : 09-27-2022
// ***********************************************************************
// <copyright file="KoloControl.cs" company="KoloDev Ltd.">
//     Copyright © 2022 KoloDev Ltd. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using CsCodeGenerator;
using CsCodeGenerator.Enums;
using Flurl.Http;
using Humanizer;
using KoloDev.GDS.QA.Accelerator.Data;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using static KoloDev.GDS.QA.Accelerator.Data.BrowserStackModels;

namespace KoloDev.GDS.QA.Accelerator.Selenium
{
    /// <summary>
    /// KoloQA Control Class
    /// </summary>
    public static class KoloControl
    {
        /// <summary>
        /// Write Output Messages to the Console.
        /// </summary>
        /// <param name="write">Content to write</param>
        public static void KoloTestWriteToLog(string write)
        {
            TestContext.WriteLine(write);
        }

        /// <summary>
        /// Generates the page class.
        /// </summary>
        /// <param name="ClassName">Name of the class.</param>
        /// <param name="pageModel">The page model.</param>
        public static void GeneratePageClass(string ClassName, GdsPageModel pageModel)
        {
            //Prep Directory and Top Levels
            if (!Directory.Exists("../../../ComplexPages")) Directory.CreateDirectory("../../../ComplexPages");
            var usingDirectives = new List<string>
            {
                "KoloDev.GDS.QA.Accelerator.Data;",
                "System.ComponentModel;"
            };
            string fileNameSpace = "KoloDev.GDS.QA.Accelerator.Pages." + ClassName + "";
            ClassModel model = new ClassModel(ClassName);
            model.SingleKeyWord = KeyWord.Static;

            FileModel modelFile = new FileModel(ClassName);
            modelFile.LoadUsingDirectives(usingDirectives);
            modelFile.Namespace = fileNameSpace;
            modelFile.Classes.Add(model);

            // Add description to the class
            var descriptionAttribute = new AttributeModel("Description")
            {
                SingleParameter = new Parameter(@"""KoloQA GDS Page Model: " + ClassName + "\"")
            };
            model.AddAttribute(descriptionAttribute);

            // Getting all properties for the class
            var methods = GenerateInputMethods(pageModel, ClassName);
            model.Methods = methods;
            // Generate Class Files
            CsGenerator csGenerator = new CsGenerator();
            csGenerator.Files.Add(modelFile);
            csGenerator.OutputDirectory = "../../../ComplexPages";
            csGenerator.CreateFiles();
            Console.Write(modelFile);
        }

        /// <summary>
        /// Generates the simple page class.
        /// </summary>
        /// <param name="ClassName">Name of the class.</param>
        /// <param name="pageModel">The page model.</param>
        public static void GenerateSimplePageClass(string ClassName, GdsPageModel pageModel)
        {
            //Prep Directory and Top Levels
            if (!Directory.Exists("../../../KoloPages")) Directory.CreateDirectory("../../../KoloPages");
            var usingDirectives = new List<string>
            {
                "KoloDev.GDS.QA.Accelerator.Data;",
                "System.ComponentModel;"
            };
            string fileNameSpace = "KoloDev.GDS.QA.Accelerator.Pages.Kolo";
            ClassModel model = new ClassModel(ClassName + "Kolo");
            model.SingleKeyWord = KeyWord.Static;

            FileModel modelFile = new FileModel(ClassName + "Simple");
            modelFile.LoadUsingDirectives(usingDirectives);
            modelFile.Namespace = fileNameSpace;
            modelFile.Classes.Add(model);

            // Add description to the class
            var descriptionAttribute = new AttributeModel("Description")
            {
                SingleParameter = new Parameter(@"""KoloQA GDS Page Model: " + ClassName + "\"")
            };
            model.AddAttribute(descriptionAttribute);

            // Getting all properties for the class
            var methods = GenerateKoloMethods(pageModel, ClassName);
            model.Methods = methods;
            // Generate Class Files
            CsGenerator csGenerator = new CsGenerator();
            csGenerator.Files.Add(modelFile);
            csGenerator.OutputDirectory = "../../../KoloPages";
            csGenerator.CreateFiles();
            Console.Write(modelFile);
        }

        /// <summary>
        /// Generates the input methods.
        /// </summary>
        /// <param name="gdsPageModel">The GDS page model.</param>
        /// <param name="ClassName">Name of the class.</param>
        /// <returns>List&lt;Method&gt;.</returns>
        public static List<Method> GenerateInputMethods(GdsPageModel gdsPageModel, string ClassName)
        {
            List<Method> methods = new List<Method>();
            try
            {
                if (gdsPageModel.TextInputs != null)
                {
                    foreach (TextInput input in gdsPageModel.TextInputs)
                    {
                        if (input.CSSSelector != null && input.Label != null)
                        {
                            string label = Checker(input.Label);

                            var meth = new Method("KoloQA", ClassName + "_TypeValueByCSS_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo"), new Parameter("string", "InputText"), new Parameter("BrowserStackBrowsers", "client") },
                                BodyLines = new List<string> { "kolo.FindByCSSSelectorThenType(\"" + input.CSSSelector + "\", InputText, client);"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);

                            var clearmeth = new Method("KoloQA", ClassName + "_ClearValueById_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.FindByCssSelectorClearInputField(\"" + input.Id + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(clearmeth);
                        }
                        if (input.Id != null && input.Label != null)
                        {
                            string label = Checker(input.Id);

                            var meth = new Method("KoloQA", ClassName + "_TypeValueById_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo"), new Parameter("string", "InputText") },
                                BodyLines = new List<string> { "kolo.FindByIdThenType(\"" + input.Id + "\", InputText);"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);

                            var clearmeth = new Method("KoloQA", ClassName + "_ClearValueByCss_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.FindByIdClearInputField(\"" + input.Id + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(clearmeth);
                        }
                    }
                }
                if (gdsPageModel.HyperLinks != null)
                {
                    foreach (HyperLink link in gdsPageModel.HyperLinks)
                    {
                        if (link.Text != null && link.XPath != null && !(link.XPath.Contains("header")))
                        {
                            string label = Checker(link.Text);

                            var meth = new Method("KoloQA", ClassName + "_ClickLink_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.ClickLinkByLinkText(\"" + link.Text.Trim().Replace("\r\n", "") + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.Buttons != null)
                {
                    foreach (Button btn in gdsPageModel.Buttons)
                    {
                        if (btn.ButtonText != null)
                        {
                            string label = Checker(btn.ButtonText);

                            var meth = new Method("KoloQA", ClassName + "_ClickButton_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.ClickLinkByLinkText(\"" + btn.ButtonText + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.Selects != null)
                {
                    foreach (Select select in gdsPageModel.Selects)
                    {
                        if (select.Id != null && select.Options != null)
                        {
                            int optionNumber = 0;
                            foreach (SelectOption option in select.Options)
                            {
                                string label = Checker(select.Id);
                                string optionlabel = Checker(select.Options[optionNumber].Text);

                                var meth = new Method("KoloQA", ClassName + "_SelectOptionFrom_" + label + "_" + optionlabel.Trim())
                                {
                                    AccessModifier = AccessModifier.Public,
                                    KeyWords = new List<KeyWord> { KeyWord.Static },
                                    Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                    BodyLines = new List<string> { "kolo.DropDownByIdThenSelectValue(\"" + select.Id + "\", \"" + select.Options[optionNumber].Text + "\");"
                                                            , "return kolo; "}
                                };
                                methods.Add(meth);
                                optionNumber++;
                            }

                        }
                        if (select.Options == null)
                        {
                            string label = Checker(select.Id);

                            var meth = new Method("KoloQA", ClassName + "_SelectOptionFrom_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo"), new Parameter("string", "SelectValue") },
                                BodyLines = new List<string> { "kolo.DropDownByIdThenSelectValue(\"" + select.Id + "\", \" string SelectValue \");"
                                                        , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.CheckBoxes != null)
                {
                    foreach (Checkbox check in gdsPageModel.CheckBoxes)
                    {
                        if (check.Label != null)
                        {
                            string label = Checker(check.Label);

                            var meth = new Method("KoloQA", ClassName + "_ClickCheckbox_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.ClickById(\"" + check.Id + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.TextOnPage != null)
                {
                    foreach (string text in gdsPageModel.TextOnPage)
                    {
                            string label = Checker(text);

                            var meth = new Method("KoloQA", ClassName + "_WaitForText_" + label)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.WaitForText(\"" + text + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var final = methods.GroupBy(m => m.Name).Select(n => n.First()).ToList();
            return final;
        }

        /// <summary>
        /// Generates the kolo methods.
        /// </summary>
        /// <param name="gdsPageModel">The GDS page model.</param>
        /// <param name="ClassName">Name of the class.</param>
        /// <returns>List&lt;Method&gt;.</returns>
        public static List<Method> GenerateKoloMethods(GdsPageModel gdsPageModel, string ClassName)
        {
            List<Method> methods = new List<Method>();
            try
            {
                if (gdsPageModel.TextInputs != null)
                {
                    foreach (TextInput input in gdsPageModel.TextInputs)
                    {
                        if (input.CSSSelector != null && input.Label != null && input.Id != null)
                        {
                            string label = Checker(input.Label);

                            var meth = new Method("KoloQA", label + "_Type_" + ClassName)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo"), new Parameter("string", "InputText"), new Parameter("BrowserStackBrowsers", "client") },
                                BodyLines = new List<string> { "try \r\n" +
                            "            { \r\n" +
                            "               kolo.FindByIdThenType(\"" + input.Id + "\", InputText); \r\n" +
                            "               return kolo; \r\n" +
                            "            }\r\n" +
                            "            catch\r\n" +
                            "            {\r\n" +
                            "               kolo.FindByCSSSelectorThenType(\"" + input.CSSSelector + "\", InputText, client);\r\n" +
                            "               return kolo; \r\n" +
                            "            }"}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.HyperLinks != null)
                {
                    foreach (HyperLink link in gdsPageModel.HyperLinks)
                    {
                        if (link.Text != null && link.XPath != null && !(link.XPath.Contains("header")))
                        {
                            string label = Checker(link.Text.Trim());
                            var meth = new Method("KoloQA", "Link_" + label + "_Click_" + ClassName)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.ClickLinkByLinkText(\"" + link.Text.Trim().Replace("\r\n", "") + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.Buttons != null)
                {
                    foreach (Button btn in gdsPageModel.Buttons)
                    {
                        if (btn.ButtonText != null)
                        {
                            string label = Checker(btn.ButtonText);

                            var meth = new Method("KoloQA", label + "_Click_" + ClassName)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.ClickLinkByLinkText(\"" + btn.ButtonText + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.Selects != null)
                {
                    foreach (Select select in gdsPageModel.Selects)
                    {
                        if (select.Id != null && select.Options != null)
                        {
                            int optionNumber = 0;
                            foreach (SelectOption option in select.Options)
                            {
                                string label = Checker(select.Id);
                                string optionlabel = Checker(select.Options[optionNumber].Text);

                                var meth = new Method("KoloQA", "Option" + optionlabel + "_Select_" + ClassName)
                                {
                                    AccessModifier = AccessModifier.Public,
                                    KeyWords = new List<KeyWord> { KeyWord.Static },
                                    Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                    BodyLines = new List<string> { "kolo.DropDownByIdThenSelectValue(\"" + select.Id + "\", \"" + select.Options[optionNumber].Text + "\");"
                                                            , "return kolo; "}
                                };
                                methods.Add(meth);
                                optionNumber++;
                            }

                        }
                        if (select.Options == null)
                        {
                            string label = Checker(select.Id);

                            var meth = new Method("KoloQA", label + "_SelectOption_" + ClassName)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo"), new Parameter("string", "SelectValue") },
                                BodyLines = new List<string> { "kolo.DropDownByIdThenSelectValue(\"" + select.Id + "\", \" string SelectValue \");"
                                                        , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.CheckBoxes != null)
                {
                    foreach (Checkbox check in gdsPageModel.CheckBoxes)
                    {
                        if (check.Label != null)
                        {
                            string label = Checker(check.Label);

                            var meth = new Method("KoloQA", label + "_Click_" + ClassName)
                            {
                                AccessModifier = AccessModifier.Public,
                                KeyWords = new List<KeyWord> { KeyWord.Static },
                                Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                                BodyLines = new List<string> { "kolo.ClickById(\"" + check.Id + "\");"
                                                            , "return kolo; "}
                            };
                            methods.Add(meth);
                        }
                    }
                }
                if (gdsPageModel.TextOnPage != null)
                {
                    foreach (string text in gdsPageModel.TextOnPage)
                    {
                        string label = Checker(text);

                        var meth = new Method("KoloQA", label + "_WaitForText_" + ClassName)
                        {
                            AccessModifier = AccessModifier.Public,
                            KeyWords = new List<KeyWord> { KeyWord.Static },
                            Parameters = new List<Parameter> { new Parameter("this KoloQA", "kolo") },
                            BodyLines = new List<string> { "kolo.WaitForText(\"" + text + "\");"
                                                            , "return kolo; "}
                        };
                        methods.Add(meth);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            var final = methods.GroupBy(m => m.Name).Select(n => n.First()).ToList();
            return final;
        }

        /// <summary>
        /// Checkers the specified inbound.
        /// </summary>
        /// <param name="inbound">The inbound.</param>
        /// <returns>System.String.</returns>
        public static string Checker(string inbound)
        {
            string clean = Regex.Replace(inbound, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
            string trim = clean.Replace(" ", "");
            string trim2 = trim.Replace(".", string.Empty);
            bool ismatch = Regex.IsMatch(trim2, @"^\d+");
            try
            {
                if (ismatch)
                {
                    int numeric = int.Parse(trim2);
                    trim2 = "Page_" + numeric.ToWords().Underscore();
                }
            }
            catch
            {
                TestContext.WriteLine("String and Integers");
            }
            try
            {
                if (Regex.IsMatch(trim2, @"^\d"))
                {
                    string final = trim2.Insert(0, "Attr_");
                    TestContext.WriteLine("MATCHED ON STARTS WITH NUM " + trim2);
                    return final;
                }
            }
            catch
            {
                TestContext.WriteLine("Appended");
            }
            return trim2;
        }

        /// <summary>
        /// Gets the screen shot base64 string.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <returns>System.String.</returns>
        public static string GetScreenShotBase64String(IWebDriver driver)
        {
            byte[] screenshot = null;
            try
            {
                screenshot = ((ITakesScreenshot)driver).GetScreenshot().AsByteArray;
            }
            catch (Exception e)
            {
                TestContext.WriteLine("KoloQA: Unable to take screenshot " + e.Message);
            }
            // From byte array to string
            string base64String = Convert.ToBase64String(screenshot);
            return base64String;
        }

        /// <summary>
        /// Takes the screenshot.
        /// </summary>
        /// <param name="Driver">The driver.</param>
        /// <param name="filename">The filename.</param>
        public static void TakeScreenshot(IWebDriver Driver, string filename)
        {
            Screenshot ss = ((ITakesScreenshot)Driver).GetScreenshot();
            string Screenshot = filename;
            ss.SaveAsFile(Screenshot);
        }

        /// <summary>
        /// Gets the test videos.
        /// </summary>
        public static async Task GetTestVideos()
        {
            try
            {
                List<Build> builds;
                string username = TestContext.Parameters["BrowserStackUserName"];
                string password = TestContext.Parameters["BrowserStackKey"];
                string url = "https://api.browserstack.com/automate/builds.json?&limit=1";
                builds = await url.WithBasicAuth(username, password).GetJsonAsync<List<Build>>();
                AutomationBuild latest = (AutomationBuild)builds.Where(e => e.AutomationBuild.Name == TestContext.Parameters["BuildName"]);
                string sessionsurl = "https://api.browserstack.com/automate/builds/" + latest.HashedId + "/sessions.json";
                List<Sessions> sessions = await sessionsurl.WithBasicAuth(username, password).GetJsonAsync<List<Sessions>>();
                foreach (var session in sessions)
                {
                    try
                    {
                        string vidurl = session.AutomationSession.VideoUrl.ToString();
                        var array = new[] { session.AutomationSession.Name.Trim(), session.AutomationSession.Os.ToUpper(), session.AutomationSession.OsVersion, session.AutomationSession.Device, session.AutomationSession.Browser, session.AutomationSession.BrowserVersion, session.AutomationSession.Status };
                        string filename = string.Join("-", array.Where(s => !string.IsNullOrEmpty(s))).Trim();
                        await vidurl.WithBasicAuth(username, password).DownloadFileAsync("TestResults/", filename + ".mp4");
                        string json = JsonSerializer.Serialize(session);
                        File.WriteAllText("TestResults/" + filename + ".json", json);

                    }
                    catch (Exception e)
                    {
                        TestContext.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                TestContext.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Strings the translater.
        /// </summary>
        /// <param name="translate">The translate.</param>
        /// <returns>System.String.</returns>
        public static string StringTranslater(string translate)
        {
            if (translate.ToLower() == "todaysday")
            {
                translate = DateTime.Today.ToString("dd");
            }
            if (translate.ToLower() == "todaysmonth")
            {
                translate = DateTime.Today.ToString("MM");
            }
            if (translate.ToLower() == "thisyear")
            {
                translate = DateTime.Today.ToString("yyyy");
            }
            if (translate.ToLower() == "tomorrowsday")
            {
                translate = DateTime.Today.AddDays(1).ToString("dd");
            }
            if (translate.ToLower() == "tomorrowsmonth")
            {
                translate = DateTime.Today.AddDays(1).ToString("MM");
            }
            if (translate.ToLower() == "generatereference")
            {
                translate = DateTime.Now.ToString("ddMMYY") + RandomStringGenerator().ToUpper();
            }
            return translate;
        }

        /// <summary>
        /// Randoms the string generator.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string RandomStringGenerator()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
    }
}
