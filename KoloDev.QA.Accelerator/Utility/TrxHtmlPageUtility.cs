using System.Text;
using TRexLib;

namespace KoloDev.GDS.QA.Accelerator.Utility;

/// <summary>
/// Utility to process a TRX file to a GDS HTML report page
/// </summary>
public static class TrxHtmlPageUtility
{
    /// <summary>
    /// TRX file to HTML via it's FileInfo
    /// </summary>
    /// <param name="file"></param>
    public static void TrxToHtml(this FileInfo file)
    {
        ProcessTrx(file);
    }

    /// <summary>
    /// TRX file to HTML via it's location
    /// </summary>
    /// <param name="fileLocation"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public static async Task TrxToHtmlAsync(string fileLocation)
    {
        TimeSpan span = TimeSpan.FromMinutes(1);
        await WaitForFile(fileLocation, span);
        var file = new FileInfo(fileLocation);
        ProcessTrx(file);
    }

    private static async Task<bool> WaitForFile(string path, TimeSpan timeout)
    {
        DateTimeOffset timeoutAt = DateTimeOffset.UtcNow + timeout;
        while (true)
        {
            if (File.Exists(path)) return true;
            if (DateTimeOffset.UtcNow >= timeoutAt) return false;
            await Task.Delay(10);
        }
    }

    private static void ProcessTrx(FileInfo file)
    {
        var trxResultSet = file.Parse();

        var template = File.ReadAllText(@"Template\GdsPageTemplate.html");

        template = template.Replace("##TESTNAME##", trxResultSet.Name);
        template = template.Replace("##TESTRUNNAME##", trxResultSet.TestRunName);
        template = template.Replace("##CREATEDTIME##", trxResultSet.CreatedTime.ToString("T"));
        template = template.Replace("##QUEUEDTIME##", trxResultSet.QueuedTime.ToString("T"));
        template = template.Replace("##COMPLETEDTIME##", trxResultSet.CompletedTime.ToString("T"));
        template = template.Replace("##TOTALTESTRUNS##", trxResultSet.Count.ToString());
        template = template.Replace("##PASSEDRESULTAMMOUNT##", trxResultSet.Passed.Count.ToString());
        template = template.Replace("##FAILEDRESULTAMMOUNT##", trxResultSet.Failed.Count.ToString());
        template = template.Replace("##NOTEXECUTEDRESULTAMMOUNT##", trxResultSet.NotExecuted.Count.ToString());
        template = template.Replace("##PASSEDRESULTS##", CreateResultSetAccordion(trxResultSet.Passed, TestOutcome.Passed));
        template = template.Replace("##FAILEDRESULTS##", CreateResultSetAccordion(trxResultSet.Failed, TestOutcome.Failed));
        template = template.Replace("##NOTEXECUTEDRESULTS##", CreateResultSetAccordion(trxResultSet.NotExecuted, TestOutcome.NotExecuted));

        if (!Directory.Exists("TestResults")) Directory.CreateDirectory("TestResults");
        File.WriteAllText(@"TestResults\" + Path.GetFileNameWithoutExtension(file.Name) + ".html", template);
    }

    private static string GetGdsTagForOutcome(TestOutcome outcome)
    {
        return outcome switch
        {
            TestOutcome.Passed => "govuk-tag--green",
            TestOutcome.Failed => "govuk-tag--red",
            TestOutcome.NotExecuted => "govuk-tag--grey",
            TestOutcome.Inconclusive => "govuk-tag--orange",
            TestOutcome.Timeout => "govuk-tag--red",
            TestOutcome.Pending => "",
            _ => "govuk-tag--grey",
        };
    }

    private static string CreateResultSetAccordion(IReadOnlyCollection<TestResult> testResults, TestOutcome type)
    {
        // If there are no results then we need to return an inset text to inform
        if (testResults.Count == 0)
            return
                $@"<div class=""govuk-inset-text"">There are no {AddSpacesToSentence(type.ToString(), true)} results to show.</div>";

        // Create string builder and add open to the accordion
        var content = new StringBuilder();
        content.Append($"<div class=\"govuk-accordion\" data-module=\"govuk-accordion\" id=\"accordion-test-result-{type.ToString()}\">");

        // Create accordion panes
        foreach (var result in testResults)
            content.AppendLine($@"<div class=""govuk-accordion__section "">
                                        <div class=""govuk-accordion__section-header"">
                                        <h2 class=""govuk-accordion__section-heading"">
                                            <span class=""govuk-accordion__section-button"" id=""accordion-default-heading-1"">
                                                {result.TestName}
                                                <span class=""govuk-tag {GetGdsTagForOutcome(result.Outcome)}"" style=""float:right;margin-right:15px;"">{result.Outcome}</span>
                                                <br>
                                                <small>{result.Duration}</small>
                                            </span>
                                        </h2>
                                        </div>
                                        <div id=""accordion-default-content-1"" class=""govuk-accordion__section-content"" aria-labelledby=""accordion-default-heading-1"">
                                            <ul class=""govuk-list"">
                                                <li>Test start time: {result.StartTime}</li>
                                                <li>Test end time: {result.EndTime}</li>
                                            </ul>
                                            <div class=""govuk-form-group"">
                                                <label class=""govuk-label"" for=""more-detail"">
                                                Test run
                                                </label>
                                                <textarea readonly class=""test-result-output"" rows=""20"">{result.StdOut}</textarea>
                                            </div>
                                        </div>
                                    </div>");

        // Close the accordion
        content.Append("</div>");

        return content.ToString();
    }

    private static string AddSpacesToSentence(string text, bool preserveAcronyms)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);

        for (var i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
                if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                    (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                     i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }
}