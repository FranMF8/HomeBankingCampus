﻿using HomeBankingMindHub.Handlers.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace HomeBankingMindHub.Handlers.Implementations
{
    public class EncryptionHandler : IEncryptionHandler
    {
        public void EncryptPassword(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();

            salt = hmac.Key;
            hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public bool ValidatePassword(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);

            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }
    }
}
