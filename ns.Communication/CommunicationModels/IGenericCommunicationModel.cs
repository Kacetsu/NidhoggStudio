namespace ns.Communication.CommunicationModels {

    public interface IGenericCommunicationModel<T> {
        string Fullname { get; }
        bool IsSelected { get; set; }
        string Name { get; }
        string UID { get; }
    }
}