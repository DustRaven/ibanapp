﻿using System;
using ibanapp;

namespace IBANApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int action;
            while ((action = Menu()) != 4)
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
                        ConvertToIban();
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

            int checksum = CalculateChecksum(iban);
            if(ValidateIban(iban, checksum))
            {
                iban[1] = checksum.ToString();
            }
            Console.Clear();
            string result = string.Concat(iban);
            Prettier.Banner($"Ihre IBAN lautet {FormatIban(ref result)}", padding: 20, centerVertical: true);
            Prettier.ShowMessage("Mit [ENTER] gelangen Sie zurück zum Menü", Prettier.MessageKind.success);
            Console.ReadLine();
        }

        private static int CalculateChecksum(string[] iban)
        {
            int checksum = decimal.ToInt32(98 - IbanToDecimal(iban) % 97);
            return decimal.ToInt32(checksum);
        }

        private static bool ValidateIban(string[] iban, int checksum)
        {
            string[] temporaryIban = new string[4];
            temporaryIban[0] = iban[0];
            temporaryIban[1] = checksum.ToString();
            temporaryIban[2] = iban[2];
            temporaryIban[3] = iban[3];

            decimal test = IbanToDecimal(temporaryIban);
            return decimal.ToInt32(test % 97) == 1;
        }

        private static decimal IbanToDecimal(string[] iban)
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

        private static void ConvertToIban()
        {
            // TODO: Bulk-Konvertierung
            Prettier.ShowMessage("Noch nicht implementiert. Mit [ENTER] zum Menü zurückkehren...", Prettier.MessageKind.info);
            Console.ReadLine();
        }

        private static void Validate()
        {
            // TODO: IBAN-Validierung
            Prettier.ShowMessage("Noch nicht implementiert. Mit [ENTER] zum Menü zurückkehren...", Prettier.MessageKind.info);
            Console.ReadLine();
        }

        private static int Menu()
        {
            Console.Clear();
            Prettier.Banner("IBAN Tool", "Version 1.0", ConsoleColor.Blue, 12);
            int action = 0;

            Console.WriteLine("(1) IBAN aus BLZ und Kontonummer generieren");
            Console.WriteLine("(2) IBAN verifizieren");
            Console.WriteLine("(3) Liste von BLZ und Kontonummer in IBAN konvertieren");
            Console.WriteLine("(4) Beenden");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Bitte wählen Sie eine Aktion: ");
            Console.ResetColor();
            int[] cursorPosition = {Console.CursorLeft, Console.CursorTop};
            bool valid = false;

            while (!valid)
            {
                valid = int.TryParse(Console.ReadLine(), out action);

                if (valid == false | (action <= 0 || action > 4))
                {
                    Prettier.ShowMessage("Bitte eine Zahl zwischen 1 und 4 eingeben!", Prettier.MessageKind.error, cursorPosition);
                    Prettier.ClearLine(cursorPosition);
                    valid = false;
                }
            }

            Prettier.ClearMessage();

            return action;
        }
    }
}