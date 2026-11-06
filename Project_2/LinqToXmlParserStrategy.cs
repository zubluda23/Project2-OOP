using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Project_2;

public class LinqToXmlParserStrategy : IXmlParserStrategy
{
    public List<Conference> Analyze(string xmlPath, FilterCriteria filter)
    {
        var doc = XDocument.Load(xmlPath);

        var query = from elem in doc.Descendants("Conference")
                    let details = elem.Element("Details")
                    let title = (string?)elem.Attribute("Title") ?? string.Empty
                    let field = (string?)elem.Attribute("Field") ?? string.Empty
                    let organizer = (string?)elem.Attribute("Organizer") ?? string.Empty
                    let fee = (decimal?)elem.Attribute("Fee") ?? 0m
                    let location = (string?)details?.Attribute("Location") ?? string.Empty
                    let startDate = (string?)details?.Attribute("StartDate") ?? string.Empty
                    let deadline = (string?)details?.Attribute("Deadline") ?? string.Empty
                    where (string.IsNullOrEmpty(filter.SelectedField) || field.Equals(filter.SelectedField, StringComparison.OrdinalIgnoreCase))
                       && (string.IsNullOrEmpty(filter.SelectedOrganizer) || organizer.Equals(filter.SelectedOrganizer, StringComparison.OrdinalIgnoreCase))
                       && (string.IsNullOrEmpty(filter.Keyword) || title.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase) || location.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase))
                    select new Conference
                    {
                        Title = title,
                        Field = field,
                        Organizer = organizer,
                        Fee = fee,
                        Location = location,
                        StartDate = startDate,
                        Deadline = deadline
                    };

        return query.ToList();
    }
}