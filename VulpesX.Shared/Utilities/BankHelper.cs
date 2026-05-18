using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class BankHelper
    {
        const string NUMBERS = "0123456789";
        const string LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ-. ";
        const string ALPHA_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const int DIVISORE_CIN = 26;
        const int DIVISORE_IBAN = 97;
        static int[] EVEN_LIST = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 };
        static int[] ODD_LIST = { 1, 0, 5, 7, 9, 13, 15, 17, 19, 21, 2, 4, 18, 20, 11, 3, 6, 8, 12, 14, 16, 10, 22, 25, 24, 23, 27, 28, 26 };

        public static (string CIN, string BBAN, string IBAN) ComputeCINBBANIBAN(int ABI, int CAB, string Account, string CountryID)
        {
            string? CIN = null;
            string? BBAN = null;
            string? IBAN = null;

            string sABI = ABI.ToString().PadLeft(5, '0');
            string sCAB = CAB.ToString().PadLeft(5, '0');
            Account = Account.Trim().PadLeft(12, '0').ToUpper();

            // codice normalizzato
            string codice = (sABI.Trim() + sCAB.Trim() + Account.Trim()).ToUpper();

            #region CIN
            // calcolo valori caratteri
            int somma = 0;

            char[] c = codice.ToCharArray();
            for (int k = 0; k < (22); k++)
            {
                int i = NUMBERS.IndexOf(c[k]);
                if (i < 0)
                    i = LETTERS.IndexOf(c[k]);

                // se ci sono caratteri errati usciamo con un valore 
                // impossibile da trovare sul cin
                if (i < 0)
                {
                    CIN = "@";
                    break;
                }

                if ((k % 2) == 0)
                {
                    // valore dispari
                    somma += ODD_LIST[i];
                }
                else
                {
                    // valore pari
                    somma += EVEN_LIST[i];
                }
            }
            if (CIN != "@")
                CIN = LETTERS.Substring(somma % DIVISORE_CIN, 1);
            #endregion
            #region BBAN
            BBAN = CIN + sABI + sCAB + Account;
            #endregion
            #region IBAN
            string tmpCode = AlphaToNumber(BBAN + CountryID.Trim() + "00");

            StringBuilder Intero = new StringBuilder();
            StringBuilder Resto = new StringBuilder();
            double divisore;
            if (!double.TryParse(DIVISORE_IBAN.ToString(), NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out divisore))
                throw new Exception("Divisore errato");
            for (int x = 0; x < tmpCode.Length; x++)
            {
                Resto.Append(tmpCode.Substring(x, 1));
                string s = Resto.ToString();
                double dividendo = 0;
                if (!double.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out dividendo))
                    throw new Exception("Dividendo Errato");
                int volte = 0;
                while (dividendo >= divisore)
                {
                    dividendo -= divisore;
                    volte++;
                }
                Intero.Append(volte);
                string r = dividendo.ToString("0");
                Resto = new System.Text.StringBuilder();
                Resto.Append(r);
            }
            string resultR = Resto.ToString();
            string resultI = Intero.ToString();
            while (resultI.StartsWith("0"))
                resultI = resultI.Substring(1);
            if (resultI == "")
                resultI = "0";

            int resto = int.Parse(resultR);
            resto = (DIVISORE_IBAN + 1) - resto;
            IBAN = CountryID.Trim() + resto.ToString("N0").Trim() + BBAN;
            #endregion

            return (CIN, BBAN, IBAN);
        }

        public static string AlphaToNumber(string InputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in InputString)
            {
                int k = ALPHA_CHARS.IndexOf(c);
                if (k != -1)
                    sb.Append(k + 10);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
