using POAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POAM
{
    public class Authentication
    {
        private bool Logged;
        private Owner CurrentUser;
        private bool Admin;

        private static Authentication _instance = null;
        public static Authentication Instance
        {
            get
            {
                if (_instance == null) _instance = new Authentication();
                return _instance;
            }
        }

        public void UserLogin(Owner user)
        {      
            Logged = true;
            Admin = false;
            CurrentUser = user;
        }

        public void AdminLogin(Owner user)
        {
            Logged = true;
            Admin = true;
            CurrentUser = user;
        }

        public void Logout()
        {
            Logged = false;
            Admin = false;
            CurrentUser = null;
        }

        public bool isLoggedIn() { return Logged; }

        public bool isAdmin() { return Admin; }

        public Owner getCurrentUser() { return CurrentUser; }

        public String HashPassword(String password, String salt)
        {
            var combinedPassword = String.Concat(password, salt);
            var sha256 = new SHA256Managed();
            var bytes = UTF8Encoding.UTF8.GetBytes(combinedPassword);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public String GetRandomSalt(Int32 size = 12)
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new Byte[size];
            random.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public Boolean ValidatePassword(String enteredPassword, String storedHash, String storedSalt)
        {
            // Consider this function as an internal function where parameters like
            // storedHash and storedSalt are read from the database and then passed.

            var hash = HashPassword(enteredPassword, storedSalt);
            return String.Equals(storedHash, hash);
        }

    }

}
