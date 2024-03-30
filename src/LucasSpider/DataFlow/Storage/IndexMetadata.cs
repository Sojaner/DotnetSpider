using System.Linq;

namespace LucasSpider.DataFlow.Storage
{
	/// <summary>
	/// Index metadata
	/// </summary>
    public class IndexMetadata
    {
        private readonly bool _isUnique;
        private readonly string _name;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="columns">Columns</param>
        /// <param name="isUnique">Is the index unique</param>
        public IndexMetadata(string[] columns, bool isUnique = false)
        {
            Columns = columns;
            _isUnique = isUnique;
            _name = $"{(_isUnique ? "UNIQUE_" : "INDEX_")}{string.Join("_", columns.Select(x=>x.ToUpper()))}";
        }

        /// <summary>
        /// Index name
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Is it a unique index?
        /// </summary>
        public bool IsUnique => _isUnique;

        /// <summary>
        /// Indexed column
        /// </summary>
        public string[] Columns { get; }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}
