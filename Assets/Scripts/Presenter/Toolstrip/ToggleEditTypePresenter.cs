using NoteEditor.Notes;
using NoteEditor.Model;
using NoteEditor.Utility;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace NoteEditor.Presenter
{
    public class ToggleEditTypePresenter : MonoBehaviour
    {
        // **************************************************
        // ボタンオブジェクト
        // **************************************************
        [SerializeField]
        Button editTypeToggleButton = default;
        [SerializeField]
        Button editTypeDetailToggleButton = default;

        // **************************************************
        // アイコン画像
        // **************************************************
        [SerializeField]
        Sprite iconSingleNotes = default;
        [SerializeField]
        Sprite iconStraightLineLongNotes = default;
        [SerializeField]
        Sprite iconLeftCurveLongNotes = default;
        [SerializeField]
        Sprite iconRightCurveLongNotes = default;
        [SerializeField]
        Sprite iconTopFlickNotes = default;
        [SerializeField]
        Sprite iconTopRightFlickNotes = default;
        [SerializeField]
        Sprite iconRightFlickNotes = default;
        [SerializeField]
        Sprite iconBottomRightFlickNotes = default;
        [SerializeField]
        Sprite iconBottomFlickNotes = default;
        [SerializeField]
        Sprite iconBottomLeftFlickNotes = default;
        [SerializeField]
        Sprite iconLeftFlickNotes = default;
        [SerializeField]
        Sprite iconTopLeftFlickNotes = default;
        [SerializeField]
        Sprite iconChangeNoteTypes = default;

        // **************************************************
        // アイコン色
        // **************************************************
        [SerializeField]
        Color singleTypeStateButtonColor = default;
        [SerializeField]
        Color longTypeStateButtonColor = default;
        [SerializeField]
        Color flickTypeStateButtonColor = default;

        // **************************************************
        // 選択タイプ保存
        // **************************************************
        private NoteTypes longNoteType = NoteTypes.StraightLineLong;
        private NoteTypes flickNoteType = NoteTypes.RightFlick;

        void Awake()
        {
            // 現在のノーツタイプを変更する
            editTypeToggleButton.OnClickAsObservable()
                .Merge(this.UpdateAsObservable().Where(_ => KeyInput.AltKeyDown()))
                .Select(_ => EditState.NoteType.Value)
                .Subscribe(editType =>
                {
                    if (editType == NoteTypes.Single)
                    {
                        EditState.NoteType.Value = longNoteType;
                    }
                    else if (editType == NoteTypes.StraightLineLong || editType == NoteTypes.LeftCurveLong || editType == NoteTypes.RightCurveLong)
                    {
                        EditState.NoteType.Value = flickNoteType;
                    }
                    else
                    {
                        EditState.NoteType.Value = NoteTypes.Single;
                    }
                });
            // ノーツ特性を変更する
            editTypeDetailToggleButton.OnClickAsObservable()
                .Select(_ => EditState.NoteType.Value)
                .Subscribe(editType =>
                {
                    if (editType == NoteTypes.StraightLineLong)
                    {
                        EditState.NoteType.Value = NoteTypes.LeftCurveLong;
                        longNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.LeftCurveLong)
                    {
                        EditState.NoteType.Value = NoteTypes.RightCurveLong;
                        longNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.RightCurveLong)
                    {
                        EditState.NoteType.Value = NoteTypes.StraightLineLong;
                        longNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.TopFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.TopRightFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.TopRightFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.RightFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.RightFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.BottomRightFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.BottomRightFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.BottomFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.BottomFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.BottomLeftFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.BottomLeftFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.LeftFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.LeftFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.TopLeftFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                    else if (editType == NoteTypes.TopLeftFlick)
                    {
                        EditState.NoteType.Value = NoteTypes.TopFlick;
                        flickNoteType = EditState.NoteType.Value;
                    }
                });

            var buttonImage = editTypeToggleButton.GetComponent<Image>();
            var detailButtonImage = editTypeDetailToggleButton.GetComponent<Image>();

            // ノーツタイプアイコンを設定する
            EditState.NoteType.Select(_ => EditState.NoteType.Value)
                .Subscribe(noteType =>
                {
                    if (noteType == NoteTypes.StraightLineLong)
                    {
                        buttonImage.sprite = iconStraightLineLongNotes;
                    }
                    else if (noteType == NoteTypes.LeftCurveLong)
                    {
                        buttonImage.sprite = iconLeftCurveLongNotes;
                    }
                    else if (noteType == NoteTypes.RightCurveLong)
                    {
                        buttonImage.sprite = iconRightCurveLongNotes;
                    }
                    else if (noteType == NoteTypes.TopFlick)
                    {
                        buttonImage.sprite = iconTopFlickNotes;
                    }
                    else if (noteType == NoteTypes.TopRightFlick)
                    {
                        buttonImage.sprite = iconTopRightFlickNotes;
                    }
                    else if (noteType == NoteTypes.RightFlick)
                    {
                        buttonImage.sprite = iconRightFlickNotes;
                    }
                    else if (noteType == NoteTypes.BottomRightFlick)
                    {
                        buttonImage.sprite = iconBottomRightFlickNotes;
                    }
                    else if (noteType == NoteTypes.BottomFlick)
                    {
                        buttonImage.sprite = iconBottomFlickNotes;
                    }
                    else if (noteType == NoteTypes.BottomLeftFlick)
                    {
                        buttonImage.sprite = iconBottomLeftFlickNotes;
                    }
                    else if (noteType == NoteTypes.LeftFlick)
                    {
                        buttonImage.sprite = iconLeftFlickNotes;
                    }
                    else if (noteType == NoteTypes.TopLeftFlick)
                    {
                        buttonImage.sprite = iconTopLeftFlickNotes;
                    }
                    else
                    {
                        buttonImage.sprite = iconSingleNotes;
                    }
                    buttonImage.color = getColor(noteType);
                    detailButtonImage.sprite = iconChangeNoteTypes;
                    detailButtonImage.color = getColor(noteType);
                });
        }

        private Color getColor(NoteTypes value)
        {
            if (value == NoteTypes.Single)
            {
                return singleTypeStateButtonColor;
            }
            else if (value == NoteTypes.StraightLineLong || value == NoteTypes.LeftCurveLong || value == NoteTypes.RightCurveLong)
            {
                return longTypeStateButtonColor;
            }
            return flickTypeStateButtonColor;
        }
    }
}
