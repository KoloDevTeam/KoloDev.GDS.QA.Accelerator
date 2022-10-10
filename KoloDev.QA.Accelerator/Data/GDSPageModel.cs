// ***********************************************************************
// Assembly         : KoloDev.GDS.QA.Accelerator
// Author           : KoloDev
// Created          : 02-07-2022
//
// Last Modified By : KoloDev
// Last Modified On : 06-28-2022
// ***********************************************************************
// <copyright file="GDSPageModel.cs" company="KoloDev Ltd.">
//     Copyright © 2022 KoloDev Ltd. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace KoloDev.GDS.QA.Accelerator.Data
{
    /// <summary>
    /// GDS Page Model
    /// </summary>
    public class GdsPageModel
    {
        /// <summary>
        /// The type of page
        /// </summary>
        /// <value>The type of the page.</value>
        public string? PageType { get; set; }
        /// <summary>
        /// Gets or sets the field sets.
        /// </summary>
        /// <value>The field sets.</value>
        public List<FieldSet>? FieldSets { get; set; }
        /// <summary>
        /// Gets or sets the patterns.
        /// </summary>
        /// <value>The patterns.</value>
        public List<string>? Patterns { get; set; }
        /// <summary>
        /// Gets or sets the accordians.
        /// </summary>
        /// <value>The accordians.</value>
        public List<Accordian>? Accordians { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [back link].
        /// </summary>
        /// <value><c>null</c> if [back link] contains no value, <c>true</c> if [back link]; otherwise, <c>false</c>.</value>
        public bool? BackLink { get; set; }
        /// <summary>
        /// Gets or sets the breadcrumbs.
        /// </summary>
        /// <value>The breadcrumbs.</value>
        public List<Breadcrumb>? Breadcrumbs { get; set; }
        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>The buttons.</value>
        public List<Button>? Buttons { get; set; }
        /// <summary>
        /// Gets or sets the check boxes.
        /// </summary>
        /// <value>The check boxes.</value>
        public List<Checkbox>? CheckBoxes { get; set; }
        /// <summary>
        /// Gets or sets the date inputs.
        /// </summary>
        /// <value>The date inputs.</value>
        public List<DateInput>? DateInputs { get; set; }
        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>The details.</value>
        public List<Detail>? Details { get; set; }
        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>The error messages.</value>
        public List<Error>? ErrorMessages { get; set; }
        /// <summary>
        /// Gets or sets the summary errors.
        /// </summary>
        /// <value>The summary errors.</value>
        public List<SummaryError>? SummaryErrors { get; set; }
        /// <summary>
        /// Gets or sets the file uploads.
        /// </summary>
        /// <value>The file uploads.</value>
        public List<FileUpload>? FileUploads { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [footer present].
        /// </summary>
        /// <value><c>null</c> if [footer present] contains no value, <c>true</c> if [footer present]; otherwise, <c>false</c>.</value>
        public bool? FooterPresent { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [header present].
        /// </summary>
        /// <value><c>null</c> if [header present] contains no value, <c>true</c> if [header present]; otherwise, <c>false</c>.</value>
        public bool? HeaderPresent { get; set; }
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public Header? Header { get; set; }
        /// <summary>
        /// Gets or sets the inset texts.
        /// </summary>
        /// <value>The inset texts.</value>
        public List<InsetText>? InsetTexts { get; set; }
        /// <summary>
        /// Gets or sets the panels.
        /// </summary>
        /// <value>The panels.</value>
        public List<Panel>? Panels { get; set; }
        /// <summary>
        /// Gets or sets the phase.
        /// </summary>
        /// <value>The phase.</value>
        public Phase? Phase { get; set; }
        /// <summary>
        /// Gets or sets the radios.
        /// </summary>
        /// <value>The radios.</value>
        public List<Radio>? Radios { get; set; }
        /// <summary>
        /// Gets or sets the selects.
        /// </summary>
        /// <value>The selects.</value>
        public List<Select>? Selects { get; set; }
        /// <summary>
        /// Gets or sets the skip link.
        /// </summary>
        /// <value>The skip link.</value>
        public SkipLink? SkipLink { get; set; }
        /// <summary>
        /// Gets or sets the summary lists.
        /// </summary>
        /// <value>The summary lists.</value>
        public List<SummaryList>? SummaryLists { get; set; }
        /// <summary>
        /// Gets or sets the tables.
        /// </summary>
        /// <value>The tables.</value>
        public List<Table>? Tables { get; set; }
        /// <summary>
        /// Gets or sets the tabs.
        /// </summary>
        /// <value>The tabs.</value>
        public List<Tab>? Tabs { get; set; }
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public List<Tag>? Tags { get; set; }
        /// <summary>
        /// Gets or sets the text inputs.
        /// </summary>
        /// <value>The text inputs.</value>
        public List<TextInput>? TextInputs { get; set; }
        /// <summary>
        /// Gets or sets the text areas.
        /// </summary>
        /// <value>The text areas.</value>
        public List<TextArea>? TextAreas { get; set; }
        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>The warnings.</value>
        public List<Warning>? Warnings { get; set; }
        /// <summary>
        /// Gets or sets the hyper links.
        /// </summary>
        /// <value>The hyper links.</value>
        public List<HyperLink>? HyperLinks { get; set; }
        /// <summary>
        /// Gets or sets the progress steps.
        /// </summary>
        /// <value>The progress steps.</value>
        public List<Step>? ProgressSteps { get; set; }
        /// <summary>
        /// Gets or sets the spans.
        /// </summary>
        /// <value>The spans.</value>
        public List<Span>? Spans { get; set; }
        /// <summary>
        /// Gets or sets the validator.
        /// </summary>
        /// <value>The validator.</value>
        public List<Validator>? Validator { get; set; }
        /// <summary>
        /// Gets or sets the text on page.
        /// </summary>
        /// <value>The text on page.</value>
        public List<string>? TextOnPage { get; set; }
    }

    /// <summary>
    /// Class Validator.
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Identifier { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
        /// <summary>
        /// Gets or sets the regex.
        /// </summary>
        /// <value>The regex.</value>
        public string? Regex { get; set; }
        /// <summary>
        /// Gets or sets the fixed.
        /// </summary>
        /// <value>The fixed.</value>
        public string? Fixed { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [fixed based].
        /// </summary>
        /// <value><c>null</c> if [fixed based] contains no value, <c>true</c> if [fixed based]; otherwise, <c>false</c>.</value>
        public bool? FixedBased { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [regex based].
        /// </summary>
        /// <value><c>null</c> if [regex based] contains no value, <c>true</c> if [regex based]; otherwise, <c>false</c>.</value>
        public bool? RegexBased { get; set; }
    }

    /// <summary>
    /// Class Span.
    /// </summary>
    public class Span
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Identifier { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
    }

    /// <summary>
    /// Class Step.
    /// </summary>
    public class Step
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>The action.</value>
        public string? Action { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Identifier { get; set; }
        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>The component.</value>
        public string? Component { get; set; }
    }

    /// <summary>
    /// Class Phase.
    /// </summary>
    public class Phase
    {
        /// <summary>
        /// Gets or sets a value indicating whether [banner present].
        /// </summary>
        /// <value><c>true</c> if [banner present]; otherwise, <c>false</c>.</value>
        public bool BannerPresent { get; set; }
        /// <summary>
        /// Gets or sets the banner tag.
        /// </summary>
        /// <value>The banner tag.</value>
        public string? BannerTag { get; set; }
        /// <summary>
        /// Gets or sets the banner text.
        /// </summary>
        /// <value>The banner text.</value>
        public string? BannerText { get; set; }
        /// <summary>
        /// Gets or sets the feedback link.
        /// </summary>
        /// <value>The feedback link.</value>
        public string? FeedbackLink { get; set; }

    }

    /// <summary>
    /// Class Header.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public string? ServiceName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show hide button].
        /// </summary>
        /// <value><c>true</c> if [show hide button]; otherwise, <c>false</c>.</value>
        public bool ShowHideButton { get; set; }
        /// <summary>
        /// Gets or sets the navigation items.
        /// </summary>
        /// <value>The navigation items.</value>
        public List<NavigationItem> NavigationItems { get; set; }
    }

    /// <summary>
    /// Class NavigationItem.
    /// </summary>
    public class NavigationItem
    {
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>The href.</value>
        public string? Href { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
    }

    /// <summary>
    /// Class FieldSet.
    /// </summary>
    public class FieldSet
    {
        /// <summary>
        /// Gets or sets the heading.
        /// </summary>
        /// <value>The heading.</value>
        public string? Heading { get; set; }
        /// <summary>
        /// Gets or sets the form components.
        /// </summary>
        /// <value>The form components.</value>
        public List<FormComponent> FormComponents { get; set; }
    }

    /// <summary>
    /// Class FormComponent.
    /// </summary>
    public class FormComponent
    {
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
    }

    /// <summary>
    /// Class Accordian.
    /// </summary>
    public class Accordian
    {
        /// <summary>
        /// Gets or sets a value indicating whether [openall link].
        /// </summary>
        /// <value><c>true</c> if [openall link]; otherwise, <c>false</c>.</value>
        public bool OpenallLink { get; set; }
        /// <summary>
        /// Gets or sets the openall link class.
        /// </summary>
        /// <value>The openall link class.</value>
        public string? OpenallLinkClass { get; set; }
        /// <summary>
        /// Gets or sets the openall link text.
        /// </summary>
        /// <value>The openall link text.</value>
        public string? OpenallLinkText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [openall link aria active].
        /// </summary>
        /// <value><c>true</c> if [openall link aria active]; otherwise, <c>false</c>.</value>
        public bool OpenallLinkAriaActive { get; set; }
        /// <summary>
        /// Gets or sets the entry count.
        /// </summary>
        /// <value>The entry count.</value>
        public int EntryCount { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? ID { get; set; }
        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public List<AccordianEntries> Entries { get; set; }

    }
    /// <summary>
    /// Class AccordianEntries.
    /// </summary>
    public class AccordianEntries
    {
        /// <summary>
        /// Gets or sets the entry text.
        /// </summary>
        /// <value>The entry text.</value>
        public string? EntryText { get; set; }
        /// <summary>
        /// Gets or sets the content of the entry.
        /// </summary>
        /// <value>The content of the entry.</value>
        public string? EntryContent { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [other content than text present].
        /// </summary>
        /// <value><c>true</c> if [other content than text present]; otherwise, <c>false</c>.</value>
        public bool OtherContentThanTextPresent { get; set; }
    }

    /// <summary>
    /// Class Breadcrumb.
    /// </summary>
    public class Breadcrumb
    {
        /// <summary>
        /// Gets or sets the bread crumb text.
        /// </summary>
        /// <value>The bread crumb text.</value>
        public string? BreadCrumbText { get; set; }
        /// <summary>
        /// Gets or sets the bread crumb link.
        /// </summary>
        /// <value>The bread crumb link.</value>
        public string? BreadCrumbLink { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [present page].
        /// </summary>
        /// <value><c>true</c> if [present page]; otherwise, <c>false</c>.</value>
        public bool PresentPage { get; set; }
    }

    /// <summary>
    /// Class Button.
    /// </summary>
    public class Button
    {
        /// <summary>
        /// Gets or sets the button text.
        /// </summary>
        /// <value>The button text.</value>
        public string? ButtonText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [prevent double click].
        /// </summary>
        /// <value><c>true</c> if [prevent double click]; otherwise, <c>false</c>.</value>
        public bool PreventDoubleClick { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Button"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        /// <value>The form.</value>
        public string? Form { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? ID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
    }

    /// <summary>
    /// Class Checkbox.
    /// </summary>
    public class Checkbox
    {
        /// <summary>
        /// Gets or sets the fieldset.
        /// </summary>
        /// <value>The fieldset.</value>
        public string? Fieldset { get; set; }
        /// <summary>
        /// Gets or sets the fieldsethint.
        /// </summary>
        /// <value>The fieldsethint.</value>
        public string? Fieldsethint { get; set; }
        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        /// <value>The hint.</value>
        public string? Hint { get; set; }
        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>The legend.</value>
        public string? Legend { get; set; }
        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        /// <value>The form.</value>
        public string? Form { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public string? Size { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string? Label { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the errormessage.
        /// </summary>
        /// <value>The errormessage.</value>
        public string? Errormessage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Checkbox"/> is hasconditional.
        /// </summary>
        /// <value><c>true</c> if hasconditional; otherwise, <c>false</c>.</value>
        public bool Hasconditional { get; set; }
        /// <summary>
        /// Gets or sets the conditionalid.
        /// </summary>
        /// <value>The conditionalid.</value>
        public string? Conditionalid { get; set; }
        /// <summary>
        /// Gets or sets the conditionalname.
        /// </summary>
        /// <value>The conditionalname.</value>
        public string? Conditionalname { get; set; }
        /// <summary>
        /// Gets or sets the conditionaltype.
        /// </summary>
        /// <value>The conditionaltype.</value>
        public string? Conditionaltype { get; set; }
        /// <summary>
        /// Gets or sets the conditionalerrormessage.
        /// </summary>
        /// <value>The conditionalerrormessage.</value>
        public string? Conditionalerrormessage { get; set; }
    }

    /// <summary>
    /// Class DateInput.
    /// </summary>
    public class DateInput
    {
        /// <summary>
        /// Gets or sets the fixed input.
        /// </summary>
        /// <value>The fixed input.</value>
        public string? FixedInput { get; set; }
        /// <summary>
        /// Gets or sets the regex input.
        /// </summary>
        /// <value>The regex input.</value>
        public string? RegexInput { get; set; }
        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        /// <value>The form.</value>
        public string? Form { get; set; }
        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>The legend.</value>
        public string? Legend { get; set; }
        /// <summary>
        /// Gets or sets the heading.
        /// </summary>
        /// <value>The heading.</value>
        public string? Heading { get; set; }
        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        /// <value>The hint.</value>
        public string? Hint { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets for.
        /// </summary>
        /// <value>For.</value>
        public string? For { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string? Pattern { get; set; }
        /// <summary>
        /// Gets or sets the autocomplete.
        /// </summary>
        /// <value>The autocomplete.</value>
        public string? Autocomplete { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [autocomplete present].
        /// </summary>
        /// <value><c>true</c> if [autocomplete present]; otherwise, <c>false</c>.</value>
        public bool AutocompletePresent { get; set; }
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Class Detail.
    /// </summary>
    public class Detail
    {
        /// <summary>
        /// Gets or sets the short text.
        /// </summary>
        /// <value>The short text.</value>
        public string? ShortText { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string? Description { get; set; }

    }

    /// <summary>
    /// Class Error.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string? Message { get; set; }
        /// <summary>
        /// Gets or sets the attached to.
        /// </summary>
        /// <value>The attached to.</value>
        public string? AttachedTo { get; set; }
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>The href.</value>
        public string? Href { get; set; }

    }

    /// <summary>
    /// Class SummaryError.
    /// </summary>
    public class SummaryError
    {
        /// <summary>
        /// Gets or sets the summary title.
        /// </summary>
        /// <value>The summary title.</value>
        public string? SummaryTitle { get; set; }
        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>The errors.</value>
        public List<Error> Errors { get; set; }
    }

    /// <summary>
    /// Class FileUpload.
    /// </summary>
    public class FileUpload
    {
        /// <summary>
        /// Gets or sets the fixed input.
        /// </summary>
        /// <value>The fixed input.</value>
        public string? FixedInput { get; set; }
        /// <summary>
        /// Gets or sets the regex input.
        /// </summary>
        /// <value>The regex input.</value>
        public string? RegexInput { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string? Label { get; set; }
        /// <summary>
        /// Gets or sets for.
        /// </summary>
        /// <value>For.</value>
        public string? For { get; set; }
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
    }

    /// <summary>
    /// Class InsetText.
    /// </summary>
    public class InsetText
    {
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
    }

    /// <summary>
    /// Class Panel.
    /// </summary>
    public class Panel
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string? Header { get; set; }
        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public string? Body { get; set; }
        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        /// <value>The reference.</value>
        public string? Reference { get; set; }
        /// <summary>
        /// Gets or sets the important.
        /// </summary>
        /// <value>The important.</value>
        public string? Important { get; set; }
    }

    /// <summary>
    /// Class Radio.
    /// </summary>
    public class Radio
    {
        /// <summary>
        /// Gets or sets the fieldset.
        /// </summary>
        /// <value>The fieldset.</value>
        public string? Fieldset { get; set; }
        /// <summary>
        /// Gets or sets the fieldsethint.
        /// </summary>
        /// <value>The fieldsethint.</value>
        public string? Fieldsethint { get; set; }
        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        /// <value>The hint.</value>
        public string? Hint { get; set; }
        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>The legend.</value>
        public string? Legend { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public string? Size { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Radio"/> is preselected.
        /// </summary>
        /// <value><c>true</c> if preselected; otherwise, <c>false</c>.</value>
        public bool Preselected { get; set; } = false;
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string? Label { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets for.
        /// </summary>
        /// <value>For.</value>
        public string? For { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string? Type { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string? Value { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Radio"/> is inline.
        /// </summary>
        /// <value><c>true</c> if inline; otherwise, <c>false</c>.</value>
        public bool Inline { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Radio"/> is stacked.
        /// </summary>
        /// <value><c>true</c> if stacked; otherwise, <c>false</c>.</value>
        public bool Stacked { get; set; }
        /// <summary>
        /// Gets or sets the hint identifier.
        /// </summary>
        /// <value>The hint identifier.</value>
        public string? HintId { get; set; }
        /// <summary>
        /// Gets or sets the hint class.
        /// </summary>
        /// <value>The hint class.</value>
        public string? HintClass { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [radio divider above].
        /// </summary>
        /// <value><c>true</c> if [radio divider above]; otherwise, <c>false</c>.</value>
        public bool RadioDividerAbove { get; set; }
        /// <summary>
        /// Gets or sets the radio divider identifier.
        /// </summary>
        /// <value>The radio divider identifier.</value>
        public string? RadioDividerId { get; set; }
        /// <summary>
        /// Gets or sets the radio divider text.
        /// </summary>
        /// <value>The radio divider text.</value>
        public string? RadioDividerText { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance has conditional.
        /// </summary>
        /// <value><c>true</c> if this instance has conditional; otherwise, <c>false</c>.</value>
        public bool HasConditional { get; set; }
        /// <summary>
        /// Gets or sets the conditional identifier.
        /// </summary>
        /// <value>The conditional identifier.</value>
        public string? ConditionalId { get; set; }
        /// <summary>
        /// Gets or sets the type of the conditional.
        /// </summary>
        /// <value>The type of the conditional.</value>
        public string? ConditionalType { get; set; }
        /// <summary>
        /// Gets or sets the name of the conditional.
        /// </summary>
        /// <value>The name of the conditional.</value>
        public string? ConditionalName { get; set; }
        /// <summary>
        /// Gets or sets the conditional class.
        /// </summary>
        /// <value>The conditional class.</value>
        public string? ConditionalClass { get; set; }
    }

    /// <summary>
    /// Class Select.
    /// </summary>
    public class Select
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string? Label { get; set; }
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        public List<SelectOption> Options { get; set; }
        /// <summary>
        /// Gets or sets the fixed input.
        /// </summary>
        /// <value>The fixed input.</value>
        public string? FixedInput { get; set; }
        /// <summary>
        /// Gets or sets the regex input.
        /// </summary>
        /// <value>The regex input.</value>
        public string? RegexInput { get; set; }
    }

    /// <summary>
    /// Class SelectOption.
    /// </summary>
    public class SelectOption
    {
        /// <summary>
        /// Gets or sets the fixed input.
        /// </summary>
        /// <value>The fixed input.</value>
        public string? FixedInput { get; set; }
        /// <summary>
        /// Gets or sets the regex input.
        /// </summary>
        /// <value>The regex input.</value>
        public string? RegexInput { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string? Value { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SelectOption"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected { get; set; }
    }

    /// <summary>
    /// Class SkipLink.
    /// </summary>
    public class SkipLink
    {
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>The href.</value>
        public string? Href { get; set; }
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the link text.
        /// </summary>
        /// <value>The link text.</value>
        public string? LinkText { get; set; }
    }

    /// <summary>
    /// Class SummaryList.
    /// </summary>
    public class SummaryList
    {
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public List<SummaryListRow> Rows { get; set; }
    }

    /// <summary>
    /// Class SummaryListRow.
    /// </summary>
    public class SummaryListRow
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string? Key { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string? Value { get; set; }
        /// <summary>
        /// Gets or sets the actions.
        /// </summary>
        /// <value>The actions.</value>
        public string? Actions { get; set; }
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>The href.</value>
        public string? Href { get; set; }
        /// <summary>
        /// Gets or sets the change.
        /// </summary>
        /// <value>The change.</value>
        public string? Change { get; set; }

    }

    /// <summary>
    /// Class Table.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string? Caption { get; set; }
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<Column> Columns { get; set; }
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public List<Row> Rows { get; set; }
    }

    /// <summary>
    /// Class Column.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string? Title { get; set; }
    }

    /// <summary>
    /// Class Row.
    /// </summary>
    public class Row
    {
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string? Value { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is tag.
        /// </summary>
        /// <value><c>true</c> if this instance is tag; otherwise, <c>false</c>.</value>
        public bool IsTag { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [followed by seperator].
        /// </summary>
        /// <value><c>true</c> if [followed by seperator]; otherwise, <c>false</c>.</value>
        public bool FollowedBySeperator { get; set; }
    }
    /// <summary>
    /// Class Tab.
    /// </summary>
    public class Tab
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string? Title { get; set; }
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>The href.</value>
        public string? Href { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Tab"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected { get; set; }
        /// <summary>
        /// Gets or sets the content of the tab.
        /// </summary>
        /// <value>The content of the tab.</value>
        public TabContent TabContent { get; set; }
    }
    /// <summary>
    /// Class TabContent.
    /// Implements the <see cref="KoloDev.GDS.QA.Accelerator.Data.GdsPageModel" />
    /// </summary>
    /// <seealso cref="KoloDev.GDS.QA.Accelerator.Data.GdsPageModel" />
    public class TabContent : GdsPageModel
    {

    }
    /// <summary>
    /// Class Tag.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public string? Colour { get; set; }
    }
    /// <summary>
    /// Class TextInput.
    /// </summary>
    public class TextInput
    {
        /// <summary>
        /// Gets or sets the fixed input.
        /// </summary>
        /// <value>The fixed input.</value>
        public string? FixedInput { get; set; }
        /// <summary>
        /// Gets or sets the regex input.
        /// </summary>
        /// <value>The regex input.</value>
        public string? RegexInput { get; set; }
        /// <summary>
        /// Gets or sets the fieldset.
        /// </summary>
        /// <value>The fieldset.</value>
        public string? Fieldset { get; set; }
        /// <summary>
        /// Gets or sets the fieldsethint.
        /// </summary>
        /// <value>The fieldsethint.</value>
        public string? Fieldsethint { get; set; }
        /// <summary>
        /// Gets or sets the hint.
        /// </summary>
        /// <value>The hint.</value>
        public string? Hint { get; set; }
        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>The legend.</value>
        public string? Legend { get; set; }
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string? Label { get; set; }
        /// <summary>
        /// Gets or sets for.
        /// </summary>
        /// <value>For.</value>
        public string? For { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the CSS selector.
        /// </summary>
        /// <value>The CSS selector.</value>
        public string? CSSSelector { get; set; }
        /// <summary>
        /// Gets or sets the type of the input.
        /// </summary>
        /// <value>The type of the input.</value>
        public string? InputType { get; set; }
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the hint text.
        /// </summary>
        /// <value>The hint text.</value>
        public string? HintText { get; set; }
        /// <summary>
        /// Gets or sets the hint text identifier.
        /// </summary>
        /// <value>The hint text identifier.</value>
        public string? HintTextId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [automatic complete].
        /// </summary>
        /// <value><c>true</c> if [automatic complete]; otherwise, <c>false</c>.</value>
        public bool AutoComplete { get; set; }
        /// <summary>
        /// Gets or sets the type of the automplete.
        /// </summary>
        /// <value>The type of the automplete.</value>
        public string? AutompleteType { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [spell check present].
        /// </summary>
        /// <value><c>true</c> if [spell check present]; otherwise, <c>false</c>.</value>
        public bool SpellCheckPresent { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [spell check value].
        /// </summary>
        /// <value><c>true</c> if [spell check value]; otherwise, <c>false</c>.</value>
        public bool SpellCheckValue { get; set; }
    }
    /// <summary>
    /// Class TextArea.
    /// </summary>
    public class TextArea
    {
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        /// <value>The label.</value>
        public string? Label { get; set; }
        /// <summary>
        /// Gets or sets for.
        /// </summary>
        /// <value>For.</value>
        public string? For { get; set; }
        /// <summary>
        /// Gets or sets the hint identifier.
        /// </summary>
        /// <value>The hint identifier.</value>
        public string? HintId { get; set; }
        /// <summary>
        /// Gets or sets the hint class.
        /// </summary>
        /// <value>The hint class.</value>
        public string? HintClass { get; set; }
        /// <summary>
        /// Gets or sets the hint text.
        /// </summary>
        /// <value>The hint text.</value>
        public string? HintText { get; set; }
        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        /// <value>The class.</value>
        public string? Class { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the rows.
        /// </summary>
        /// <value>The rows.</value>
        public string? Rows { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [character count].
        /// </summary>
        /// <value><c>true</c> if [character count]; otherwise, <c>false</c>.</value>
        public bool CharacterCount { get; set; }
        /// <summary>
        /// Gets or sets the character count number.
        /// </summary>
        /// <value>The character count number.</value>
        public string? CharacterCountNumber { get; set; }

    }
    /// <summary>
    /// Class Warning.
    /// </summary>
    public class Warning
    {
        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The icon.</value>
        public string? Icon { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
        /// <summary>
        /// Gets or sets the associated with.
        /// </summary>
        /// <value>The associated with.</value>
        public string? AssociatedWith { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Warning"/> is generic.
        /// </summary>
        /// <value><c>true</c> if generic; otherwise, <c>false</c>.</value>
        public bool Generic { get; set; }
    }
    /// <summary>
    /// Class HyperLink.
    /// </summary>
    public class HyperLink
    {
        /// <summary>
        /// Gets or sets the x path.
        /// </summary>
        /// <value>The x path.</value>
        public string? XPath { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string? Text { get; set; }
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>The href.</value>
        public string? Href { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is relative.
        /// </summary>
        /// <value><c>true</c> if this instance is relative; otherwise, <c>false</c>.</value>
        public bool IsRelative { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is parameter.
        /// </summary>
        /// <value><c>true</c> if this instance is parameter; otherwise, <c>false</c>.</value>
        public bool IsParameter { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [no associated control].
        /// </summary>
        /// <value><c>true</c> if [no associated control]; otherwise, <c>false</c>.</value>
        public bool NoAssociatedControl { get; set; }
        /// <summary>
        /// Gets or sets the asssociated control.
        /// </summary>
        /// <value>The asssociated control.</value>
        public string? AsssociatedControl { get; set; }
    }
}