using NoteEditor.Model;
using UniRx;

namespace NoteEditor.Presenter
{
    public class SpecialSpinBoxPresenter : SpinBoxPresenterBase
    {
        protected override ReactiveProperty<int> GetReactiveProperty()
        {
            return EditData.Special;
        }
    }
}
