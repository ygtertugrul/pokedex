using System.Collections.Generic;

namespace Poke.Api.Model
{
    public class Pokemon
    {
        public string name { get; set; }
        public List<Description> flavor_text_entries { get; set; }
        public Habitat habitat { get; set; }
        public bool is_legendary { get; set; }
    }

    public class Habitat
    {
        public string name { get; set; }
    }

    public class Description
    {
        public string flavor_text { get; set; }
        public Language language { get; set; }
    }

    public class Language
    {
        public string name { get; set; }
    }
}

