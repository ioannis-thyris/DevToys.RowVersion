using System.Runtime.CompilerServices;

namespace DevToys.RowVersionConverter;

/// <summary>
/// Handles row version conversion between different formats (Base64, ULong, Hexadecimal, and Byte Array).
/// </summary>
public class RowVersionConverter
{
    private const int ROWVERSION_BYTE_COUNT = 8;
    private const int MAX_HEX_LENGTH = 16;

    private const string ERROR_INVALID_BASE64 = "Invalid Base64 format";
    private const string ERROR_INVALID_ULONG = "Invalid ULong format";
    private const string ERROR_INVALID_HEXADECIMAL = "Invalid hexadecimal format";

    /// <summary>
    /// Converts a Base64 string to all other formats.
    /// </summary>
    /// <param name="base64">The Base64 string to convert.</param>
    /// <returns>A conversion result containing all formats or an error.</returns>
    public ConversionResult ConvertFromBase64(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
        {
            return ConversionResult.CreateEmpty();
        }

        Span<byte> rowVersionBytes = stackalloc byte[ROWVERSION_BYTE_COUNT];

        if (!Convert.TryFromBase64String(base64, rowVersionBytes, out int bytesWritten) || bytesWritten != ROWVERSION_BYTE_COUNT)
        {
            return ConversionResult.CreateFailed(InputType.Base64, base64, ERROR_INVALID_BASE64);
        }

        var hexadecimal = BuildHexString(rowVersionBytes);
        var byteArray = string.Join(", ", rowVersionBytes.ToArray());
        var ulongValue = GetULongValue(rowVersionBytes);

        return ConversionResult.CreateSuccessful(base64, ulongValue, hexadecimal, byteArray);
    }

    /// <summary>
    /// Converts a ULong string to all other formats.
    /// </summary>
    /// <param name="ulongValue">The ULong string to convert.</param>
    /// <returns>A conversion result containing all formats or an error.</returns>
    public ConversionResult ConvertFromULong(string ulongValue)
    {
        if (string.IsNullOrWhiteSpace(ulongValue))
        {
            return ConversionResult.CreateEmpty();
        }

        if (!ulong.TryParse(ulongValue, out ulong parsedUlong))
        {
            return ConversionResult.CreateFailed(InputType.ULong, ulongValue, ERROR_INVALID_ULONG);
        }

        Span<byte> rowVersionBytes = stackalloc byte[ROWVERSION_BYTE_COUNT];

        if (!BitConverter.TryWriteBytes(rowVersionBytes, parsedUlong))
        {
            return ConversionResult.CreateFailed(InputType.ULong, ulongValue, ERROR_INVALID_ULONG);
        }

        if (BitConverter.IsLittleEndian)
        {
            rowVersionBytes.Reverse();
        }

        var base64 = Convert.ToBase64String(rowVersionBytes);
        var hexadecimal = BuildHexString(rowVersionBytes);
        var byteArray = string.Join(", ", rowVersionBytes.ToArray());

        return ConversionResult.CreateSuccessful(base64, ulongValue, hexadecimal, byteArray);
    }

    /// <summary>
    /// Converts a hexadecimal string to all other formats.
    /// </summary>
    /// <param name="hexadecimal">The hexadecimal string to convert.</param>
    /// <returns>A conversion result containing all formats or an error.</returns>
    public ConversionResult ConvertFromHexadecimal(string hexadecimal)
    {
        if (string.IsNullOrWhiteSpace(hexadecimal))
        {
            return ConversionResult.CreateEmpty();
        }

        Span<char> cleanedHexChars = stackalloc char[MAX_HEX_LENGTH];

        if (!TryParseHexString(hexadecimal.AsSpan(), cleanedHexChars, out int charsWritten) || charsWritten != MAX_HEX_LENGTH)
        {
            return ConversionResult.CreateFailed(InputType.Hexadecimal, hexadecimal, ERROR_INVALID_HEXADECIMAL);
        }

        try
        {
            Span<byte> rowVersionBytes = stackalloc byte[ROWVERSION_BYTE_COUNT];
            var hexBytes = Convert.FromHexString(cleanedHexChars.Slice(0, charsWritten));
            hexBytes.CopyTo(rowVersionBytes);

            var base64 = Convert.ToBase64String(rowVersionBytes);
            var byteArray = string.Join(", ", rowVersionBytes.ToArray());
            var ulongValue = GetULongValue(rowVersionBytes);

            return ConversionResult.CreateSuccessful(base64, ulongValue, hexadecimal, byteArray);
        }
        catch (Exception)
        {
            return ConversionResult.CreateFailed(InputType.Hexadecimal, hexadecimal, ERROR_INVALID_HEXADECIMAL);
        }
    }

    /// <summary>
    /// Converts a span of bytes representing a row version into its string representation of an unsigned 64-bit
    /// integer.
    /// </summary>
    /// <remarks>If the system architecture is little-endian, the byte order of <paramref
    /// name="rowVersionBytes"/> is reversed before converting to an unsigned 64-bit integer.</remarks>
    /// <param name="rowVersionBytes">A span of bytes representing the row version. The span must contain exactly 8 bytes.</param>
    /// <returns>A string representation of the unsigned 64-bit integer value derived from the row version bytes.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetULongValue(Span<byte> rowVersionBytes)
    {
        // A local copy could be used here to avoid modifying the original span.
        if (BitConverter.IsLittleEndian)
        {
            rowVersionBytes.Reverse();
        }

        return BitConverter.ToUInt64(rowVersionBytes).ToString();
    }

    /// <summary>
    /// Attempts to parse a hexadecimal string and writes the cleaned hex characters to the destination span.
    /// </summary>
    /// <param name="source">The source hexadecimal string to parse.</param>
    /// <param name="destination">The destination span to write cleaned hex characters to.</param>
    /// <param name="charsWritten">The number of characters written to the destination.</param>
    /// <returns>True if the parsing was successful, false otherwise.</returns>
    private static bool TryParseHexString(ReadOnlySpan<char> source, Span<char> destination, out int charsWritten)
    {
        charsWritten = 0;

        var cleaned = source.Trim();

        if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            cleaned = cleaned.Slice(2);
        }

        Span<char> hexCharBuffer = stackalloc char[MAX_HEX_LENGTH];

        foreach (char c in cleaned)
        {
            if (c == ' ' || c == '-')
            {
                continue;
            }
            if (!char.IsAsciiHexDigit(c))
            {
                return false;
            }
            if (charsWritten >= hexCharBuffer.Length)
            {
                return false;
            }

            hexCharBuffer[charsWritten++] = c;
        }

        if (charsWritten != MAX_HEX_LENGTH || charsWritten % 2 != 0)
        {
            return false;
        }

        hexCharBuffer.Slice(0, charsWritten).CopyTo(destination);
        return true;
    }

    /// <summary>
    /// Builds a hexadecimal string representation of the given bytes with "0x" prefix.
    /// </summary>
    /// <param name="bytes">The bytes to convert to hexadecimal string.</param>
    /// <returns>A hexadecimal string representation of the bytes.</returns>
    private static string BuildHexString(ReadOnlySpan<byte> bytes)
    {
        Span<char> hexStringBuffer = stackalloc char[2 + (bytes.Length * 2)];

        hexStringBuffer[0] = '0';
        hexStringBuffer[1] = 'x';

        for (int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            hexStringBuffer[2 + (i * 2)] = GetHexChar(b >> 4); // High nibble
            hexStringBuffer[2 + (i * 2) + 1] = GetHexChar(b & 0xF); // Low nibble
        }

        return hexStringBuffer.ToString();
    }

    /// <summary>
    /// Converts a numeric value (0-15) to its corresponding hexadecimal character.
    /// </summary>
    /// <param name="value">The numeric value to convert (0-15).</param>
    /// <returns>The hexadecimal character representation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char GetHexChar(int value)
    {
        return (char)(value < 10 ? '0' + value : 'A' + value - 10);
    }
}