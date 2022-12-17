using System;

namespace Nanoray.Shrike
{
    /// <summary>
    /// Represents a sequence matcher exception.
    /// </summary>
    public sealed class SequenceMatcherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceMatcherException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SequenceMatcherException(string message) : base(message) { }
    }
}
