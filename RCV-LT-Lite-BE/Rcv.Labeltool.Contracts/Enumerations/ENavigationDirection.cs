namespace Rcv.LabelTool.Contracts.Enumerations
{
    /// <summary>
    /// Direction of navigation.
    /// </summary>
    public enum ENavigationDirection
    {
        /// <summary>
        /// Get previous imagelabel without segements or labels.
        /// </summary>
        PreviousBlank,

        /// <summary>
        /// Get previous imagelabel.
        /// </summary>
        Previous,

        /// <summary>
        /// Get next imagelabel.
        /// </summary>
        Next,

        /// <summary>
        /// Get next imagelabel without segments or labels.
        /// </summary>
        NextBlank,

        /// <summary>
        /// Get last first imagelabel of trailing imagelabels without segments or labels.
        /// </summary>
        LastBlank,

        /// <summary>
        /// Get random imagelabel without segments or labels.
        /// </summary>
        Blank,

        /// <summary>
        /// Direct navigation to index.
        /// </summary>
        Direct
    }
}
