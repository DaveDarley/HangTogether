using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace HangTogether
{
    public class SecureMdp
    {
        /*
         * Fonction qui s'occupe d'encrypter les mdp des users lorsqu'il signUp.
         * Personne doit avoir acces au mdp de l'user incluant le developpeur
         * Src:https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-6.0
         */
        
        public static string encryptPassword(string mdpToEncrypt)
        {

            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password:mdpToEncrypt,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            
            return hashed;
        }
        
        
    }
}