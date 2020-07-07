using RCV.Babel.Attribute;
using RCV.Babel.DataContracts;
using RCV.Babel.Models;
using System.Collections.Generic;

namespace Rcv.Babel.Tests.Mock
{
    public class Car : ITranslatable
    {
        [Translatable]
        public string Steering { get; set; }

        [Translatable]
        public string Gearbox { get; set; }

        public ICollection<Translation> Translations
        {
            get
            {
                return new List<Translation>()
                {
                    new Translation()
                    {
                        Culture = "de-DE",
                        Texts = new List<TranslationText>()
                        {
                            new TranslationText() { Name = nameof(Steering), Text = "Lenkung" },
                            new TranslationText() {Name= nameof(Gearbox), Text = "Getriebe"}
                        }
                    },
                    new Translation()
                    {
                        Culture = "en-US",
                        Texts = new List<TranslationText>()
                        {
                            new TranslationText() { Name = nameof(Steering), Text = "Steering" },
                            new TranslationText() {Name= nameof(Gearbox), Text = "Gearbox"}
                        }
                    }
                };
            }
            set
            {

            }
        }
    }
}
