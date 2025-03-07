﻿namespace NetCord.Gateway.Voice;

public class VoiceReceiveEventArgs
{
    public VoiceReceiveEventArgs(uint ssrc, ulong userId, ReadOnlyMemory<byte> frame)
    {
        Ssrc = ssrc;
        UserId = userId;
        Frame = frame;
    }

    public uint Ssrc { get; }

    public ulong UserId { get; }

    /// <summary>
    /// Opus frame.
    /// </summary>
    public ReadOnlyMemory<byte> Frame { get; }
}
