using Stephens.Camera;

namespace Stephens.Player
{
    public interface IPlayerUI
    {
        UICamera UICamera { get; set; }
        
        void Init(UICamera uiCamera)
        {
            UICamera = uiCamera;
        }
    }
}