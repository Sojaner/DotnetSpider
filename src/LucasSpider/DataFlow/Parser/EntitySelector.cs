using System;

namespace LucasSpider.DataFlow.Parser
{
    /// <summary>
    /// Entity selector
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntitySelector : Selector
    {
        /// <summary>
        /// Take the first Take entities from the final parsed result
        /// </summary>
        public int Take { get; set; }

        /// <summary>
        /// Set the direction of Take, the default is to take from the head
        /// </summary>
        public bool TakeByDescending { get; set; }
    }
}
