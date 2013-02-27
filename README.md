Raven.TimeZones
===============

Provides a geospatial index of [IANA/Olson Timezones][1] of the world.

Basically, given a latitude and longitude, you can determine the correct timezone.

== Usage ==

    // Set up the index.  Do this after your main .Initialize() call.
    documentStore.InitializeTimeZones();

    // Import the timezone map shapefile. Just do this one time.
    documentStore.ImportTimeZoneShapes(@"path/to/tz_world.shp");

    // After the index is built, you can use it easily whenever you need it
    var zone = session.GetZoneForLocation(latitude, longitude);

A unit test, and the shapefile, are both in the repository.

== Acknowledgements ==

Special thanks to:
 - Eric Muller, for the [source data][2].
 - Andrew Lin, for the inspiration from his excellent [javascript timezone picker][3].
 - Simon Bartlett, for his help [when I got stuck][4].

==== TODO ====
 - Download the shapefile data automatically over http from the original source.
 - Use Simon's [Geo library][5].
 - Try implementing as GeoJSON or some other format than WKT.
 - Performance testing.

 [1]: http://www.iana.org/time-zones
 [2]: http://efele.net/maps/tz/world/
 [3]: https://github.com/dosx/timezone-picker
 [4]: https://groups.google.com/d/topic/ravendb/a6xFRI8nKZc/discussion
 [5]: https://github.com/sibartlett/Geo
