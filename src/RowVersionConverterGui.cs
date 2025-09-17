using DevToys.Api;
using System.ComponentModel.Composition;
using static DevToys.Api.GUI;

namespace DevToys.RowVersionConverter;

[Export(typeof(IGuiTool))]
[Name("RowVersionConverter")]                                                                  // A unique, internal name of the tool.
[ToolDisplayInformation(
    IconFontName = "FluentSystemIcons",                                                        // This font is available by default in DevToys
    IconGlyph = '\ue474',                                                                      // An icon that represents a pizza
    GroupName = PredefinedCommonToolGroupNames.Converters,                                     // The group in which the tool will appear in the side bar.
    ResourceManagerAssemblyIdentifier = nameof(RowVersionConverterResourceAssemblyIdentifier), // The Resource Assembly Identifier to use
    ResourceManagerBaseName = "DevToys.RowVersionConverter.RowVersionConverterExtension",      // The full name (including namespace) of the resource file containing our localized texts
    ShortDisplayTitleResourceName = nameof(RowVersionConverterExtension.ShortDisplayTitle),    // The name of the resource to use for the short display title
    LongDisplayTitleResourceName = nameof(RowVersionConverterExtension.LongDisplayTitle),
    DescriptionResourceName = nameof(RowVersionConverterExtension.Description),
    AccessibleNameResourceName = nameof(RowVersionConverterExtension.AccessibleName))]
internal sealed class RowVersionConverterGui : IGuiTool
{
    private readonly IUIInfoBar _infoBar = InfoBar();
    private readonly IUISingleLineTextInput _base64Input = SingleLineTextInput();
    private readonly IUISingleLineTextInput _ulongInput = SingleLineTextInput();
    private readonly IUISingleLineTextInput _hexadecimalInput = SingleLineTextInput();
    private readonly IUISingleLineTextInput _byteArrayInput = SingleLineTextInput();

    private readonly IUISingleLineTextInput[] _inputs;
    private readonly RowVersionConverter _converter = new();

    private bool _isUpdating;

    public RowVersionConverterGui()
    {
        _inputs = [_base64Input, _ulongInput, _hexadecimalInput, _byteArrayInput];
    }

    public UIToolView View =>
        new UIToolView(
            Stack()
                .Vertical()
                .LargeSpacing()
                .WithChildren(

                    _base64Input
                        .Title("Base64")
                        .OnTextChanged(OnBase64TextChanged)
                        .CanCopyWhenEditable(),

                    _ulongInput
                        .Title("ULong")
                        .OnTextChanged(OnULongTextChanged)
                        .CanCopyWhenEditable(),

                    _hexadecimalInput
                        .Title("Hexadecimal")
                        .OnTextChanged(OnHexadecimalTextChanged)
                        .CanCopyWhenEditable(),

                     _byteArrayInput
                        .Title("Bytes")
                        .ReadOnly(),

                    _infoBar
                        .Close()
                        .NonClosable()
                        .Warning()

                ));

    public void OnDataReceived(string dataTypeName, object? parsedData) => throw new NotImplementedException();

    private void OnBase64TextChanged(string text)
    {
        if (_isUpdating)
        {
            return;
        }

        var result = _converter.ConvertFromBase64(text);
        UpdateUI(_base64Input, result);
    }

    private void OnULongTextChanged(string text)
    {
        if (_isUpdating)
        {
            return;
        }

        var result = _converter.ConvertFromULong(text);
        UpdateUI(_ulongInput, result);
    }

    private void OnHexadecimalTextChanged(string text)
    {
        if (_isUpdating)
        {
            return;
        }

        var result = _converter.ConvertFromHexadecimal(text);
        UpdateUI(_hexadecimalInput, result);
    }

    private void UpdateUI(IUISingleLineTextInput source, ConversionResult result)
    {
        _isUpdating = true;

        if (result.IsSuccess)
        {
            ShowResults(result);
        }
        else
        {
            ClearAllInputsExceptSource(source);
            ShowError(result.ErrorMessage);
        }

        _isUpdating = false;
    }

    private void ShowResults(ConversionResult result)
    {
        _base64Input.Text(result.Base64);
        _ulongInput.Text(result.ULong);
        _hexadecimalInput.Text(result.Hexadecimal);
        _byteArrayInput.Text(result.ByteArray);
        _infoBar.Close();
    }

    private void ClearAllInputsExceptSource(IUISingleLineTextInput source)
    {
        foreach (var input in _inputs)
        {
            if (input != source)
            {
                input.Text(string.Empty);
            }
        }
    }

    private void ShowError(string message)
    {
        _infoBar.Title(message);
        _infoBar.Open();
    }
}
