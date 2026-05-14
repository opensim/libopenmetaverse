/*
 * Copyright (c) 2006-2016, openmetaverse.co
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 * All rights reserved.
 *
 * - Redistribution and use in source and binary forms, with or without
 *   modification, are permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * - Neither the name of the openmetaverse.co nor the names
 *   of its contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace OpenMetaverse
{
    public static partial class Utils
    {
        public const float E = MathF.E;
        public const float LOG10E = 0.4342945f;
        public const float LOG2E = 1.442695f;
        public const float PI = MathF.PI;
        public const float TWO_PI = MathF.PI * 2.0f;
        public const float PI_OVER_TWO = MathF.PI / 2.0f;
        public const float PI_OVER_FOUR = MathF.PI / 4.0f;
        /// <summary>Used for converting degrees to radians</summary>
        public const float DEG_TO_RAD = MathF.PI / 180.0f;
        /// <summary>Used for converting radians to degrees</summary>
        public const float RAD_TO_DEG = 180.0f / MathF.PI;

        /// <summary>Provide a single instance of the CultureInfo class to
        /// help parsing in situations where the grid assumes an en-us 
        /// culture</summary>
        public static readonly System.Globalization.CultureInfo EnUsCulture =
            new System.Globalization.CultureInfo("en-us", false);

        /// <summary>UNIX epoch in DateTime format</summary>
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1);

        public static readonly byte[] EmptyBytes = Array.Empty<byte>();

        #region String Arrays

        private static readonly string[] _AssetTypeNames =
        [
            "texture",    //  0
            "sound",      //  1
            "callcard",   //  2
            "landmark",   //  3
            "script",     //  4
            "clothing",   //  5
            "object",     //  6
            "notecard",   //  7
            "category",   //  8
            string.Empty, //  9
            "lsltext",    // 10
            "lslbyte",    // 11
            "txtr_tga",   // 12
            "bodypart",   // 13
            string.Empty, // 14
            string.Empty, // 15
            string.Empty, // 16
            "snd_wav",    // 17
            "img_tga",    // 18
            "jpeg",       // 19
            "animatn",    // 20
            "gesture",    // 21
            "simstate",   // 22
            string.Empty, // 23
            "link",       // 24
            "link_f",     // 25
            string.Empty, // 26
            string.Empty, // 27
            string.Empty, // 28
            string.Empty, // 29
            string.Empty, // 30
            string.Empty, // 31
            string.Empty, // 32
            string.Empty, // 33
            string.Empty, // 34
            string.Empty, // 35
            string.Empty, // 36
            string.Empty, // 37
            string.Empty, // 38
            string.Empty, // 39
            string.Empty, // 40
            string.Empty, // 41
            string.Empty, // 42
            string.Empty, // 43
            string.Empty, // 44
            string.Empty, // 45
            string.Empty, // 46
            string.Empty, // 47
            string.Empty, // 48
            "mesh",       // 49
            string.Empty, // 50
            string.Empty, // 51
            string.Empty, // 52
            string.Empty, // 53
            string.Empty, // 54
            string.Empty, // 55
            "settings",   // 56
            "material"    // 57
        ];

        private static readonly string[] _FolderTypeNames =
        [
            "texture",    //  0
            "sound",      //  1
            "callcard",   //  2
            "landmark",   //  3
            string.Empty, //  4
            "clothing",   //  5
            "object",     //  6
            "notecard",   //  7
            "root_inv",   //  8
            string.Empty, //  9
            "lsltext",    // 10
            string.Empty, // 11
            string.Empty, // 12
            "bodypart",   // 13
            "trash",      // 14
            "snapshot",   // 15
            "lstndfnd",   // 16
            string.Empty, // 17
            string.Empty, // 18
            string.Empty, // 19
            "animatn",    // 20
            "gesture",    // 21
            string.Empty, // 22
            "favorite",   // 23
            string.Empty, // 24
            "settings",   // 25
            "material",   // 26
            "ensemble",   // 27
            "ensemble",   // 28
            "ensemble",   // 29
            "ensemble",   // 30
            "ensemble",   // 31
            "ensemble",   // 32
            "ensemble",   // 33
            "ensemble",   // 34
            "ensemble",   // 35
            "ensemble",   // 36
            "ensemble",   // 37
            "ensemble",   // 38
            "ensemble",   // 39
            "ensemble",   // 40
            "ensemble",   // 41
            "ensemble",   // 42
            "ensemble",   // 43
            "ensemble",   // 44
            "ensemble",   // 45
            "current",    // 46
            "outfit",     // 47
            "my_otfts",   // 48
            "mesh",       // 49
            "inbox",      // 50
            "outbox",     // 51
            "basic_rt",   // 52
            "merchant",   // 53
            "stock",      // 54
        ];

        private static readonly string[] _InventoryTypeNames =
        [
            "texture",    //  0
            "sound",      //  1
            "callcard",   //  2
            "landmark",   //  3
            string.Empty, //  4
            string.Empty, //  5
            "object",     //  6
            "notecard",   //  7
            "category",   //  8
            "root",       //  9
            "script",     // 10
            string.Empty, // 11
            string.Empty, // 12
            string.Empty, // 13
            string.Empty, // 14
            "snapshot",   // 15
            string.Empty, // 16
            "attach",     // 17
            "wearable",   // 18
            "animation",  // 19
            "gesture",    // 20
            string.Empty, // 21
            "mesh",       // 22
            string.Empty, // 23
            string.Empty, // 24
            "settings",   // 25
            "material", // 26
        ];

        private static readonly string[] _SaleTypeNames =
        [
            "not",
            "orig",
            "copy",
            "cntn"
        ];

        private static readonly string[] _AttachmentPointNames =
        [
            string.Empty,
            "ATTACH_CHEST",
            "ATTACH_HEAD",
            "ATTACH_LSHOULDER",
            "ATTACH_RSHOULDER",
            "ATTACH_LHAND",
            "ATTACH_RHAND",
            "ATTACH_LFOOT",
            "ATTACH_RFOOT",
            "ATTACH_BACK",
            "ATTACH_PELVIS",
            "ATTACH_MOUTH",
            "ATTACH_CHIN",
            "ATTACH_LEAR",
            "ATTACH_REAR",
            "ATTACH_LEYE",
            "ATTACH_REYE",
            "ATTACH_NOSE",
            "ATTACH_RUARM",
            "ATTACH_RLARM",
            "ATTACH_LUARM",
            "ATTACH_LLARM",
            "ATTACH_RHIP",
            "ATTACH_RULEG",
            "ATTACH_RLLEG",
            "ATTACH_LHIP",
            "ATTACH_LULEG",
            "ATTACH_LLLEG",
            "ATTACH_BELLY",
            "ATTACH_LPEC",
            "ATTACH_RPEC",
            "ATTACH_HUD_CENTER_2",
            "ATTACH_HUD_TOP_RIGHT",
            "ATTACH_HUD_TOP_CENTER",
            "ATTACH_HUD_TOP_LEFT",
            "ATTACH_HUD_CENTER_1",
            "ATTACH_HUD_BOTTOM_LEFT",
            "ATTACH_HUD_BOTTOM",
            "ATTACH_HUD_BOTTOM_RIGHT",
            "ATTACH_NECK",
            "ATTACH_AVATAR_CENTER",
            "ATTACH_LHAND_RING1",
            "ATTACH_RHAND_RING1",
            "ATTACH_TAIL_BASE",
            "ATTACH_TAIL_TIP",
            "ATTACH_LWING",
            "ATTACH_RWING",
            "ATTACH_FACE_JAW",
            "ATTACH_FACE_LEAR",
            "ATTACH_FACE_REAR",
            "ATTACH_FACE_LEYE",
            "ATTACH_FACE_REYE",
            "ATTACH_FACE_TONGUE",
            "ATTACH_GROIN",
            "ATTACH_HIND_LFOOT",
            "ATTACH_HIND_RFOOT"
    ];

        #endregion String Arrays

        #region Math

        /// <summary>
        /// Clamp a given value between a range
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum allowable value</param>
        /// <param name="max">Maximum allowable value</param>
        /// <returns>A value inclusively between lower and upper</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            return (value < max) ? (value > min ? value : min) : max;
        }

        /// <summary>
        /// Clamp a given value between a range
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum allowable value</param>
        /// <param name="max">Maximum allowable value</param>
        /// <returns>A value inclusively between lower and upper</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            return (value < max) ? (value > min ? value : min) : max;
        }

        /// <summary>
        /// Clamp a given value between a range
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum allowable value</param>
        /// <param name="max">Maximum allowable value</param>
        /// <returns>A value inclusively between lower and upper</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            return (value < max) ? (value > min ? value : min) : max;
        }

        /// <summary>
        /// Round a floating-point value to the nearest integer
        /// </summary>
        /// <param name="val">Floating point number to round</param>
        /// <returns>Integer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(float val)
        {
            return (int)Math.Floor(val + 0.5f);
        }

        /// <summary>
        /// Test if a single precision float is a finite number
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(float value)
        {
            return !(Single.IsNaN(value) || Single.IsInfinity(value));
        }

        /// <summary>
        /// Test if a double precision float is a finite number
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(double value)
        {
            return !(Double.IsNaN(value) || Double.IsInfinity(value));
        }

        /// <summary>
        /// Get the distance between two floating-point values
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Second value</param>
        /// <returns>The distance between the two values</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float value1, float value2)
        {
            return Math.Abs(value1 - value2);
        }

        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            if (amount <= 0f)
                return value1;
            if (amount >= 1f)
                return value2;

            // All transformed to double not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount;
            double sSquared = s * s;
            double sCubed = sSquared * s;
            
            return (float)((2d * v1 - 2d * v2 + t2 + t1) * sCubed +
                    (3d * v2 - 3d * v1 - 2d * t1 - t2) * sSquared +
                    t1 * s + v1);
        }

        public static double Hermite(double value1, double tangent1, double value2, double tangent2, double amount)
        {
            if (amount <= 0d)
                return value1;
            if (amount >= 1f)
                return value2;

            // All transformed to double not to lose precission
            // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
            double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount;
            double sSquared = s * s;
            double sCubed = sSquared * s;

            return (2d * v1 - 2d * v2 + t2 + t1) * sCubed +
                    (3d * v2 - 3d * v1 - 2d * t1 - t2) * sSquared +
                    t1 * s + v1;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float SmoothStep(float value1, float value2, float amount)
        {
            return Utils.Hermite(value1, 0f, value2, 0f, amount);
        }

        public static double SmoothStep(double value1, double value2, double amount)
        {
            return Utils.Hermite(value1, 0f, value2, 0f, amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(float radians)
        {
            // Factor = 180 / pi
            return radians * RAD_TO_DEG;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(float degrees)
        {
            // Factor = pi / 180
            return degrees * DEG_TO_RAD;
        }


        /// <summary>
        /// Generate a random double precision floating point value
        /// </summary>
        /// <returns>Random value of type double</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RandomDouble()
        {
            return Random.Shared.NextDouble();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproxEqual(float a, float b, float tolerance, float reltolerance = float.Epsilon)
        {
            float dif = MathF.Abs(a - b);
            if (dif <= tolerance)
                return true;

            a = MathF.Abs(a);
            b = MathF.Abs(b);
            if (b > a)
                a = b;
            return dif <= a * reltolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproxZero(float a, float tolerance)
        {
            return MathF.Abs(a) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproxZero(float a)
        {
            return MathF.Abs(a) <= 1e-6;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproxEqual(float a, float b)
        {
            float dif = MathF.Abs(a - b);
            if (dif <= 1e-6f)
                return true;

            a = MathF.Abs(a);
            b = MathF.Abs(b);
            if (b > a)
                a = b;
            return dif <= a * float.Epsilon;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHash(int a, int b)
        {
            //return ((a << 5) + a) ^ b;
            return 65599 * a + b;
        }

        #endregion Math

        /// <summary>
        /// Calculate the MD5 hash of a given string
        /// </summary>
        /// <param name="value">The string to hash</param>
        /// <returns>The MD5 hash as a string</returns>
        public static string MD5String(string value)
        {
            byte[] hash = MD5(Encoding.UTF8.GetBytes(value));

            // Convert the hash to a hex string
            StringBuilder digest = osStringBuilderCache.Acquire();
            foreach (byte b in hash)
                digest.AppendFormat(Utils.EnUsCulture, "{0:x2}", b);

            return osStringBuilderCache.GetStringAndRelease(digest);
        }

        /// <summary>
        /// Compute the MD5 hash for a byte array
        /// </summary>
        /// <param name="data">Byte array to compute the hash for</param>
        /// <returns>MD5 hash of the input data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] MD5(byte[] data)
        {
            return System.Security.Cryptography.MD5.HashData(data);
        }

        /// <summary>
        /// Compute the SHA1 hash for a byte array
        /// </summary>
        /// <param name="data">Byte array to compute the hash for</param>
        /// <returns>SHA1 hash of the input data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA1(byte[] data)
        {
            return System.Security.Cryptography.SHA1.HashData(data);
        }

        /// <summary>
        /// Calculate the SHA1 hash of a given string
        /// </summary>
        /// <param name="value">The string to hash</param>
        /// <returns>The SHA1 hash as a string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA1String(string value)
        {
            byte[] hash = SHA1(Encoding.UTF8.GetBytes(value));

            // Convert the hash to a hex string
            StringBuilder digest = osStringBuilderCache.Acquire();
            foreach (byte b in hash)
                digest.AppendFormat(Utils.EnUsCulture, "{0:x2}", b);

            return osStringBuilderCache.GetStringAndRelease(digest);
        }

        /// <summary>
        /// Compute the SHA256 hash for a byte array
        /// </summary>
        /// <param name="data">Byte array to compute the hash for</param>
        /// <returns>SHA256 hash of the input data</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA256(byte[] data)
        {
            return System.Security.Cryptography.SHA1.HashData(data);
        }

        /// <summary>
        /// Calculate the SHA256 hash of a given string
        /// </summary>
        /// <param name="value">The string to hash</param>
        /// <returns>The SHA256 hash as a string</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA256String(string value)
        {
            byte[] hash = SHA256(Encoding.UTF8.GetBytes(value));

            // Convert the hash to a hex string
            StringBuilder digest = osStringBuilderCache.Acquire();
            foreach (byte b in hash)
                digest.AppendFormat(Utils.EnUsCulture, "{0:x2}", b);

            return osStringBuilderCache.GetStringAndRelease(digest);
        }

        /// <summary>
        /// Calculate the MD5 hash of a given string
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>An MD5 hash in string format, with $1$ prepended</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5llPassword(string password)
        {
            byte[] hash = MD5(ASCIIEncoding.Default.GetBytes(password));

            StringBuilder digest = osStringBuilderCache.Acquire();
            // Convert the hash to a hex string
            digest.Append("$1$");
            foreach (byte b in hash)
                digest.AppendFormat(Utils.EnUsCulture, "{0:x2}", b);

            return osStringBuilderCache.GetStringAndRelease(digest);

        }

    }
}
