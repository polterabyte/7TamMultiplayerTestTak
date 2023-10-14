namespace STamMultiplayerTestTak.Package.UI
{
    public interface IUIElement
    {
        public bool IsShow { get; }
        public void Show();
        public void Hide();
    }
}