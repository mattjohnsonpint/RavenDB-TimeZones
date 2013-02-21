using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Topology.Utilities;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Connection;

namespace Raven.TimeZones
{
    internal class DataImporter
    {
        private readonly IDocumentStore _documentStore;
        private readonly string _databaseName;
        private readonly IDatabaseCommands _databaseCommands;

        public DataImporter(IDocumentStore documentStore, string databaseName = null)
        {
            _documentStore = documentStore;
            _databaseName = databaseName;

            _databaseCommands = _databaseName == null
                                    ? _documentStore.DatabaseCommands.ForSystemDatabase()
                                    : _documentStore.DatabaseCommands.ForDatabase(_databaseName);
        }

        public void ImportShapefile(string path, bool replaceExistingShapes = false)
        {
            if (_databaseCommands.Head("ZoneShapes/1") != null)
            {
                if (!replaceExistingShapes)
                    throw new InvalidOperationException(
                        "There are existing zone shapes in the database.  If you would like to replace them, set the replaceExistingShapes parameter to true.");

                while (_databaseCommands.GetStatistics().StaleIndexes.Contains("ZoneShapesIndex"))
                    Thread.Sleep(100);

                _databaseCommands.DeleteByIndex("ZoneShapesIndex", new IndexQuery(), allowStale: false);
            }

            var sw = new Stopwatch();
            sw.Start();

            using (var bulkInsert = _documentStore.BulkInsert(_databaseName))
            using (var fs = FeatureSet.Open(path))
            {
                var writer = new WktWriter();
                var numRows = fs.NumRows();
                for (int i = 0; i < numRows; i++)
                {
                    var shape = fs.GetShape(i, false);

                    var zone = (string) shape.Attributes[0];

                    var geometry = shape.ToGeometry();
                    var wkt = writer.Write((Geometry) geometry);

                    var zoneShape = new ZoneShape { Zone = zone, Shape = wkt };
                    bulkInsert.Store(zoneShape, "ZoneShapes/" + (i + 1));
                }

                sw.Stop();
                Debug.WriteLine("Imported {0} shapes in {1:N1} seconds.", numRows, sw.ElapsedMilliseconds / 1000D);
            }
        }
    }
}
