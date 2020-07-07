namespace Rcv.LabelTool.Web.Utilities
{
    /// <summary>
    /// Util for objectclass colors.
    /// </summary>
    public static class ObjectClassColorUtil
    {
        /// <summary>
        /// Get default color for objectclasses.
        /// </summary>
        /// <param name="objectClassId">Id of objectclass</param>
        /// <returns>Color code for default color</returns>
        public static string GetDefaultColorCode(uint objectClassId)
        {
            switch (objectClassId % 0x0F)
            {
                case 0:
                    return "0xFF0000";
                case 1:
                    return "0x212FED";
                case 2:
                    return "0x306B13";
                case 3:
                    return "0xE9E369";
                case 4:
                    return "0x8960D8";
                case 5:
                    return "0xE508C7";
                case 6:
                    return "0xFF8000";
                case 7:
                    return "0x8590E0";
                case 8:
                    return "0xACCFF5";
                case 9:
                    return "0x793212";
                case 10:
                    return "0x8BA592";
                case 11:
                    return "0x1D6563";
                case 12:
                    return "0x709EBC";
                case 13:
                    return "0x8C8FF0";
                case 14:
                    return "0x1AEFA7";
                case 15:
                    return "0x5D5152";
                default:
                    return "0xFFFFFF";
            }
        }
    }
}