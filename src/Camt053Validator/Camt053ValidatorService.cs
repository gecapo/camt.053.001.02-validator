using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

public sealed class Camt053ValidatorService : ICamt053ValidatorService
{
    public void Validate(string xmlFilePath)
    {
        if (!File.Exists(xmlFilePath) || !File.Exists(Camt053Constants.XsdFilePath))
        {
            Console.WriteLine("Error: XML or XSD file not found.");
            return;
        }

        if (ValidateXml(xmlFilePath))
        {
            ValidateIntegrity(xmlFilePath);
        }
    }

    private bool ValidateXml(string xmlFile)
    {
        XmlSchemaSet schemaSet = new XmlSchemaSet();
        schemaSet.Add(null, Camt053Constants.XsdFilePath);

        XmlReaderSettings settings = new XmlReaderSettings
        {
            Schemas = schemaSet,
            ValidationType = ValidationType.Schema
        };
        settings.ValidationEventHandler += ValidationCallback!;

        bool isValid = true;
        try
        {
            using (XmlReader reader = XmlReader.Create(xmlFile, settings))
            {
                while (reader.Read()) { }
            }
            Console.WriteLine("XML schema validation passed.");
        }
        catch (Exception ex)
        {
            isValid = false;
            Console.WriteLine($"Validation failed: {ex.Message}");
        }
        return isValid;
    }

    private void ValidationCallback(object sender, ValidationEventArgs e)
    {
        Console.WriteLine($"Validation error: {e.Message}");
    }

    private void ValidateIntegrity(string xmlFile)
    {
        XDocument doc = XDocument.Load(xmlFile);
        var transactions = doc.Descendants("Ntry");

        decimal openingBalance = GetBalance(doc, Camt053Constants.OpeningBalanceType);
        decimal closingBalance = GetBalance(doc, Camt053Constants.ClosingBalanceType);
        decimal totalCredits = GetTransactionSum(transactions, Camt053Constants.CreditIndicator);
        decimal totalDebits = GetTransactionSum(transactions, Camt053Constants.DebitIndicator);

        if (openingBalance + totalCredits - totalDebits != closingBalance)
        {
            Console.WriteLine("Integrity check failed: Opening balance + Credits - Debits does not match Closing balance.");
        }
        else
        {
            Console.WriteLine("Integrity check passed.");
        }
    }

    private static decimal GetBalance(XDocument doc, string balanceType) => doc.Descendants("Bal")
            .Where(b => b.Element("Tp")?.Value == balanceType)
            .Select(b => decimal.TryParse(b.Element("Amt")?.Value, out var bal) ? bal : 0)
            .FirstOrDefault();

    private static decimal GetTransactionSum(IEnumerable<XElement> transactions, string type) =>
        transactions.Where(t => t.Element("CdtDbtInd")?.Value == type)
            .Sum(t => decimal.TryParse(t.Element("Amt")?.Value, out var bal) ? bal : 0);
}
