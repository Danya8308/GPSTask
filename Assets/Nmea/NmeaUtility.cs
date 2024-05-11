using System;
using System.Collections.Generic;
using UnityEngine;

public static class NmeaUtility<TData>
{
    private static readonly Dictionary<Type, NmeaParser<TData>> _cachedParsers = new Dictionary<Type, NmeaParser<TData>>();

    public static IEnumerable<TData> ParseFromFile<TParser>(TextAsset file)
        where TParser : NmeaParser<TData>, new()
    {
        return GetOrInit<TParser>().ParseFromFile(file);
    }

    public static IEnumerable<TData> ParseMany<TParser>(string sentences)
        where TParser : NmeaParser<TData>, new()
    {
        return GetOrInit<TParser>().ParseMany(sentences);
    }

    public static TData Parse<TParser>(string sentence)
        where TParser : NmeaParser<TData>, new()
    {
        return GetOrInit<TParser>().Parse(sentence);
    }

    public static bool TryParse<TParser>(string sentence, out TData value)
        where TParser : NmeaParser<TData>, new()
    {
        return GetOrInit<TParser>().TryParse(sentence, out value);
    }

    private static TParser GetOrInit<TParser>()
        where TParser : NmeaParser<TData>, new()
    {
        Type parserType = typeof(TParser);

        if (_cachedParsers.TryGetValue(parserType, out NmeaParser<TData> value) == false)
        {
            _cachedParsers[parserType] = value = new TParser();
        }

        return value as TParser;
    }
}