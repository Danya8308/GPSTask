using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class NmeaParser<TData>
{
    public const char FirstChar = '$';
    public const string AnyType = "ANY";

    public virtual string ParseTypeName => AnyType;

    public IEnumerable<TData> ParseFromFile(TextAsset file)
    {
        return ParseMany(file.text);
    }

    public IEnumerable<TData> ParseMany(string sentences)
    {
        if (sentences.StartsWith(FirstChar) == false)
        {
            throw GetCannotParseErrorException(sentences);
        }

        foreach (var value in sentences.Split(FirstChar, StringSplitOptions.RemoveEmptyEntries))
        {
            if (TryParseSentence(value, false, out TData data) == true)
            {
                yield return data;
            }
        }
    }

    public TData Parse(string sentence)
    {
        if (TryParse(sentence, out TData data) == true)
        {
            return data;
        }

        throw GetCannotParseErrorException(sentence);
    }

    public bool TryParse(string sentence, out TData value)
    {
        return TryParseSentence(sentence, true, out value);
    }

    protected Exception GetCannotParseErrorException(string sentence)
    {
        return new Exception($"Неверный формат предложения: \"{sentence}\". {nameof(ParseTypeName)} - {ParseTypeName}.");
    }

    protected static void CannotParseErrorMessage(string fieldName, string part, string sentence)
    {
        Debug.LogError($"Невозможно извечь {fieldName} из части \"{part}\" предложения \"{sentence}\"");
    }

    protected abstract TData ParseSentence(string value);

    private bool TryParseSentence(string sentence, bool startsWithDollar, out TData value)
    {
        value = default;

        if (string.IsNullOrEmpty(ParseTypeName) == true || (startsWithDollar == true && sentence.StartsWith(FirstChar) == false))
        {
            return false;
        }

        bool result = TryParseDataType(sentence, out string currentDataType);

        if (result == false || (ParseTypeName != AnyType && currentDataType != ParseTypeName))
        {
            return false;
        }

        value = ParseSentence(sentence);

        return true;
    }

    private static bool TryParseDataType(string sentence, out string value)
    {
        if (sentence.Length < 5)
        {
            value = null;
            return false;
        }

        value = sentence.Substring(2, 3);
        return true;
    }
}