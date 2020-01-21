using System;

namespace IBANApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();
        }

        static void GenerateIban()
        {
            string accountNumber;
            string bankNumber;
            string[] iban = new string[4];

            Console.Clear();
            Console.Write("Bitte geben Sie eine Kontonummer ein: ");
            accountNumber = Console.ReadLine();

            Console.Write("Bitte die BLZ eingeben: ");
            bankNumber = Console.ReadLine();

            iban[0] = "DE";
            iban[1] = "00";
            iban[2] = bankNumber;
            iban[3] = accountNumber;

            iban[1] = CalculateChecksum(iban).ToString();
        }

        static int CalculateChecksum(string[] iban)
        {
            char[] countryCode = iban[0].ToCharArray();
            int[] letterToNumber = new int[2];
            letterToNumber[0] = countryCode[0] - 55;
            letterToNumber[1] = countryCode[1] - 55;

            string completeString = iban[2] + iban[3] + letterToNumber[0] + letterToNumber[1] + iban[1];
            double toCalculate = double.Parse(completeString);
            double checksum = 98 - (toCalculate % 97);

            return 0;
        }

        static void ValidateIban()
        {

        }

        static void ConvertToIban()
        {

        }

        static void Menu()
        {
            int action = 0;

            Console.WriteLine("(1) IBAN aus BLZ und Kontonummer generieren");
            Console.WriteLine("(2) IBAN verifizieren");
            Console.WriteLine("(3) Liste von BLZ und Kontonummer in IBAN konvertieren");
            Console.WriteLine("(4) Beenden");
            Console.Write("Bitte wählen Sie eine Aktion: ");

            while (action <= 0 || action > 4)
            {
                bool valid = int.TryParse(Console.ReadLine(), out action);
                if (!valid)
                {
                    Console.WriteLine("Bitte eine Zahl zwischen 1 und 4 eingeben!");
                }
            }

            if (action == 1)
            {
                GenerateIban();
            }

            if (action == 2)
            {
                ValidateIban();
            }

            if (action == 3)
            {
                ConvertToIban();
            }
        }
    }
}
