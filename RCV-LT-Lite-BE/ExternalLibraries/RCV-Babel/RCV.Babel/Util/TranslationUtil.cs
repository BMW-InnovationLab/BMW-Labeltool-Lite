using RCV.Babel.Attribute;
using RCV.Babel.DataContracts;
using RCV.Babel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RCV.Babel.Util
{
    /// <summary>
    /// Utility for translation of data.
    /// </summary>
    public static class TranslationUtil
    {
        /// <summary>
        /// Translate the translatable object to culture specific language.
        /// </summary>
        /// <param name="translatableObject">Translatable object to translate</param>
        /// <param name="culture">Culture to translate into</param>
        public static void Translate(this ITranslatable translatableObject, string culture)
        {
            #region validation

            if (translatableObject == null)
            {
                throw new ArgumentNullException(nameof(translatableObject));
            }

            if (string.IsNullOrEmpty(culture))
            {
                throw new ArgumentNullException(nameof(culture));
            }

            #endregion

            // check if translations are avaiable
            if (translatableObject.Translations != null)
            {
                // search for translation with target culture
                Translation translation = translatableObject.GetTranslation(culture);

                // search for fallback translation
                if (translation == null)
                {
                    translation = translatableObject.GetTranslationFallBack(culture);
                }

                if (translation != null)
                {
                    // translate all properties by name
                    Type translationType = translatableObject.GetType();
                    foreach (TranslationText translationText in translation.Texts)
                    {
                        translationType.GetProperty(translationText.Name).SetValue(translatableObject, translationText.Text);
                    }
                }
            }
        }

        /// <summary>
        /// Create translation structure at scope of tranlatable object.
        /// </summary>
        /// <param name="translatableObject">Translateable object</param>
        /// <param name="culture">Culture which is used to store translation</param>
        public static void SaveTranslation(this ITranslatable translatableObject, string culture)
        {
            #region validation

            if (translatableObject == null)
            {
                throw new ArgumentNullException(nameof(translatableObject));
            }

            if (string.IsNullOrEmpty(culture))
            {
                throw new ArgumentNullException(nameof(culture));
            }

            #endregion

            // create translation structure if not exists            
            if (translatableObject.Translations == null)
            {
                translatableObject.Translations = new List<Translation>();
            }

            // check if translation for specified culture already exists
            Translation translation = translatableObject.GetTranslation(culture);
            if (translation == null)
            {
                // create translation for specified culture
                translation = new Translation() { Culture = culture };
                translatableObject.Translations.Add(translation);
            }

            List<TranslationText> translationTexts = new List<TranslationText>();

            // get all properties with 'translatable' annotation which are of type string
            Type translationType = translatableObject.GetType();
            foreach (PropertyInfo translationProperty in translationType.GetTranslatableProperties())
            {
                translationTexts.Add(new TranslationText()
                {
                    Name = translationProperty.Name,
                    Text = (string)translationProperty.GetValue(translatableObject)
                });
            }

            translation.Texts = translationTexts;

            if (!string.IsNullOrEmpty(Instance.DefaultCulture))
            {
                // reset culture dependend properties to default language
                translatableObject.Translate(Instance.DefaultCulture);
            }
        }

        #region private methods

        /// <summary>
        /// Get translation structure for specified culture.
        /// </summary>
        /// <param name="translatableObject">Object which should be translated</param>
        /// <param name="culture">Culture for translation</param>
        /// <returns>Translation of specific culture or NULL</returns>
        private static Translation GetTranslation(this ITranslatable translatableObject, string culture)
        {
            #region validation

            if (string.IsNullOrEmpty(culture))
            {
                throw new ArgumentNullException(nameof(culture));
            }

            #endregion

            Translation translation = translatableObject.Translations.FirstOrDefault(o => o.Culture.Equals(culture, StringComparison.CurrentCultureIgnoreCase));

            if (translation == null)
            {
                translation = translatableObject.Translations.FirstOrDefault(o => o.Culture.StartsWith(culture, StringComparison.CurrentCultureIgnoreCase));
            }

            return translation;
        }

        /// <summary>
        /// Get fallback translation structure for specified culture.
        /// </summary>
        /// <param name="translatableObject">Object which should be translated</param>
        /// <param name="culture">Culture for translation</param>
        /// <returns>Fallback translation for specific culture or NULL</returns>
        private static Translation GetTranslationFallBack(this ITranslatable translatableObject, string culture)
        {
            #region validation

            if (string.IsNullOrEmpty(culture))
            {
                throw new ArgumentNullException(nameof(culture));
            }

            #endregion

            return translatableObject.Translations.FirstOrDefault(o => o.Culture.StartsWith(culture, StringComparison.CurrentCultureIgnoreCase)); ;
        }

        /// <summary>
        /// Get list of translatable properties for given type.
        /// </summary>
        /// <param name="type">Type where to find translatable properties</param>
        /// <returns>Propertyinfo for translatable properties</returns>
        private static IEnumerable<PropertyInfo> GetTranslatableProperties(this Type type)
        {
            return type.GetProperties().Where(o => System.Attribute.IsDefined(o, typeof(Translatable)) && o.PropertyType == typeof(string));
        }

        #endregion
    }
}
