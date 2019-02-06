﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Installer.Core
{
    /// <summary>
    /// A wrapper for an installation package
    /// </summary>
    public sealed class InstallationPackage
    {
        private const string TargetFileName = "install.bin";
        private const string FileMagic = "ISE";

        /// <summary>
        /// Installation flags
        /// </summary>
        public enum InstallFlags
        {
            /// <summary>
            /// Is this an offline installation (no scoring hook)
            /// </summary>
            Offline = 0,
            /// <summary>
            /// Disables image patching (configuration of vulnerabilities to presets)
            /// </summary>
            NoPatch = 2,
            /// <summary>
            /// Does this installation use the Dynamix Online Subsystem
            /// </summary>
            OnlineSubsystemDynamix = 4,
            /// <summary>
            /// Does this installation use the Artitus Online Subsystem
            /// </summary>
            OnlineSubsystemArtitus = 8,
        }

        internal List<byte> RawData;

        /* Temp cause idk where to put this while designing

            #install.bin typedef
            byte[4] Magic = { (byte)'S', (byte)'E', (byte)'I', 0x0 };
            uint16 NumCheckDefs;
            uint16* CheckDefPtr;
            uint Flags;
            uint Reserved;
            byte[16] UniqueID; //Used for verification
            CheckDefinition[NumVulnDefs]
        */
        private enum PackageFields : int
        {
            Magic = 0x0,
            NumCheckDefs = 0x4,
            CheckDefPtr = 0x6,
            Flags = 0x8,
            Reserved = 0xA,
            UniqueID = 0x10,
        }

        /// <summary>
        /// Magic of the file
        /// </summary>
        internal byte[] Magic
        {
            get
            {
                try
                {
                    return RawData.GetBytes((int)PackageFields.Magic, 4);
                }
                catch
                {
                    return new byte[4];
                }
            }
            set
            {
                RawData.SetBytes((int)PackageFields.Magic, value);
            }
            
        }

        /// <summary>
        /// Number of check definitions
        /// </summary>
        internal ushort NumCheckDefs
        {
            get
            {
                return BitConverter.ToUInt16(RawData.GetBytes((int)PackageFields.NumCheckDefs, sizeof(ushort)), 0);
            }
            set
            {
                RawData.SetBytes((int)PackageFields.NumCheckDefs, BitConverter.GetBytes(value));
            }
        }

        /// <summary>
        /// Ptr to CheckDefs
        /// </summary>
        private ushort CheckDefsPtr
        {
            get
            {
                return BitConverter.ToUInt16(RawData.GetBytes((int)PackageFields.CheckDefPtr, sizeof(ushort)), 0);
            }
            set
            {
                RawData.SetBytes((int)PackageFields.CheckDefPtr, BitConverter.GetBytes(value));
            }
        }

        /// <summary>
        /// Flags for the installation
        /// </summary>
        internal uint Flags
        {
            get
            {
                return BitConverter.ToUInt32(RawData.GetBytes((int)PackageFields.Flags, sizeof(uint)), 0);
            }
            set
            {
                RawData.SetBytes((int)PackageFields.Flags, BitConverter.GetBytes(value));
            }
        }

        /// <summary>
        /// Just some reserved space. Probably a crc32 eventually
        /// </summary>
        private uint Reserved
        {
            get
            {
                return BitConverter.ToUInt32(RawData.GetBytes((int)PackageFields.Reserved, sizeof(uint)), 0);
            }
            set
            {
                RawData.SetBytes((int)PackageFields.Reserved, BitConverter.GetBytes(value));
            }
        }

        private byte[] UniqueID
        {
            get
            {
                try
                {
                    return RawData.GetBytes((int)PackageFields.UniqueID, 16);
                }
                catch
                {
                    return new byte[16]; //should be impossible
                }
            }
            set
            {
                RawData.SetBytes((int)PackageFields.UniqueID, value);
            }
        }

        /// <summary>
        /// An installation configuration
        /// </summary>
        /// <param name="data">The data to parse into an installation package</param>
        public InstallationPackage(byte[] data)
        {
            RawData = data.ToList();
            if (Encoding.ASCII.GetString(Magic, 0, 3) != FileMagic)
                throw new FormatException("Installation package is an unrecognized format");
        }

    }
}