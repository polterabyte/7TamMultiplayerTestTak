﻿namespace MillstonesGame.UI
{
    public interface IPopup : IUIElement
    {
        public bool Interactable { get; set; }
    }

    public interface ISplashScreen : IUIElement
    {
    }
}