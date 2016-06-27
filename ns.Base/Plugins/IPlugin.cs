namespace ns.Base.Plugins {

    public interface IPlugin : INode {

        /// <summary>
        /// Pres the run.
        /// </summary>
        /// <returns></returns>
        bool PreRun();

        /// <summary>
        /// Runs this instance.
        /// </summary>
        /// <returns></returns>
        bool Run();

        /// <summary>
        /// Posts the run.
        /// </summary>
        /// <returns></returns>
        bool PostRun();

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        /// <returns></returns>
        bool Terminate();
    }
}