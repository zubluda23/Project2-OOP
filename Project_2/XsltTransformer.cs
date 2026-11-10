using System.Xml.Xsl;

namespace Project_2;

public class XsltTransformer
{

    public bool TransformToHtml(string xmlPath, string xslPath, string outputHtmlPath)
    {
        var transform = new XslCompiledTransform();
        transform.Load(xslPath);
        transform.Transform(xmlPath, outputHtmlPath);
        return true;
    }
}