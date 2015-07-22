namespace NServiceBus
{
    /// <summary>
    /// Represents a name of an endpoint instance.
    /// </summary>
    public sealed class EndpointInstanceName
    {
        readonly string name;

        internal EndpointInstanceName(EndpointName endpointName, string discriminator)
        {
            name = endpointName + discriminator;
        }

        /// <summary>
        /// Creates a top-level logical address for this endpoint instance.
        /// </summary>
        public LogicalAddress CreateTopLevelLogicalAddress()
        {
            return LogicalAddress.CreateTopLevel(name);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return name;
        }

        private bool Equals(EndpointInstanceName other)
        {
            return string.Equals(name, other.name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((EndpointInstanceName) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            return (name != null ? name.GetHashCode() : 0);
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        public static bool operator ==(EndpointInstanceName left, EndpointInstanceName right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        public static bool operator !=(EndpointInstanceName left, EndpointInstanceName right)
        {
            return !Equals(left, right);
        }
    }

}