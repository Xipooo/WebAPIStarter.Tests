using System.IO.Compression;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebAPIStarterData;
using WebAPIStarterData.Models;
using WebAPIStarter.Services.AddressService;
using Xunit;
using FluentAssertions;

namespace WebAPIStarter.Tests.Services.AddressService
{
    public class InMemoryDatabaseAddressServiceTests
    {
        private DbContextOptions<WebAPIStarterContext> options;
        private WebAPIStarterContext context;

        public InMemoryDatabaseAddressServiceTests()
        {
            options = new DbContextOptionsBuilder<WebAPIStarterContext>().UseInMemoryDatabase("mockdb-AddressService").Options;
            context = new WebAPIStarterContext(options);
        }
        [Fact]
        public void Add_WhenCalledWithValidAddress_SavesAddressToDatabase()
        {
            //Given

            var SUT = new InMemoryDatabaseAddressService(context);

            AddressType addressType = new AddressType { AddressTypeName = "Home" };
            addressType = context.AddressTypes.Add(addressType).Entity;
            Address fakeAddress = new Address
            {
                Line1 = "123 Main St.",
                City = "Mainville",
                Zipcode = "00011",
                Country = "USA",
                AddressTypeId = addressType.Id
            };

            //When
            SUT.Add(fakeAddress);

            //Then
            context.Addresses.Any(a =>
                a.Line1 == fakeAddress.Line1 &&
                a.City == fakeAddress.City &&
                a.Zipcode == fakeAddress.Zipcode
                ).Should().BeTrue();
        }
    }
}