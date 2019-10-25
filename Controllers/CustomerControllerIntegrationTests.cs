using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using WebAPIStarter;
using WebAPIStarterData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebAPIStarter.Services.CustomerService;
using WebAPIStarter.Services.AddressService;
using WebAPIStarterData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System.Net.Http;

namespace WebAPIStarter.Tests.Controllers
{
    public class CustomerControllerIntegrationTests
    {
        private IWebHostBuilder webHostBuilder => new WebHostBuilder()
            .UseEnvironment("Test")
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services
                        .AddDbContext<WebAPIStarterContext>(options =>
                            options.UseInMemoryDatabase("mockdb-CustomerControllerIntegration")
                        );
                    services.AddTransient<ICustomerService, InMemoryDatabaseCustomerService>();
                    services.AddTransient<IAddressService, DatabaseAddressService>();
                    services.AddMvc(options => options.EnableEndpointRouting = false);
                });

        private TestServer server => new TestServer(webHostBuilder);

        private HttpClient client => server.CreateClient();

        public CustomerControllerIntegrationTests()
        {
            SeedDatabase();
        }

        [Fact]
        public async Task GetOne_WhenCalledThroughWebClientWithCustomerId_ReturnsCustomerFromDatabase()
        {

            //When
            var response = await client.GetAsync("api/Customer/1");
            response.EnsureSuccessStatusCode();
            Customer body = JsonConvert.DeserializeObject<Customer>(await response.Content.ReadAsStringAsync());

            //Then
            body.Should().BeEquivalentTo(new Customer
            {
                Id = 1,
                FirstName = "Steve",
                LastName = "Bishop",
                Email = "steve.bishop@galvanize.com"
            });
        }

        private void SeedDatabase()
        {
            using (var serviceScope = server.Host.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<WebAPIStarterContext>();
                context.Database.EnsureDeleted();

                IList<Customer> fakeCustomers = new List<Customer> {
                new Customer {
                    Id = 1,
                    FirstName = "Steve",
                    LastName = "Bishop",
                    Email = "steve.bishop@galvanize.com",
                    CustomerAddresses = new List<CustomerAddress> {
                        new CustomerAddress{ CustomerId = 1, AddressId = 1 },
                        new CustomerAddress{ CustomerId = 1, AddressId = 2 }
                    }
                }
            };

                IList<AddressType> fakeAddressTypes = new List<AddressType> {
                new AddressType { Id = 1, AddressTypeName = "Home" },
                new AddressType { Id = 2, AddressTypeName = "Work" }
            };

                IList<Address> fakeAddresses = new List<Address> {

                new Address {
                    Id = 1,
                    Line1 = "123 Fake St.",
                    City = "Fakeville",
                    StateProvince = "Fakington",
                    Zipcode = "00001",
                    AddressTypeId = 1
                },
                new Address {
                    Id = 2,
                    Line1 = "456 Double Ave.",
                    City = "Doubleton",
                    StateProvince = "Doublisconsin",
                    Zipcode = "11111",
                    AddressTypeId = 2
                }
            };
                context.Customers.AddRange(fakeCustomers);
                context.Addresses.AddRange(fakeAddresses);
                context.SaveChanges();
            }
        }
    }
}