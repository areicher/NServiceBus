namespace NServiceBus
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a logical address (independent of transport).
    /// </summary>
    public sealed class LogicalAddress
    {
        readonly string qualifier;
        readonly LogicalAddress parent;

        private LogicalAddress(LogicalAddress parent, string qualifier)
        {
            Guard.AgainstNullAndEmpty(qualifier, "qualifier");
            this.parent = parent;
            this.qualifier = qualifier;
        }

        /// <summary>
        /// The parent logical address.
        /// </summary>
        public LogicalAddress Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// The qualifier in the parent's namespace.
        /// </summary>
        public string Qualifier
        {
            get { return qualifier; }
        }

        /// <summary>
        /// Creates a top level logical address.
        /// </summary>
        public static LogicalAddress CreateTopLevel(string root)
        {
            Guard.AgainstNullAndEmpty(root, "root");
            return new LogicalAddress(null, root);
        }

        /// <summary>
        /// Creates a new logical address derived from this one.
        /// </summary>
        /// <param name="subscopeQualifier">The qualifier for this sub scope.</param>
        /// <returns>A logical address scoped inside this one.</returns>
        public LogicalAddress Subscope(string subscopeQualifier)
        {
            return new LogicalAddress(this, subscopeQualifier);
        }

        /// <summary>
        /// Enumerates the parts of a full qualified logical address.
        /// </summary>
        public IEnumerable<string> GetNameParts()
        {
            if (parent != null)
            {
                foreach (var namePart in parent.GetNameParts())
                {
                    yield return namePart;
                }
            }
            yield return qualifier;
        }

        /// <summary>
        /// Returns the string representation of the endpoint name.
        /// </summary>
        public override string ToString()
        {
            return GetParentPartWithDot() + Qualifier;
        }

        object GetParentPartWithDot()
        {
            return parent != null
                ? parent + "."
                : "";
        }

        bool Equals(LogicalAddress other)
        {
            return string.Equals(Qualifier, other.Qualifier) && Equals(parent, other.parent);
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
            return obj is LogicalAddress && Equals((LogicalAddress) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Qualifier != null ? Qualifier.GetHashCode() : 0)*397) ^ (parent != null ? parent.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        public static bool operator ==(LogicalAddress left, LogicalAddress right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        public static bool operator !=(LogicalAddress left, LogicalAddress right)
        {
            return !Equals(left, right);
        }
    }
}