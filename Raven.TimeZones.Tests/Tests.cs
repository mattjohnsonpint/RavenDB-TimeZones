using System;
using System.Diagnostics;
using Raven.Tests.Helpers;
using Xunit;

namespace Raven.TimeZones.Tests
{
    public class Tests : RavenTestBase
    {
        [Fact]
        public void Full_Usage_Test()
        {
            using (var documentStore = NewDocumentStore())
            {
                var sw = new Stopwatch();
                sw.Start();
                documentStore.InitializeTimeZones();
                Debug.WriteLine("Initialized in {0} ms.", sw.ElapsedMilliseconds);

                try
                {
                    // Import the timezone shapes.  For this test, just import the USA.  Source: http://efele.net/maps/tz/us/
                    sw.Restart();
                    documentStore.ImportTimeZoneShapes(@".\Data\Shapes\tz_us.shp");
                    Debug.WriteLine("Shapefile loaded in {0} ms.", sw.ElapsedMilliseconds);
                }
                catch (InvalidOperationException)
                {
                    // ignore when zones have already been added to the database
                    Debug.WriteLine("Zones were previously imported.");
                }

                // NOTE: It can take 3 to 5 minutes to index this data!

                sw.Restart();
                WaitForIndexing(documentStore);
                Debug.WriteLine("Indexing completed in {0} ms.", sw.ElapsedMilliseconds);

                using (var session = documentStore.OpenSession())
                {
                    sw.Restart();
                    var zone = session.GetZoneForLocation(33.45, -112.066667); // Phoenix, Arizona, USA
                    Debug.WriteLine("Query took {0} ms.", sw.ElapsedMilliseconds);
                    Assert.Equal("America/Phoenix", zone);
                }
            }
        }
    }
}
