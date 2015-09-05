namespace ns.Base.Plugins {
    public interface IPlugin {
        bool Initialize();
        bool Finalize();
        bool PreRun();
        bool Run();
        bool PostRun();
        bool Terminate();
    }
}
