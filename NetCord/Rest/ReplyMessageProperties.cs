﻿namespace NetCord.Rest;

public partial class ReplyMessageProperties
{
    public string? Content { get; set; }
    public NonceProperties? Nonce { get; set; }
    public bool Tts { get; set; }
    public IEnumerable<AttachmentProperties>? Attachments { get; set; }
    public IEnumerable<EmbedProperties>? Embeds { get; set; }
    public AllowedMentionsProperties? AllowedMentions { get; set; }
    public bool? FailIfNotExists { get; set; }
    public IEnumerable<ComponentProperties>? Components { get; set; }
    public IEnumerable<ulong>? StickerIds { get; set; }
    public MessageFlags? Flags { get; set; }

    public MessageProperties ToMessageProperties(ulong messageReferenceId)
    {
        return new()
        {
            Content = Content,
            Nonce = Nonce,
            Tts = Tts,
            Attachments = Attachments,
            Embeds = Embeds,
            AllowedMentions = AllowedMentions ?? new(),
            MessageReference = new(messageReferenceId, FailIfNotExists.GetValueOrDefault(true)),
            Components = Components,
            StickerIds = StickerIds,
            Flags = Flags,
        };
    }

    public static implicit operator ReplyMessageProperties(string content) => new()
    {
        Content = content,
    };
}
