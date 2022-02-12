using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string? PageType { get; set; }
        public List<FieldSet>? FieldSets { get; set; }
        public List<string>? Patterns { get; set; }
        public List<Accordian>? Accordians { get; set; }
        public bool? BackLink { get; set; }
        public List<Breadcrumb>? Breadcrumbs { get; set; }
        public List<Button>? Buttons { get; set; }
        public List<Checkbox>? CheckBoxes { get; set; }
        public List<DateInput>? DateInputs { get; set; }
        public List<Detail>? Details { get; set; }
        public List<Error>? ErrorMessages { get; set; }
        public List<SummaryError>? SummaryErrors { get; set; }
        public List<FileUpload>? FileUploads { get; set; }
        public bool? FooterPresent { get; set; }
        public bool? HeaderPresent { get; set; }
        public Header? Header { get; set; }
        public List<InsetText>? InsetTexts { get; set; }
        public List<Panel>? Panels { get; set; }
        public Phase? Phase { get; set; }
        public List<Radio>? Radios { get; set; }
        public List<Select>? Selects { get; set; }
        public SkipLink? SkipLink { get; set; }
        public List<SummaryList>? SummaryLists { get; set; }
        public List<Table>? Tables { get; set; }
        public List<Tab>? Tabs { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<TextInput>? TextInputs { get; set; }
        public List<TextArea>? TextAreas { get; set; }
        public List<Warning>? Warnings { get; set; }
        public List<HyperLink>? HyperLinks { get; set; }
        public List<Step>? ProgressSteps { get; set; }
        public List<Span>? Spans { get; set; }
        public List<Validator>? Validator { get; set; }
    }

    public class Validator
    {
        public string? Identifier { get; set; }
        public string? Type { get; set; }
        public string? Regex { get; set; }
        public string? Fixed { get; set; }
        public bool? FixedBased { get; set; }
        public bool? RegexBased { get; set; }
    }

    public class Span
    {
        public string? Text { get; set; }
        public string? Identifier { get; set; }
        public string? Type { get; set; }
    }

    public class Step
    {
        public string? Action { get; set; }
        public string? Type { get; set; }
        public string? Identifier { get; set; }
        public string? Component { get; set; }
    }

    public class Phase
    {
        public bool BannerPresent { get; set; }
        public string? BannerTag { get; set; }
        public string? BannerText { get; set; }
        public string? FeedbackLink { get; set; }

    }

    public class Header
    {
        public string? Type { get; set; }
        public string? ServiceName { get; set; }
        public bool ShowHideButton { get; set; }
        public List<NavigationItem> NavigationItems { get; set; }
    }

    public class NavigationItem
    {
        public string? Class { get; set; }
        public string? Href { get; set; }
        public string? Text { get; set; }
    }

    public class FieldSet
    {
        public string? Heading { get; set; }
        public List<FormComponent> FormComponents { get; set; }
    }

    public class FormComponent
    {
        public string? Class { get; set; }
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Type { get; set; }
    }

    public class Accordian
    {
        public bool OpenallLink { get; set; }
        public string? OpenallLinkClass { get; set; }
        public string? OpenallLinkText { get; set; }
        public bool OpenallLinkAriaActive { get; set; }
        public int EntryCount { get; set; }
        public string? ID { get; set; }
        public List<AccordianEntries> Entries { get; set; }

    }
    public class AccordianEntries
    {
        public string? EntryText { get; set; }
        public string? EntryContent { get; set; }

        public string? Id { get; set; }

        public bool OtherContentThanTextPresent { get; set; }
    }

    public class Breadcrumb
    {
        public string? BreadCrumbText { get; set; }
        public string? BreadCrumbLink { get; set; }
        public bool PresentPage { get; set; }
    }

    public class Button
    {
        public string? ButtonText { get; set; }
        public bool PreventDoubleClick { get; set; }
        public bool Enabled { get; set; }
        public string? Type { get; set; }
        public string? Form { get; set; }
        public string? ID { get; set; }
        public string? Name { get; set; }
    }

    public class Checkbox
    {
        public string? Fieldset { get; set; }
        public string? Fieldsethint { get; set; }
        public string? Hint { get; set; }
        public string? Legend { get; set; }
        public string? Form { get; set; }
        public string? Size { get; set; }
        public string? Label { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Errormessage { get; set; }
        public bool Hasconditional { get; set; }
        public string? Conditionalid { get; set; }
        public string? Conditionalname { get; set; }
        public string? Conditionaltype { get; set; }
        public string? Conditionalerrormessage { get; set; }
    }

    public class DateInput
    {
        public string? FixedInput { get; set; }
        public string? RegexInput { get; set; }
        public string? Form { get; set; }
        public string? Legend { get; set; }
        public string? Heading { get; set; }
        public string? Hint { get; set; }
        public string? Id { get; set; }
        public string? For { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Pattern { get; set; }
        public string? Autocomplete { get; set; }
        public bool AutocompletePresent { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class Detail
    {
        public string? ShortText { get; set; }
        public string? Description { get; set; }

    }

    public class Error
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
        public string? AttachedTo { get; set; }
        public string? Href { get; set; }

    }

    public class SummaryError
    {
        public string? SummaryTitle { get; set; }
        public List<Error> Errors { get; set; }
    }

    public class FileUpload
    {
        public string? FixedInput { get; set; }
        public string? RegexInput { get; set; }
        public string? Label { get; set; }
        public string? For { get; set; }
        public string? Class { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
    }

    public class InsetText
    {
        public string? Class { get; set; }
        public string? Text { get; set; }
    }

    public class Panel
    {
        public string? Header { get; set; }
        public string? Body { get; set; }
        public string? Reference { get; set; }
        public string? Important { get; set; }
    }

    public class Radio
    {
        public string? Fieldset { get; set; }
        public string? Fieldsethint { get; set; }
        public string? Hint { get; set; }
        public string? Legend { get; set; }
        public string? Size { get; set; }
        public bool Preselected { get; set; } = false;
        public string? Label { get; set; }
        public string? Id { get; set; }
        public string? For { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
        public bool Inline { get; set; }
        public bool Stacked { get; set; }
        public string? HintId { get; set; }
        public string? HintClass { get; set; }
        public bool RadioDividerAbove { get; set; }
        public string? RadioDividerId { get; set; }
        public string? RadioDividerText { get; set; }
        public bool HasConditional { get; set; }
        public string? ConditionalId { get; set; }
        public string? ConditionalType { get; set; }
        public string? ConditionalName { get; set; }
        public string? ConditionalClass { get; set; }
    }

    public class Select
    {
        public string? Label { get; set; }
        public string? Class { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<SelectOption> Options { get; set; }
        public string? FixedInput { get; set; }
        public string? RegexInput { get; set; }
    }

    public class SelectOption
    {
        public string? FixedInput { get; set; }
        public string? RegexInput { get; set; }
        public string? Value { get; set; }
        public string? Text { get; set; }
        public bool Selected { get; set; }
    }

    public class SkipLink
    {
        public string? Href { get; set; }
        public string? Class { get; set; }
        public string? LinkText { get; set; }
    }

    public class SummaryList
    {
        public List<SummaryListRow> Rows { get; set; }
    }

    public class SummaryListRow
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
        public string? Actions { get; set; }
        public string? Href { get; set; }
        public string? Change { get; set; }

    }

    public class Table
    {
        public string? Caption { get; set; }
        public List<Column> Columns { get; set; }
        public List<Row> Rows { get; set; }
    }

    public class Column
    {
        public string? Class { get; set; }
        public string? Title { get; set; }
    }

    public class Row
    {
        public string? Class { get; set; }
        public string? Value { get; set; }
        public bool IsTag { get; set; }
        public bool FollowedBySeperator { get; set; }
    }
    public class Tab
    {
        public string? Title { get; set; }
        public string? Class { get; set; }
        public string? Href { get; set; }
        public string? Text { get; set; }
        public bool Selected { get; set; }
        public TabContent TabContent { get; set; }
    }
    public class TabContent : GdsPageModel
    {

    }
    public class Tag
    {
        public string? Class { get; set; }
        public string? Text { get; set; }
        public string? Colour { get; set; }
    }
    public class TextInput
    {
        public string? FixedInput { get; set; }
        public string? RegexInput { get; set; }
        public string? Fieldset { get; set; }
        public string? Fieldsethint { get; set; }
        public string? Hint { get; set; }
        public string? Legend { get; set; }
        public string? Label { get; set; }
        public string? For { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? CSSSelector { get; set; }
        public string? InputType { get; set; }
        public string? Class { get; set; }
        public string? HintText { get; set; }
        public string? HintTextId { get; set; }
        public bool AutoComplete { get; set; }
        public string? AutompleteType { get; set; }
        public bool SpellCheckPresent { get; set; }
        public bool SpellCheckValue { get; set; }
    }
    public class TextArea
    {
        public string? Label { get; set; }
        public string? For { get; set; }
        public string? HintId { get; set; }
        public string? HintClass { get; set; }
        public string? HintText { get; set; }
        public string? Class { get; set; }
        public string? Name { get; set; }
        public string? Id { get; set; }
        public string? Rows { get; set; }
        public bool CharacterCount { get; set; }
        public string? CharacterCountNumber { get; set; }

    }
    public class Warning
    {
        public string? Icon { get; set; }
        public string? Text { get; set; }
        public string? AssociatedWith { get; set; }
        public bool Generic { get; set; }
    }
    public class HyperLink
    {
        public string? XPath { get; set; }
        public string? Text { get; set; }
        public string? Href { get; set; }
        public string? Name { get; set; }
        public string? Id { get; set; }
        public bool IsRelative { get; set; }
        public bool IsParameter { get; set; }
        public bool NoAssociatedControl { get; set; }
        public string? AsssociatedControl { get; set; }
    }
}