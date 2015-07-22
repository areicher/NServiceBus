namespace NServiceBus.Core.Tests.Msmq
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class LogicalAddressTests
    {
        [Test]
        public void Should_return_single_part_if_there_is_no_parent()
        {
            var address = LogicalAddress.CreateTopLevel("Root");
            var parts = address.GetNameParts().ToArray();

            Assert.AreEqual(1, parts.Length);
            Assert.AreEqual("Root", parts[0]);
        }

        [Test] 
        public void Should_return_multi_part_for_child_address()
        {
            var address = LogicalAddress.CreateTopLevel("Root").Subscope("First").Subscope("Second");
            var parts = address.GetNameParts().ToArray();

            Assert.AreEqual(3, parts.Length);
            Assert.AreEqual("Root", parts[0]);
            Assert.AreEqual("First", parts[1]);
            Assert.AreEqual("Second", parts[2]);
        }
    }
}