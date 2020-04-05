namespace FileGenerator.Generation
{
    public class EntryInfo
    {
        /// <summary>
        /// Service length: dot, space, \r, \n
        /// </summary>
        public const int ServiceLength = 4;
        
        /// <summary>
        /// Shortest entry should have at least one digit and one character
        /// </summary>
        public static readonly int MinLength = ServiceLength + 2;

        public int NumberLength { get; set; }
        
        public int LineLength { get; set; }
        
        public bool IsDuplicated { get; set; }
        
        public int Length => ServiceLength + NumberLength + LineLength;
    }
}