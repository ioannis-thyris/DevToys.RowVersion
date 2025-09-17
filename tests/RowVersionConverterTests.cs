namespace DevToys.RowVersionConverter.Tests;

public class RowVersionConverterTests
{
    private readonly RowVersionConverter _converter = new();

    #region ConvertFromBase64 Tests

    [Theory]
    [InlineData("AAAAAAKVTXs=", "43339131", "0x0000000002954D7B")]
    [InlineData("AAAAAAAKN2g=", "669544", "0x00000000000A3768")]
    [InlineData("AAAAAAAEzRE=", "314641", "0x000000000004CD11")]
    [InlineData("AAAAACn1h7k=", "703956921", "0x0000000029F587B9")]
    [InlineData("AAAAAB1Mjn8=", "491556479", "0x000000001D4C8E7F")]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidBase64IsProvided(string base64Input, string expectedULong, string expectedHex)
    {
        // Act
        var result = _converter.ConvertFromBase64(base64Input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(base64Input, result.Base64);
        Assert.Equal(expectedULong, result.ULong);
        Assert.Equal(expectedHex, result.Hexadecimal);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenMaxUInt64Base64IsProvided()
    {
        // Arrange
        string base64Input = "//////////8="; // Max UInt64 value

        // Act
        var result = _converter.ConvertFromBase64(base64Input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(base64Input, result.Base64);
        Assert.Equal("18446744073709551615", result.ULong); // Max UInt64
        Assert.Equal("0xFFFFFFFFFFFFFFFF", result.Hexadecimal);
        Assert.Equal("255, 255, 255, 255, 255, 255, 255, 255", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenZeroBase64IsProvided()
    {
        // Arrange
        string base64Input = "AAAAAAAAAAA="; // Zero value

        // Act
        var result = _converter.ConvertFromBase64(base64Input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(base64Input, result.Base64);
        Assert.Equal("0", result.ULong);
        Assert.Equal("0x0000000000000000", result.Hexadecimal);
        Assert.Equal("0, 0, 0, 0, 0, 0, 0, 0", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenInvalidBase64IsProvided()
    {
        // Arrange
        string invalidBase64 = "InvalidBase64!";

        // Act
        var result = _converter.ConvertFromBase64(invalidBase64);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(invalidBase64, result.Base64);
        Assert.Empty(result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid Base64 format", result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenBase64HasIncorrectLength()
    {
        // Arrange
        string shortBase64 = "AAA="; // Too short (4 bytes instead of 8)

        // Act
        var result = _converter.ConvertFromBase64(shortBase64);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(shortBase64, result.Base64);
        Assert.Empty(result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid Base64 format", result.ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldReturnEmptyResult_WhenBase64IsNullOrWhitespace(string input)
    {
        // Arrange & Act
        var result = _converter.ConvertFromBase64(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Empty(result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    #endregion

    #region ConvertFromULong Tests

    [Theory]
    [InlineData("43339131", "AAAAAAKVTXs=", "0x0000000002954D7B")]
    [InlineData("669544", "AAAAAAAKN2g=", "0x00000000000A3768")]
    [InlineData("314641", "AAAAAAAEzRE=", "0x000000000004CD11")]
    [InlineData("703956921", "AAAAACn1h7k=", "0x0000000029F587B9")]
    [InlineData("491556479", "AAAAAB1Mjn8=", "0x000000001D4C8E7F")]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidULongIsProvided(string ulongInput, string expectedBase64, string expectedHex)
    {
        // Act
        var result = _converter.ConvertFromULong(ulongInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedBase64, result.Base64);
        Assert.Equal(ulongInput, result.ULong);
        Assert.Equal(expectedHex, result.Hexadecimal);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenZeroULongIsProvided()
    {
        // Arrange
        string ulongInput = "0";

        // Act
        var result = _converter.ConvertFromULong(ulongInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("AAAAAAAAAAA=", result.Base64);
        Assert.Equal(ulongInput, result.ULong);
        Assert.Equal("0x0000000000000000", result.Hexadecimal);
        Assert.Equal("0, 0, 0, 0, 0, 0, 0, 0", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenMaxUInt64ULongIsProvided()
    {
        // Arrange
        string ulongInput = ulong.MaxValue.ToString();

        // Act
        var result = _converter.ConvertFromULong(ulongInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("//////////8=", result.Base64);
        Assert.Equal(ulongInput, result.ULong);
        Assert.Equal("0xFFFFFFFFFFFFFFFF", result.Hexadecimal);
        Assert.Equal("255, 255, 255, 255, 255, 255, 255, 255", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenInvalidULongIsProvided()
    {
        // Arrange
        string invalidUlong = "NotANumber";

        // Act
        var result = _converter.ConvertFromULong(invalidUlong);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Equal(invalidUlong, result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid ULong format", result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenNegativeNumberIsProvided()
    {
        // Arrange
        string negativeUlong = "-1";

        // Act
        var result = _converter.ConvertFromULong(negativeUlong);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Equal(negativeUlong, result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid ULong format", result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenULongExceedsMaxValue()
    {
        // Arrange
        string oversizedUlong = "18446744073709551616"; // Max UInt64 + 1

        // Act
        var result = _converter.ConvertFromULong(oversizedUlong);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Equal(oversizedUlong, result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid ULong format", result.ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldReturnEmptyResult_WhenULongIsNullOrWhitespace(string input)
    {
        // Arrange & Act
        var result = _converter.ConvertFromULong(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Empty(result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    #endregion

    #region ConvertFromHexadecimal Tests

    [Theory]
    [InlineData("0x0000000002954D7B", "AAAAAAKVTXs=", "43339131")]
    [InlineData("0x00000000000A3768", "AAAAAAAKN2g=", "669544")]
    [InlineData("0x000000000004CD11", "AAAAAAAEzRE=", "314641")]
    [InlineData("0x0000000029F587B9", "AAAAACn1h7k=", "703956921")]
    [InlineData("0x000000001D4C8E7F", "AAAAAB1Mjn8=", "491556479")]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidHexWithPrefixIsProvided(string hexInput, string expectedBase64, string expectedULong)
    {
        // Act
        var result = _converter.ConvertFromHexadecimal(hexInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedBase64, result.Base64);
        Assert.Equal(expectedULong, result.ULong);
        Assert.Equal(hexInput, result.Hexadecimal);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidHexWithoutPrefixIsProvided()
    {
        // Arrange
        string hexInput = "0000000002954D7B";

        // Act
        var result = _converter.ConvertFromHexadecimal(hexInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("AAAAAAKVTXs=", result.Base64);
        Assert.Equal("43339131", result.ULong);
        Assert.Equal(hexInput, result.Hexadecimal);
        Assert.Equal("0, 0, 0, 0, 2, 149, 77, 123", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidHexWithUppercaseIsProvided()
    {
        // Arrange
        string hexInput = "0xFFFFFFFFFFFFFFFF";

        // Act
        var result = _converter.ConvertFromHexadecimal(hexInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("//////////8=", result.Base64);
        Assert.Equal("18446744073709551615", result.ULong);
        Assert.Equal(hexInput, result.Hexadecimal);
        Assert.Equal("255, 255, 255, 255, 255, 255, 255, 255", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidHexWithLowercaseIsProvided()
    {
        // Arrange
        string hexInput = "0xffffffffffffffff";

        // Act
        var result = _converter.ConvertFromHexadecimal(hexInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("//////////8=", result.Base64);
        Assert.Equal("18446744073709551615", result.ULong);
        Assert.Equal(hexInput, result.Hexadecimal);
        Assert.Equal("255, 255, 255, 255, 255, 255, 255, 255", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidHexWithSpacesIsProvided()
    {
        // Arrange
        string hexInput = "0x00 00 00 00 02 95 4D 7B";

        // Act
        var result = _converter.ConvertFromHexadecimal(hexInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("AAAAAAKVTXs=", result.Base64);
        Assert.Equal("43339131", result.ULong);
        Assert.Equal(hexInput, result.Hexadecimal);
        Assert.Equal("0, 0, 0, 0, 2, 149, 77, 123", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenValidHexWithDashesIsProvided()
    {
        // Arrange
        string hexInput = "0x00-00-00-00-02-95-4D-7B";

        // Act
        var result = _converter.ConvertFromHexadecimal(hexInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("AAAAAAKVTXs=", result.Base64);
        Assert.Equal("43339131", result.ULong);
        Assert.Equal(hexInput, result.Hexadecimal);
        Assert.Equal("0, 0, 0, 0, 2, 149, 77, 123", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnSuccessfulConversionWithAllFormats_WhenZeroHexIsProvided()
    {
        // Arrange
        string hexInput = "0x0000000000000000";

        // Act
        var result = _converter.ConvertFromHexadecimal(hexInput);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("AAAAAAAAAAA=", result.Base64);
        Assert.Equal("0", result.ULong);
        Assert.Equal(hexInput, result.Hexadecimal);
        Assert.Equal("0, 0, 0, 0, 0, 0, 0, 0", result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenInvalidHexCharactersAreProvided()
    {
        // Arrange
        string invalidHex = "0x123XYZ456789123";

        // Act
        var result = _converter.ConvertFromHexadecimal(invalidHex);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Empty(result.ULong);
        Assert.Equal(invalidHex, result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid hexadecimal format", result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenHexIsTooShort()
    {
        // Arrange
        string shortHex = "0x1234";

        // Act
        var result = _converter.ConvertFromHexadecimal(shortHex);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Empty(result.ULong);
        Assert.Equal(shortHex, result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid hexadecimal format", result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenHexIsTooLong()
    {
        // Arrange
        string longHex = "0x000000001404F3B1FF";

        // Act
        var result = _converter.ConvertFromHexadecimal(longHex);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Empty(result.ULong);
        Assert.Equal(longHex, result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid hexadecimal format", result.ErrorMessage);
    }

    [Fact]
    public void ShouldReturnErrorResult_WhenHexHasOddNumberOfCharacters()
    {
        // Arrange
        string oddHex = "0x00000001404F3B"; // 15 characters (odd)

        // Act
        var result = _converter.ConvertFromHexadecimal(oddHex);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Empty(result.ULong);
        Assert.Equal(oddHex, result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Equal("Invalid hexadecimal format", result.ErrorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldReturnEmptyResult_WhenHexadecimalIsNullOrWhitespace(string input)
    {
        // Arrange & Act
        var result = _converter.ConvertFromHexadecimal(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Base64);
        Assert.Empty(result.ULong);
        Assert.Empty(result.Hexadecimal);
        Assert.Empty(result.ByteArray);
        Assert.Null(result.ErrorMessage);
    }

    #endregion

}