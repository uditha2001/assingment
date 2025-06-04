using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using userService.Api.repository.interfaces;
using userService.Api.service;

namespace UserService.Test.Unit.Services
    {
    public class UserServiceIMPLTest
        {
        private readonly UserServiceIMPL userServiceIMPL;
        private readonly Mock<IUserRepository> userRepositoryMock;
        }
    }
