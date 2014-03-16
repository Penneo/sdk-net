using Microsoft.VisualStudio.TestTools.UnitTesting;
using Penneo;

namespace PenneoTests
{
    [TestClass]
    public class Init
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            PenneoConnector.Initialize(null, null);
        }
    }
}