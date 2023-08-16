﻿namespace NetCord.Gateway.Compression;

public interface IGatewayCompression : IDisposable
{
    public string Name { get; }

    public ReadOnlySpan<byte> Decompress(ReadOnlyMemory<byte> payload);
}
