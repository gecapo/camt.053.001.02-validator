# Camt.053.001.02 Validator

A high-performance C# service for validating and verifying **camt.053.001.02** bank statement XML files. Ensures schema compliance and financial integrity checks for banking transactions.

## 🚀 Features

- ✅ **XSD Schema Validation** – Ensures XML files conform to the official **camt.053.001.02** schema.
- ✅ **Integrity Checks** – Verifies opening balance, transactions, and closing balance consistency.
- ✅ **Extensible & Modular** – Implements an interface-driven approach (`ICamt053ValidatorService`) for easy integration.

## 📌 Installation & Usage

1. **Clone the repository**:

   ```sh
   git clone https://github.com/gecapo/camt.053.001.02-validator.git
   cd camt053-validator
   ```

2. **Run the validator**:

   ```sh
   dotnet run -- path-to-xml-file.xml
   ```

## ⚙️ Technologies Used

- **C# (.NET)**
- **XML & XSD Schema Validation**
- **LINQ to XML**

## 🤝 Contributing

Contributions are welcome! If you find an issue or have an enhancement, feel free to submit a pull request.

## 🐜 License

This project is licensed under the MIT License.

---

Built with precision for financial data integrity. 💡

