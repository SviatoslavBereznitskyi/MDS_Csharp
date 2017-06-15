using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDS
{
  
    public class MDS
    {
         public static double[,] fullmds(double[,] d, int dim)
        {
            double[,] result = new double[dim, d.GetLength(0)];
            Data.randomize(ref result);
            double[] evals = new double[result.GetLength(0)];
            Data.squareEntries(ref d);
            Data.doubleCenter(ref d);
            Data.multiply(ref d, -0.5D);
            Data.eigen(ref d, ref result, ref evals);
            for (int i = 0; i < result.GetLength(0); i++)
            {
                evals[i] = Math.Sqrt(evals[i]);
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] *= evals[i];
                }
            }
            return result;
        }

        public static double[,] classicalScaling(double[,] distances, int dim)
        {
            double[,] d = Data.copyMatrix(distances);
            return fullmds(d, dim);
        }

        public static double[,] distanceScaling(double[,] d, int dim)
        {
            double[,] x = classicalScaling(d, dim);
            SMACOF smacof = new SMACOF(d, x);
            smacof.iterate(100, 3, 10 * 60000);
            return smacof.getPositions();
        }
        public static double[,] distanceScalingLandmark(double[,] d, int dim)
        {
            double[,] x = classicalScaling(d, dim);
            LandmarkMDS smacof = new LandmarkMDS(d, x);
            smacof.iterate(100, 15, 10 * 60000);
            return smacof.getPositions();
        }
    }

}
