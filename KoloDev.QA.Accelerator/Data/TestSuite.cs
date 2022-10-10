// ***********************************************************************
// Assembly         : KoloDev.GDS.QA.Accelerator
// Author           : KoloDev
// Created          : 02-07-2022
//
// Last Modified By : KoloDev
// Last Modified On : 02-08-2022
// ***********************************************************************
// <copyright file="TestSuite.cs" company="KoloDev Ltd.">
//     Copyright © 2022 KoloDev Ltd. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
        /// <value>The test cases.</value>
        public List<KoloTestCase>? TestCases { get; set; }

        /// <summary>
        /// Class KoloTestCase.
        /// </summary>
        public partial class KoloTestCase
        {
            /// <summary>
            /// Gets or sets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public string Id { get; set; }
            /// <summary>
            /// Gets or sets the test steps.
            /// </summary>
            /// <value>The test steps.</value>
            public List<KoloTestSteps> TestSteps { get; set; }
        }

        /// <summary>
        /// Class KoloTestSteps.
        /// </summary>
        public partial class KoloTestSteps
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the description.
            /// </summary>
            /// <value>The description.</value>
            public string Description { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="KoloTestSteps"/> is passed.
            /// </summary>
            /// <value><c>true</c> if passed; otherwise, <c>false</c>.</value>
            public bool Passed { get; set; }
            /// <summary>
            /// Gets or sets the step number.
            /// </summary>
            /// <value>The step number.</value>
            public int StepNumber { get; set; }
        }
    }
}
