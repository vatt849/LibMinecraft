using LibMinecraft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LibMinecraft.Model.Blocks;
using LibMinecraft.Server;
using LibMinecraft.Model.Entities;
using System.IO;

namespace LibMinecraftUnitTests
{
    /// <summary>
    ///This is a test class for WorldTest and is intended
    ///to contain all WorldTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WorldTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            MultiplayerServer mServer = new MultiplayerServer(new MinecraftServer());
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GetBlock
        ///</summary>
        [TestMethod()]
        public void GetBlockTest()
        {
            Level Level = new Level(new DefaultGenerator());

            Block b = Level.Overworld.GetBlock(new Vector3(1, 15, 1));
            Assert.IsInstanceOfType(b, typeof(GrassBlock));

            Level.Overworld.SetBlock(new Vector3(1, 15, 1), new AirBlock());

            b = Level.Overworld.GetBlock(new Vector3(1, 15, 1));
            Assert.IsInstanceOfType(b, typeof(AirBlock));
        }

        [TestMethod()]
        public void BlockUpdateTest()
        {
            Level Level = new Level(new DefaultGenerator());

            // Set blocks
            Level.Overworld.SetBlock(new Vector3(1, 16, 1), new StoneBlock());
            Level.Overworld.SetBlock(new Vector3(1, 17, 1), new TorchBlock());

            Block b = Level.Overworld.GetBlock(new Vector3(1, 17, 1));

            Assert.IsInstanceOfType(b, typeof(TorchBlock));

            // Remove stone
            Level.Overworld.SetBlock(new Vector3(1, 16, 1), new AirBlock());

            // Check if torch is there
            b = Level.Overworld.GetBlock(new Vector3(1, 17, 1));

            Assert.IsInstanceOfType(b, typeof(AirBlock));
        }

        [TestMethod()]
        public void AnvilSaveTest()
        {
            Level level = new Level(new DefaultGenerator());
            // Generate some columns
            for (int x = -5; x < 5; x++)
            {
                for (int z = -5; z < 5; z++)
                {
                    level.Overworld.GenerateColumn(new Vector3(x, 0, z));
                    level.Nether.GenerateColumn(new Vector3(x, 0, z));
                    level.TheEnd.GenerateColumn(new Vector3(x, 0, z));
                }
            }

            MultiplayerServer mserver = new MultiplayerServer(new MinecraftServer());
            level.Save("world", mserver);

            Assert.IsTrue(Directory.Exists("world"));
            Assert.IsTrue(Directory.Exists("world/DIM1"));
            Assert.IsTrue(Directory.Exists("world/DIM-1"));
            Assert.IsTrue(Directory.Exists("world/region"));
            Assert.IsTrue(Directory.Exists("world/players"));
            Assert.IsTrue(Directory.Exists("world/data"));
            Assert.IsTrue(Directory.Exists("world/DIM1/region"));
            Assert.IsTrue(Directory.Exists("world/DIM-1/region"));
            Assert.IsTrue(File.Exists("world/level.dat"));
        }
    }
}
