using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SituationCenterBackServer.Controllers;
using SituationCenterBackServer.Models;
using SituationCenterBackServer.Models.VoiceChatModels;
using SituationCenterBackServer.Models.VoiceChatModels.ResponseTypes;
using SituatuionCenterTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituatuionCenterTests.Controllers
{
    [TestClass]
    public class UnrealAPIControllerTest
    {
#pragma warning disable CS0169 // The field 'UnrealAPIControllerTest.userManagerMock' is never used
        private Mock<UserManager<ApplicationUser>> userManagerMock;
#pragma warning restore CS0169 // The field 'UnrealAPIControllerTest.userManagerMock' is never used
        private Mock<IOptions<UnrealAPIConfiguration>> configMock;
        private Mock<IRoomManager> roomManagerMock;
        private Mock<ILogger<UnrealApiController>> loggerMock;

        [TestInitialize]
        public void InitDefaults()
        {
            //userManagerMock = new Mock<UserManager>();
            configMock = new Mock<IOptions<UnrealAPIConfiguration>>();
            configMock
                .Setup(O => O.Value)
                .Returns(new UnrealAPIConfiguration());
            roomManagerMock = new Mock<IRoomManager>();
            loggerMock = new Mock<ILogger<UnrealApiController>>();
        }


        [TestMethod]
        public void GetRoomDataEmpty()
        {
            roomManagerMock
                .Setup(UM => UM.Rooms)
                .Returns(new List<Room>());

            //var controller = new UnrealApiController(
            //    null, configMock.Object, roomManagerMock.Object, loggerMock.Object);
            //var roomsData = controller.GetRoomsData() as GetRoomsInfo;

            //Assert.IsNotNull(roomsData);
            //Assert.IsTrue(roomsData.Success);
            //Assert.IsNotNull(roomsData.Rooms);
            //Assert.AreEqual(roomsData.Rooms.Count(), 0);
        }
        

        [TestMethod]
        public void GetRoomDataReturnRigthCount()
        {
            int count = 115;
            roomManagerMock
                .Setup(UM => UM.Rooms)
                .Returns(new List<Room>().Add(count, 
                    () => new Room(
                        new Mock<ApplicationUser>().Object, 0)));
            //var controller = new UnrealApiController(
             //   null, configMock.Object, roomManagerMock.Object, loggerMock.Object);
            //var roomsData = controller.GetRoomsData() as GetRoomsInfo;

            //Assert.IsNotNull(roomsData);
            //Assert.IsTrue(roomsData.Success);
            //Assert.IsNotNull(roomsData.Rooms);
            //Assert.AreEqual(roomsData.Rooms.Count(), count);
        }
    }
}
