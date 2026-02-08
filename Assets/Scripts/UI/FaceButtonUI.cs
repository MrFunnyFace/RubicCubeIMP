using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FaceButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    private int index;
    private Action<int> callback;

    public void Setup(int index, Action<int> onClick)
    {
        this.index = index;
        callback = onClick;

        if (label != null)
            label.text = index.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        callback?.Invoke(index);
    }
}
