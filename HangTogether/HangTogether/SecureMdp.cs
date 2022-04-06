using System;
using System.Security.Cryptography;
using System.Text;
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
        
        public static string encryptPassword(string mdpToEncrypt, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password:mdpToEncrypt,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            
            return hashed;
        }

        
        // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
        public static byte[] getSaltForEncryption()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            return salt;
        }
        
        //FireBase accepte que des strings en valeurs alors je dois convertir 
        //mon byte array de "salt" en string pour le stocker dans ma DB et que je 
        //recupere le "salt" je dois le convertir en byte array pour pouvoir verifier
        // si le mdp entre par le user lors du log-in est pareil que celui qu'il avait entre
        // lors de son sign-up

        //Src: https://www.techiedelight.com/convert-byte-array-to-string-csharp/
        public static string byteArraySaltToString(byte[] salt)
        {
            return Convert.ToBase64String(salt);
        }

        public static byte[] stringToByteArraySalt(string salt)
        {
            return Convert.FromBase64String(salt);
        }


    }
}