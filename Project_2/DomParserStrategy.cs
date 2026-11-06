using System;
using System.Collections.Generic;
using System.Xml;

namespace Project_2;

public class DomParserStrategy : IXmlParserStrategy
{
    public List<Conference> Analyze(string xmlPath, FilterCriteria filter)
    {
        var results = new List<Conference>();
        var doc = new XmlDocument();
        doc.Load(xmlPath);

        XmlNodeList? nodes = doc.SelectNodes("//Conference");
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

            if (decimal.TryParse(node.Attributes["Fee"]?.Value, out decimal fee))
                conf.Fee = fee;

            XmlNode? details = node.SelectSingleNode("Details");
            if (details?.Attributes != null)
            {
                conf.Location = details.Attributes["Location"]?.Value ?? string.Empty;
                conf.StartDate = details.Attributes["StartDate"]?.Value ?? string.Empty;
                conf.Deadline = details.Attributes["Deadline"]?.Value ?? string.Empty;
            }

            bool matchField = string.IsNullOrEmpty(filter.SelectedField) || conf.Field.Equals(filter.SelectedField, StringComparison.OrdinalIgnoreCase);
            bool matchOrg = string.IsNullOrEmpty(filter.SelectedOrganizer) || conf.Organizer.Equals(filter.SelectedOrganizer, StringComparison.OrdinalIgnoreCase);
            bool matchKey = string.IsNullOrEmpty(filter.Keyword) || conf.Title.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) || conf.Location.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase);

            if (matchField && matchOrg && matchKey)
                results.Add(conf);
        }

        return results;
    }
}