using System;
using System.IO;
using System.Net;
using NUnit.Framework;

namespace GossipMesh.Core.Tests
{
    [TestFixture]
    public class StreamExtensionsTests
    {
        [Test]
        public void ReadIPAddress_WithNullStream_ThrowsNullReferenceException()
        {
            // arrange
            Stream stream = null;

            // assert
            Assert.Throws(typeof(NullReferenceException), () => stream.ReadIPAddress());
        }

        [Test]
        public void ReadIPAddress_WithCompleteStream_ReturnsIPAddress()
        {
            // arrange
            var adressBuffer = new byte[] { 192, 168, 0, 1 };
            Stream stream = new MemoryStream(adressBuffer, false);

            // act
            var actual = stream.ReadIPAddress();

            // assert
            var expected = new IPAddress(adressBuffer);
         
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReadPort_WithNullStream_ThrowsNullReferenceException()
        {
            // arrange
            Stream stream = null;

            // assert
            Assert.Throws(typeof(NullReferenceException), () => stream.ReadPort());
        }

        [Test]
        public void ReadPort_WithCompleteStream_ReturnsPort()
        {
            // arrange
            var portBuffer = new byte[] { 255, 255 };
            Stream stream = new MemoryStream(portBuffer, false);

            // act
            var actual = stream.ReadPort();

            // assert
            var expected = ushort.MaxValue;
         
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReadIPEndPoint_WithNullStream_ThrowsNullReferenceException()
        {
            // arrange
            Stream stream = null;

            // assert
            Assert.Throws(typeof(NullReferenceException), () => stream.ReadIPEndPoint());
        }

        [Test]
        public void ReadIPEndPoint_WithCompleteStream_ReturnsIPEndPoint()
        {
            // arrange
            var ipEndPointBuffer = new byte[] { 192, 168, 0, 1, 255, 255 };
            Stream stream = new MemoryStream(ipEndPointBuffer, false);

            // act
            var actual = stream.ReadIPEndPoint();

            // assert
            var expectedAddress = new IPAddress(new byte[] { 192, 168, 0, 1 });
            var expected = new IPEndPoint(expectedAddress, ushort.MaxValue);
            
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WriteIPAddress_WithNullStream_ThrowsNullReferenceException()
        {
            // arrange
            Stream stream = null;
            IPAddress ipAddress = new IPAddress(new byte[] { 192, 168, 0, 1 });

            // assert
            Assert.Throws(typeof(NullReferenceException), () => stream.WriteIPAddress(ipAddress));
        }

        [Test]
        public void WriteIPAddress_WithNullIPAddress_ThrowsArgumentNullException()
        {
            // arrange
            byte[] buffer = new byte[4];
            Stream stream = new MemoryStream(buffer, true);
            IPAddress ipAddress = null;

            // assert
            Assert.Throws(typeof(ArgumentNullException), () => stream.WriteIPAddress(ipAddress));
        }

        [Test]
        public void WriteIPAddress_WithEnoughCapacity_WritesSuccessfully()
        {
            // arrange
            byte[] buffer = new byte[4];
            Stream stream = new MemoryStream(buffer, true);

            byte[] ipAddressBytes = new byte[] { 192, 168, 0, 1 };
            IPAddress ipAddress = new IPAddress(ipAddressBytes);

            // act
            stream.WriteIPAddress(ipAddress);

            // assert
            Assert.AreEqual(ipAddressBytes, buffer);
        }             

        [Test]
        public void WritePort_WithNullStream_ThrowsNullReferenceException()
        {
            // arrange
            Stream stream = null;
            ushort port = ushort.MaxValue;

            // assert
            Assert.Throws(typeof(NullReferenceException), () => stream.WritePort(port));
        }

        [Test]
        public void WritePort_WithEnoughCapacity_WritesSuccessfully()
        {
            // arrange
            byte[] buffer = new byte[2];
            Stream stream = new MemoryStream(buffer, true);

            ushort port = ushort.MaxValue;

            // act
            stream.WritePort(port);

            // assert
            var expected = new byte[] { 255, 255 };

            Assert.AreEqual(expected, buffer);
        }

        [Test]
        public void WriteIPEndPoint_WithNullStream_ThrowsNullReferenceException()
        {
            // arrange
            Stream stream = null;
            IPAddress ipAddress = new IPAddress(new byte[] { 192, 168, 0, 1 });
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, ushort.MaxValue);

            // assert
            Assert.Throws(typeof(NullReferenceException), () => stream.WriteIPEndPoint(ipEndPoint));
        }

        [Test]
        public void WriteIPEndPoint_WithNullIPEndPoint_ThrowsArgumentNullException()
        {
            // arrange
            byte[] buffer = new byte[6];
            Stream stream = new MemoryStream(buffer, true);
            IPEndPoint ipEndPoint = null;

            // assert
            Assert.Throws(typeof(ArgumentNullException), () => stream.WriteIPEndPoint(ipEndPoint));
        }

        [Test]
        public void WriteIPEndPoint_WithEnoughCapacity_WritesSuccessfully()
        {
            // arrange
            byte[] buffer = new byte[6];
            Stream stream = new MemoryStream(buffer, true);

            IPAddress ipAddress = new IPAddress(new byte[] { 192, 168, 0, 1 });
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, ushort.MaxValue);

            // act
            stream.WriteIPEndPoint(ipEndPoint);

            // assert
            var expected = new byte[] { 192, 168, 0, 1, 255, 255 };

            Assert.AreEqual(buffer, expected);
        }

    }
}