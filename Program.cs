using System;
using System.IO;
using System.Text;
using ibanapp;

namespace IBANApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int action;
            while ((action = MainMenu()) != 4)
            {
                switch (action)
                {
                    case 1:
                        Generate();
                        break;
                    case 2:
                        Validate();
                        break;
                    case 3:
                        BulkConvert();
                        break;
                }
            }

            Console.Clear();
            Prettier.Banner("Vielen Dank für die Benutzung des Programms!", "Auf Wiedersehen!", padding: 10);
        }

        private static void Generate()
        {
            string[] iban = new string[4];

            Console.Clear();
            Console.Write("Bitte geben Sie eine Kontonummer ein: ");
            string accountNumber = Console.ReadLine()?.Replace(" ", string.Empty);

            Console.Write("Bitte die BLZ eingeben: ");
            string bankNumber = Console.ReadLine()?.Replace(" ", string.Empty);

            iban[0] = "DE";
            iban[1] = "00";
            iban[2] = bankNumber;
            iban[3] = accountNumber;

            int checksum = CalculateChecksum(ref iban);
            if(ValidateIban(ref iban, checksum))
            {
                iban[1] = checksum.ToString();
            }
            Console.Clear();
            string result = string.Concat(iban);
            Prettier.Banner($"Ihre IBAN lautet {FormatIban(ref result)}", padding: 20, centerVertical: true);
            Prettier.ShowMessage("Mit [ENTER] gelangen Sie zurück zum Menü", Prettier.MessageKind.Success);
            Console.ReadLine();
        }

        private static int CalculateChecksum(ref string[] iban)
        {
            int checksum = decimal.ToInt32(98 - IbanToDecimal(ref iban) % 97);
            return decimal.ToInt32(checksum);
        }

        private static bool ValidateIban(ref string[] iban, int checksum)
        {
            iban[1] = checksum.ToString();

            decimal test = IbanToDecimal(ref iban);
            return decimal.ToInt32(test % 97) == 1;
        }

        private static decimal IbanToDecimal(ref string[] iban)
        {
            char[] countryCode = iban[0].ToCharArray();
            int[] letterToNumber = new int[2];
            letterToNumber[0] = countryCode[0] - 55;
            letterToNumber[1] = countryCode[1] - 55;

            return decimal.Parse(iban[2] + iban[3] + letterToNumber[0] + letterToNumber[1] + iban[1]);
        }

        private static ref string FormatIban(ref string iban)
        {
            for (int i = 4; i <= iban.Length; i += 4)
            {
                iban = iban.Insert(i, " ");
                i++;
            }

            return ref iban;
        }

        private static void BulkConvert()
        {
            int action;
            while ((action = BulkMenu()) != 3)
            {
                switch (action)
                {
                    case 1:
                        AccountToIban();
                        break;
                    case 2:
                        IbanToAccount();
                        break;
                }
            }
        }

        private static void AccountToIban()
        {
            Console.Clear();
            Prettier.Banner("Massenkonvertierung", "Klassische Kontodaten zu IBAN", padding: 10);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte geben Sie den Namen der zu konvertierenden Datei an: ");
            int[] cursorPosition = {Console.CursorLeft, Console.CursorTop};
            string fileName = GetFilename(cursorPosition);

            using (StreamReader reader = new StreamReader(fileName))
            {
                string headers = "";
                if ((headers = reader.ReadLine()) != null)
                {

                }

            }

            Console.ReadLine();
        }

        private static void IbanToAccount()
        {
            // TODO: IBAN to Account implementierung
            Prettier.ShowMessage("Noch nicht implementiert. Mit [ENTER] zum Menü zurückkehren...", Prettier.MessageKind.Info);
            Console.ReadLine();
        }

        private static void ShowBulkInstructions()
        {
            Console.WriteLine("Die Daten müssen als .csv Datei in folgendem Format vorliegen:");
            Console.WriteLine("Kontonummer,BLZ");
            Console.WriteLine("12345678,2105017000");
        }

        private static string GetFilename(int[] cursorPosition)
        {
            bool valid = false;
            string fileName = "";

            while (!valid)
            {
                valid = (fileName = Console.ReadLine()) != null;

                if (!valid | !File.Exists(fileName))
                {
                    Prettier.ShowMessage($"Die Datei {fileName} existiert nicht oder kann nicht gelesen werden!", Prettier.MessageKind.Error, cursorPosition);
                    Prettier.ClearLine(cursorPosition);
                    valid = false;
                }
            }

            return fileName;
        }

        private static void Validate()
        {
            // TODO: IBAN-Validierung
            Prettier.ShowMessage("Noch nicht implementiert. Mit [ENTER] zum Menü zurückkehren...", Prettier.MessageKind.Info);
            Console.ReadLine();
        }

        private static int BulkMenu()
        {
            Console.Clear();
            Prettier.Banner("Massenkonvertierung", padding: 10);
            Console.WriteLine();

            Console.WriteLine("(1) Kontonummern und Bankleitzahlen -> IBAN");
            Console.WriteLine("(2) IBAN -> Kontonummern und Bankleitzahlen");
            Console.WriteLine();
            Console.WriteLine("(3) Zurück zum Hauptmenü");

            return GetUserChoice(1, 3);
        }

        private static int MainMenu()
        {
            Console.Clear();
            Prettier.Banner("IBAN Tool", "Version 1.0", ConsoleColor.Blue, 12);
            Console.WriteLine();

            Console.WriteLine("(1) IBAN aus BLZ und Kontonummer generieren");
            Console.WriteLine("(2) IBAN verifizieren");
            Console.WriteLine("(3) Liste von BLZ und Kontonummer in IBAN konvertieren");
            Console.WriteLine();
            Console.WriteLine("(4) Beenden");

            return GetUserChoice(1, 4);
        }

        private static int GetUserChoice(int min, int max)
        {
            int action = 0;
            bool valid = false;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte wählen Sie eine Aktion: ");
            Console.ResetColor();
            int[] cursorPosition = {Console.CursorLeft, Console.CursorTop};

            while (!valid)
            {
                valid = int.TryParse(Console.ReadLine(), out action);

                if (valid == false | (action < min || action > max))
                {
                    Prettier.ShowMessage($"Bitte eine Zahl zwischen {min} und {max} eingeben!", Prettier.MessageKind.Error, cursorPosition);
                    Prettier.ClearLine(cursorPosition);
                    valid = false;
                }
            }

            Prettier.ClearMessage();

            return action;
        }
    }
}