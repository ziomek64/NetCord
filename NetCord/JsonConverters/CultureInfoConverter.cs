﻿using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

public class CultureInfoConverter : JsonConverter<CultureInfo>
{
    public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override CultureInfo ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => new(reader.GetString()!);

    public override void WriteAsPropertyName(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString());
    }
}
