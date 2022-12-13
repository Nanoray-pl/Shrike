using System;

namespace Shockah.CommonModCode.IL
{
	public sealed class ILMatcherException : Exception
	{
		public ILMatcherException(string message) : base(message) { }
	}
}