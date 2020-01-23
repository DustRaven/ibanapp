using System;

namespace IBANApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Header("IBAN Tool", "Version 1.0", ConsoleColor.Blue, 12);
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
            Console.WriteLine("Ihre IBAN lautet {0}", iban[0] + iban[1] + iban[2] + iban[3]);
        }

        static int CalculateChecksum(string[] iban)
        {
            int checksum = decimal.ToInt32(98 - IbanToDecimal(iban) % 97);
            return decimal.ToInt32(checksum);
        }

        static bool ValidateIban(string[] iban, int checksum)
        {
            string[] temporaryIban = new string[4];
            temporaryIban[0] = iban[0];
            temporaryIban[1] = checksum.ToString();
            temporaryIban[2] = iban[2];
            temporaryIban[3] = iban[3];

            decimal test = IbanToDecimal(temporaryIban);
            decimal result = test % 97;
            return decimal.ToInt32(test % 97) == 1;
        }

        static decimal IbanToDecimal(string[] iban)
        {
            char[] countryCode = iban[0].ToCharArray();
            int[] letterToNumber = new int[2];
            letterToNumber[0] = countryCode[0] - 55;
            letterToNumber[1] = countryCode[1] - 55;
            
            return decimal.Parse(iban[2] + iban[3] + letterToNumber[0] + letterToNumber[1] + iban[1]);
        }

        static void ConvertToIban()
        {

        }

        static void Validate()
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
                Validate();
            }

            if (action == 3)
            {
                ConvertToIban();
            }
        }
        public static void Header(string title, string subtitle = "", ConsoleColor foreGroundColor = ConsoleColor.White, int padding = 0)
        {
            Console.Title = title + (subtitle != "" ? " - " + subtitle : "");
            int maxWidth = (title.Length > subtitle.Length ? title.Length + padding + 2 : subtitle.Length + padding + 2);

            

            string titleString = PadString(maxWidth, title);
            string subtitleString = PadString(maxWidth, subtitle);

            string titleContent = CenterText(titleString, "║");
            string subtitleContent = CenterText(subtitleString, "║");
            string borderLine = new string('═', maxWidth);

            Console.ForegroundColor = foreGroundColor;
            Console.WriteLine(CenterText($"╔{borderLine}╗"));
            Console.WriteLine(titleContent);
            if (!string.IsNullOrEmpty(subtitle))
            {
                Console.WriteLine(subtitleContent);
            }
            Console.WriteLine(CenterText($"╚{borderLine}╝"));
            Console.ResetColor();
        }

        public static string CenterText(string content, string decorationString = "")
        {
            int decoLength = decorationString != "" ? 2 * decorationString.Length : 1;
            string left = new string(' ', (Console.WindowWidth / 2 - content.Length / 2) - decoLength);
            return string.Format(left + decorationString + content + decorationString);
        }

        public static string PadString(int width, string content)
        {
            string padding = new string(' ', (width - content.Length) / 2);
            return padding + content + padding;
        }
    }
}
