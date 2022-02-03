# ExecutableFileFormat Detector [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.ExecutableFileFormatDetector.svg)](https://www.nuget.org/packages/Toolbelt.ExecutableFileFormatDetector/)

## Summary

This is a .NET class library that detects PE32, PE64, ELF, and Mach-O executable file formats.

## Compatibility

.NET Standard 2.1 (compatible with .NET5,6, or later)

## Usage

1. Install this library to your project via NuGet.

```shell
> dotnet add package ExecutableFileFormatDetector
```

2. Call the `DetectFormat()` static method in the `ExecutableFileFormat` class.

```csharp
using Toolbelt;
...
var format = ExecutableFileFormat.DetectFormat(@"C:\bin\foo.exe");
// 👆 "format" might be ExecutableFileFormatType.PE32 or PE64.
```


## Release Notes

[Release notes](https://github.com/jsakamoto/Toolbelt.ExecutableFileFormatDetector/blob/master/RELEASE-NOTES.txt)

## License

[Mozilla Public License, version 2.0](https://github.com/jsakamoto/Toolbelt.ExecutableFileFormatDetector/blob/master/LICENSE)
