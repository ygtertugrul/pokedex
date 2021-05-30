using System.ComponentModel;

namespace Poke.Api.Model
{
    public enum TranslationType
    {
        Yoda,
        Shakespeare,
        JustForTest
    }

    public enum PokeHabitat
    {
        [Description("cave")]
        Cave
    }
}