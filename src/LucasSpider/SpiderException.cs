using System;

namespace LucasSpider
{
    public class SpiderException : Exception
    {
        public SpiderException(string msg) : base(msg)
        {
        }
    }
}