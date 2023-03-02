using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NoteEditor.Notes;

namespace NoteEditor.Presenter
{
    public class CategoryTypeList : MonoBehaviour
    {
        [SerializeField]
        private Dropdown dropdown;

        void Start()
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(
                new List<string>(
                    System.Enum.GetNames(typeof(CategoryType))
                    )
                );
        }
    }
}
