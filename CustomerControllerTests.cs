using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPIStarter.Controllers;
using WebAPIStarter.Models;
using Xunit;
using WebAPIStarter.Services.CustomerService;

namespace WebAPIStarter.Tests
{
    public class CustomerControllerTests
    {

        private Mock<ICustomerService> mockService = new Mock<ICustomerService>();

        [Fact]
        public void GetAll_WhenCalled_ReturnsOKResult()
        {
            // Arrange
            CustomerController customerController = new CustomerController(mockService.Object);

            // Act
            var getResult = customerController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(getResult);
        }

        [Fact]
        public void Create_WhenCalled_WithValidCustomer_ReturnsCreatedAtActionResult()
        {
            // Given
            CustomerController customerController = new CustomerController(mockService.Object);
            Customer newCustomer = new Customer
            {
                FirstName = "Bilbo",
                LastName = "Baggins",
                Email = "Bilbo@TheShire.net"
            };
            mockService.Setup(serv => serv.Add(newCustomer)).Returns(newCustomer);
            // When
            var getResult = customerController.Create(newCustomer);

            // Then
            Assert.IsType<CreatedAtActionResult>(getResult);
        }

        [Fact]
        public void GetOne_WhenCalled_ReturnsCustomer()
        {        
            var fakeCustomer = new Customer { Id = 1, FirstName = "Steve", LastName = "Bishop", Email = "steve.bishop@galvanize.com" };
            mockService.Setup(serv => serv.GetOne(1)).Returns(fakeCustomer);
            CustomerController SUT = new CustomerController(mockService.Object);

            // Act
            var getResult = (OkObjectResult)SUT.GetOne(1);

            // Assert
            Customer expected = new Customer { Id = 1, FirstName = "Steve", LastName = "Bishop", Email = "steve.bishop@galvanize.com" };
            getResult.Value.Should().BeEquivalentTo(expected);
        }
    }
}
