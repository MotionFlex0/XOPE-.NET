﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using XOPE_UI.Model;
using XOPE_UI.Settings;
using XOPE_UI.Spy.DispatcherMessageType;
using XOPE_UI.Spy.Type;
using XOPE_UI.View;

namespace XOPE_UI.Presenter
{
    public class FilterViewTabPresenter
    {
        IFilterViewTab _view;
        IUserSettings _settings;
        SpyManager _spyManager; // Never use this directly. Use SpyManager instead

        public SpyManager SpyManager
        {
            private get => _spyManager ?? 
                throw new InvalidOperationException("SpyManager needs to be initialised prior to its use");
            set => _spyManager = value;
        }

        public IUserSettings Settings
        {
            private get => _settings ??
                throw new InvalidOperationException("Settings needs to be initialised prior to its use");
            set => _settings = value;
        }

        public FilterViewTabPresenter(IFilterViewTab view)
        {
            _view = view;
        }

        public void AddFilterButtonClicked()
        {
            if (!SpyManager.IsAttached)
                return;

            FilterEntry filter = _view.ShowFilterEditorDialog();
            if (filter == null)
                return;

            filter.Activated = Settings.GetValue<bool>(UserSettingsKey.FILTER_ENABLED_BY_DEFAULT);

            EventHandler<IncomingMessage> addPacketFilterCallback = (object sender, IncomingMessage response) =>
            {
                if (response.Type != UiMessageType.JOB_RESPONSE_SUCCESS)
                {
                    _view.ShowFailedToAddFilterMessage(response.Json.Value<string>("errorMessage"));
                    return;
                }

                filter.FilterId = response.Json.Value<string>("filterId");
                _view.AddFilter(filter);

            };

            SpyManager.MessageDispatcher.Send(new AddPacketFilter(addPacketFilterCallback)
            {
                SocketId = filter.SocketId,
                PacketType = filter.PacketType,
                OldValue = filter.OldValue,
                NewValue = filter.NewValue,
                ReplaceEntirePacket = false,
                RecursiveReplace = filter.RecursiveReplace,
                Activated = filter.Activated,
                DropPacket = filter.DropPacket
            });
        }

        public void DeleteFilterButtonClicked(FilterEntry filter)
        {
            if (!SpyManager.IsAttached)
                return;

            if (filter != null)
            {
                if (SpyManager.MessageDispatcher != null)
                {
                    EventHandler<IncomingMessage> callback = (sender, e) =>
                    {
                        if (e.Type != UiMessageType.JOB_RESPONSE_SUCCESS)
                            Console.WriteLine("Failed to delete that filter.");
                    };

                    SpyManager.MessageDispatcher.Send(new DeletePacketFilter(callback)
                    {
                        FilterId = filter.FilterId
                    });
                }
                _view.RemoveFilter(filter);
            }
        }

        public string GetFiltersAsJson()
        {
            int filterCount = _view.Filters.Count;
            FilterEntry[] filterEntries = new FilterEntry[filterCount];

            this._view.Filters.CopyTo(filterEntries, 0);

            return JsonConvert.SerializeObject(filterEntries);
        }

        public void LoadFilters(IEnumerable<FilterEntry> filterEntries)
        {
            if (_view.Filters.Count > 0)
                if (!_view.ShowFilterListClearConfirmation())
                    return;

            _view.Filters.Clear();
            foreach (FilterEntry filterEntry in filterEntries)
            {
                filterEntry.Activated = false;
                _view.Filters.Add(filterEntry);
            }
        }

        public void ShowFilterEditor(FilterEntry filter)
        {
            if (!SpyManager.IsAttached)
                return;

            FilterEntry updatedFilter = _view.ShowFilterEditorDialog(filter);
            if (updatedFilter == null)
                return;

            if (SpyManager.MessageDispatcher != null)
                SpyManager.MessageDispatcher.Send(new ModifyPacketFilter() { Filter = filter });
        }

        public void ToggleFilterActivated(FilterEntry filter, bool isActivated)
        {
            if (SpyManager.MessageDispatcher != null)
            {
                SpyManager.MessageDispatcher.Send(new ToggleActivatePacketFilter() 
                { 
                    FilterId = filter.FilterId, 
                    Activated = isActivated 
                });
            }
        }
    }
}
