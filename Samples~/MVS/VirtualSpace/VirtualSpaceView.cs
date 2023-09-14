using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.Integration.P2P.WebRTC.MVS.VirtualSpace
{
    public class VirtualSpaceView : MonoBehaviour
    {
        [SerializeField] private TMP_Text roleText;
        [SerializeField] private Button backButton;

        public IObservable<Unit> OnBackButtonClicked => backButton.OnClickAsObservable().TakeUntilDestroy(this);

        public void ShowRole(string role) => roleText.text = role;
    }
}
