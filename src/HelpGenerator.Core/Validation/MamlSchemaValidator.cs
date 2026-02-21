using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace HelpGenerator.Core.Validation
{
    public sealed record SchemaValidationIssue(
        XmlSeverityType Severity,
        string Message,
        int LineNumber,
        int LinePosition
    );

    public sealed record SchemaValidationResult(
        bool IsValid,
        IReadOnlyList<SchemaValidationIssue> Issues
    );

    public static class MamlSchemaValidator
    {
        public static XmlSchemaSet LoadSchemaSetFromFolder(string schemaFolder)
        {
            if (string.IsNullOrWhiteSpace(schemaFolder))
                throw new ArgumentException("Schema folder must be provided.", nameof(schemaFolder));

            if (!Directory.Exists(schemaFolder))
                throw new DirectoryNotFoundException($"Schema folder not found: {schemaFolder}");

            var entry = Path.Combine(schemaFolder, "Maml.xsd");
            if (!File.Exists(entry))
                throw new FileNotFoundException("Maml.xsd not found in schema folder.", entry);

            var set = new XmlSchemaSet
            {
                // allow xsd:include resolution from disk
                XmlResolver = new XmlUrlResolver()
            };

            using var reader = XmlReader.Create(entry, new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit
            });

            set.Add(null, reader);
            set.Compile();

            return set;
        }

        public static SchemaValidationResult Validate(XDocument doc, XmlSchemaSet schemaSet)
        {
            if (doc is null) throw new ArgumentNullException(nameof(doc));
            if (schemaSet is null) throw new ArgumentNullException(nameof(schemaSet));

            var issues = new List<SchemaValidationIssue>();

            doc.Validate(schemaSet, (o, e) =>
            {
                var ex = e.Exception;

                issues.Add(new SchemaValidationIssue(
                    e.Severity,
                    e.Message,
                    ex?.LineNumber ?? 0,
                    ex?.LinePosition ?? 0
                ));
            }, addSchemaInfo: true);

            var isValid = issues.All(i => i.Severity != XmlSeverityType.Error);
            return new SchemaValidationResult(isValid, issues);
        }
    }
}
