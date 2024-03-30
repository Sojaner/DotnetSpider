namespace LucasSpider.DataFlow.Storage
{
	/// <summary>
	/// Memory type
	/// </summary>
    public enum StorageMode
    {
        /// <summary>
        /// Perform insert directly
        /// </summary>
        Insert,

        /// <summary>
        /// Insert unique data
        /// </summary>
        InsertIgnoreDuplicate,

        /// <summary>
        /// Insert if the primary key does not exist, update if it exists
        /// </summary>
        InsertAndUpdate,

        /// <summary>
        /// Direct update
        /// </summary>
        Update
    }
}
