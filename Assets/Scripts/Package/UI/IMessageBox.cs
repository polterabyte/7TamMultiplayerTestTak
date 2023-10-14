namespace STamMultiplayerTestTak.Package.UI
{
    public interface IMessageBox<T> : IPopup
    {
        public T InOutData { get; set; }
    }
}