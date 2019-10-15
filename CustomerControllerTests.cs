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
        [Fact]
        public void GetAll_WhenCalled_ReturnsOKResult()
        {
            // Arrange
            CustomerController customerController = new CustomerController();

            // Act
            var getResult = customerController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(getResult);
        }

        [Fact]
        public void Create_WhenCalled_WithValidCustomer_ReturnsCreatedAtActionResult()
        {
            // Given
            CustomerController customerController = new CustomerController();
            Customer newCustomer = new Customer
            {
                FirstName = "Bilbo",
                LastName = "Baggins",
                Email = "Bilbo@TheShire.net"
            };

            // When
            var getResult = customerController.Create(newCustomer);

            // Then
            Assert.IsType<CreatedAtActionResult>(getResult);
        }

        [Fact]
        public void GetOne_WhenCalled_ReturnsCustomer()
        {
            // Arrange
            // var customers = new List<Customer> {
            //    new Customer { Id = 1, FirstName = "Steve", LastName = "Bishop", Email = "steve.bishop@galvanize.com" },
            //    new Customer { Id = 2, FirstName = "Marla", LastName = "Gonzales", Email = "mGone@home.net" },
            //    new Customer { Id = 3, FirstName = "Alfred", LastName = "Pennyworth", Email = "alfred@thebatcave.org" }
            // };
        
            var mockService = new Mock<ICustomerService>();
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
