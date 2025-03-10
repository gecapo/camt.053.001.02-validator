class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: Camt053Validator <path-to-xml>");
            return;
        }

        string xmlFilePath = args[0];
        var validator = new Camt053ValidatorService();
        validator.Validate(xmlFilePath);
    }
}
