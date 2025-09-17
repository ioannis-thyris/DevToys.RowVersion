using System.Diagnostics.CodeAnalysis;

namespace DevToys.RowVersionConverter;

/// <summary>
/// Result of a conversion operation.
/// </summary>
public readonly struct ConversionResult
{
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public bool IsSuccess { get; init; }
    public string Base64 { get; init; } = string.Empty;
    public string ULong { get; init; } = string.Empty;
    public string Hexadecimal { get; init; } = string.Empty;
    public string ByteArray { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }

    private ConversionResult(bool isSuccess, string base64, string ulongValue, string hexadecimal, string byteArray, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Base64 = base64;
        ULong = ulongValue;
        Hexadecimal = hexadecimal;
        ByteArray = byteArray;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates an empty <see cref="ConversionResult"/> instance with default values.
    /// </summary>
    /// <returns>A <see cref="ConversionResult"/> instance initialized with default values, indicating a conversion
    /// with no associated data or messages.</returns>
    public static ConversionResult CreateEmpty()
    {
        return new ConversionResult(true, string.Empty, string.Empty, string.Empty, string.Empty, null);
    }

    /// <summary>
    /// Creates a successful <see cref="ConversionResult"/> instance with the specified conversion values.
    /// </summary>
    /// <param name="base64">The Base64 representation of the converted value.</param>
    /// <param name="ulongValue">The unsigned long integer representation of the converted value.</param>
    /// <param name="hexadecimal">The hexadecimal representation of the converted value.</param>
    /// <param name="byteArray">The byte array representation of the converted value.</param>
    /// <returns>A <see cref="ConversionResult"/> instance indicating a successful conversion with the provided values.</returns>
    public static ConversionResult CreateSuccessful(string base64, string ulongValue, string hexadecimal, string byteArray)
    {
        return new ConversionResult(true, base64, ulongValue, hexadecimal, byteArray, null);
    }

    /// <summary>
    /// Creates a failed <see cref="ConversionResult"/> instance with the specified input type, original input, and
    /// error message.
    /// </summary>
    /// <param name="inputType">The type of the input that caused the failure. Must be one of the defined <see cref="InputType"/> values.</param>
    /// <param name="originalInput">The original input value that could not be successfully converted. This value is used to populate the
    /// corresponding field in the result.</param>
    /// <param name="errorMessage">A message describing the reason for the failure.</param>
    /// <returns>A <see cref="ConversionResult"/> instance representing a failed conversion. The result will have 
    /// <see cref="ConversionResult.IsSuccess"/> set to <see langword="false"/>, and the appropriate field (Base64,
    /// ULong, or Hexadecimal) populated based on the specified <paramref name="inputType"/>.</returns>
    public static ConversionResult CreateFailed(InputType inputType, string originalInput, string errorMessage)
    {
        var base64 = string.Empty;
        var uLong = string.Empty;
        var hexadecimal = string.Empty;

        switch (inputType)
        {
            case InputType.Base64:
                base64 = originalInput;
                break;
            case InputType.ULong:
                uLong = originalInput;
                break;
            case InputType.Hexadecimal:
                hexadecimal = originalInput;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputType), inputType, $"Invalid {nameof(InputType)} was provided");
        }

        return new ConversionResult(false, base64, uLong, hexadecimal, string.Empty, errorMessage);
    }
}

/// <summary>
/// Enumeration of input types for conversion operations.
/// </summary>
public enum InputType
{
    Base64,
    ULong,
    Hexadecimal
}
