using System;

namespace IBANApp
{
    class Program
    {
        static void Main(string[] args)
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
            Banner("Vielen Dank für die Benutzung des Programms!", "Auf Wiedersehen!", padding: 10);
        }

        static void Generate()
        {
            string accountNumber;
            string bankNumber;
            string[] iban = new string[4];

            Console.Clear();
            Console.Write("Bitte geben Sie eine Kontonummer ein: ");
            accountNumber = Console.ReadLine()?.Replace(" ", string.Empty);

            Console.Write("Bitte die BLZ eingeben: ");
            bankNumber = Console.ReadLine()?.Replace(" ", string.Empty);

            iban[0] = "DE";
            iban[1] = "00";
            iban[2] = bankNumber;
            iban[3] = accountNumber;

            iban[1] = CalculateChecksum(iban).ToString();
            Console.Clear();
            string result = string.Concat(iban);
            Banner($"Ihre IBAN lautet {FormatIban(ref result)}", padding: 20, centerVertical: true);
            ShowMessage("Mit [ENTER] gelangen Sie zurück zum Menü", 's');
            Console.ReadLine();
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

        static ref string FormatIban(ref string iban)
        {
            for (int i = 4; i <= iban.Length; i += 4)
            {
                iban = iban.Insert(i, " ");
                i++;
            }

            return ref iban;
        }

        static void ConvertToIban()
        {
        }

        static void Validate()
        {
        }

        static int Menu()
        {
            Console.Clear();
            Banner("IBAN Tool", "Version 1.0", ConsoleColor.Blue, 12);
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
                    ShowMessage("Bitte eine Zahl zwischen 1 und 4 eingeben!", 'e', cursorPosition);
                    ClearLine(cursorPosition);
                    valid = false;
                }
            }

            ClearMessage();

            return action;
        }

        public static void Banner(string title, string subtitle = "", ConsoleColor foreGroundColor = ConsoleColor.White,
            int padding = 0, bool centerVertical = false)
        {
            int maxTitlePad = CalculateMaxPadding(padding, title.Length);
            int maxSubtitlePad = CalculateMaxPadding(padding, subtitle.Length);

            Console.Title = title + (subtitle != "" ? " - " + subtitle : "");
            int maxWidth = (title.Length > subtitle.Length
                ? title.Length + maxTitlePad + 2
                : subtitle.Length + maxSubtitlePad + 2);


            string titleString = PadString(maxWidth, title);
            string subtitleString = PadString(maxWidth, subtitle);

            string titleContent = CenterText(titleString, "║");
            string subtitleContent = CenterText(subtitleString, "║");
            string borderLine = new string('═', maxWidth);

            Console.ForegroundColor = foreGroundColor;
            if (centerVertical)
            {
                int contentHeight = 3 + (subtitle != "" ? 1 : 0);
                Console.SetCursorPosition(0, (Console.WindowHeight - contentHeight) / 2);
            }
            Console.WriteLine(CenterText($"╔{borderLine}╗"));
            Console.WriteLine(titleContent);
            if (!string.IsNullOrEmpty(subtitle))
            {
                Console.WriteLine(subtitleContent);
            }

            Console.WriteLine(CenterText($"╚{borderLine}╝"));
            Console.ResetColor();
        }

        public static int CalculateMaxPadding(int padding, int textLength)
        {
            if (Console.WindowWidth < textLength + 2 + padding)
            {
                return Console.WindowWidth - 2 - textLength;
            }

            return padding;
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

        public static void ShowMessage(string message, char kind, int[] cursorPosition = null)
        {
            if (cursorPosition == null)
            {
                cursorPosition = new[] {Console.CursorLeft, Console.CursorTop};
            }

            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.SetCursorPosition(Console.WindowWidth / 2 - (message.Length / 2), Console.WindowHeight - 10);
            switch (kind)
            {
                case 'e':
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case 'i':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 's':
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            Console.Write(message);
            Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
            Console.ForegroundColor = consoleColor;
        }

        public static void ClearMessage()
        {
            int[] cursorPosition = {Console.CursorLeft, Console.CursorTop};
            Console.SetCursorPosition(1, Console.WindowHeight - 10);
            string clearString = new string(' ', Console.WindowWidth - 1);
            Console.Write(clearString);
            Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
        }

        public static void ClearHeight(int height)
        {
            int[] cursorPosition = new[] {Console.CursorLeft, Console.CursorTop};

            Console.SetCursorPosition(0, Console.WindowHeight - height);
            for (int postition = 0; postition < height; postition++)
            {
                string clear = new string(' ', Console.WindowWidth);
                Console.Write(clear);
            }

            Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
        }

        public static void ClearLine(int[] cursorPosition)
        {
            int widthToClear = Console.WindowWidth - cursorPosition[0];
            string clear = new string(' ', widthToClear);
            Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
            Console.Write(clear);
            Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
        }
    }
}