using System.Collections.Generic;

namespace Prototypes.Services.Samples
{
    /// <summary>
    /// Concrete implementation of a sample service installer.
    /// Demonstrates how to register multiple interface types for services.
    /// </summary>
    public class ConcretePresetServiceInstaller : ServiceInstallerBehaviour
    {
        /// <inheritdoc />
        public override void Install(GameServiceLocator locator)
        {
            var serviceA = new List<int>();
            var serviceB = new Dictionary<int, float>();
            var serviceC = (IEnumerable<int>)serviceA;

            locator.Add<IList<int>>(serviceA);
            locator.Add<IDictionary<int, float>>(serviceB);
            locator.Add<IEnumerable<int>>(serviceC);

            _ = "Final initialization touch may be included here.";
        }
    }
}