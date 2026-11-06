using System.Collections.Generic;

namespace Project_2;

public interface IXmlParserStrategy
{
    List<Conference> Analyze(string xmlPath, FilterCriteria filter);
}