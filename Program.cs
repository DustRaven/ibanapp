using System;

namespace IBANApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Menu());
        }

        static void GenerateIban()
        {

        }

        static void ValidateIban()
        {

        }

        static void ConvertToIban()
        {

        }

        static int Menu()
        {
            int action;

            Console.WriteLine("(1) IBAN aus BLZ und Kontonummer generieren");
            Console.WriteLine("(2) IBAN verifizieren");
            Console.WriteLine("(3) Liste von BLZ und Kontonummer in IBAN konvertieren");
            Console.WriteLine("(4) Beenden");
            Console.Write("Bitte wählen Sie eine Aktion: ");

            Console.ReadLine(out action);
            if (action > 0 && action < 5)
            {
                return action;
            }

            Console.WriteLine("Bitte Aktion 1-4 wählen!");
            Menu();
        }
    }
}
