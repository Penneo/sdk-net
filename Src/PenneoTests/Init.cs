using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Penneo;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace PenneoTests
{
   
    [SetUpFixture]
    public class Init
    {
      
        [SetUp]
        public void AssemblyInit()
        {
            PenneoConnector.Initialize(null, null);
        }
    }
}