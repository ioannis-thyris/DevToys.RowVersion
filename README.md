# DevToys.RowVersionConverter

A DevToys extension that converts a SQL Server [RowVersion](https://learn.microsoft.com/en-us/sql/t-sql/data-types/rowversion-transact-sql) (8-byte binary value) between:

- Base64 (8 bytes encoded)
- Unsigned 64-bit integer (ulong)
- Hexadecimal (16 hex chars; optional 0x; spaces or dashes allowed)
- Byte list (comma‑separated decimal bytes)

## Behavior
- Empty or whitespace input returns empty outputs.
- Invalid input clears the other fields and shows a warning.
- Hex parsing ignores spaces and dashes and accepts upper or lower case.
- Numeric/Base64/hex kept consistent via endian normalization.

## Installation
1. Open DevToys v2.
2. Open the `Manage Extensions` panel.
3. Click on `Find more extensions online`.
4. Find the "DevToys.RowVersionConverter" package and download it.
5. Back in DevToys click on `Install an extension`.
4. Locate the downloaded `nupkg` file.
5. Find the tool under Converters.

## Usage
Enter a value in one of the editable fields (Base64, ULong, Hex) to convert to the other formats. Bytes field is read-only.
