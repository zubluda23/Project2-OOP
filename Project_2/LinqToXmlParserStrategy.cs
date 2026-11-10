using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Project_2;

public class LinqToXmlParserStrategy : IXmlParserStrategy
{
    public List<Conference> Analyze(string xmlPath, FilterCriteria filter)
    {
        var doc = XDocument.Load(xmlPath);

       
        IEnumerable<XElement> query = doc.Descendants("Conference");

      
        if (!string.IsNullOrWhiteSpace(filter.SelectedField))
        {
            query = query.Where(elem => string.Equals((string?)elem.Attribute("Field"), filter.SelectedField, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.SelectedOrganizer))
        {
            query = query.Where(elem => string.Equals((string?)elem.Attribute("Organizer"), filter.SelectedOrganizer, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(elem =>
            {
                string title = (string?)elem.Attribute("Title") ?? string.Empty;
                string location = (string?)elem.Element("Details")?.Attribute("Location") ?? string.Empty;
                return title.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) ||
                       location.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase);
            });
        }


        return query.Select(elem =>
        {
            var details = elem.Element("Details");
            string? feeStr = (string?)elem.Attribute("Fee");
            decimal.TryParse(feeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fee);

            return new Conference
            {
                Title = (string?)elem.Attribute("Title") ?? string.Empty,
                Field = (string?)elem.Attribute("Field") ?? string.Empty,
                Organizer = (string?)elem.Attribute("Organizer") ?? string.Empty,
                Fee = fee,
                Location = (string?)details?.Attribute("Location") ?? string.Empty,
                StartDate = (string?)details?.Attribute("StartDate") ?? string.Empty,
                Deadline = (string?)details?.Attribute("Deadline") ?? string.Empty
            };
        }).ToList();
    }
}