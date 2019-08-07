using FileHelpers;

namespace SpellParser
{
    [DelimitedRecord(",")]
    public class RawSpell
    {
        public string Element { get; set; }
        public string Group { get; set; }
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Domains { get; set; }

        public string SpellName { get; set; }
        public string P { get; set; }
        public string R { get; set; }
        public string D { get; set; }
        public string Type { get; set; }
        public string Complex { get; set; }
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string SpellMechanics { get; set; }

        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Techniques { get; set; }

        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Adjuncts { get; set; }

        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Fallacies { get; set; }

        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Signs { get; set; }

        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Incantation { get; set; }
    }

}
