using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Project_2;

public class DomParserStrategy : IXmlParserStrategy
{
    public List<Conference> Analyze(string xmlPath, FilterCriteria filter)
    {
        var results = new List<Conference>();
        var doc = new XmlDocument();
        doc.Load(xmlPath);

        var xpathConditions = new List<string>();

        if (!string.IsNullOrWhiteSpace(filter.SelectedField))
        {
            xpathConditions.Add($"@Field=\"{filter.SelectedField}\"");
        }

        if (!string.IsNullOrWhiteSpace(filter.SelectedOrganizer))
        {
            xpathConditions.Add($"@Organizer=\"{filter.SelectedOrganizer}\"");
        }

        string xpath = "//Conference";
        if (xpathConditions.Count > 0)
        {
            xpath += "[" + string.Join(" and ", xpathConditions) + "]";
        }

        XmlNodeList? nodes = doc.SelectNodes(xpath);
        if (nodes == null) return results;

        foreach (XmlNode node in nodes)
        {
            if (node.Attributes == null) continue;

            var conf = new Conference
            {
                Title = node.Attributes["Title"]?.Value ?? string.Empty,
                Field = node.Attributes["Field"]?.Value ?? string.Empty,
                Organizer = node.Attributes["Organizer"]?.Value ?? string.Empty
            };

            string? feeStr = node.Attributes["Fee"]?.Value;
            if (decimal.TryParse(feeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fee))
            {
                conf.Fee = fee;
            }

            XmlNode? details = node.SelectSingleNode("Details");
            if (details?.Attributes != null)
            {
                conf.Location = details.Attributes["Location"]?.Value ?? string.Empty;
                conf.StartDate = details.Attributes["StartDate"]?.Value ?? string.Empty;
                conf.Deadline = details.Attributes["Deadline"]?.Value ?? string.Empty;
            }

            bool matchKey = string.IsNullOrEmpty(filter.Keyword) ||
                            conf.Title.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) ||
                            conf.Location.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase);

            if (matchKey)
            {
                results.Add(conf);
            }
        }

        return results;
    }
}