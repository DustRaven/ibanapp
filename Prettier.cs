using System;
using System.Collections.Generic;
using System.Text;

namespace ibanapp
{
    class Prettier
    {
        public enum MessageKind
        {
            info,
            error,
            success
        }
        /// <summary>
        /// Shows a bordered banner
        /// </summary>
        /// <param name="title">The main text</param>
        /// <param name="subtitle">The subtext</param>
        /// <param name="foreGroundColor">The foreground color as ConsoleColor</param>
        /// <param name="padding">Space to add left and right</param>
        /// <param name="centerVertical">Show the banner in the screen center</param>
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

        private static int CalculateMaxPadding(int padding, int textLength)
        {
            if (Console.WindowWidth < textLength + 2 + padding)
            {
                return Console.WindowWidth - 2 - textLength;
            }

            return padding;
        }

        private static string CenterText(string content, string decorationString = "")
        {
            int decoLength = decorationString != "" ? 2 * decorationString.Length : 1;
            string left = new string(' ', (Console.WindowWidth / 2 - content.Length / 2) - decoLength);
            return string.Format(left + decorationString + content + decorationString);
        }

        private static string PadString(int width, string content)
        {
            string padding = new string(' ', (width - content.Length) / 2);
            return padding + content + padding;
        }

        /// <summary>
        /// Shows a message at the bottom of the screen
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="kind">error, information or success</param>
        /// <param name="cursorPosition"></param>
        public static void ShowMessage(string message, MessageKind kind, int[] cursorPosition = null)
        {
            if (cursorPosition == null)
            {
                cursorPosition = new[] {Console.CursorLeft, Console.CursorTop};
            }

            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.SetCursorPosition(Console.WindowWidth / 2 - (message.Length / 2), Console.WindowHeight - 10);
            switch (kind)
            {
                case MessageKind.error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageKind.info:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case MessageKind.success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            Console.Write(message);
            Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
            Console.ForegroundColor = consoleColor;
        }

        /// <summary>
        /// Clears a shown message
        /// </summary>
        public static void ClearMessage()
        {
            int[] cursorPosition = {Console.CursorLeft, Console.CursorTop};
            Console.SetCursorPosition(1, Console.WindowHeight - 10);
            string clearString = new string(' ', Console.WindowWidth - 1);
            Console.Write(clearString);
            Console.SetCursorPosition(cursorPosition[0], cursorPosition[1]);
        }

        /// <summary>
        /// Clears the screen from a given line downwards
        /// </summary>
        /// <param name="height">Count of lines to preserve</param>
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

        /// <summary>
        /// Clears a line from a given cursor position to the end of the line
        /// </summary>
        /// <param name="cursorPosition"></param>
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
