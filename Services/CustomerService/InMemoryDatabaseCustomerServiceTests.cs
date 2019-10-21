using System;
using WebAPIStarterData;
using Microsoft.EntityFrameworkCore;
using Xunit;
using WebAPIStarterData.Models;
using WebAPIStarter.Services.CustomerService;
using FluentAssertions;

namespace WebAPIStarter.Tests.Services.CustomerService
{
    public class InMemoryDatabaseCustomerServiceTests : IDisposable
    {
        private WebAPIStarterContext context;

        public InMemoryDatabaseCustomerServiceTests()
        {
            var options = new DbContextOptionsBuilder<WebAPIStarterContext>().UseInMemoryDatabase("mockdb-CustomerService").Options;
            context = new WebAPIStarterContext(options);
        }

        [Fact]
        public void Add_WhenCalled_AddsCustomerToContext()
        {
            //Given
            context.Database.EnsureDeleted();
            Customer fakeCustomer = new Customer
            {
                FirstName = "Steve",
                LastName = "Bishop",
                Email = "steve.bishop@galvanize.com"
            };
            var SUT = new InMemoryDatabaseCustomerService(context);

            //When
            var newCustomer = SUT.Add(fakeCustomer);

            //Then
            context.Customers.Find(newCustomer.Id).Should().BeEquivalentTo(newCustomer);
        }

        [Fact]
        public void Delete_WhenCalledWithExistingCustomer_RemovesCustomerFromContext()
        {
            //Given
            context.Database.EnsureDeleted();
            Customer fakeCustomer = new Customer
            {
                FirstName = "Steve",
                LastName = "Bishop",
                Email = "steve.bishop@galvanize.com"
            };
            var newCustomer = context.Customers.Add(fakeCustomer).Entity;
            var SUT = new InMemoryDatabaseCustomerService(context);

            //When
            SUT.Delete(newCustomer);

            //Then
            context.Customers.Find(newCustomer.Id).Should().BeNull();
        }




        public void Dispose()
        {
            context.Dispose();
        }
    }
}