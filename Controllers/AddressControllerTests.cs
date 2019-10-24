using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPIStarter.Services.AddressService;
using WebAPIStarterData.Models;
using Xunit;

namespace WebAPIStarter.Tests.Controllers
{
    public class AddressControllerTests
    {
        private Mock<IAddressService> MockAddressService => new Mock<IAddressService>();

        private List<Address> fakeAddresses => new List<Address> {
                new Address { Line1 = "123 Main St.", City = "Mainville", Zipcode = "00011", Country = "USA", AddressTypeId = 1 },
                new Address { Line1 = "456 Simple Ave.", City = "Simpleville", Zipcode = "00012", Country = "USA", AddressTypeId = 1 },
                new Address { Line1 = "1111 Basic Pl.", City = "Basicville", Zipcode = "11111", Country = "USA", AddressTypeId = 1 }
            };


        [Fact]
        public void Post_WhenCalledWithAddress_CallsAddressServiceAddMethod()
        {
            //Given
            var mock = MockAddressService;
            var fakeAddress = fakeAddresses.First();
            var SUT = new AddressController(mock.Object);
            //When
            SUT.Post(fakeAddress);
            //Then
            mock.Verify(a => a.Add(fakeAddress), Times.Once);
        }

        [Fact]
        public void Post_WhenCalledWithAddress_ReturnsCreatedAtActionResult()
        {
            //Given
            var SUT = new AddressController(MockAddressService.Object);
            //When
            IActionResult result = SUT.Post(fakeAddresses.First());
            //Then
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public void Post_WhenCalledWithNullArgument_ReturnsBadRequestObjectResult()
        {
            //Given
            var mock = MockAddressService;
            mock.Setup(serv => serv.Add(null)).Throws<ArgumentException>();
            var SUT = new AddressController(mock.Object);

            //When
            IActionResult result = SUT.Post(null);

            //Then
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void GetOne_WhenCalledWithAddressId_CallsGetOneOnAddressService()
        {
            //Given
            var mock = MockAddressService;
            var SUT = new AddressController(mock.Object);

            //When
            IActionResult result = SUT.GetOne(fakeAddresses.First().Id);

            //Then
            mock.Verify(a => a.GetOne(fakeAddresses.First().Id), Times.Once());
        }
    }

}

internal class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    internal IActionResult GetOne(long id)
    {
        return Ok(_addressService.GetOne(id));
    }

    internal IActionResult Post(Address newAddress)
    {
        try
        {
            _addressService.Add(newAddress);
            return CreatedAtAction("", "", null, null);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}