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
            context.SaveChanges();
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

        [Fact]
        public void Add_WhenCalledWithAddressMissingLine1_ThrowsArgumentException()
        {
            //Given
            var SUT = new InMemoryDatabaseAddressService(context);
            Address fakeAddress = new Address();
            context.Database.EnsureDeleted();

            //When
            Action act = () => SUT.Add(fakeAddress);

            //Then
            act.Should().Throw<ArgumentException>().Where(err => err.Message.Equals("Line1 not set on address."));
        }

        [Fact]
        public void Add_WhenCalledWithInvalidAddressTypeId_ThrowsArgumentException()
        {
            //Given
            var SUT = new InMemoryDatabaseAddressService(context);
            Address fakeAddress = new Address
            {
                Line1 = "123 Main St.",
                AddressTypeId = 1
            };
            context.Database.EnsureDeleted();

            //When
            Action act = () => SUT.Add(fakeAddress);

            //Then
            act.Should().Throw<ArgumentException>().Where(err => err.Message.Equals("Given addressTypeId not found in database."));
        }

        [Fact]
        public void GetOne_WhenCalledWithIdOfAddress_ReturnsThatAddress()
        {
            //Given
            var SUT = new InMemoryDatabaseAddressService(context);
            AddressType fakeAddressType = new AddressType { AddressTypeName = "Home" };
            fakeAddressType = context.AddressTypes.Add(fakeAddressType).Entity;
            Address fakeAddress = new Address
            {
                Line1 = "123 Main St.",
                City = "Mainville",
                Zipcode = "00011",
                Country = "USA",
                AddressTypeId = fakeAddressType.Id
            };
            fakeAddress = context.Addresses.Add(fakeAddress).Entity;
            context.SaveChanges();

            //When
            Address result = SUT.GetOne(fakeAddress.Id);

            //Then
            result.Should().BeEquivalentTo(fakeAddress);
        }

        [Fact]
        public void Delete_WhenCalledWithAddressId_RemovesAddressFromDatabase()
        {
            //Given
            var SUT = new InMemoryDatabaseAddressService(context);
            AddressType fakeAddressType = new AddressType { AddressTypeName = "Home" };
            fakeAddressType = context.AddressTypes.Add(fakeAddressType).Entity;
            Address fakeAddress = new Address
            {
                Line1 = "123 Main St.",
                City = "Mainville",
                Zipcode = "00011",
                Country = "USA",
                AddressTypeId = fakeAddressType.Id
            };
            fakeAddress = context.Addresses.Add(fakeAddress).Entity;
            context.SaveChanges();

            //When
            SUT.Delete(fakeAddress.Id);

            //Then
            context.Addresses.Find(fakeAddress.Id).Should().BeNull();
        }
    }
}