////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;

namespace FileSystemTest
{
    public class CanWrite : IMFTestInterface
    {
        [SetUp]
        public InitializeResult Initialize()
        {
            // These tests rely on underlying file system so we need to make
            // sure we can format it before we start the tests.  If we can't
            // format it, then we assume there is no FS to test on this platform.

            // delete the directory DOTNETMF_FS_EMULATION
            try
            {
                IOTests.IntializeVolume();
                Directory.CreateDirectory(TestDir);
                Directory.SetCurrentDirectory(TestDir);
            }
            catch (Exception ex)
            {
                Log.Exception("Skipping: Unable to initialize file system " + ex.Message);
                return InitializeResult.Skip;
            }
            return InitializeResult.ReadyToGo;
        }


        [TearDown]
        public void CleanUp()
        {
        }



        #region local vars
        private const string TestDir = "CanWrite";
        private const string fileName = "test.tmp";
        private FileStream fs = null;
        #endregion local vars

        #region Helper methods
        private bool TestCanWrite(FileAccess access, bool expected)
        {
            using (FileStream fs2 = new FileStream(fileName, FileMode.Open, access))
            {
                bool success = (fs2.CanWrite == expected);
                if (!success)
                {
                    Log.Exception("Unexpected result - Expected " + expected);
                }
                return success;
            }
        }
        #endregion Helper methods

        #region Test Cases
        [TestMethod]
        public MFTestResults MainlineCases()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            fs = new FileStream(fileName, FileMode.Create);
            fs.Close();

            MFTestResults result = MFTestResults.Pass;
            try
            {
                Log.Comment("Open Read, can't write");
                if (!TestCanWrite(FileAccess.Read, false))
                    return MFTestResults.Fail;

                Log.Comment("Open Write, can write");
                if (!TestCanWrite(FileAccess.Write, true))
                    return MFTestResults.Fail;

                Log.Comment("Open Read/Write, can write");
                if (!TestCanWrite(FileAccess.ReadWrite, true))
                    return MFTestResults.Fail;

                Log.Comment("Closed Stream, can't write");
                if (fs.CanWrite)
                    return MFTestResults.Fail;
            }
            catch (Exception ex)
            {
                Log.Exception("Unexpected exception: " + ex.Message);
                return MFTestResults.Fail;
            }

            return result;
        }
        #endregion Test Cases

        public MFTestMethod[] Tests
        {
            get
            {
                return new MFTestMethod[] 
                {
                    new MFTestMethod( MainlineCases, "MainlineCases" ),
                };
             }
        }
    }
}
