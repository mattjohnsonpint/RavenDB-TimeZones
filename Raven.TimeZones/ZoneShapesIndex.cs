using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Raven.TimeZones
{
    public class ZoneShapesIndex : AbstractIndexCreationTask<ZoneShape>
    {
        public ZoneShapesIndex()
        {
            Map = shapes => from shape in shapes
                            select new
                                {
                                    shape.Zone,
                                    _ = SpatialGenerate("location", shape.Shape, SpatialSearchStrategy.GeohashPrefixTree, 3)
                                };
        }
    }
}
