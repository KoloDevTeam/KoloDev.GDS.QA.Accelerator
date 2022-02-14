using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using KoloDev.GDS.QA.Accelerator.Data;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;

namespace KoloDev.GDS.QA.Accelerator.Utility
{
    public class GdsPageModelGenerator
    {
        public async Task<GdsPageModel> GDSPageGeneratorAsync(IWebDriver driver, string element = "", List<string> ignores = null)
        {
            string PageHtml;
            // GDS Page Model Creation
            TestContext.WriteLine("GDS PAGE GENERATOR - Started");
            TestContext.WriteLine("KoloQA: ---GDS PAGE GENERATOR----------------------------------------------------------------------");
            TestContext.WriteLine("KoloQA: -------------------------------------------------------------------------------------------");
            GdsPageModel pageModel = new GdsPageModel();
            if(element.Length > 1)
            {
                IWebElement Iweb = driver.FindElement(By.CssSelector(element));
                PageHtml = Iweb.GetAttribute("innerHTML");

            }
            else
            {
                PageHtml = driver.PageSource;
            }
            
            pageModel = AccordianDetect(PageHtml, pageModel);
            pageModel = await BackLinkDetectAsync(PageHtml, pageModel, ignores);
            pageModel = await BreadCrumbsDetectAsync(PageHtml, pageModel);
            pageModel = await ButttonDetectAsync(PageHtml, pageModel);
            pageModel = await CheckboxDetectAsync(PageHtml, pageModel, ignores);
            pageModel = await DateInputDetectAsync(PageHtml, pageModel);
            pageModel = await DetailsDetectAsync(PageHtml, pageModel);
            pageModel = await RadiosDetectAsync(PageHtml, pageModel);
            pageModel = await TextInputsDetectAsync(PageHtml, pageModel);
            pageModel = await HyperLinksDetectAsync(PageHtml, pageModel);
            pageModel = await SelectsDetectAsync(PageHtml, pageModel, ignores);
            
            // Serialise Page Model and print out to console.
            var pagejson = JsonConvert.SerializeObject(pageModel, Formatting.Indented);
            // Console.WriteLine(pagejson);
            return pageModel;
        }

        public void GetTextFromPage(string PageHtml)
        {
            try
            {
                var pagemaster = new HtmlDocument();
                pagemaster.LoadHtml(PageHtml);
                var document = pagemaster.DocumentNode;
                var nodes = document.SelectNodes("//*[not(self::script or self::style)]/text()[normalize-space()]");

                foreach(var node in nodes)
                {
                    TestContext.WriteLine("Page Text " + node.InnerText);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static int CountHtmlNodes(IEnumerable<HtmlNode> nodes)
        {
            int count = 0;
            foreach (var item in nodes)
            {
                count++;
            }
            return count;
        }

        public GdsPageModel AccordianDetect(string PageHtml, GdsPageModel gDSPage)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            IEnumerable<HtmlNode> nodes = document.QuerySelectorAll("[class=\"govuk-accordion\"]");
            if (nodes != null)
            {
                gDSPage.Accordians = new List<Accordian>();
                foreach (var accordian in nodes)
                {
                    TestContext.WriteLine("KoloQA: ---Accordian Found: " + accordian.Id);
                    Accordian accord = new Accordian();
                    accord.ID = accordian.Id;
                    // Accordian Open All Link
                    try
                    {
                        var openlink = accordian.QuerySelector("button");
                        if (openlink != null)
                        {
                            accord.OpenallLink = true;
                            accord.OpenallLinkClass = openlink.Attributes["class"].Value.ToString();
                            string isactive = openlink.Attributes["aria-expanded"].Value.ToString();
                            if (isactive == "true")
                            {
                                accord.OpenallLinkAriaActive = true;
                            }
                            else
                            {
                                accord.OpenallLinkAriaActive = false;
                            }
                            accord.OpenallLinkText = openlink.InnerText.Trim();
                            TestContext.WriteLine("KoloQA: ---Open All Link Found on Accordian : " + accordian.Id + " With Link Text : " + openlink.InnerText.Trim());
                        }
                        try
                        {
                            int entrynos = 0;
                            IEnumerable<HtmlNode> entries = accordian.QuerySelectorAll(".govuk-accordion__section");
                            entrynos = CountHtmlNodes(entries);
                            TestContext.WriteLine("KoloQA: ---Number of Entries in Accordain: " + entrynos.ToString());
                            accord.Entries = new List<AccordianEntries>();
                            foreach (var entry in entries)
                            {
                                AccordianEntries accordianEntries = new AccordianEntries();
                                accordianEntries.Id = entry.Id;
                                var accordianheader = entry.QuerySelector("button");
                                var innerText = entry.QuerySelector("p");
                                accordianEntries.EntryText = accordianheader.InnerText.Trim();
                                accordianEntries.EntryContent = innerText.InnerText.Trim();
                                accordianEntries.Id = entry.Id;
                                accordianEntries.OtherContentThanTextPresent = innerText.InnerText.Trim().Contains("<");
                                TestContext.WriteLine("KoloQA: ---Accordian Header Text: " + accordianheader.InnerText.Trim());
                                TestContext.WriteLine("KoloQA: ---Inner Text of Accordian: " + innerText.InnerText.Trim());
                                TestContext.WriteLine("KoloQA: ---Inner Text Xpath: " + entry.XPath);
                                TestContext.WriteLine("KoloQA: ---Contains Anything Other Than Text: " + accordianEntries.OtherContentThanTextPresent.ToString());
                                accord.Entries.Add(accordianEntries);
                                // Add Entries to Page Model  
                            }
                            accord.EntryCount = entrynos;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            TestContext.WriteLine("KoloQA: No Entries or Error in retrieving Content of Entries");
                        }
                    }
                    catch (Exception)
                    {
                        TestContext.WriteLine("KoloQA: No Open All Link Found");
                    }
                    // Entries within the Accordian
                    gDSPage.Accordians.Add(accord);
                }
            }
            else
            {
                TestContext.WriteLine("KoloQA: ---No Accordians Found on Page ------------------------------------------------------------");
            }
            return gDSPage;
        }
        public async Task<GdsPageModel> BackLinkDetectAsync(string PageHtml, GdsPageModel gDSPage, List<string> ignores = null)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            HtmlNode node = document.QuerySelector(".govuk-back-link");
            if (node != null)
            {
                if (ignores != null && !ignores.Contains(node.Id))
                {
                    gDSPage.BackLink = true;
                }
                
            }
            return gDSPage;
        }
        public async Task<GdsPageModel> BreadCrumbsDetectAsync(string PageHtml, GdsPageModel gDSPage)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            IEnumerable<HtmlNode> nodes = document.QuerySelectorAll(".govuk-breadcrumbs__list");
            gDSPage.Breadcrumbs = new List<Breadcrumb>();

            if (nodes != null)
            {
                foreach (var crumb in nodes)
                {
                    try
                    {
                        int entrynos = 0;
                        IEnumerable<HtmlNode> entries = crumb.QuerySelectorAll(".govuk-breadcrumbs__list-item");
                        entrynos = CountHtmlNodes(entries);
                        TestContext.WriteLine("KoloQA: ---Number of BreadCrumbs on Page: " + entrynos.ToString());
                        TestContext.WriteLine("Found BreadCrumbs within Page");
                        // accord.Entries = new List<AccordianEntries>();
                        foreach (var entry in entries)
                        {
                            Breadcrumb breadcrumb = new Breadcrumb();
                            var link = entry.QuerySelector("a");
                            if (link != null)
                            {
                                breadcrumb.BreadCrumbText = link.InnerText.Trim();
                                breadcrumb.BreadCrumbLink = link.Attributes["href"].Value.ToString();
                                breadcrumb.PresentPage = false;
                            }
                            else
                            {
                                var open = entry.SelectSingleNode("//li[@aria-current]");
                                breadcrumb.PresentPage = true;
                                breadcrumb.BreadCrumbText = open.InnerText.Trim();
                            }
                            gDSPage.Breadcrumbs.Add(breadcrumb);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return gDSPage;
        }
        public async Task<GdsPageModel> ButttonDetectAsync(string PageHtml, GdsPageModel gDSPage, List<string> ignores = null)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            IEnumerable<HtmlNode> nodes = document.QuerySelectorAll("button");
            gDSPage.Buttons = new List<Button>();
            if (nodes != null)
            {
                foreach (var button in nodes)
                {
                    if (ignores != null && !ignores.Contains(button.Id))
                    {
                        Button but = new Button();
                        try
                        {
                            but.Type = "Default";
                            if (button.InnerText != null)
                            {
                                but.ButtonText = button.InnerText.Trim();
                                TestContext.WriteLine("KoloQA: Button Text: " + button.InnerText.Trim());
                            }
                            else if (button.Attributes["value"].ToString() != null)
                            {
                                but.ButtonText = button.Attributes["value"].ToString();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        try
                        {
                            if (button.Attributes["class"].Value.ToLower().Contains("secondary"))
                            {
                                but.Type = "Secondary";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        try
                        {
                            if (button.Attributes["class"].Value.ToLower().Contains("warning"))
                            {
                                but.Type = "Warning";
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        if (button.Attributes["class"].Value != null && button.Attributes["class"].Value.ToLower().Contains("disabled"))
                        {
                            but.Enabled = false;
                        }
                        else
                        {
                            but.Enabled = true;
                        }
                        try
                        {
                            if (button.Attributes["disabled"].Value != null && button.Attributes["disabled"].Value.ToLower().Contains("disabled"))
                            {
                                but.Enabled = false;
                            }
                        }
                        catch
                        {
                            but.Enabled = true;
                        }
                        try
                        {
                            if (button.Attributes["data-prevent-double-click"].Value != null && button.Attributes["data-prevent-double-click"].Value.ToLower().Contains("true"))
                            {
                                but.PreventDoubleClick = true;
                            }
                        }
                        catch
                        {
                            but.PreventDoubleClick = false;
                        }
                        try
                        {
                            if (button.Attributes["name"].Value != null)
                            {
                                but.Name = button.Attributes["name"].Value;
                            }
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: No Name Value fround for Button: " + button.InnerText.Trim());
                        }
                        try
                        {
                            if (button.Id != null)
                            {
                                but.ID = button.Id;
                            }
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: No ID Value fround for Button: " + button.InnerText.Trim());
                            but.ID = null;
                        }
                        try
                        {
                            if (button.Attributes["form"].Value != null)
                            {
                                but.Form = button.Attributes["form"].Value;
                            }
                        }
                        catch
                        {
                            TestContext.WriteLine("KoloQA: No Form Value fround for Button: " + button.InnerText.Trim());
                            but.Form = null;
                        }
                        gDSPage.Buttons.Add(but);
                    }
                }
            }
            return gDSPage;
        }
        public async Task<GdsPageModel> CheckboxDetectAsync(string PageHtml, GdsPageModel gDSPageModel, List<string> ignores = null)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            bool checkboxes = false;
            try
            {

                var checkboxcheck = document.QuerySelectorAll("input[type='checkbox']");
                int any = CountHtmlNodes(checkboxcheck);
                if (any > 0)
                {
                    checkboxes = true;
                }
                else
                {
                    TestContext.WriteLine("No Checkboxes within Page");
                }
                if (checkboxes == true)
                {
                    gDSPageModel.CheckBoxes = new List<Checkbox>();
                    // See if there is a fieldset containing checkboxes
                    try
                    {
                        string leg = "";
                        string frmhint = "";
                        var validate = document.QuerySelectorAll("input[type='checkbox']");

                        TestContext.WriteLine("Found Checkboxes within Page");
                        foreach (var node in validate)
                        {
                            if (ignores != null && !ignores.Contains(node.Id))
                            {
                                Checkbox chk = new Checkbox();
                                try
                                {
                                    if (leg.Length > 1)
                                    {
                                        chk.Legend = leg;
                                    }
                                    if (frmhint.Length > 1)
                                    {
                                        chk.Fieldsethint = frmhint;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Fieldset for checks 1 " + e.Message);
                                }
                                //Input
                                try
                                {
                                    //ID
                                    chk.Id = node.Id;
                                    TestContext.Write("CHECKBOX ID " + chk.Id);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Fieldset for checks 2 " + e.Message);
                                }
                                try
                                {
                                    //Name
                                    chk.Name = node.Attributes["name"].Value;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Fieldset for checks 3 " + e.Message);
                                }
                                try
                                {
                                    //Label
                                    var label = document.QuerySelector("*[for='" + chk.Id + "']");
                                    chk.Label = label.InnerText.Trim();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Fieldset for checks 4" + e.Message);
                                }
                                gDSPageModel.CheckBoxes.Add(chk);
                            }
                        }
                        return gDSPageModel;

                    }
                    catch (Exception)
                    {
                        TestContext.WriteLine("KoloQA: ---No Fieldset Found enclosing Checkboxes");
                    }
                }
            }

            catch (Exception)
            {

            }
            return gDSPageModel;
        }
        public async Task<GdsPageModel> DateInputDetectAsync(string PageHtml, GdsPageModel gDSPageModel)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            bool dateinput = false;
            gDSPageModel.DateInputs = new List<DateInput>();
            try
            {
                var datein = document.QuerySelectorAll("input[type='number']");
                int any = CountHtmlNodes(datein);
                if (any > 0)
                {
                    dateinput = true;
                    TestContext.WriteLine("Possible Date Field found within Page");
                    TestContext.WriteLine("KoloQA: Possible Date Field found within Page");
                }
                if (dateinput == true)
                {
                    foreach (var node in datein)
                    {
                        DateInput date = new DateInput();
                        if (node.Id.ToLower().Contains("day"))
                        {
                            date.Type = "Day";
                        }
                        if (node.Id.ToLower().Contains("month"))
                        {
                            date.Type = "Month";
                        }
                        if (node.Id.ToLower().Contains("year"))
                        {
                            date.Type = "Year";
                        }
                        if (node.Name.ToLower().Contains("day"))
                        {
                            date.Type = "Day";
                        }
                        if (node.Name.ToLower().Contains("month"))
                        {
                            date.Type = "Month";
                        }
                        if (node.Name.ToLower().Contains("year"))
                        {
                            date.Type = "Year";
                        }
                        date.Id = node.Id;
                        gDSPageModel.DateInputs.Add(date);
                    }
                }
                else
                {
                    bool dateinput2 = false;
                    var datein2 = document.QuerySelectorAll("input[id*='date']");
                    int any2 = CountHtmlNodes(datein2);
                    if (any2 > 0)
                    {
                        dateinput2 = true;
                        TestContext.WriteLine("Possible Date Field found within Page by label in Attribute");
                        TestContext.WriteLine("KoloQA: Possible Date Field found within Page by label in Attribute");
                    }
                    if (dateinput2 == true)
                    {
                        foreach (var node in datein2)
                        {
                            DateInput date = new DateInput();
                            if (node.Id.ToLower().Contains("day"))
                            {
                                date.Type = "Day";
                            }
                            if (node.Id.ToLower().Contains("month"))
                            {
                                date.Type = "Month";
                            }
                            if (node.Id.ToLower().Contains("year"))
                            {
                                date.Type = "Year";
                            }
                            if (node.Name.ToLower().Contains("day"))
                            {
                                date.Type = "Day";
                            }
                            if (node.Name.ToLower().Contains("month"))
                            {
                                date.Type = "Month";
                            }
                            if (node.Name.ToLower().Contains("year"))
                            {
                                date.Type = "Year";
                            }
                            date.Id = node.Id;
                            gDSPageModel.DateInputs.Add(date);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return gDSPageModel;
        }
        public async Task<GdsPageModel> DetailsDetectAsync(string PageHtml, GdsPageModel gDSPageModel)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            IEnumerable<HtmlNode> nodes = document.QuerySelectorAll("details");
            if (nodes != null)
            {
                gDSPageModel.Details = new List<Detail>();
                foreach (var detail in nodes)
                {
                    HtmlNode summary = detail.QuerySelector("summary > span");
                    TestContext.WriteLine("---Details Found: " + summary.InnerText);
                    HtmlNode detailled = detail.QuerySelector("div");
                    Detail detailed = new Detail();
                    detailed.ShortText = summary.InnerText.Trim();
                    detailed.Description = detailled.InnerText.Trim();
                    Console.WriteLine(summary.InnerText.Trim());
                    Console.WriteLine(detailled.InnerText.Trim());
                }
            }
            return gDSPageModel;
        }
        public GdsPageModel ErrorMessagesDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel SummaryErrorsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel FileUploadsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel FooterPresentDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel HeaderPresentDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel HeaderDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel InsetTextsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel PanelsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel PhaseDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public async Task<GdsPageModel> RadiosDetectAsync(string PageHtml, GdsPageModel gDSPageModel)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            bool radios = false;
            try
            {

                var radiocheck = document.QuerySelectorAll("input[type='radio']");
                int any = CountHtmlNodes(radiocheck);
                if (any > 0)
                {
                    radios = true;
                    TestContext.WriteLine("No Radio Buttons within Page");
                }
                if (radios == true)
                {
                    gDSPageModel.Radios = new List<Radio>();
                    // See if there is a fieldset containing checkboxes
                    try
                    {
                        string leg = "";
                        string frmhint = "";
                        var validate = document.QuerySelectorAll("input[type=\"radio\"]");

                        TestContext.WriteLine("Found Radio Buttons within Page");
                        try
                        {
                            var legend = document.QuerySelector("fieldset > legend");
                            TestContext.WriteLine("Radio Buttons Fieldset Legend: " + legend.InnerText.Trim());
                            TestContext.WriteLine("KoloQA: Radio Buttons Fieldset Legend: " + legend.InnerText.Trim());
                            leg = legend.InnerText.Trim();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            TestContext.WriteLine("KoloQA: ---No Legend found for Radio Buttons Fieldset.");
                        }
                        try
                        {
                            var hint = document.QuerySelector("fieldset > span[class='govuk-hint']");
                            TestContext.WriteLine("Radio Buttons Fieldset Hint: " + hint.InnerText.Trim());
                            TestContext.WriteLine("KoloQA: Radio Buttons Fieldset Hint: " + hint.InnerText.Trim());
                            frmhint = hint.InnerText.Trim();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        foreach (var node in validate)
                        {
                            Radio rad = new Radio();
                            try
                            {
                                if (leg.Length > 1)
                                {
                                    rad.Legend = leg;
                                }
                                if (frmhint.Length > 1)
                                {
                                    rad.Fieldsethint = frmhint;
                                }
                            }
                            catch (Exception)
                            {

                            }
                            //Input
                            try
                            {
                                //ID
                                rad.Id = node.Id;
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                //Name
                                rad.Name = node.Attributes["name"].Value;
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                //Label
                                var label = document.QuerySelector("*[for='" + rad.Id + "']");
                                rad.Label = label.InnerText.Trim();
                            }
                            catch (Exception)
                            {

                            }
                            gDSPageModel.Radios.Add(rad);
                        }
                        return gDSPageModel;

                    }
                    catch (Exception)
                    {
                        TestContext.WriteLine("KoloQA: ---No Fieldset Found enclosing Checkboxes");
                    }
                }
            }

            catch (Exception)
            {

            }
            return gDSPageModel;
        }

        public async Task<GdsPageModel> SelectsDetectAsync(string PageHtml, GdsPageModel gDSPageModel, List<string> ignores = null)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            bool options = false;
            try
            {

                var selects = document.QuerySelectorAll("select");
                int any = CountHtmlNodes(selects);
                if (any > 0)
                {
                    options = true;
                    TestContext.WriteLine("Select Drop Downs found within Page");
                }
                if (options == true)
                {
                    gDSPageModel.Selects = new List<Select>();

                    foreach (var selector in selects)
                    {
                        if (ignores != null && !ignores.Contains(selector.Id))
                        {
                            Select select = new Select();
                            try
                            {
                                select.Id = selector.Id;
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                var label = document.QuerySelector("*[for='" + select.Id + "']");
                                select.Label = label.InnerText.Trim();
                            }
                            catch (Exception)
                            {
                                select.Label = "";
                            }
                            try
                            {
                                select.Name = selector.Name;
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                select.Options = new List<SelectOption>();
                                var choicelist = document.QuerySelectorAll("#" + select.Id + " > option");
                                foreach (var opt in choicelist)
                                {
                                    SelectOption optional = new SelectOption();
                                    try
                                    {
                                        optional.Value = opt.Attributes["value"].Value;
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    try
                                    {
                                        optional.Text = opt.InnerText.Trim();
                                        TestContext.WriteLine(opt.InnerText);

                                    }
                                    catch (Exception)
                                    {

                                    }
                                    select.Options.Add(optional);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            if (select.Label.Length > 1)
                            {
                                string dropdownvalues = "";
                                foreach (SelectOption opt in select.Options)
                                {
                                    dropdownvalues += opt.Text.ToString() + "|";
                                }
                                dropdownvalues = dropdownvalues.Remove(dropdownvalues.Length - 1);
                            }
                            gDSPageModel.Selects.Add(select);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return gDSPageModel;
        }
        public GdsPageModel SkipLinkDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel SummaryListsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel TablesDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel TabsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public GdsPageModel TagsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public async Task<GdsPageModel> TextInputsDetectAsync(string PageHtml, GdsPageModel gDSPageModel)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            bool textcheck = false;
            try
            {
                var textinputs = document.QuerySelectorAll("input[class='govuk-input'],input[type=color],input[type=date],input[type=datetime-local],input[type=email],input[type=file],input[type=image],input[type=month],input[type=number],input[type=password],input[type=range],input[type=reset],input[type=search],input[type=submit],input[type=tel],input[type=text],input[type=time],input[type=url],input[type=week]");

                int any = CountHtmlNodes(textinputs);
                if (any > 0)
                {
                    textcheck = true;
                    TestContext.WriteLine("Text Inputs within Page");
                }
                if (textcheck == true)
                {
                    gDSPageModel.TextInputs = new List<TextInput>();
                    // See if there is a fieldset containing checkboxes
                    try
                    {
                        string leg = "";
                        string frmhint = "";
                        var validate = document.QuerySelectorAll("input[class='govuk-input'],input[type=color],input[type=date],input[type=datetime-local],input[type=email],input[type=file],input[type=image],input[type=month],input[type=number],input[type=password],input[type=range],input[type=reset],input[type=search],input[type=submit],input[type=tel],input[type=text],input[type=time],input[type=url],input[type=week]");
                        foreach (var node in validate)
                        {
                            TextInput txt = new TextInput();
                            try
                            {
                                if (leg.Length > 1)
                                {
                                    txt.Legend = leg;
                                }
                                if (frmhint.Length > 1)
                                {
                                    txt.Fieldsethint = frmhint;
                                }
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                txt.Id = node.Id;
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                txt.CSSSelector = "#" + node.Id;
                            }
                            catch
                            {

                            }
                            try
                            {
                                //Name
                                txt.Name = node.Attributes["name"].Value;
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                //Label
                                var label = document.QuerySelector("*[for='" + txt.Id + "']");
                                txt.Label = label.InnerText.Trim();
                            }
                            catch (Exception)
                            {

                            }
                            try
                            {
                                //Label
                                txt.InputType = node.Attributes["type"].Value;
                            }
                            catch (Exception)
                            {

                            }
                            if (txt.Label.Contains("Day") || txt.Label.Contains("Month") || txt.Label.Contains("Year"))
                            {
                                TestContext.WriteLine("Matched in Date Field not Number Input. Adding to Page Model Regardless");
                            }
                            gDSPageModel.TextInputs.Add(txt);
                        }
                    }
                    catch (Exception)
                    {
                        TestContext.WriteLine("KoloQA: ---No Fieldset Found enclosing Inputs");
                    }
                }
            }

            catch (Exception)
            {

            }
            return gDSPageModel;
        }

        //public async Task<GdsPageModel> TextAreasDetect(string PageHtml, GdsPageModel gDSPageModel)
        //{
        //    var pagemaster = new HtmlDocument();
        //    pagemaster.LoadHtml(PageHtml);
        //    var document = pagemaster.DocumentNode;
        //    bool textcheck = false;
        //    try
        //    {
        //        var textinputs = document.QuerySelectorAll("textarea");

        //        int any = CountHtmlNodes(textinputs);
        //        if (any > 0)
        //        {
        //            textcheck = true;
        //            TestContext.WriteLine("Text Areas within Page");
        //        }
        //        if (textcheck == true)
        //        {
        //            gDSPageModel.TextAreas = new List<TextAreas>();

        //        }

        //        return gDSPageModel;
        //    }
        //    catch
        //    {
        //        TestContext.WriteLine("KoloQA: Failure to process TextAreas");
        //    }
        //}
        public GdsPageModel WarningsDetect(string PageHtml, GdsPageModel gDSPageModel)
        {
            return gDSPageModel;
        }
        public async Task<GdsPageModel> HyperLinksDetectAsync(string PageHtml, GdsPageModel gDSPageModel)
        {
            var pagemaster = new HtmlDocument();
            pagemaster.LoadHtml(PageHtml);
            var document = pagemaster.DocumentNode;
            IEnumerable<HtmlNode> links = document.QuerySelectorAll("a");
            gDSPageModel.HyperLinks = new List<HyperLink>();
            try
            {
                foreach (HtmlNode value in links)
                {
                    bool writeit = true;
                    HyperLink link = new HyperLink();
                    try
                    {
                        link.Text = value.InnerText.Trim();
                        TestContext.WriteLine("KoloQA: Link Text:  " + value.InnerText.Trim());
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        link.Id = value.Id;
                        TestContext.WriteLine("KoloQA: Link Id:  " + value.Id);
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        link.XPath = value.XPath.ToString();
                        TestContext.WriteLine("KoloQA: XPath:      " + value.XPath.ToString());
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        link.Href = value.Attributes["href"].Value.ToString();
                        TestContext.WriteLine("KoloQA: Href        " + value.Attributes["href"].Value.ToString());

                        link.IsRelative = IsAbsoluteUrl(value.Attributes["href"].Value.ToString());
                        bool IsRelative = IsAbsoluteUrl(value.Attributes["href"].Value.ToString());

                        link.IsParameter = IsParameter(value.Attributes["href"].Value.ToString());
                        bool IsParam = IsParameter(value.Attributes["href"].Value.ToString());

                        TestContext.WriteLine("KoloQA: IsRelative  " + IsRelative.ToString().ToLower());
                        TestContext.WriteLine("KoloQA: IsParameter " + IsParam.ToString().ToLower());
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        if (value.Attributes["target"].Value.ToString() != null)
                        {
                            if (value.Attributes["target"].Value.ToString().Contains("blank"))
                            {
                                writeit = false;
                                TestContext.WriteLine("KoloQA: Detected Blank Assignment: " + value.Attributes["target"].Value.ToString());
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    try
                    {
                        if (value.Attributes["href"].Value.ToString() != null)
                        {
                            if (value.Attributes["href"].Value.ToString().Contains("lang"))
                            {
                                writeit = false;
                                TestContext.WriteLine("KoloQA: Detected Language Switcher: " + value.Attributes["href"].Value.ToString());
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        if (writeit == true)
                        {
                            if (link.Text.Contains("Skip to main content") || link.Text.Contains("Find out more about cookies") || link.Text.Contains("Cookies")
                                || link.Text.Contains("GOV.UK") || link.Text.Contains("Privacy notice") || link.Text.Contains("Terms and conditions")
                                || link.Text.Contains("Cookies notice") || link.Text.Contains("Open Government Licence")
                                || link.Text.Contains("© Crown copyright") || link.Text.Contains("Help") || link.Text.Contains("help"))
                            {
                                TestContext.WriteLine("KoloQA: Ignoring " + link.Text + "");

                            }
                            else
                            {
                                bool sent = false;
                                if (link.Text == "Back")
                                {
                                    link.Id = "Back";
                                    sent = true;
                                }
                                if (link.Text.ToLower() == "start now" && (sent == false))
                                {
                                    sent = true;
                                }
                                if (link.Id.Length > 1 && (sent == false))
                                {
                                    sent = true;
                                }
                                if (link.Href.Length > 1 && (sent == false))
                                {
                                    sent = true;
                                }
                                if (link.XPath.Length > 1 && (sent == false))
                                {
                                    sent = true;
                                }
                                gDSPageModel.HyperLinks.Add(link);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine(ex.Message);
            }

            return gDSPageModel;
        }
        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Relative, out result);
        }
        public static bool IsParameter(string url)
        {
            bool result = url.Trim().StartsWith("?");
            return result;
        }
    }
}
