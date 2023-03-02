using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NoteEditor.Presenter
{
    public class SEVolumePresenter : MonoBehaviour
    {
        [SerializeField]
        Slider volumeController = default;
        [SerializeField]
        Image image = default;
        [SerializeField]
        Sprite iconSound2 = default;
        [SerializeField]
        Sprite iconSound = default;
        [SerializeField]
        Sprite iconMute = default;

        [SerializeField]
        private AudioSource seAudioSource = default;

        void Awake()
        {
            volumeController.OnValueChangedAsObservable()
                .Subscribe(volume =>
                {
                    seAudioSource.volume = volume;
                    if (volume == 0.0f)
                    {
                        image.sprite = iconMute;
                    }
                    else if (volume < 0.6f)
                    {
                        image.sprite = iconSound;
                    }
                    else
                    {
                        image.sprite = iconSound2;
                    }
                });
        }
    }
}
