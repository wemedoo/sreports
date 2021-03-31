using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Common.Localization
{
    using System.Collections.Generic;
    using System.Globalization;

    public enum SpokenLang
    {
        [Spoken.Lang("Afaraf", "aa", "aar")]
        Afar,
        [Spoken.Lang("Аҧсуа", "ab", "abk")]
        Abkhazian,
        [Spoken.Lang("Afrikaans", "af", "afr")]
        Afrikaans,
        [Spoken.Lang("Akan", "ak", "aka")]
        Akan,
        [Spoken.Lang("አማርኛ", "am", "amh")]
        Amharic,
        [Spoken.Lang("‫العربية", "ar", "ara")]
        Arabic,
        [Spoken.Lang("Aragonés", "an", "arg")]
        Aragonese,
        [Spoken.Lang("অসমীয়া", "as", "asm")]
        Assamese,
        [Spoken.Lang("авар мацӀ", "av", "ava")]
        Avaric,
        [Spoken.Lang("Avestan", "ae", "ave")]
        Avestan,
        [Spoken.Lang("Aymar aru", "ay", "aym")]
        Aymara,
        [Spoken.Lang("Azərbaycan dili", "az", "aze")]
        Azerbaijani,
        [Spoken.Lang("башҡорт теле", "ba", "bak")]
        Bashkir,
        [Spoken.Lang("Bamanankan", "bm", "bam")]
        Bambara,
        [Spoken.Lang("Беларуская", "be", "bel")]
        Belarusian,
        [Spoken.Lang("বাংলা", "bn", "ben")]
        Bengali,
        [Spoken.Lang("Bislama", "bi", "bis")]
        Bislama,
        [Spoken.Lang("བོད་ཡིག", "bo", "bod")]
        Tibetan,
        [Spoken.Lang("Bosanski jezik", "bs", "bos")]
        Bosnian,
        [Spoken.Lang("Brezhoneg", "br", "bre")]
        Breton,
        [Spoken.Lang("български език", "bg", "bul")]
        Bulgarian,
        [Spoken.Lang("Català", "ca", "cat")]
        Catalan,
        [Spoken.Lang("Česky", "cs", "ces")]
        Czech,
        [Spoken.Lang("Chamoru", "ch", "cha")]
        Chamorro,
        [Spoken.Lang("нохчийн мотт", "ce", "che")]
        Chechen,
        [Spoken.Lang("Словѣньскъ", "cu", "chu")]
        ChurchSlavic,
        [Spoken.Lang("чӑваш чӗлхи", "cv", "chv")]
        Chuvash,
        [Spoken.Lang("Kernewek", "kw", "cor")]
        Cornish,
        [Spoken.Lang("Corsu", "co", "cos")]
        Corsican,
        [Spoken.Lang("ᓀᐦᐃᔭᐍᐏᐣ", "cr", "cre")]
        Cree,
        [Spoken.Lang("Cymraeg", "cy", "cym")]
        Welsh,
        [Spoken.Lang("Dansk", "da", "dan")]
        Danish,
        [Spoken.Lang("Deutsch", "de", "deu")]
        German,
        [Spoken.Lang("‫ދިވެހި", "dv", "div")]
        Dhivehi,
        [Spoken.Lang("རྫོང་ཁ", "dz", "dzo")]
        Dzongkha,
        [Spoken.Lang("Ελληνικά", "el", "ell")]
        ModernGreek,
        [Spoken.Lang("English", "en", "eng")]
        English,
        [Spoken.Lang("Esperanto", "eo", "epo")]
        Esperanto,
        [Spoken.Lang("Eesti keel", "et", "est")]
        Estonian,
        [Spoken.Lang("Euskara", "eu", "eus")]
        Basque,
        [Spoken.Lang("Ɛʋɛgbɛ", "ee", "ewe")]
        Ewe,
        [Spoken.Lang("Føroyskt", "fo", "fao")]
        Faroese,
        [Spoken.Lang("‫فارسی", "fa", "fas")]
        Persian,
        [Spoken.Lang("Vosa Vakaviti", "fj", "fij")]
        Fijian,
        [Spoken.Lang("Suomen kieli", "fi", "fin")]
        Finnish,
        [Spoken.Lang("Français", "fr", "fra")]
        French,
        [Spoken.Lang("Frysk", "fy", "fry")]
        WesternFrisian,
        [Spoken.Lang("Fulfulde", "ff", "ful")]
        Fulah,
        [Spoken.Lang("Gàidhlig", "gd", "gla")]
        ScottishGaelic,
        [Spoken.Lang("Gaeilge", "ga", "gle")]
        Irish,
        [Spoken.Lang("Galego", "gl", "glg")]
        Galician,
        [Spoken.Lang("Ghaelg", "gv", "glv")]
        Manx,
        [Spoken.Lang("Avañe'ẽ", "gn", "grn")]
        Guarani,
        [Spoken.Lang("ગુજરાતી", "gu", "guj")]
        Gujarati,
        [Spoken.Lang("Kreyòl ayisyen", "ht", "hat")]
        Haitian,
        [Spoken.Lang("‫هَوُسَ", "ha", "hau")]
        Hausa,
        [Spoken.Lang("Serbo-Croatian", "sh", "hbs")]
        SerboCroatian,
        [Spoken.Lang("‫עברית", "he", "heb")]
        Hebrew,
        [Spoken.Lang("Otjiherero", "hz", "her")]
        Herero,
        [Spoken.Lang("हिन्दी", "hi", "hin")]
        Hindi,
        [Spoken.Lang("Hiri Motu", "ho", "hmo")]
        HiriMotu,
        [Spoken.Lang("Hrvatski", "hr", "hrv")]
        Croatian,
        [Spoken.Lang("magyar", "hu", "hun")]
        Hungarian,
        [Spoken.Lang("Հայերեն", "hy", "hye")]
        Armenian,
        [Spoken.Lang("Igbo", "ig", "ibo")]
        Igbo,
        [Spoken.Lang("Ido", "io", "ido")]
        Ido,
        [Spoken.Lang("ꆇꉙ", "ii", "iii")]
        SichuanYi,
        [Spoken.Lang("ᐃᓄᒃᑎᑐᑦ", "iu", "iku")]
        Inuktitut,
        [Spoken.Lang("Interlingue", "ie", "ile")]
        Interlingue,
        [Spoken.Lang("Interlingua", "ia", "ina")]
        Interlingua,
        [Spoken.Lang("Bahasa Indonesia", "id", "ind")]
        Indonesian,
        [Spoken.Lang("Iñupiaq", "ik", "ipk")]
        Inupiaq,
        [Spoken.Lang("Íslenska", "is", "isl")]
        Icelandic,
        [Spoken.Lang("Italiano", "it", "ita")]
        Italian,
        [Spoken.Lang("Basa Jawa", "jv", "jav")]
        Javanese,
        [Spoken.Lang("日本語", "ja", "jpn")]
        Japanese,
        [Spoken.Lang("Kalaallisut", "kl", "kal")]
        Kalaallisut,
        [Spoken.Lang("ಕನ್ನಡ", "kn", "kan")]
        Kannada,
        [Spoken.Lang("कश्मीरी", "ks", "kas")]
        Kashmiri,
        [Spoken.Lang("ქართული", "ka", "kat")]
        Georgian,
        [Spoken.Lang("Kanuri", "kr", "kau")]
        Kanuri,
        [Spoken.Lang("Қазақ тілі", "kk", "kaz")]
        Kazakh,
        [Spoken.Lang("ភាសាខ្មែរ", "km", "khm")]
        Khmer,
        [Spoken.Lang("Gĩkũyũ", "ki", "kik")]
        Kikuyu,
        [Spoken.Lang("Kinyarwanda", "rw", "kin")]
        Kinyarwanda,
        [Spoken.Lang("кыргыз тили", "ky", "kir")]
        Kirghiz,
        [Spoken.Lang("коми кыв", "kv", "kom")]
        Komi,
        [Spoken.Lang("KiKongo", "kg", "kon")]
        Kongo,
        [Spoken.Lang("한국어", "ko", "kor")]
        Korean,
        [Spoken.Lang("Kuanyama", "kj", "kua")]
        Kuanyama,
        [Spoken.Lang("Kurdî", "ku", "kur")]
        Kurdish,
        [Spoken.Lang("ພາສາລາວ", "lo", "lao")]
        Lao,
        [Spoken.Lang("Latine", "la", "lat")]
        Latin,
        [Spoken.Lang("Latviešu valoda", "lv", "lav")]
        Latvian,
        [Spoken.Lang("Limburgs", "li", "lim")]
        Limburgan,
        [Spoken.Lang("Lingála", "ln", "lin")]
        Lingala,
        [Spoken.Lang("Lietuvių kalba", "lt", "lit")]
        Lithuanian,
        [Spoken.Lang("Lëtzebuergesch", "lb", "ltz")]
        Luxembourgish,
        [Spoken.Lang("kiluba", "lu", "lub")]
        LubaKatanga,
        [Spoken.Lang("Luganda", "lg", "lug")]
        Ganda,
        [Spoken.Lang("Kajin M̧ajeļ", "mh", "mah")]
        Marshallese,
        [Spoken.Lang("മലയാളം", "ml", "mal")]
        Malayalam,
        [Spoken.Lang("मराठी", "mr", "mar")]
        Marathi,
        [Spoken.Lang("македонски јазик", "mk", "mkd")]
        Macedonian,
        [Spoken.Lang("Fiteny malagasy", "mg", "mlg")]
        Malagasy,
        [Spoken.Lang("Malti", "mt", "mlt")]
        Maltese,
        [Spoken.Lang("Монгол", "mn", "mon")]
        Mongolian,
        [Spoken.Lang("Te reo Māori", "mi", "mri")]
        Maori,
        [Spoken.Lang("Bahasa Melayu", "ms", "msa")]
        Malay,
        [Spoken.Lang("ဗမာစာ", "my", "mya")]
        Burmese,
        [Spoken.Lang("Ekakairũ Naoero", "na", "nau")]
        Nauru,
        [Spoken.Lang("Diné bizaad", "nv", "nav")]
        Navajo,
        [Spoken.Lang("Ndébélé", "nr", "nbl")]
        SouthNdebele,
        [Spoken.Lang("isiNdebele", "nd", "nde")]
        NorthNdebele,
        [Spoken.Lang("Owambo", "ng", "ndo")]
        Ndonga,
        [Spoken.Lang("नेपाली", "ne", "nep")]
        Nepali,
        [Spoken.Lang("Nederlands", "nl", "nld")]
        Dutch,
        [Spoken.Lang("Norsk nynorsk", "nn", "nno")]
        NorwegianNynorsk,
        [Spoken.Lang("Norsk bokmål", "nb", "nob")]
        NorwegianBokmål,
        [Spoken.Lang("Norsk", "no", "nor")]
        Norwegian,
        [Spoken.Lang("ChiCheŵa", "ny", "nya")]
        Nyanja,
        [Spoken.Lang("Occitan", "oc", "oci")]
        Occitan,
        [Spoken.Lang("ᐊᓂᔑᓈᐯᒧᐎᓐ", "oj", "oji")]
        Ojibwa,
        [Spoken.Lang("ଓଡ଼ିଆ", "or", "ori")]
        Oriya,
        [Spoken.Lang("Afaan Oromoo", "om", "orm")]
        Oromo,
        [Spoken.Lang("Ирон ӕвзаг", "os", "oss")]
        Ossetian,
        [Spoken.Lang("ਪੰਜਾਬੀ", "pa", "pan")]
        Panjabi,
        [Spoken.Lang("पािऴ", "pi", "pli")]
        Pali,
        [Spoken.Lang("Polski", "pl", "pol")]
        Polish,
        [Spoken.Lang("Português", "pt", "por")]
        Portuguese,
        [Spoken.Lang("‫پښتو", "ps", "pus")]
        Pushto,
        [Spoken.Lang("Runa Simi", "qu", "que")]
        Quechua,
        [Spoken.Lang("Rumantsch grischun", "rm", "roh")]
        Romansh,
        [Spoken.Lang("Română", "ro", "ron")]
        Romanian,
        [Spoken.Lang("kiRundi", "rn", "run")]
        Rundi,
        [Spoken.Lang("русский язык", "ru", "rus")]
        Russian,
        [Spoken.Lang("Yângâ tî sängö", "sg", "sag")]
        Sango,
        [Spoken.Lang("संस्कृतम्", "sa", "san")]
        Sanskrit,
        [Spoken.Lang("සිංහල", "si", "sin")]
        Sinhala,
        [Spoken.Lang("Slovenčina", "sk", "slk")]
        Slovak,
        [Spoken.Lang("Slovenščina", "sl", "slv")]
        Slovenian,
        [Spoken.Lang("Davvisámegiella", "se", "sme")]
        NorthernSami,
        [Spoken.Lang("Gagana fa'a Samoa", "sm", "smo")]
        Samoan,
        [Spoken.Lang("chiShona", "sn", "sna")]
        Shona,
        [Spoken.Lang("सिन्धी", "sd", "snd")]
        Sindhi,
        [Spoken.Lang("Soomaaliga", "so", "som")]
        Somali,
        [Spoken.Lang("seSotho", "st", "sot")]
        SouthernSotho,
        [Spoken.Lang("Español", "es", "spa")]
        Spanish,
        [Spoken.Lang("Shqip", "sq", "sqi")]
        Albanian,
        [Spoken.Lang("sardu", "sc", "srd")]
        Sardinian,
        [Spoken.Lang("српски језик", "sr", "srp")]
        Serbian,
        [Spoken.Lang("SiSwati", "ss", "ssw")]
        Swati,
        [Spoken.Lang("Basa Sunda", "su", "sun")]
        Sundanese,
        [Spoken.Lang("Kiswahili", "sw", "swa")]
        Swahili,
        [Spoken.Lang("Svenska", "sv", "swe")]
        Swedish,
        [Spoken.Lang("Reo Mā`ohi", "ty", "tah")]
        Tahitian,
        [Spoken.Lang("தமிழ்", "ta", "tam")]
        Tamil,
        [Spoken.Lang("татарча", "tt", "tat")]
        Tatar,
        [Spoken.Lang("తెలుగు", "te", "tel")]
        Telugu,
        [Spoken.Lang("тоҷикӣ", "tg", "tgk")]
        Tajik,
        [Spoken.Lang("Tagalog", "tl", "tgl")]
        Tagalog,
        [Spoken.Lang("ไทย", "th", "tha")]
        Thai,
        [Spoken.Lang("ትግርኛ", "ti", "tir")]
        Tigrinya,
        [Spoken.Lang("faka Tonga", "to", "ton")]
        Tonga,
        [Spoken.Lang("seTswana", "tn", "tsn")]
        Tswana,
        [Spoken.Lang("xiTsonga", "ts", "tso")]
        Tsonga,
        [Spoken.Lang("Türkmen", "tk", "tuk")]
        Turkmen,
        [Spoken.Lang("Türkçe", "tr", "tur")]
        Turkish,
        [Spoken.Lang("Twi", "tw", "twi")]
        Twi,
        [Spoken.Lang("Uyƣurqə", "ug", "uig")]
        Uighur,
        [Spoken.Lang("українська мова", "uk", "ukr")]
        Ukrainian,
        [Spoken.Lang("‫اردو", "ur", "urd")]
        Urdu,
        [Spoken.Lang("O'zbek", "uz", "uzb")]
        Uzbek,
        [Spoken.Lang("tshiVenḓa", "ve", "ven")]
        Venda,
        [Spoken.Lang("Tiếng Việt", "vi", "vie")]
        Vietnamese,
        [Spoken.Lang("Volapük", "vo", "vol")]
        Volapük,
        [Spoken.Lang("Walon", "wa", "wln")]
        Walloon,
        [Spoken.Lang("Wollof", "wo", "wol")]
        Wolof,
        [Spoken.Lang("isiXhosa", "xh", "xho")]
        Xhosa,
        [Spoken.Lang("‫ייִדיש", "yi", "yid")]
        Yiddish,
        [Spoken.Lang("Yorùbá", "yo", "yor")]
        Yoruba,
        [Spoken.Lang("Saɯ cueŋƅ", "za", "zha")]
        Zhuang,
        [Spoken.Lang("中文", "zh", "zho")]
        Chinese,
        [Spoken.Lang("isiZulu", "zu", "zul")]
        Zulu,
    }

    public static class Spoken
    {
        [System.AttributeUsage(System.AttributeTargets.All)]
        public class LangAttribute : Attribute
        {
            public LangAttribute(string name, string iso6391, string iso6393)
            {
                NativeName = name;
                Iso6391 = iso6391;
                Iso6393 = iso6393;
            }

            public string Iso6391 { get; }
            public string Iso6393 { get; }
            public string NativeName { get; }

        }

        static readonly Dictionary<string, SpokenLang> isoMap = new Dictionary<string, SpokenLang>();
        //static readonly Dictionary<SpokenLang, CultureInfo> langToCultureInfo = new Dictionary<SpokenLang, CultureInfo>();

        static Spoken()
        {

            foreach (SpokenLang lang in Enum.GetValues(typeof(SpokenLang)))
            {
                var langAttr = lang.GetLangAttribute();
                isoMap.Add(langAttr.Iso6391, lang);
                isoMap.Add(langAttr.Iso6393, lang);
                isoMap.Add(lang.ToString(), lang);
                isoMap.Add(langAttr.NativeName, lang);
            }
        }

        public static bool TryParseLanguageString(string str, out SpokenLang result)
       => isoMap.TryGetValue(str, out result);

        public static SpokenLang ParseLanguageString(string str)
        {
            if (!TryParseLanguageString(str, out var result))
                throw new ArgumentException($"unknown language string: {str}");

            return result;
        }

        public static LangAttribute GetLangAttribute(this SpokenLang enumVal)
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(LangAttribute), false);
            return (attributes.Length > 0) ? (LangAttribute)attributes[0] : null;
        }

        public static CultureInfo ToCultureInfo(this SpokenLang language) =>
         CultureInfo.GetCultureInfo(language.GetLangAttribute().Iso6391);

    }
}