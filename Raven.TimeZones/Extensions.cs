using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client;

namespace Raven.TimeZones
{
    public static class Extensions
    {
        public static void InitializeTimeZones(this IDocumentStore documentStore)
        {
            var index = new ZoneShapesIndex();
            documentStore.ExecuteIndex(index);

            // Query once to initialize spatial stuff.  Take the hit now to prevent delay on first query later.
            using (var session = documentStore.OpenSession())
                session.GetZoneForLocation(0, 0);
        }

        public static void ImportTimeZoneShapes(this IDocumentStore documentStore, string databaseName, string shapefilePath, bool replaceExistingShapes = false)
        {
            var importer = new DataImporter(documentStore, databaseName);
            importer.ImportShapefile(shapefilePath, replaceExistingShapes);
        }

        public static void ImportTimeZoneShapes(this IDocumentStore documentStore, string shapefilePath, bool replaceExistingShapes = false)
        {
            var importer = new DataImporter(documentStore);
            importer.ImportShapefile(shapefilePath, replaceExistingShapes);
        }

        public static string GetZoneForLocation(this IDocumentSession session, double latitude, double longitude)
        {
            // note: WKT uses lon/lat ordering
            var point = string.Format(CultureInfo.InvariantCulture, "POINT ({0} {1})", longitude, latitude);

            var results = session.Query<ZoneShape, ZoneShapesIndex>()
                                .Customize(x => x.RelatesToShape("location", point, SpatialRelation.Intersects))
                                .ToList();

            foreach (var x in results)
                Debug.WriteLine(x.Zone);

            var result = results.FirstOrDefault();

            return result == null ? null : result.Zone;
        }
    }
}
