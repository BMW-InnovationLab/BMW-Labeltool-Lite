namespace RCV.Babel.Attribute
{
    /// <summary>
    /// Attribute for properties which marks a property as translatable.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class Translatable : System.Attribute
    {
        /// <summary>
        /// ctor.
        /// </summary>
        public Translatable()
        {
        }
    }
}
