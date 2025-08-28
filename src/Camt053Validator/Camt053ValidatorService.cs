using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Camt053Validator
{
    /// <summary>
    /// Provides functionality to validate CAMT.053 bank statement XML files against
    /// a schema and perform integrity checks on balances and transactions.
    /// </summary>
    public sealed class Camt053ValidatorService : ICamt053ValidatorService
    {
        private readonly string _xsdPath;

        public Camt053ValidatorService(string xsdFilePath = null)
        {
            _xsdPath = string.IsNullOrWhiteSpace(xsdFilePath)
                ? Camt053Constants.XsdFilePath
                : xsdFilePath;
        }

        public ValidationResult Validate(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                throw new ArgumentException("XML file path must be specified.", nameof(xmlFilePath));

            var result = new ValidationResult();

            // Check for file existence
            if (!File.Exists(xmlFilePath))
            {
                result.IsSchemaValid = false;
                result.Messages.Add($"XML file '{xmlFilePath}' was not found.");
                return result;
            }
            if (!File.Exists(_xsdPath))
            {
                result.IsSchemaValid = false;
                result.Messages.Add($"XSD file '{_xsdPath}' was not found.");
                return result;
            }

            // Schema validation
            result.IsSchemaValid = ValidateXmlSchema(xmlFilePath, result.Messages);

            // Integrity validation
            if (result.IsSchemaValid)
                result.IsIntegrityValid = ValidateIntegrity(xmlFilePath, result.Messages);

            return result;
        }

        private bool ValidateXmlSchema(string xmlFile, IList<string> messages)
        {
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, _xsdPath);

            var settings = new XmlReaderSettings
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema
            };
            settings.ValidationEventHandler += (_, e) =>
            {
                messages.Add($"Schema validation error: {e.Message}");
            };

            try
            {
                using var reader = XmlReader.Create(xmlFile, settings);
                while (reader.Read()) { }
            }
            catch (Exception ex)
            {
                messages.Add($"Schema validation exception: {ex.Message}");
                return false;
            }

            return !messages.Any(m => m.StartsWith("Schema validation error"));
        }

        private bool ValidateIntegrity(string xmlFile, IList<string> messages)
        {
            XDocument doc = XDocument.Load(xmlFile);
            IEnumerable<XElement> transactions = doc.Descendants("Ntry");

            decimal openingBalance = GetBalance(doc, Camt053Constants.OpeningBalanceType);
            decimal closingBalance = GetBalance(doc, Camt053Constants.ClosingBalanceType);
            decimal totalCredits = GetTransactionSum(transactions, Camt053Constants.CreditIndicator);
            decimal totalDebits = GetTransactionSum(transactions, Camt053Constants.DebitIndicator);

            if (openingBalance + totalCredits - totalDebits != closingBalance)
            {
                messages.Add("Integrity check failed: opening balance + credits - debits does not equal closing balance.");
                return false;
            }

            return true;
        }

        private static decimal GetBalance(XDocument doc, string balanceType)
        {
            return doc.Descendants("Bal")
                      .Where(b => b.Element("Tp")?.Value == balanceType)
                      .Select(b => decimal.TryParse(b.Element("Amt")?.Value, out var bal) ? bal : 0)
                      .FirstOrDefault();
        }

        private static decimal GetTransactionSum(IEnumerable<XElement> transactions, string type)
        {
            return transactions
                .Where(t => t.Element("CdtDbtInd")?.Value == type)
                .Sum(t => decimal.TryParse(t.Element("Amt")?.Value, out var bal) ? bal : 0);
        }
    }
}
