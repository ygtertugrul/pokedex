namespace Poke.Api.Model
{
    public class TranslationResponse
    {
        public Contents contents { get; set; }
    }

    public class Contents
    {
        public string translated { get; set; }
    }
}