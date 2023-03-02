using NoteEditor.Common;
using NoteEditor.Notes;
using NoteEditor.Model;
using NoteEditor.Utility;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace NoteEditor.Presenter
{
    public class EditNotesPresenter : SingletonMonoBehaviour<EditNotesPresenter>
    {
        [SerializeField]
        CanvasEvents canvasEvents = default;

        public readonly Subject<Note> RequestForEditNote = new Subject<Note>();
        public readonly Subject<Note> RequestForRemoveNote = new Subject<Note>();
        public readonly Subject<Note> RequestForAddNote = new Subject<Note>();
        public readonly Subject<Note> RequestForChangeNoteStatus = new Subject<Note>();

        void Awake()
        {
            Audio.OnLoad.First().Subscribe(_ => Init());
        }

        void Init()
        {
            var closestNoteAreaOnMouseDownObservable = canvasEvents.NotesRegionOnMouseDownObservable
                .Where(_ => !KeyInput.CtrlKey())
                .Where(_ => !Input.GetMouseButtonDown(1))
                .Where(_ => 0 <= NoteCanvas.ClosestNotePosition.Value.num);

            // クリックイベント追加？
            closestNoteAreaOnMouseDownObservable
                .Where(_ => EditState.NoteType.Value == NoteTypes.Single)
                .Where(_ => !KeyInput.ShiftKey())
                // ロングノーツ
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.StraightLineLong))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.LeftCurveLong))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.RightCurveLong))
                // フリック
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.TopFlick))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.TopRightFlick))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.RightFlick))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.BottomRightFlick))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.BottomFlick))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.BottomLeftFlick))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.LeftFlick))
                .Merge(closestNoteAreaOnMouseDownObservable
                    .Where(_ => EditState.NoteType.Value == NoteTypes.TopLeftFlick))
                .Subscribe(_ =>
                {
                    if (EditData.Notes.ContainsKey(NoteCanvas.ClosestNotePosition.Value))
                    {
                        EditData.Notes[NoteCanvas.ClosestNotePosition.Value].OnClickObservable.OnNext(Unit.Default);
                    }
                    else
                    {
                        RequestForEditNote.OnNext(
                           new Note(
                               NoteCanvas.ClosestNotePosition.Value,
                               EditState.NoteType.Value,
                               NotePosition.None,
                               EditState.LongNoteTailPosition.Value));
                    }
                });

            // Start editing of long note
            closestNoteAreaOnMouseDownObservable
                .Where(_ => EditState.NoteType.Value == NoteTypes.Single)
                .Where(_ => KeyInput.ShiftKey())
                .Do(_ => EditState.NoteType.Value = NoteTypes.StraightLineLong)
                .Subscribe(_ => RequestForAddNote.OnNext(
                    new Note(
                        NoteCanvas.ClosestNotePosition.Value,
                        NoteTypes.StraightLineLong,
                        NotePosition.None,
                        NotePosition.None)));

            // Finish editing long note by press-escape or right-click
            this.UpdateAsObservable()
                .Where(_ => EditState.NoteType.Value == NoteTypes.StraightLineLong)
                .Where(_ => Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                .Subscribe(_ => EditState.NoteType.Value = NoteTypes.Single);

            var finishEditLongNoteObservable = EditState.NoteType.Where(editType => editType == NoteTypes.Single);

            finishEditLongNoteObservable.Subscribe(_ => EditState.LongNoteTailPosition.Value = NotePosition.None);

            RequestForRemoveNote.Buffer(RequestForRemoveNote.ThrottleFrame(1))
                .Select(b => b.OrderBy(note => note.position.ToSamples(Audio.Source.clip.frequency, EditData.BPM.Value)).ToList())
                .Subscribe(notes => EditCommandManager.Do(
                    new Command(
                        () => notes.ForEach(RemoveNote),
                        () => notes.ForEach(AddNote))));

            RequestForAddNote.Buffer(RequestForAddNote.ThrottleFrame(1))
                .Select(b => b.OrderBy(note => note.position.ToSamples(Audio.Source.clip.frequency, EditData.BPM.Value)).ToList())
                .Subscribe(notes => EditCommandManager.Do(
                    new Command(
                        () => notes.ForEach(AddNote),
                        () => notes.ForEach(RemoveNote))));

            RequestForChangeNoteStatus.Select(note => new { current = note, prev = EditData.Notes[note.position].note })
                .Buffer(RequestForChangeNoteStatus.ThrottleFrame(1))
                .Select(b => b.OrderBy(note => note.current.position.ToSamples(Audio.Source.clip.frequency, EditData.BPM.Value)).ToList())
                .Subscribe(notes => EditCommandManager.Do(
                    new Command(
                        () => notes.ForEach(x => ChangeNoteStates(x.current)),
                        () => notes.ForEach(x => ChangeNoteStates(x.prev)))));

            // 追加・削除イベント追加
            RequestForEditNote.Subscribe(note =>
            {
                if (note.type == NoteTypes.Single)
                {
                    (EditData.Notes.ContainsKey(note.position)
                        ? RequestForRemoveNote
                        : RequestForAddNote)
                    .OnNext(note);
                }
                else if (note.type == NoteTypes.TopFlick
                || note.type == NoteTypes.TopRightFlick
                || note.type == NoteTypes.RightFlick
                || note.type == NoteTypes.BottomRightFlick
                || note.type == NoteTypes.BottomFlick
                || note.type == NoteTypes.BottomLeftFlick
                || note.type == NoteTypes.LeftFlick
                || note.type == NoteTypes.TopLeftFlick)
                {
                    (EditData.Notes.ContainsKey(note.position)
                        ? RequestForRemoveNote
                        : RequestForAddNote)
                    .OnNext(note);
                }
                else if (note.type == NoteTypes.StraightLineLong
                || note.type == NoteTypes.LeftCurveLong
                || note.type == NoteTypes.RightCurveLong)
                {
                    if (!EditData.Notes.ContainsKey(note.position))
                    {
                        RequestForAddNote.OnNext(note);
                        return;
                    }

                    var noteObject = EditData.Notes[note.position];
                    (noteObject.note.type == NoteTypes.StraightLineLong
                        ? RequestForRemoveNote
                        : RequestForChangeNoteStatus)
                    .OnNext(noteObject.note);
                }
            });
        }

        public void AddNote(Note note)
        {
            if (EditData.Notes.ContainsKey(note.position))
            {
                if (!EditData.Notes[note.position].note.Equals(note))
                    RequestForChangeNoteStatus.OnNext(note);

                return;
            }

            var noteObject = new NoteObject();
            noteObject.SetState(note);
            noteObject.Init();
            EditData.Notes.Add(noteObject.note.position, noteObject);
        }

        void ChangeNoteStates(Note note)
        {
            if (!EditData.Notes.ContainsKey(note.position))
                return;

            EditData.Notes[note.position].SetState(note);
        }

        void RemoveNote(Note note)
        {
            if (!EditData.Notes.ContainsKey(note.position))
                return;

            var noteObject = EditData.Notes[note.position];
            noteObject.Dispose();
            EditData.Notes.Remove(noteObject.note.position);
        }
    }
}
