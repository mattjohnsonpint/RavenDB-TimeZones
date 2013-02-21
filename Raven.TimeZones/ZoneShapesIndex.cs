﻿using System.Linq;
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
                                    _ = SpatialGenerate("location", shape.Shape)
                                };
        }
    }
}
