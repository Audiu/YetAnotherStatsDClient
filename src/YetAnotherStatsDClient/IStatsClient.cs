namespace YetAnotherStatsDClient
{
    public interface IStatsClient
    {
        /// <summary>
        /// Send a count to the stats server
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="value"></param>
        void Count(string metric, int value = 1);

        /// <summary>
        /// Send a gauge value to the stats server
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="value"></param>
        /// <param name="changeValue">If true, change the value (+/- prefix), otherwise, set the value</param>
        void Gauge(string metric, double value, bool changeValue = false);

        /// <summary>
        /// Send timing information to the stats server
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="value"></param>
        void Timing(string metric, int value);

        /// <summary>
        /// Send timing information to the stats server
        /// </summary>
        /// <param name="metric"></param>
        /// <param name="value"></param>
        void Timing(string metric, long value);
    }
}
