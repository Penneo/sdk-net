using NUnit.Framework;
using Penneo;

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