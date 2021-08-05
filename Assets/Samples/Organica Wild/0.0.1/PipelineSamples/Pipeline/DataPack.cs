namespace Samples.Organica_Wild._0._0._1.PipelineSamples.Pipeline
{
    public class DataPack
    {
        public int seed;
        public string type;
        public int casual;
        public float pathAverage;
        public float pathSum;
        public int cyclesAmount;
        public double areaSizeAverage;
        public int landmarkTypesAmount;
        public int colorsAmount;
        public int silhouettesAmount;
        public double timeRunning;
        public double timeWalking;
        public double timeAll;
        public int failedActivations;
        public float averageAreaDistance;

        public string ToCsvString()
        {
            return
                $"{seed},{type},{casual},{pathAverage},{pathSum},{cyclesAmount},{areaSizeAverage}," +
                $"{landmarkTypesAmount},{colorsAmount},{silhouettesAmount},{timeRunning},{timeWalking},{timeAll},{failedActivations},{averageAreaDistance}";
        }
    }
}