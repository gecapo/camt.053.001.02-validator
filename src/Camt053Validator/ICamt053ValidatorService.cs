namespace Camt053Validator
{
    /// <summary>
    /// Defines the contract for CAMT.053 validators.
    /// </summary>
    public interface ICamt053ValidatorService
    {
        /// <summary>
        /// Validates a given CAMT.053 XML file and returns a result object
        /// describing schema and integrity outcomes.
        /// </summary>
        ValidationResult Validate(string xmlFilePath);
    }
}
