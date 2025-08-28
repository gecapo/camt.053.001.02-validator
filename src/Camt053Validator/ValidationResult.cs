using System.Collections.Generic;

namespace Camt053Validator
{
    /// <summary>
    /// Represents the outcome of a CAMT.053 validation.  This object collects
    /// schema and integrity results as well as any messages generated during
    /// validation.  A caller can inspect <see cref="IsValid"/> to determine
    /// overall success and examine <see cref="Messages"/> for diagnostics.
    /// </summary>
    public record ValidationResult
    {
        /// <summary>
        /// True when the XML is valid according to the XSD schema.
        /// </summary>
        public bool IsSchemaValid { get; init; } = true;

        /// <summary>
        /// True when all integrity checks (balances, credits and debits) pass.
        /// </summary>
        public bool IsIntegrityValid { get; init; } = true;

        /// <summary>
        /// Diagnostic messages collected during validation.
        /// </summary>
        public List<string> Messages { get; } = new();

        /// <summary>
        /// True when both the schema and integrity validations succeed and no
        /// diagnostic messages were added.
        /// </summary>
        public bool IsValid => IsSchemaValid && IsIntegrityValid && Messages.Count == 0;
    }
}
