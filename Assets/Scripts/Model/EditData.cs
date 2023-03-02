using NoteEditor.Notes;
using NoteEditor.Utility;
using System.Collections.Generic;
using UniRx;

namespace NoteEditor.Model
{
    public class EditData : SingletonMonoBehaviour<EditData>
    {
        ReactiveProperty<string> name_ = new ReactiveProperty<string>();
        ReactiveProperty<int> size_ = new ReactiveProperty<int>(1);
        ReactiveProperty<int> special_ = new ReactiveProperty<int>(0);
        ReactiveProperty<int> maxBlock_ = new ReactiveProperty<int>(5);
        ReactiveProperty<int> LPB_ = new ReactiveProperty<int>(4);
        ReactiveProperty<int> BPM_ = new ReactiveProperty<int>(120);
        ReactiveProperty<int> offsetSamples_ = new ReactiveProperty<int>(0);
        ReactiveProperty<int> level_ = new ReactiveProperty<int>(1);
        Dictionary<NotePosition, NoteObject> notes_ = new Dictionary<NotePosition, NoteObject>();

        public static ReactiveProperty<string> Name { get { return Instance.name_; } }
        public static ReactiveProperty<int> Size { get { return Instance.size_; } }
        public static ReactiveProperty<int> Special { get { return Instance.special_; } }
        public static ReactiveProperty<int> MaxBlock { get { return Instance.maxBlock_; } }
        public static ReactiveProperty<int> LPB { get { return Instance.LPB_; } }
        public static ReactiveProperty<int> BPM { get { return Instance.BPM_; } }
        public static ReactiveProperty<int> OffsetSamples { get { return Instance.offsetSamples_; } }
        public static ReactiveProperty<int> Level { get { return Instance.level_; } }
        public static Dictionary<NotePosition, NoteObject> Notes { get { return Instance.notes_; } }
    }
}
