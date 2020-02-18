/*
 * Projekt: IBAN App
 * Klasse: FIA95
 * Namen: Samuel Wulf, Dennis Wilhelmy
 * Datum: DateTime.Now()
 */

using System;
using System.IO;
using ibanapp;

namespace IBANApp
{
    public class Program
    {
        public static bool DebugEnabled;
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                if (args[0] == "debug")
                {
                    DebugEnabled = true;
                    LogHelper.Log("Main | Program started. Debugging enabled.");
                }
            }

            int action;
            while ((action = MainMenu()) != 6)
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
                    case 5:
                        DebugEnabled = !DebugEnabled;
                        break;
                }
            }

            Console.Clear();
            Prettier.Banner("Vielen Dank für die Benutzung des Programms!", "Auf Wiedersehen!", padding: 10);
        }

        private static void Generate()
        {
            Console.Clear();
            Prettier.Banner("IBAN generieren", foreGroundColor: ConsoleColor.Blue, padding: 10);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("Bitte geben Sie eine Kontonummer ein: ");
            CursorPosition cursorPosition = new CursorPosition(Console.CursorLeft, Console.CursorTop);
            string accountNumber = GetBankData(cursorPosition, 'k');
            if(DebugEnabled)
                LogHelper.Log("Generate | Account number entered: " + accountNumber);
            
            Console.Write("Bitte die BLZ eingeben: ");
            cursorPosition.UpdatePosition(Console.CursorLeft, Console.CursorTop);
            string bankNumber = GetBankData(cursorPosition, 'b');
            if(DebugEnabled)
                LogHelper.Log("Generate | Bank number entered: " + bankNumber);

            string[] iban = GenerateIban(bankNumber, accountNumber);
            
            Console.Clear();
            string result = string.Concat(iban);
            if(DebugEnabled)
                LogHelper.Log("Generate | Generated IBAN: " + result);

            Prettier.Banner($"Ihre IBAN lautet {FormatIban(ref result)}", padding: 20, centerVertical: true);
            Prettier.ShowMessage("Mit [ENTER] gelangen Sie zurück zum Menü", Prettier.MessageKind.Success);
            Console.ReadLine();
        }

        private static string[] GenerateIban(string bankNumber, string accountNumber)
        {
            string[] iban = {"DE", "00", bankNumber, accountNumber};

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
            if(DebugEnabled)
                LogHelper.Log("CalculateChecksum | Checksum: " + checksum);
            return decimal.ToInt32(checksum);
        }

        private static bool ValidateIban(ref string[] iban, int checksum)
        {
            iban[1] = checksum.ToString();
            decimal test = IbanToDecimal(ref iban);
            bool valid = decimal.ToInt32(test % 97) == 1;
            if(DebugEnabled)
                LogHelper.Log("ValidateIban | Checksum valid: " + valid);
            return valid;
        }

        private static decimal IbanToDecimal(ref string[] iban)
        {
            char[] countryCode = iban[0].ToCharArray();
            int[] letterToNumber = new int[2];
            letterToNumber[0] = countryCode[0] - 55;
            letterToNumber[1] = countryCode[1] - 55;

            decimal decimalIban = decimal.Parse(iban[2] + iban[3] + letterToNumber[0] + letterToNumber[1] + iban[1]);
            if(DebugEnabled)
                LogHelper.Log("IbanToDecimal | Decimal Iban: " + decimalIban);
            return decimalIban;
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
            CursorPosition cursorPosition = new CursorPosition(Console.CursorLeft, Console.CursorTop);
            string fileName = GetFilename(cursorPosition);
            if(DebugEnabled)
                LogHelper.Log("BulkConvert | Chosen filename (full path): " + Path.GetFullPath(fileName));

            string[] bankdata = ConvertBankData(fileName);
            string outFileName = WriteConvertedData(bankdata);

            Prettier.ShowMessage($"Die Daten wurden in der Datei {outFileName} gespeichert. Mit [ENTER] zum Menü...", Prettier.MessageKind.Success);
            Console.ReadLine();
        }

        private static void BulkValidate()
        {
            Console.Clear();
            Prettier.Banner("Massenvalidierung", "IBAN- Validierung", foreGroundColor:ConsoleColor.Blue, padding: 10);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte geben Sie den Namen der zu validierenden Datei an: ");
            CursorPosition cursorPosition = new CursorPosition(Console.CursorLeft, Console.CursorTop);
            string fileName = GetFilename(cursorPosition);

            long[] composition = GetComposition(fileName);
            string[] result = new string[composition[1]];

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                int counter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] iban = new string[4];
                    if (composition[0] == 1) continue;
                    iban[0] = line.Substring(0, 2);
                    iban[1] = line.Substring(2, 2);
                    iban[2] = line.Substring(4, 8);
                    iban[3] = line.Substring(12, 10);

                    int checksum = int.Parse(iban[1]);
                    if (!ValidateIban(ref iban, checksum))
                    {
                        result[counter] = line;
                        counter++;
                    }
                }
            }
            

            WriteInvalidIbans(result);
        }

        private static void WriteInvalidIbans(string[] ibanList)
        {
            string fileName = "invalid.csv";
            using(StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("Invalid IBANs");
                foreach (string iban in ibanList)
                {
                    if (iban != null)
                    {
                        writer.WriteLine(iban);
                    }
                }
            }

            Prettier.ShowMessage($"Fehlerhafte IBANs wurden in die Datei {fileName} geschrieben. Mit [ENTER] zum Menü...", Prettier.MessageKind.Success);
            Console.ReadLine();
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
            CursorPosition cursorPosition = new CursorPosition(Console.CursorLeft, Console.CursorTop);
            string fileName = GetFilename(cursorPosition, false);
            if(DebugEnabled)
                LogHelper.Log("WriteConvertedData | Chosen filename (full path): " + Path.GetFullPath(fileName));

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
                    if (line.Contains("IBAN") | line.Contains("Konto"))
                    {
                        composition[0] = 1;
                    }
                    composition[1]++;
                }
            }

            return composition;
        }

        private static string GetFilename(CursorPosition cursorPosition, bool mustExist = true)
        {
            bool valid = false;
            string fileName = "";

            while (!valid)
            {
                valid = (fileName = Console.ReadLine()?.Replace("\"", string.Empty)) != null;

                if (!valid | !File.Exists(fileName) & mustExist)
                {
                    Prettier.ShowMessage($"Die Datei {fileName} existiert nicht oder kann nicht gelesen werden!", Prettier.MessageKind.Error, cursorPosition);
                    Prettier.ClearLine(cursorPosition);
                    valid = false;
                }
            }

            Prettier.ClearMessage();
            return fileName;
        }

        private static string GetBankData(CursorPosition cursorPosition, char type)
        {
            bool valid = false;
            long input = 0;
            int requiredLength = type == 'b' ? 8 : 10;
            while(!valid)
            {
                valid = long.TryParse(Console.ReadLine()?.Replace(" ", string.Empty), out input);
                if (!valid)
                {
                    Prettier.ShowMessage($"Ihre Eingabe ist ungültig!", Prettier.MessageKind.Error, cursorPosition);
                    Prettier.ClearLine(cursorPosition);
                }

                if (input.ToString().Length != requiredLength)
                {
                    Prettier.ShowMessage($"Ihre Eingabe ist zu lang oder zu kurz!", Prettier.MessageKind.Error, cursorPosition);
                    Prettier.ClearLine(cursorPosition);
                    valid = false;
                }
            }
            Prettier.ClearMessage();
            return input.ToString();
        }

        private static void Validate()
        {
            Console.Clear();
            Prettier.Banner("IBAN-Validierung", foreGroundColor: ConsoleColor.Blue, padding: 10);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte geben sie eine Iban ein: ");
            string [] number = new string [3];
            string iban = Console.ReadLine()?.Replace(" ", "");
            if (iban != null)
            {
                if (iban.Length != 22  )
                {
                    Prettier.ShowMessage("Fehler: Fehlerhafte IBAN. Mit [ENTER] zum Menü zurückkehren...", Prettier.MessageKind.Error);
                    Console.ReadLine();
                }
                else
                {
                    string[] ibanArray =
                        {iban.Substring(0, 2), iban.Substring(2, 2), iban.Substring(4, 8), iban.Substring(12, 10)};
                    bool valid = ValidateIban(ref ibanArray, int.Parse(ibanArray[1]));
                    number[1] = iban.Substring(12, 10);
                    number[2] = iban.Substring(4, 8);
                    string validString = valid ? "gültig" : "ungültig";
                    //Soll es eine ausgabe geben mit den einzelnen Daten oder nur eine Ausgabe die bestätigt, dass es eine IBAN ist?
                    Prettier.Banner($"Konto-Nr.: {number[1]} BLZ: {number[2]}. Die IBAN ist {validString}.", padding: 20, centerVertical: true);
                    //Console.WriteLine("Konto-Nr.:" + number[1] + "\n" + "BLZ:" + number[2]);

                    Console.ReadLine();
                }
            }
        }

        private static int MainMenu()
        {
            Console.Clear();
            if (DebugEnabled)
            {
                Prettier.DebugIndicator();
            }
            Prettier.Banner("IBAN Tool", "Version 1.0", ConsoleColor.Blue, 12);
            Console.WriteLine();

            Console.WriteLine("(1) IBAN aus BLZ und Kontonummer generieren");
            Console.WriteLine("(2) IBAN verifizieren");
            Console.WriteLine("(3) Liste von BLZ und Kontonummer in IBAN konvertieren");
            Console.WriteLine("(4) Liste von IBANs validieren");
            Console.WriteLine();
            if (!DebugEnabled)
            {
                Console.WriteLine("(5) Debug-Log aktivieren");
            }
            else
            {
                Console.WriteLine("(5) Debug-Log deaktivieren");
            }
            Console.WriteLine("(6) Beenden");

            return GetUserChoice(1, 6);
        }

        private static int GetUserChoice(int min, int max)
        {
            int action = 0;
            bool valid = false;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte wählen Sie eine Aktion: ");
            Console.ResetColor();
            CursorPosition cursorPosition = new CursorPosition(Console.CursorLeft, Console.CursorTop);

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