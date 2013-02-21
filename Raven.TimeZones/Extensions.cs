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
            documentStore.ExecuteIndex(new ZoneShapesIndex());
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

            var result = session.Query<ZoneShape>()
                                .Customize(x => x.RelatesToShape("location", point, SpatialRelation.Intersects))
                                .FirstOrDefault();

            return result == null ? null : result.Zone;
        }
    }
}
