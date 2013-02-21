using System;
using System.Diagnostics;
using Raven.Client.Embedded;
using Raven.Tests.Helpers;
using Xunit;

namespace Raven.TimeZones.Tests
{
    public class Tests : RavenTestBase
    {
        [Fact]
        public void Full_Usage_Test()
        {
            using (var documentStore = new EmbeddableDocumentStore())
            {
                documentStore.Initialize();
                documentStore.InitializeTimeZones();

                try
                {
                    // Import the timezone shapes.  Source: http://efele.net/maps/tz/world/
                    documentStore.ImportTimeZoneShapes(@".\Data\Shapes\tz_world.shp");
                }
                catch (InvalidOperationException)
                {
                    // ignore when zones have already been added to the database
                    Debug.WriteLine("Zones were previously imported.");
                }

                // when debugging, pause here to watch in raven studio
                WaitForUserToContinueTheTest(documentStore);

                var sw = new Stopwatch();
                WaitForIndexing(documentStore);
                Debug.WriteLine("Indexing completed in {0} ms.", sw.ElapsedMilliseconds);

                using (var sesion = documentStore.OpenSession())
                {
                    sw.Restart();
                    
                    var zone = sesion.GetZoneForLocation(33.45, -112.066667); // Phoenix, Arizona, USA
                    
                    Debug.WriteLine("Query took {0} ms.", sw.ElapsedMilliseconds);

                    Assert.Equal("America/Phoenix", zone);
                }

                // when debugging, pause here to watch in raven studio
                WaitForUserToContinueTheTest(documentStore);
            }
        }
    }
}
