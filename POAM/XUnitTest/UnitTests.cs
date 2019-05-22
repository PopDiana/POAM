using Microsoft.AspNetCore.Mvc;
using POAM;
using POAM.Controllers;
using POAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class UnitTests
    {
       
        public Apartment GenerateApartment()
        {

            var dummyApartment = new Apartment();
            dummyApartment.Building = "DummyBuilding";
            dummyApartment.NoTenants = 3;
            dummyApartment.FlatNo = "3";
            dummyApartment.Street = "DummyStreet";

            return dummyApartment;
        }

        public void RemoveApartment(POAMDbContext context, string building)
        {

            var existingApartment = context.Apartment.FirstOrDefault(a => a.Building == building);

            if (existingApartment != null)
            {
                context.Remove(existingApartment);
                context.SaveChanges();
            }
        }

        public void RemoveContract(POAMDbContext context, string contractProvider)
        {

            var existingContract = context.Contract.FirstOrDefault(c => c.Provider == contractProvider);

            if (existingContract != null)
            {
                context.Remove(existingContract);
                context.SaveChanges();
            }
        }

        public Contract GenerateContract()
        {

            var dummyContract = new Contract();
            dummyContract.Price = 30;
            dummyContract.Date = DateTime.Now;
            dummyContract.Provider = "dummyContract";
            dummyContract.Type = "Gas";

            return dummyContract;
        }

        public void RemoveAdmin(POAMDbContext context)
        {

            var existingOwner = context.Owner.FirstOrDefault(o => o.Username == "dummyAdmin");

            if (existingOwner != null)
            {
                context.Remove(existingOwner);
                context.SaveChanges();
            }
        }

        public Owner CreateUser(POAMDbContext context)
        {
            var dummyUser = GenerateUser(context);
   
            var existingOwner = context.Owner.FirstOrDefault(o => o.Username == "dummyUser");

            if (existingOwner == null)
            {
                context.Add(dummyUser);
                context.SaveChanges();
            }

            return dummyUser;
        }

        public Owner GenerateUser(POAMDbContext context)
        {

            var dummyUser = new Owner();
            dummyUser.Username = "dummyUser";
            var dummyPassword = "Pa55w*rd";
            dummyUser.PassSalt = Authentication.Instance.GetRandomSalt();
            dummyUser.Password = Authentication.Instance.HashPassword(dummyPassword, dummyUser.PassSalt);
            dummyUser.FullName = "Dummy User";
            dummyUser.Email = "dummyuser@mail.com";
            dummyUser.Telephone = "0333333333333";
            dummyUser.IsAdmin = false;

            return dummyUser;
        }


        public Owner CreateAdmin(POAMDbContext context)
        {

            var dummyAdmin = new Owner();
            dummyAdmin.Username = "dummyAdmin";
            var dummyPassword = "Pa55w*rd";
            dummyAdmin.PassSalt = Authentication.Instance.GetRandomSalt();
            dummyAdmin.Password = Authentication.Instance.HashPassword(dummyPassword, dummyAdmin.PassSalt);
            dummyAdmin.FullName = "Dummy Admin";
            dummyAdmin.Email = "dummyadmin@mail.com";
            dummyAdmin.Telephone = "0333333333333";
            dummyAdmin.IsAdmin = true;

            var existingOwner = context.Owner.FirstOrDefault(o => o.Username == "dummyAdmin");

            if (existingOwner == null)
            {
                context.Add(dummyAdmin);
                context.SaveChanges();
            }

            return dummyAdmin;
        }

        public void RemoveUser(POAMDbContext context)
        {

            var existingOwner = context.Owner.FirstOrDefault(o => o.Username == "dummyUser");

            if (existingOwner != null)
            {
                context.Remove(existingOwner);
                context.SaveChanges();
            }
        }

  
        [Fact]
        public async Task UserSuccessfulLogin()
        {

            var context = new POAMDbContext();

            var ownerController = new OwnerController(context);

            var result = await ownerController.Login(CreateUser(context));
         
            var redirectResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(redirectResult);
            Assert.True(string.IsNullOrEmpty(redirectResult.ViewName) || redirectResult.ViewName == "MainPage");

        }

  
        [Fact]
        public async Task AdminSuccessfulLogin()
        {

            var context = new POAMDbContext();

            var ownerController = new OwnerController(context);

            var result = await ownerController.Login(CreateAdmin(context));

            var redirectResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(redirectResult);
            Assert.True(string.IsNullOrEmpty(redirectResult.ViewName) || redirectResult.ViewName == "MainPage");
            RemoveAdmin(context);
        }
       

        [Fact]
        public async Task LoginWrongPassword()
        {

            var context = new POAMDbContext();

            var ownerController = new OwnerController(context);

            var user = CreateUser(context);
            user.Password = "Pa55w.rd";

            var result = await ownerController.Login(user);

            var redirectResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(redirectResult);
            Assert.True(string.IsNullOrEmpty(redirectResult.ViewName) || redirectResult.ViewName == "Login");

        }

        [Fact]
        public async Task LoginUsernameNotExisting()
        {

            var context = new POAMDbContext();

            var ownerController = new OwnerController(context);

            var user = CreateUser(context);
            user.Username = "bob";
            var result = await ownerController.Login(user);

            var redirectResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(redirectResult);
            Assert.True(string.IsNullOrEmpty(redirectResult.ViewName) || redirectResult.ViewName == "Login");

        }



        [Fact]
        public async Task AddOwnerSuccessful()
        {

            var context = new POAMDbContext();

            var ownerController = new OwnerController(context);

            RemoveAdmin(context);
            RemoveUser(context);

            Authentication.Instance.AdminLogin(CreateAdmin(context));
            var user = GenerateUser(context);
            var result = await ownerController.AddOwner(user);

            var existingOwner = context.Owner.FirstOrDefault(o => o.Username == user.Username);

            Assert.NotNull(existingOwner);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("OwnersList", redirectResult.ActionName);

            Authentication.Instance.Logout();
            RemoveAdmin(context);
        }


        [Fact]
        public async Task AddOwnerFailedUsernameExisting()
        {

            var context = new POAMDbContext();

            var ownerController = new OwnerController(context);

            RemoveAdmin(context);

            var user = CreateUser(context);
            Authentication.Instance.AdminLogin(CreateAdmin(context));

            var result = await ownerController.AddOwner(user);

            var redirectResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(redirectResult);
            Assert.True(string.IsNullOrEmpty(redirectResult.ViewName) || redirectResult.ViewName == "AddOwner");
            Authentication.Instance.Logout();
            RemoveAdmin(context);
        }

        [Fact]
        public async Task AddOwnerNotAdmin()
        {

            var context = new POAMDbContext();

            var ownerController = new OwnerController(context);

     
            RemoveUser(context);

            var user = GenerateUser(context);
            var result = await ownerController.AddOwner(user);

            var existingOwner = context.Owner.FirstOrDefault(o => o.Username == user.Username);

            Assert.Null(existingOwner);
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("~/Home/Index", redirectResult.Url);

        }


        [Fact]
        public async Task AddApartmentSuccessful()
        {

            var context = new POAMDbContext();

            RemoveUser(context);

            var user = CreateUser(context);

            Authentication.Instance.UserLogin(user);

            RemoveApartment(context,"DummyBuilding");

            var apartment = GenerateApartment();

            var apartmentController = new ApartmentController(context);

            var result = await apartmentController.AddApartment(apartment);

            var existingApartment = context.Apartment.FirstOrDefault(a => a.Building == "DummyBuilding");

           
            Assert.NotNull(existingApartment);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("MainPage", redirectResult.ActionName);

            Authentication.Instance.Logout();
            RemoveApartment(context, "DummyBuilding");
        }


        [Fact]
        public async Task AddApartmentUserNotLoggedIn()
        {

            var context = new POAMDbContext();

            RemoveApartment(context, "DummyBuilding");

            var apartment = GenerateApartment();

            var apartmentController = new ApartmentController(context);

            var result = await apartmentController.AddApartment(apartment);

            var existingApartment = context.Apartment.FirstOrDefault(a => a.Building == "DummyBuilding");

            Assert.Null(existingApartment);
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("~/Home/Index", redirectResult.Url);

        }

        [Fact]
        public async Task AddContractSuccessful()
        {
            var context = new POAMDbContext();

            RemoveContract(context, "dummyContract");

            var contract = GenerateContract();

            RemoveAdmin(context);

            Authentication.Instance.AdminLogin(CreateAdmin(context));


            var contractController = new ContractController(context);

            var result = await contractController.AddContract(contract);

            var existingContract = context.Contract.FirstOrDefault(c => c.Provider == "dummyContract");


            Assert.NotNull(existingContract);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("ContractsList", redirectResult.ActionName);

            Authentication.Instance.Logout();
            RemoveContract(context, "dummyContract");
            RemoveAdmin(context);

        }

        [Fact]
        public async Task AddContractNotAdmin()
        {
            var context = new POAMDbContext();

            RemoveContract(context, "dummyContract");

            var contract = GenerateContract();

            RemoveUser(context);

            Authentication.Instance.UserLogin(CreateUser(context));


            var contractController = new ContractController(context);

            var result = await contractController.AddContract(contract);

            var existingContract = context.Contract.FirstOrDefault(c => c.Provider == "dummyContract");


            Assert.Null(existingContract);
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("~/MainPage", redirectResult.Url);

            Authentication.Instance.Logout();

        }


        [Fact]
        public async Task FinalizeContractSuccesful()
        {
            var context = new POAMDbContext();

            RemoveContract(context, "dummyContract");

            var contract = GenerateContract();

            RemoveAdmin(context);

            Authentication.Instance.AdminLogin(CreateAdmin(context));


            var contractController = new ContractController(context);

            await contractController.AddContract(contract);

            var result = await contractController.FinalizeContract(contract.IdContract);

            var existingContract = context.Contract.FirstOrDefault(c => c.Provider == "dummyContract");


            Assert.Null(existingContract);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("ContractsList", redirectResult.ActionName);

            Authentication.Instance.Logout();
            RemoveAdmin(context);
        }


        [Fact]
        public async Task FinalizeContractNotAdmin()
        {
            var context = new POAMDbContext();

            RemoveContract(context, "dummyContract");

            var contract = GenerateContract();

            RemoveAdmin(context);

            Authentication.Instance.AdminLogin(CreateAdmin(context));


            var contractController = new ContractController(context);

            await contractController.AddContract(contract);

            Authentication.Instance.Logout();
            RemoveAdmin(context);


            var result = await contractController.FinalizeContract(contract.IdContract);

            var existingContract = context.Contract.FirstOrDefault(c => c.Provider == "dummyContract");


            Assert.NotNull(existingContract);
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("~/MainPage", redirectResult.Url);

        }

        [Fact]
        public async Task MainPageNotLoggedIn()
        {
            var context = new POAMDbContext();

            var apartmentController = new ApartmentController(context);

            var result = await apartmentController.MainPage();

            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.NotNull(redirectResult);
            Assert.Equal("~/Home/Index", redirectResult.Url);

        }

       [Fact]
        public async Task ApartmentsOwnedLoggedIn()
        {
            var context = new POAMDbContext();

            RemoveUser(context);

            Authentication.Instance.UserLogin(CreateUser(context));

            var apartmentController = new ApartmentController(context);

            var result = await apartmentController.ApartmentsOwned();


            var redirectResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(redirectResult);
            Assert.True(string.IsNullOrEmpty(redirectResult.ViewName) || redirectResult.ViewName == "ApartmentsOwned");
            var model = Assert.IsAssignableFrom<IEnumerable<Apartment>>(redirectResult.ViewData.Model);
            Authentication.Instance.Logout();
            RemoveUser(context);

        }


    }
}
