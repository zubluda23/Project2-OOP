using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Project_2
{
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

        public List<Conference> FilterConferences(string xmlPath, FilterCriteria filter)
        {
            if (currentStrategy == null)
                return new List<Conference>();

            return currentStrategy.Analyze(xmlPath, filter);
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

        public void ExportToHtml(string xmlPath, string xslPath, string outputPath)
        {
            xsltTransformer.TransformToHtml(xmlPath, xslPath, outputPath);
        }
    }
}