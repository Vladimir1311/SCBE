using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.Models.Rooms;
using SituationCenterCore.Data.DatabaseAbstraction;
using SituationCenterCore.Models.Rooms;
using SituationCenterCore.Models.Rooms.Security;
using System;
using System.Linq;

namespace SituationCenterCore.Tests.Models.Rooms.Security
{
    [TestClass]
    public class RoomSecurityManagerTests
    {
        #region CreateRules

        [TestMethod]
        public void CreatePublicRoom_GeneratePublicRuleType()
        {
            var (mockRepo, secureManager, room) = GetDefalt();
            secureManager.CreatePublicRule(room);
            Assert.IsNotNull(room.SecurityRule);
            Assert.AreEqual(room.SecurityRule.PrivacyRule, PrivacyRoomType.Public);
        }

        #region CreateInvationRule

        [TestMethod]
        public void CreateInvationRule_GenerateInvationRuleAndContainsAllGuids_GivenFiveGuids()
        {
            var (mockRepo, secureManager, room) = GetDefalt();
            var guids = Enumerable.Range(0, 5).Select(N => Guid.NewGuid()).ToArray();
            secureManager.CreateInvationRule(room, guids);
            Assert.IsNotNull(room.SecurityRule);
            Assert.AreEqual(PrivacyRoomType.InvationPrivate, room.SecurityRule.PrivacyRule);
            var targetData = string.Join("\n", guids.Select(G => G.ToString()));
            Assert.AreEqual(targetData, room.SecurityRule.Data);
        }

        [TestMethod]
        [DataTestMethod]
        [EmptyArrayDataRow(typeof(Guid))]
        [DataRow(null)]
        public void CreateInvationRule_ThrowsEmptyInvationRoom_GivenZeroGuidsArrayOrNull(Guid[] guids)
        {
            var (mockRepo, secureManager, room) = GetDefalt();

            var exception = Assert.ThrowsException<StatusCodeException>(
                () => secureManager.CreateInvationRule(room, guids));
            Assert.AreEqual(StatusCode.EmptyInvationRoom, exception.StatusCode);
        }

        #endregion CreateInvationRule

        #region CreatePasswordRule

        [TestMethod]
        public void CreatePasswordRule_GeneratePasswordRuleWithPasswordInData_Given6DigitsPassword()
        {
            var (mockRepo, secureManager, room) = GetDefalt();
            var password = new string('3', 6);
            secureManager.CreatePasswordRule(room, password);
            Assert.IsNotNull(room.SecurityRule);
            Assert.AreEqual(PrivacyRoomType.Password, room.SecurityRule.PrivacyRule);
            Assert.AreEqual(password, room.SecurityRule.Data);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow("12345")]
        [DataRow("abcd21")]
        [DataRow("234451234")]
        [DataRow(null)]
        public void CreatePasswordRule_ThrowExceptionIncorrectRoomPassword_GivenNonDigitsPasswordOrNo6Length(string password)
        {
            var (mockRepo, secureManager, room) = GetDefalt();
            var exception = Assert.ThrowsException<StatusCodeException>(
                () => secureManager.CreatePasswordRule(room, password));
            Assert.AreEqual(StatusCode.IncorrectRoomPassword, exception.StatusCode);
        }

        #endregion CreatePasswordRule

        #endregion CreateRules

        private (Mock<IRepository>, IRoomSecurityManager, Room) GetDefalt()
        {
            var mock = new Mock<IRepository>();
            return (mock, new RoomSecurityManager(mock.Object, new EmptyLogger<RoomSecurityManager>()), new Room());
        }
    }

    internal class EmptyLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new Disp();
        }

        private class Disp : IDisposable
        {
            public void Dispose()
            {
            }
        }

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        { }
    }

    internal class EmptyArrayDataRow : DataRowAttribute
    {
        public EmptyArrayDataRow(Type type) : base(Array.CreateInstance(type, 0))
        { }
    }
}