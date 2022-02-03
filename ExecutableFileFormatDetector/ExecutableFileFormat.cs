namespace Toolbelt;

public static class ExecutableFileFormat
{
    private const int MaxHeaderBytesLength = 4;

    public static ExecutableFileFormatType DetectFormat(string path)
    {
        if (new FileInfo(path).Length < MaxHeaderBytesLength) return ExecutableFileFormatType.Unknown;
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return DetectFormat(stream);
    }

    public static ExecutableFileFormatType DetectFormat(Stream stream)
    {
        Span<byte> DOSHeaderBytes = stackalloc byte[] { (byte)'M', (byte)'Z' };
        Span<byte> NEHeaderBytes = stackalloc byte[] { (byte)'P', (byte)'E', 0, 0 };
        Span<byte> PE32SignatureBytes = stackalloc byte[] { 0x4c, 0x01 };
        Span<byte> PE64SignatureBytes = stackalloc byte[] { 0x64, 0x86 };
        Span<byte> ELFHeaderBytes = stackalloc byte[] { 0x7f, (byte)'E', (byte)'L', (byte)'F' };
        Span<byte> MachOHeaderBytes = stackalloc byte[] { 0xcf, 0xfa, 0xed, 0xfe };

        Span<byte> headerBytes = stackalloc byte[MaxHeaderBytesLength];

        stream.Read(headerBytes);
        if (headerBytes[..2].SequenceEqual(DOSHeaderBytes))
        {
            Span<byte> buff = new byte[4];
            const int offsetOfNEHeaderOffset = 60;
            stream.Seek(offsetOfNEHeaderOffset, SeekOrigin.Begin);
            stream.Read(buff);
            var posOfNEHeader = BitConverter.ToInt32(buff);

            stream.Seek(posOfNEHeader, SeekOrigin.Begin);
            stream.Read(buff);
            if (!buff.SequenceEqual(NEHeaderBytes)) return ExecutableFileFormatType.Unknown;

            stream.Read(buff[..2]);
            if (buff[..2].SequenceEqual(PE32SignatureBytes)) return ExecutableFileFormatType.PE32;
            if (buff[..2].SequenceEqual(PE64SignatureBytes)) return ExecutableFileFormatType.PE64;

            return ExecutableFileFormatType.Unknown;
        }
        if (headerBytes[..4].SequenceEqual(ELFHeaderBytes)) return ExecutableFileFormatType.ELF;
        if (headerBytes[..4].SequenceEqual(MachOHeaderBytes)) return ExecutableFileFormatType.MachO;

        return ExecutableFileFormatType.Unknown;
    }
}
