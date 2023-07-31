﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.Integration.P2P.WebRTC.MVS.GroupSelectionScreen
{
    public class GroupSelectionScreenView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown roleDropdown;
        [SerializeField] private TMP_InputField groupNameInputField;
        [SerializeField] private TMP_Dropdown groupDropdown;
        [SerializeField] private Button updateButton;
        [SerializeField] private Button goButton;

        public IObservable<PeerRole> OnRoleChanged =>
            roleDropdown.onValueChanged.AsObservable()
                .Select(index => Roles[index]).TakeUntilDestroy(this);

        public IObservable<string> OnGroupNameChanged =>
            groupNameInputField.onEndEdit.AsObservable().TakeUntilDestroy(this);

        public IObservable<string> OnGroupChanged =>
            groupDropdown.onValueChanged.AsObservable()
                .Select(index => groupNames[index]).TakeUntilDestroy(this);

        public IObservable<Unit> OnUpdateButtonClicked => updateButton.OnClickAsObservable().TakeUntilDestroy(this);
        public IObservable<Unit> OnGoButtonClicked => goButton.OnClickAsObservable().TakeUntilDestroy(this);

        private static readonly List<PeerRole> Roles = new List<PeerRole> { PeerRole.Host, PeerRole.Client };
        private readonly List<string> groupNames = new List<string>();

        public void Initialize()
        {
            roleDropdown.options = Roles.Select(role => new TMP_Dropdown.OptionData(role.ToString())).ToList();
            groupDropdown.options = new List<TMP_Dropdown.OptionData>();

            OnRoleChanged.Subscribe(SwitchInputMode).AddTo(this);
            OnGroupNameChanged.Subscribe(_ => CanGo(PeerRole.Host));
        }

        private void SwitchInputMode(PeerRole role)
        {
            groupNameInputField.gameObject.SetActive(role == PeerRole.Host);
            groupDropdown.gameObject.SetActive(role == PeerRole.Client);
            updateButton.gameObject.SetActive(role == PeerRole.Client);
            CanGo(role);
        }

        private void CanGo(PeerRole role) =>
            goButton.gameObject.SetActive(
                (role == PeerRole.Host && groupNameInputField.text.Length > 0)
                || (role == PeerRole.Client && groupDropdown.options.Count > 0));

        public void SetInitialValues(PeerRole role)
        {
            roleDropdown.value = Roles.IndexOf(role);
            groupNameInputField.text = string.Empty;
            groupDropdown.value = 0;
            SwitchInputMode(role);
        }

        public void UpdateGroupNames(string[] groupNames)
        {
            this.groupNames.Clear();
            this.groupNames.AddRange(groupNames);
            groupDropdown.options
                = this.groupNames.Select(groupName => new TMP_Dropdown.OptionData(groupName)).ToList();
            groupDropdown.value = 0;
            CanGo(PeerRole.Client);
        }
    }
}
