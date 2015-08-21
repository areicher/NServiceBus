﻿namespace NServiceBus
{
    /// <summary>
    /// Represents a name of a logical endpoint endpoint.
    /// </summary>
    public sealed class EndpointName
    {
        readonly string name;

        /// <summary>
        /// Creates a new logical endpoint name.
        /// </summary>
        public EndpointName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Creates a name for an instance of a non-scaled out endpoint.
        /// </summary>
        public EndpointInstanceName CreateSingleInstanceName()
        {
            return new EndpointInstanceName(this, "");
        }

        /// <summary>
        /// Creates a name for an instance of this endpoint using a provided host-dependent discriminator (e.g. machine name, role instance name etc.).
        /// </summary>
        /// <param name="instanceDiscriminator">A discriminator (e.g. machine name, role instance name etc.).</param>
        public EndpointInstanceName CreateInstanceName(string instanceDiscriminator)
        {
            Guard.AgainstNullAndEmpty(instanceDiscriminator, "instanceDiscriminator");
            return new EndpointInstanceName(this, instanceDiscriminator);
        }

        /// <summary>
        /// Returns the string representation of the endpoint name.
        /// </summary>
        public override string ToString()
        {
            return name;
        }

        bool Equals(EndpointName other)
        {
            return string.Equals(name, other.name);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
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
            return obj is EndpointName && Equals((EndpointName) obj);
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
        public static bool operator ==(EndpointName left, EndpointName right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        public static bool operator !=(EndpointName left, EndpointName right)
        {
            return !Equals(left, right);
        }
    }
}