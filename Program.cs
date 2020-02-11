using System;
using System.IO;
using ibanapp;

namespace IBANApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int action;
            while ((action = MainMenu()) != 5)
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
                    case 4:
                        BulkValidate();
                        break;
                }
            }

            Console.Clear();
            Prettier.Banner("Vielen Dank für die Benutzung des Programms!", "Auf Wiedersehen!", padding: 10);
        }

        private static void Generate()
        {
            Console.Clear();
            Console.Write("Bitte geben Sie eine Kontonummer ein: ");
            string accountNumber = Console.ReadLine()?.Replace(" ", string.Empty);

            Console.Write("Bitte die BLZ eingeben: ");
            string bankNumber = Console.ReadLine()?.Replace(" ", string.Empty);

            string[] iban = GenerateIban(bankNumber, accountNumber);
            
            Console.Clear();
            string result = string.Concat(iban);
            Prettier.Banner($"Ihre IBAN lautet {FormatIban(ref result)}", padding: 20, centerVertical: true);
            Prettier.ShowMessage("Mit [ENTER] gelangen Sie zurück zum Menü", Prettier.MessageKind.Success);
            Console.ReadLine();
        }

        private static string[] GenerateIban(string bankNumber, string accountNumber)
        {
            string[] iban = new string[4];

            iban[0] = "DE";
            iban[1] = "00";
            iban[2] = bankNumber;
            iban[3] = accountNumber;

            int checksum = CalculateChecksum(ref iban);
            if(ValidateIban(ref iban, checksum))
            {
                iban[1] = checksum.ToString();
            }

            return iban;
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
            Console.Clear();
            Prettier.Banner("Massenkonvertierung", "Klassische Kontodaten zu IBAN", ConsoleColor.Blue, 10);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte geben Sie den Namen der zu konvertierenden Datei an: ");
            int[] cursorPosition = { Console.CursorLeft, Console.CursorTop };
            string fileName = GetFilename(cursorPosition);

            string[] bankdata = ConvertBankData(fileName);
            string outFileName = WriteConvertedData(bankdata);

            Prettier.ShowMessage($"Die Daten wurden in der Datei {outFileName} gespeichert. Mit [ENTER] zum Menü...", Prettier.MessageKind.Success);
            Console.ReadLine();
        }

        private static void BulkValidate()
        {
            Console.Clear();
            Prettier.Banner("Massenvalidierung", "IBAN-Validierung", padding: 10);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte geben Sie den Namen der zu validierenden Datei an: ");
            int[] cursorPosition = { Console.CursorLeft, Console.CursorTop };
            string fileName = GetFilename(cursorPosition);

            long[] composition = GetComposition(fileName);
            bool[] invalid = new bool[composition[1]];

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                int counter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] iban = new string[4];
                    if(composition[0] == 1) continue;
                    iban[0] = line.Substring(0, 2);
                    iban[1] = line.Substring(2, 2);
                    iban[2] = line.Substring(4, 8);
                    iban[3] = line.Substring(12, 10);

                    int checksum = int.Parse(iban[1]);
                    invalid[counter] = ValidateIban(ref iban, checksum);
                    counter++;
                }
            }
        }

        private static string[] ConvertBankData(string fileName)
        {
            long[] composition = GetComposition(fileName);
            string[] converted = new string[composition[1]];
            int counter = 0;
            using (StreamReader reader = new StreamReader(fileName))
            {
                if (composition[0] == 1)
                {
                    reader.ReadLine();
                }
                
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(",");
                    if (values.Length > 1)
                    {
                        converted[counter] = string.Concat(GenerateIban(values[0], values[1]));
                    }
                    counter++;
                }
            }

            return converted;
        }

        private static string WriteConvertedData(string[] bankData)
        {
            Console.Clear();
            Prettier.Banner("Massenkonvertierung", "Klassische Kontodaten zu IBAN", ConsoleColor.Blue, 10);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte geben Sie den Namen der Zieldatei an: ");
            int[] cursorPosition = { Console.CursorLeft, Console.CursorTop };
            string fileName = GetFilename(cursorPosition, false);

            using (StreamWriter writer = new StreamWriter(fileName, false))
            {
                writer.WriteLine("IBAN");
                foreach (string iban in bankData)
                {
                    writer.WriteLine(iban);
                }
            }

            return fileName;
        }

        private static long[] GetComposition(string fileName)
        {
            long[] composition = new long[2];
            composition[0] = 0;

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains("IBAN"))
                    {
                        composition[0] = 1;
                    }
                    composition[1]++;
                }
            }

            return composition;
        }

        private static string GetFilename(int[] cursorPosition, bool mustExist = true)
        {
            bool valid = false;
            string fileName = "";

            while (!valid)
            {
                valid = (fileName = Console.ReadLine()) != null;

                if (!valid | !File.Exists(fileName) & mustExist)
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
            Console.WriteLine("(4) Liste von IBANs validieren");
            Console.WriteLine();
            Console.WriteLine("(5) Beenden");

            return GetUserChoice(1, 5);
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