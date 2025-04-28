namespace MiniHotel.Application.Common.Helpers
{
    public static class TransliterationHelper
    {
        private static readonly Dictionary<char, string> _translitMap = new()
        {
            {'а',"a"}, {'б',"b"}, {'в',"v"}, {'г',"h"}, {'ґ',"g"}, {'д',"d"}, {'е',"e"}, {'є',"ye"}, {'ж',"zh"},
            {'з',"z"}, {'и',"y"}, {'і',"i"}, {'ї',"yi"}, {'й',"y"}, {'к',"k"}, {'л',"l"}, {'м',"m"}, {'н',"n"},
            {'о',"o"}, {'п',"p"}, {'р',"r"}, {'с',"s"}, {'т',"t"}, {'у',"u"}, {'ф',"f"}, {'х',"kh"}, {'ц',"ts"},
            {'ч',"ch"}, {'ш',"sh"}, {'щ',"shch"}, {'ь',""}, {'ю',"yu"}, {'я',"ya"}, {'’',"-"}
        };

        public static string Transliterate(string input)
        {
            input = input.ToLowerInvariant();
            var result = new System.Text.StringBuilder();

            foreach (var ch in input)
            {
                result.Append(_translitMap.ContainsKey(ch) ? _translitMap[ch] : ch);
            }

            return result.ToString();
        }
    }
}
