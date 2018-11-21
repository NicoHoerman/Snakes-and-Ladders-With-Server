namespace Wrapper.Contracts
{
    public interface IErrorView : IView
    {
        void SetContent(string lastInput, string errorMessage);
        
    }


}

