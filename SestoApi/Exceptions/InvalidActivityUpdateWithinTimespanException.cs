using System;
namespace sesto.api.Exceptions
{
    class InvalidActivityUpdateWithinTimespanException : Exception
    {
        public InvalidActivityUpdateWithinTimespanException()
        {

        }

        public InvalidActivityUpdateWithinTimespanException(string name)
            : base(String.Format("The user attempted to update their results within the specified rate limiting timespan.", name))
        {

        }

    }
}
