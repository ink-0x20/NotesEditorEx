using NoteEditor.Model;
using UniRx;

namespace NoteEditor.Presenter
{
    public class SizeSpinBoxPresenter : SpinBoxPresenterBase
    {
        protected override ReactiveProperty<int> GetReactiveProperty()
        {
            return EditData.Size;
        }
    }
}
