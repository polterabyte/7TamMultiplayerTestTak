namespace MillstonesGame.UI
{
    public interface IMessageBox<T> : IPopup
    {
        public T InOutData { get; set; }
    }
}