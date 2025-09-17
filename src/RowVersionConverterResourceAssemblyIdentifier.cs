using DevToys.Api;
using System.ComponentModel.Composition;

namespace DevToys.RowVersionConverter;

[Export(typeof(IResourceAssemblyIdentifier))]
[Name(nameof(RowVersionConverterResourceAssemblyIdentifier))]
internal sealed class RowVersionConverterResourceAssemblyIdentifier : IResourceAssemblyIdentifier
{
    public ValueTask<FontDefinition[]> GetFontDefinitionsAsync() => throw new NotImplementedException();
}
