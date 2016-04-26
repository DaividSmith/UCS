/*
 * Program : Ultrapowa Clash Server
 * Description : A C# Writted 'Clash of Clans' Server Emulator !
 *
 * Authors:  Jean-Baptiste Martin <Ultrapowa at Ultrapowa.com>,
 *           And the Official Ultrapowa Developement Team
 *
 * Copyright (c) 2016  UltraPowa
 * All Rights Reserved.
 */

using UCS.Core.Crypto.Sodium.Exceptions;

namespace UCS.Core.Crypto.Sodium
{
    /// <summary>
    ///     Scalar Multiplication
    /// </summary>
    public static class ScalarMult
    {
        #region Private Methods

        //TODO: Add documentation header
        //TODO: Unit test(s)
        private static byte Primitive()
        {
            return SodiumLibrary.crypto_scalarmult_primitive();
        }

        #endregion Private Methods

        #region Private Fields

        private const int BYTES = 32;
        private const int SCALAR_BYTES = 32;

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        ///     Diffie-Hellman (function computes the public key)
        /// </summary>
        /// <param name="secretKey">A secret key.</param>
        /// <returns>A computed public key.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        public static byte[] Base(byte[] secretKey)
        {
            //validate the length of the scalar
            if (secretKey == null || secretKey.Length != SCALAR_BYTES)
                throw new KeyOutOfRangeException("secretKey", secretKey == null ? 0 : secretKey.Length,
                    string.Format("secretKey must be {0} bytes in length.", SCALAR_BYTES));

            var publicKey = new byte[SCALAR_BYTES];
            SodiumLibrary.crypto_scalarmult_base(publicKey, secretKey);

            return publicKey;
        }

        //TODO: Add documentation header
        public static int Bytes()
        {
            return SodiumLibrary.crypto_scalarmult_bytes();
        }

        /// <summary>
        ///     Diffie-Hellman (function computes a secret shared by the two keys)
        /// </summary>
        /// <param name="secretKey">A secret key.</param>
        /// <param name="publicKey">A public key.</param>
        /// <returns>A computed secret shared.</returns>
        /// <exception cref="KeyOutOfRangeException"></exception>
        public static byte[] Mult(byte[] secretKey, byte[] publicKey)
        {
            //validate the length of the scalar
            if (secretKey == null || secretKey.Length != SCALAR_BYTES)
                throw new KeyOutOfRangeException("secretKey", secretKey == null ? 0 : secretKey.Length,
                    string.Format("secretKey must be {0} bytes in length.", SCALAR_BYTES));

            //validate the length of the group element
            if (publicKey == null || publicKey.Length != BYTES)
                throw new KeyOutOfRangeException("publicKey", publicKey == null ? 0 : publicKey.Length,
                    string.Format("publicKey must be {0} bytes in length.", BYTES));

            var secretShared = new byte[BYTES];
            SodiumLibrary.crypto_scalarmult(secretShared, secretKey, publicKey);

            return secretShared;
        }

        //TODO: Add documentation header
        public static int ScalarBytes()
        {
            return SodiumLibrary.crypto_scalarmult_scalarbytes();
        }

        #endregion Public Methods
    }
}