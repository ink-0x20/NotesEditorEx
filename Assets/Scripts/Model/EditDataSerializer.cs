using NoteEditor.DTO;
using NoteEditor.Notes;
using NoteEditor.Presenter;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoteEditor.Model
{
    public class EditDataSerializer
    {
        public static string Serialize()
        {
            var dto = new MusicDTO.EditData();
            dto.BPM = EditData.BPM.Value;
            dto.maxLane = EditData.MaxBlock.Value;
            dto.offset = EditData.OffsetSamples.Value;
            dto.name = Path.GetFileNameWithoutExtension(EditData.Name.Value);
            dto.level = EditData.Level.Value;

            var sortedNoteObjects = EditData.Notes.Values
                .Where(note => !((note.note.type == NoteTypes.StraightLineLong || note.note.type == NoteTypes.RightCurveLong || note.note.type == NoteTypes.LeftCurveLong) && EditData.Notes.ContainsKey(note.note.prev)))
                .OrderBy(note => note.note.position.ToSamples(Audio.Source.clip.frequency, EditData.BPM.Value));

            dto.notes = new List<MusicDTO.Note>();

            foreach (var noteObject in sortedNoteObjects)
            {
                if (noteObject.note.type == NoteTypes.Single
                    || noteObject.note.type == NoteTypes.TopFlick
                    || noteObject.note.type == NoteTypes.TopRightFlick
                    || noteObject.note.type == NoteTypes.RightFlick
                    || noteObject.note.type == NoteTypes.BottomRightFlick
                    || noteObject.note.type == NoteTypes.BottomFlick
                    || noteObject.note.type == NoteTypes.BottomLeftFlick
                    || noteObject.note.type == NoteTypes.LeftFlick
                    || noteObject.note.type == NoteTypes.TopLeftFlick)
                {
                    dto.notes.Add(ToDTO(noteObject));
                }
                else if (noteObject.note.type == NoteTypes.StraightLineLong
                    || noteObject.note.type == NoteTypes.RightCurveLong
                    || noteObject.note.type == NoteTypes.LeftCurveLong)
                {
                    var current = noteObject;
                    var note = ToDTO(noteObject);

                    while (EditData.Notes.ContainsKey(current.note.next))
                    {
                        var nextObj = EditData.Notes[current.note.next];
                        note.notes.Add(ToDTO(nextObj));
                        current = nextObj;
                    }

                    dto.notes.Add(note);
                }
            }

            return UnityEngine.JsonUtility.ToJson(dto);
        }

        public static void Deserialize(string json)
        {
            var editData = UnityEngine.JsonUtility.FromJson<MusicDTO.EditData>(json);
            var notePresenter = EditNotesPresenter.Instance;

            EditData.BPM.Value = (int)editData.BPM;
            EditData.MaxBlock.Value = editData.maxLane;
            EditData.OffsetSamples.Value = editData.offset;
            EditData.Level.Value = editData.level == 0 ? 1 : editData.level;

            foreach (var note in editData.notes)
            {
                if (note.type == 1)
                {
                    notePresenter.AddNote(ToNoteObject(note));
                    continue;
                }

                var longNoteObjects = new[] { note }.Concat(note.notes)
                    .Select(note_ =>
                    {
                        notePresenter.AddNote(ToNoteObject(note_));
                        return EditData.Notes[ToNoteObject(note_).position];
                    })
                    .ToList();

                for (int i = 1; i < longNoteObjects.Count; i++)
                {
                    longNoteObjects[i].note.prev = longNoteObjects[i - 1].note.position;
                    longNoteObjects[i - 1].note.next = longNoteObjects[i].note.position;
                }

                EditState.LongNoteTailPosition.Value = NotePosition.None;
            }
        }

        static MusicDTO.Note ToDTO(NoteObject noteObject)
        {
            var note = new MusicDTO.Note();
            note.num = noteObject.note.position.num;
            note.lane = noteObject.note.position.block;
            note.LPB = noteObject.note.position.LPB;
            // ノーツサイズ
            note.size = noteObject.note.position.size;
            // 特殊ノーツ
            note.special = noteObject.note.position.special;
            // 型変換
            int type = (int)noteObject.note.type;
            note.type = type;
            note.notes = new List<MusicDTO.Note>();
            return note;
        }

        public static Note ToNoteObject(MusicDTO.Note musicNote)
        {
            // 型変換
            NoteTypes type = (NoteTypes)musicNote.type;
            if (!System.Enum.IsDefined(typeof(NoteTypes), musicNote.type))
            {
                // 存在しないタイプはシングルノーツで処理
                type = NoteTypes.Single;
            }
            // ノーツタイプの改良前互換
            if (type == NoteTypes.BeforeSingle)
            {
                type = NoteTypes.Single;
            }
            else if (type == NoteTypes.BeforeLong)
            {
                type = NoteTypes.StraightLineLong;
            }
            // ノーツサイズの改良前互換(デフォルト1)
            int size = musicNote.size == 0 ? 1 : musicNote.size;
            return new Note(
                new NotePosition(size, musicNote.special, (int)musicNote.LPB, (int)musicNote.num, musicNote.lane),
                type);
        }
    }
}
