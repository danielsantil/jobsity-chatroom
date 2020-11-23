using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Controllers;
using JobsityChatroom.WebAPI.Models.Authentication;
using JobsityChatroom.WebAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace JobsityChatroom.UnitTest
{
    public class AuthenticationTest : IDisposable
    {
        private readonly IUserService userService;
        private readonly IUserStore<ApplicationUser> userStore;
        private readonly Mock<UserManager<ApplicationUser>> userManagerMock;
        private readonly ApplicationUser testUser;

        public AuthenticationTest()
        {
            testUser = new ApplicationUser
            {
                Id = "1",
                Email = "test@test.com",
                UserName = "testuser"
            };

            userStore = Mock.Of<IUserStore<ApplicationUser>>();
            userManagerMock = new Mock<UserManager<ApplicationUser>>(userStore,
                null, null, null, null, null, null, null, null);
            userManagerMock.SetReturnsDefault(Task.FromResult(IdentityResult.Success));

            userService = new UserService(userManagerMock.Object);
        }

        [Fact]
        public async Task RegisterNewUser_Success()
        {
            // mocking user manager response
            userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(testUser);

            var response = await userService.Register(new SignupViewModel
            {
                Email = testUser.Email,
                Username = testUser.UserName,
                Password = "password"
            });

            Assert.NotNull(response);
        }

        [Fact]
        public async Task RegisterNewUser_Fail()
        {
            ApplicationUser nullUser = null;
            userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(nullUser);

            var response = await userService.Register(new SignupViewModel
            {
                Email = testUser.Email,
                Username = testUser.UserName,
                Password = "password"
            });

            Assert.Null(response);
        }

        [Fact]
        public async Task LoginUser_Success()
        {
            userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(testUser);
            userManagerMock.Setup(x => x.CheckPasswordAsync(testUser, It.IsAny<string>()))
                .ReturnsAsync(true);

            var response = await userService.Login(new AuthViewModel
            {
                Username = testUser.UserName,
                Password = "password"
            });

            Assert.NotNull(response);
        }

        [Fact]
        public async Task LoginUser_Fail()
        {
            userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(testUser);
            // ensuring password fails
            userManagerMock.Setup(x => x.CheckPasswordAsync(testUser, It.IsAny<string>()))
                .ReturnsAsync(false);

            var response = await userService.Login(new AuthViewModel
            {
                Username = testUser.UserName,
                Password = "password"
            });

            Assert.Null(response);
        }

        public void Dispose()
        {
            userStore.Dispose();
        }
    }
}
