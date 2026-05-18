using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class TextHelper
    {
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string? ReverseAndScrumble(string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                var result = new string(charArray);
                return result.Substring(2, 2) + result.Substring(0, 2);
            }
            else
            {
                return null;
            }
        }

        public static bool CheckEmailAddress(string EmailEddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailEddress))
                    return false;

                _ = new MailAddress(EmailEddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static Tuple<int, int>? ExtractDayMonth(string? Text)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    if (Text.Length == 4)
                    {
                        return new Tuple<int, int>(int.Parse(Text.Substring(0, 2)), int.Parse(Text.Substring(2, 2)));
                    }
                    else
                    {
                        if (Text.Length == 3)
                        {
                            return new Tuple<int, int>(int.Parse(Text.Substring(0, 1)), int.Parse(Text.Substring(1, 2)));
                        }
                        else
                        { return null; }
                    }
                }
                else { return null; }
            }
            catch { return null; }
        }

        #region Base62 encoder
        private const string DEFAULT_CHARACTER_SET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public static string EncodeToBase62(long Source)
        {
            string? result = null;
            do
            {
                result = string.Format("{0}{1}",
                    DEFAULT_CHARACTER_SET[(int)(Source % 62)],
                    result);
                Source /= 62;
            } while (Source > 0);
            return result;
        }
        #endregion

        public static string SanitizeAccents(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return string.Empty;
            return Input.Replace("à", "a").Replace("è", "e").Replace("é", "e").Replace("ì", "i").Replace("ò", "o").Replace("ù", "u");
        }

        public static string? SanitizeSpecialCharacters(string Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;
            // ATTENZIONE, non e' un errore, il secondo - non e' un - ma una porcheria (–) copincollata da chissa' dove
            Input = Input.Replace("-", string.Empty).Replace("–", string.Empty);
            Regex reg = new Regex("['’\"_&#^@|£$€%°§ç/]");
            return reg.Replace(Input, string.Empty);
        }

        public static string? SanitizeFull(string? Input)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;
            return SanitizeSpecialCharacters(SanitizeAccents(Input));
        }

        public static string? Truncate(string? Input, int MaxLength)
        {
            if (string.IsNullOrWhiteSpace(Input))
                return null;
            return Input.Length <= MaxLength ? Input.Trim() : Input.Substring(0, MaxLength - 1);
        }
    }

}
