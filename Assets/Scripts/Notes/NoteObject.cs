using NoteEditor.GLDrawing;
using NoteEditor.Model;
using NoteEditor.Presenter;
using NoteEditor.Utility;
using System;
using System.Linq;
using UniRx;
using UnityEngine;

namespace NoteEditor.Notes
{
    public class NoteObject : IDisposable
    {
        public Note note = new Note();
        public ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
        public Subject<Unit> LateUpdateObservable = new Subject<Unit>();
        public Subject<Unit> OnClickObservable = new Subject<Unit>();
        public Color NoteColor { get { return noteColor_.Value; } }
        ReactiveProperty<Color> noteColor_ = new ReactiveProperty<Color>();

        Color selectedStateColor = new Color(255 / 255f, 0 / 255f, 255 / 255f);
        // ノーツ色定義
        Color longNoteColor = new Color(40 / 255f, 255 / 255f, 70 / 255f);
        Color singleNoteColor = new Color(170 / 255f, 255 / 255f, 255 / 255f);
        Color flickNoteColor = new Color(255 / 255f, 130 / 255f, 200 / 255f);
        Color specialNoteColor = new Color(255 / 255f, 255 / 255f, 0 / 255f);
        Color invalidStateColor = new Color(255 / 255f, 0 / 255f, 0 / 255f);

        ReactiveProperty<NoteTypes> noteType = new ReactiveProperty<NoteTypes>();
        CompositeDisposable disposable = new CompositeDisposable();

        public void Init()
        {
            disposable = new CompositeDisposable(
                isSelected,
                LateUpdateObservable,
                OnClickObservable,
                noteColor_,
                noteType);

            var editPresenter = EditNotesPresenter.Instance;
            noteType = this.ObserveEveryValueChanged(_ => note.type).ToReactiveProperty();

            // ノーツ色を設定
            disposable.Add(noteType.Where(_ => !isSelected.Value)
                .Merge(isSelected.Select(_ => noteType.Value))
                .Select(type => type)
                .Subscribe(noteType => noteColor_.Value = getNoteColor(noteType)));

            disposable.Add(isSelected.Where(selected => selected)
                .Subscribe(_ => noteColor_.Value = selectedStateColor));

            var mouseDownObservable = OnClickObservable
                .Select(_ => EditState.NoteType.Value)
                .Where(_ => NoteCanvas.ClosestNotePosition.Value.Equals(note.position));

            // ノーツ削除処理を登録
            disposable.Add(mouseDownObservable
                .Where(editType => editType == NoteTypes.Single
                || editType == NoteTypes.TopFlick
                || editType == NoteTypes.TopRightFlick
                || editType == NoteTypes.RightFlick
                || editType == NoteTypes.BottomRightFlick
                || editType == NoteTypes.BottomFlick
                || editType == NoteTypes.BottomLeftFlick
                || editType == NoteTypes.LeftFlick
                || editType == NoteTypes.TopLeftFlick)
                .Where(editType => editType == noteType.Value)
                .Subscribe(_ => editPresenter.RequestForRemoveNote.OnNext(note)));

            // ノーツ削除処理を登録
            disposable.Add(mouseDownObservable
                .Where(editType => editType == NoteTypes.StraightLineLong
                || editType == NoteTypes.LeftCurveLong
                || editType == NoteTypes.RightCurveLong)
                .Where(editType => editType == noteType.Value)
                .Subscribe(_ =>
                {
                    if (EditData.Notes.ContainsKey(EditState.LongNoteTailPosition.Value) && note.prev.Equals(NotePosition.None))
                    {
                        var currentTailNote = new Note(EditData.Notes[EditState.LongNoteTailPosition.Value].note);
                        currentTailNote.next = note.position;
                        editPresenter.RequestForChangeNoteStatus.OnNext(currentTailNote);

                        var selfNote = new Note(note);
                        selfNote.prev = currentTailNote.position;
                        editPresenter.RequestForChangeNoteStatus.OnNext(selfNote);
                    }
                    else
                    {
                        if (EditData.Notes.ContainsKey(note.prev) && !EditData.Notes.ContainsKey(note.next))
                            EditState.LongNoteTailPosition.Value = note.prev;

                        editPresenter.RequestForRemoveNote.OnNext(new Note(note.position, EditState.NoteType.Value, note.next, note.prev));
                        RemoveLink();
                    }
                }));

            // ロングノーツの更新処理イベント
            var longNoteUpdateObservable = LateUpdateObservable
                .Where(_ => noteType.Value == NoteTypes.StraightLineLong
                || noteType.Value == NoteTypes.LeftCurveLong
                || noteType.Value == NoteTypes.RightCurveLong);

            // ロングノーツ描写登録？
            disposable.Add(longNoteUpdateObservable
                .Where(_ => EditData.Notes.ContainsKey(note.next))
                .Select(_ => ConvertUtils.NoteToCanvasPosition(note.next))
                .Merge(longNoteUpdateObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.StraightLineLong
                    || EditState.NoteType.Value == NoteTypes.LeftCurveLong
                    || EditState.NoteType.Value == NoteTypes.RightCurveLong)
                    .Where(_ => EditState.LongNoteTailPosition.Value.Equals(note.position))
                    .Select(_ => ConvertUtils.ScreenToCanvasPosition(Input.mousePosition)))
                .Select(nextPosition => new Line(
                    ConvertUtils.CanvasToScreenPosition(ConvertUtils.NoteToCanvasPosition(note.position)),
                    ConvertUtils.CanvasToScreenPosition(nextPosition),
                    isSelected.Value || EditData.Notes.ContainsKey(note.next) && EditData.Notes[note.next].isSelected.Value ? selectedStateColor
                        : 0 < nextPosition.x - ConvertUtils.NoteToCanvasPosition(note.position).x ? longNoteColor : invalidStateColor))
                .Subscribe(line => GLLineDrawer.Draw(line)));
        }

        private Color getNoteColor(NoteTypes noteTypes)
        {
            if (EditData.Special.Value != 0)
            {
                return specialNoteColor;
            }
            if (noteTypes == NoteTypes.Single)
            {
                return singleNoteColor;
            }
            if (noteTypes == NoteTypes.StraightLineLong || noteTypes == NoteTypes.LeftCurveLong || noteTypes == NoteTypes.RightCurveLong)
            {
                return longNoteColor;
            }
            return flickNoteColor;
        }

        void RemoveLink()
        {
            if (EditData.Notes.ContainsKey(note.prev))
                EditData.Notes[note.prev].note.next = note.next;

            if (EditData.Notes.ContainsKey(note.next))
                EditData.Notes[note.next].note.prev = note.prev;
        }

        void InsertLink(NotePosition position)
        {
            if (EditData.Notes.ContainsKey(note.prev))
                EditData.Notes[note.prev].note.next = position;

            if (EditData.Notes.ContainsKey(note.next))
                EditData.Notes[note.next].note.prev = position;
        }

        public void SetState(Note note)
        {
            if (note.type == NoteTypes.Single)
            {
                RemoveLink();
            }
            if (note.type == NoteTypes.TopFlick
                || note.type == NoteTypes.TopRightFlick
                || note.type == NoteTypes.RightFlick
                || note.type == NoteTypes.BottomRightFlick
                || note.type == NoteTypes.BottomFlick
                || note.type == NoteTypes.BottomLeftFlick
                || note.type == NoteTypes.LeftFlick
                || note.type == NoteTypes.TopLeftFlick)
            {
                RemoveLink();
            }

            this.note = note;

            if (note.type == NoteTypes.StraightLineLong)
            {
                InsertLink(note.position);
                EditState.LongNoteTailPosition.Value = EditState.LongNoteTailPosition.Value.Equals(note.prev)
                    ? note.position
                    : NotePosition.None;
            }
        }

        public void Dispose()
        {
            disposable.Dispose();
        }
    }
}
