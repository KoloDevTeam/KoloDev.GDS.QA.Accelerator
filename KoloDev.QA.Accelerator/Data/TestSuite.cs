namespace KoloDev.GDS.QA.Accelerator.Data
{
    /// <summary>
    /// Kolo Test Suite Model
    /// </summary>
    public class KoloTestSuite
    {
        /// <summary>
        /// Test Cases
        /// </summary>
        public List<KoloTestCase>? TestCases { get; set; }

        public partial class KoloTestCase
        {
            public string Id { get; set; }
            public List<KoloTestSteps> TestSteps { get; set; }
        }

        public partial class KoloTestSteps
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Passed { get; set; }
            public int StepNumber { get; set; }
        }
    }
}
