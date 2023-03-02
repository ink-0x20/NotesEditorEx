using NoteEditor.Model;
using NoteEditor.Notes;
using System.Collections;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

namespace NoteEditor.Presenter
{
    public class MusicLoader : MonoBehaviour
    {
        [SerializeField]
        private Dropdown categoryDropdown = default;
        [SerializeField]
        private Dropdown difficultyDropdown = default;

        void Awake()
        {
            ResetEditor();
        }

        public void Load(string fileName)
        {
            StartCoroutine(LoadMusic(fileName));
        }

        IEnumerator LoadMusic(string fileName)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + Path.Combine(MusicSelector.DirectoryPath.Value, fileName), AudioType.OGGVORBIS))
            {
                yield return www.SendWebRequest();

                EditCommandManager.Clear();
                ResetEditor();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Audio.Source.clip = DownloadHandlerAudioClip.GetContent(www);
                    EditData.Name.Value = fileName;
                    LoadEditData();
                    Audio.OnLoad.OnNext(Unit.Default);
                }
            }
        }

        void LoadEditData()
        {
            var fileName = Path.ChangeExtension(EditData.Name.Value, "json");
            // カテゴリ・難易度追加
            Regex regex = new Regex(".json$");
            string selectCategoryType = categoryDropdown.options[categoryDropdown.value].text;
            if (selectCategoryType == CategoryType.NONE.ToString())
            {
                fileName = regex.Replace(fileName,
                      "_" + difficultyDropdown.options[difficultyDropdown.value].text
                    + ".json");
            }
            else
            {
                fileName = regex.Replace(fileName,
                      "_" + selectCategoryType
                    + "_" + difficultyDropdown.options[difficultyDropdown.value].text
                    + ".json");
            }
            var directoryPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes");
            var filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                EditDataSerializer.Deserialize(json);
            }
        }

        public void ResetEditor()
        {
            Audio.TimeSamples.Value = 0;
            Audio.SmoothedTimeSamples.Value = 0;
            Audio.IsPlaying.Value = false;
            Audio.Source.clip = null;
            EditState.NoteType.Value = NoteTypes.Single;
            EditState.LongNoteTailPosition.Value = NotePosition.None;
            EditData.BPM.Value = 120;
            EditData.OffsetSamples.Value = 0;
            EditData.Name.Value = "Notes Editor Ex";
            EditData.MaxBlock.Value = Settings.MaxBlock;
            EditData.LPB.Value = 4;
            EditData.Level.Value = 1;

            foreach (var note in EditData.Notes.Values)
            {
                note.Dispose();
            }

            EditData.Notes.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}
