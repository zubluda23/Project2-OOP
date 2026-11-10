using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Project_2;

public class SaxParserStrategy : IXmlParserStrategy
{
    public List<Conference> Analyze(string xmlPath, FilterCriteria filter)
    {
        var results = new List<Conference>();
        var settings = new XmlReaderSettings { IgnoreWhitespace = true };

        using var reader = XmlReader.Create(xmlPath, settings);
        Conference? currentConf = null;

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Conference")
            {
                currentConf = new Conference
                {
                    Title = reader.GetAttribute("Title") ?? string.Empty,
                    Field = reader.GetAttribute("Field") ?? string.Empty,
                    Organizer = reader.GetAttribute("Organizer") ?? string.Empty
                };
               
                string? feeStr = reader.GetAttribute("Fee");
                if (decimal.TryParse(feeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedFee))
                {
                    currentConf.Fee = parsedFee;
                }

                if (reader.IsEmptyElement)
                {
                    if (IsMatch(currentConf, filter))
                    {
                        results.Add(currentConf);
                    }
                    currentConf = null;
                }
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Details" && currentConf != null)
            {
                currentConf.Location = reader.GetAttribute("Location") ?? string.Empty;
                currentConf.StartDate = reader.GetAttribute("StartDate") ?? string.Empty;
                currentConf.Deadline = reader.GetAttribute("Deadline") ?? string.Empty;
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Conference" && currentConf != null)
            {
                if (IsMatch(currentConf, filter))
                {
                    results.Add(currentConf);
                }
                currentConf = null;
            }
        }

        return results;
    }

    private static bool IsMatch(Conference c, FilterCriteria f)
    {
        bool matchField = string.IsNullOrEmpty(f.SelectedField) || c.Field.Equals(f.SelectedField, StringComparison.OrdinalIgnoreCase);
        bool matchOrg = string.IsNullOrEmpty(f.SelectedOrganizer) || c.Organizer.Equals(f.SelectedOrganizer, StringComparison.OrdinalIgnoreCase);
        bool matchKey = string.IsNullOrEmpty(f.Keyword) || c.Title.Contains(f.Keyword, StringComparison.OrdinalIgnoreCase) || c.Location.Contains(f.Keyword, StringComparison.OrdinalIgnoreCase);

        return matchField && matchOrg && matchKey;
    }
}