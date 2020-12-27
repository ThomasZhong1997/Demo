using System;
using ThomasGIS.TrajectoryPackage.QuickIO;
using ThomasGIS.TrajectoryPackage;
using ThomasGIS.Coordinates;
using ThomasGIS.Vector;
using ThomasGIS.TrajectoryPackage.MapMatch;
using ThomasGIS.Network;

namespace MapMatch
{
    public class NewTrajectorySetCreator : TrajectorySetCreator
    {
        public NewTrajectorySetCreator() : base()
        {
            
        }

        public override GnssPoint Reader(string line)
        {
            string[] items = line.Split(',');
            double longitude = Convert.ToDouble(items[2]);
            double latitude = Convert.ToDouble(items[3]);
            double timeStamp = Convert.ToDouble(items[1]);
            string ID = items[0];
            return new GnssPoint(ID, longitude, latitude, timeStamp);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TrajectorySet newSet = new TrajectorySet("gnss_point.csv", new NewTrajectorySetCreator(), CoordinateGenerator.CRSParseFromEPSG(4326));

            IShapefile roadShapefile = VectorFactory.OpenShapefile("road.shp", VectorOpenMode.Common);

            IMapMatch mapMatchFunction = new HMMMapMatch(roadShapefile, 0.000173);

            IMapMatch mapMatchFunction2 = new HMMMapMatch(roadShapefile, 0.000173, roadShapefile.GetBoundaryBox(), "Direction");

            TrajectorySet matchedSet = new TrajectorySet();

            foreach (Trajectory trajectory in newSet.TrajectoryList)
            {
                Trajectory mapMatchedTrajectory = mapMatchFunction.MatchRoute(trajectory, 0.000173 * 2, 10);

                matchedSet.AddTrajectory(mapMatchedTrajectory);
            }

            IShapefile matchedShapefile = matchedSet.ExportToShapefile(CoordinateGenerator.CRSParseFromEPSG(4326));

            matchedShapefile.ExportShapefile("matchedTrajectory2.shp");
        }
    }
}
