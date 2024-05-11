using System;
using System.Globalization;

public sealed class RMCNmeaParser : NmeaParser<RMCNmeaData>
{
    public override string ParseTypeName => "RMC";

    protected override RMCNmeaData ParseSentence(string value)
    {
        var result = new RMCNmeaData();
        string[] splitted = value.Split(',');

        for (int i = 0; i < splitted.Length; i++)
        {
            string cannotPartName = null;
            string part = splitted[i];

            switch (i)
            {
                case 1:
                    string time = string.Join(string.Empty, part.Split('.'));

                    if (TimeSpan.TryParseExact(time, "hhmmssff", null, out TimeSpan sendTime) == true)
                    {
                        result.SendDate = result.SendDate.Add(sendTime);
                    }
                    else
                    {
                        cannotPartName = nameof(sendTime);
                    }

                    break;

                case 3:
                    if (TryParseCoordinate(part, out float latitude) == true)
                    {
                        result.Latitude = latitude;
                    }
                    else
                    {
                        cannotPartName = nameof(latitude);
                    }

                    break;

                case 4:
                    if (part == "S")
                    {
                        result.Latitude *= -1;
                    }

                    break;

                case 5:
                    if (TryParseCoordinate(part, out float longitude) == true)
                    {
                        result.Longitude = longitude;
                    }
                    else
                    {
                        cannotPartName = nameof(longitude);
                    }

                    break;

                case 6:
                    if (part == "W")
                    {
                        result.Longitude *= -1;
                    }

                    break;

                case 9:
                    if (DateTime.TryParseExact(part, "ddMMyy", null, default, out DateTime sendDateTime) == true)
                    {
                        result.SendDate = sendDateTime.Add(result.SendDate.TimeOfDay);
                    }
                    else
                    {
                        cannotPartName = nameof(sendDateTime);
                    }

                    break;

                default:
                    continue;
            }

            if (cannotPartName != null)
            {
                CannotParseErrorMessage(cannotPartName, part, value);
            }
        }

        return result;
    }

    private static bool TryParseCoordinate(string stringValue, out float value)
    {
        if (TryParse(stringValue, out value) == false)
        {
            return false;
        }

        float minutes = value % 100f;
        float degrees = (value - minutes) / 100f;
        value = degrees + (minutes / 60f);

        return true;
    }

    private static bool TryParse(string stringValue, out float value)
    {
        return float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
    }
}