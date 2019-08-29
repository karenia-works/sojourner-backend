using System;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using Sojourner.Models;

namespace Sojourner.Tests
{
    [TestFixture]
    public class UserTest
    {
        [Test]
        public void testUserPasswordHash()
        {
            var user = new User()
            {
                password = "password"
            };
            user.hashMyPassword();
            Console.WriteLine(user.password);

            Assert.That(user.checkPassword("password"));
        }
    }
}
