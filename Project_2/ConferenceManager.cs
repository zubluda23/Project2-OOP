using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Project_2;

public class ConferenceManager
{
    private IXmlParserStrategy? currentStrategy;
    private readonly XsltTransformer xsltTransformer;

    public ConferenceManager()
    {
        xsltTransformer = new XsltTransformer();
    }

    public void SetStrategy(IXmlParserStrategy strategy)
    {
        currentStrategy = strategy;
    }

   
    public void SetStrategyBySelection(bool isSax, bool isDom)
    {
        if (isSax)
            currentStrategy = new SaxParserStrategy();
        else if (isDom)
            currentStrategy = new DomParserStrategy();
        else
            currentStrategy = new LinqToXmlParserStrategy();
    }

    public List<Conference> FilterConferences(string xmlPath, FilterCriteria filter)
    {
        if (currentStrategy == null)
            return new List<Conference>();

        return currentStrategy.Analyze(xmlPath, filter);
    }

  
    public string FormatSearchResults(string xmlPath, FilterCriteria filter)
    {
        var results = FilterConferences(xmlPath, filter);
        if (results.Count == 0)
        {
            return "За заданими критеріями конференцій не знайдено.";
        }

        var sb = new StringBuilder();
        sb.AppendLine($"Знайдено результатів: {results.Count}\n");
        foreach (var item in results)
        {
            sb.AppendLine(item.GetInfo());
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public (List<string> Fields, List<string> Organizers) LoadAttributes(string xmlPath)
    {
        var doc = XDocument.Load(xmlPath);

        var fields = doc.Descendants("Conference")
                        .Select(x => (string?)x.Attribute("Field"))
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Distinct()
                        .Select(s => s!)
                        .ToList();

        var organizers = doc.Descendants("Conference")
                            .Select(x => (string?)x.Attribute("Organizer"))
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Distinct()
                            .Select(s => s!)
                            .ToList();

        return (fields, organizers);
    }


    public bool ExportToHtml(string xmlPath, out string htmlPath)
    {
        string baseDir = AppContext.BaseDirectory;
        string xslPath = Path.Combine(baseDir, "transform.xsl");

        string outputFolder = Path.GetDirectoryName(xmlPath) ?? baseDir;
        htmlPath = Path.Combine(outputFolder, "ConferencesReport.html");

        if (!File.Exists(xslPath))
        {
            throw new FileNotFoundException($"Файл transform.xsl не знайдено за шляхом:\n{xslPath}");
        }

        return xsltTransformer.TransformToHtml(xmlPath, xslPath, htmlPath);
    }
}