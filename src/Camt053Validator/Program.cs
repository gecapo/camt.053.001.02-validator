using System;

namespace Camt053Validator
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Camt053Validator <path-to-xml>");
                return;
            }

            string xmlFilePath = args[0];
            var validator = new Camt053ValidatorService();
            ValidationResult result = validator.Validate(xmlFilePath);

            if (result.IsValid)
                Console.WriteLine("Validation succeeded: the file conforms to the schema and passes integrity checks.");
            else
                Console.WriteLine("Validation failed.");

            foreach (string message in result.Messages)
                Console.WriteLine(message);
        }
    }
}
