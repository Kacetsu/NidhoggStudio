namespace ns.Base.Plugins {

    public interface IPlugin : INode {

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        bool TryPreRun();

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns></returns>
        bool TryRun();

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        bool TryPostRun();

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        /// <returns></returns>
        bool Terminate();
    }
}