using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiteDB;

namespace AppImageLauncher.Classes;
public enum AppState { Idle, Running, Error }
public class Application : INotifyPropertyChanged
{
    [BsonId] 
    public int Id { get; set; }
    private string? _name;
    private string? _appImagePath;
    private string? _iconPath;
    private string? _argument;
    public List<string>? Categories { get; set; }
    private AppState _state = AppState.Idle;
    private TimeSpan _runTime;

    public string? Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(Name);
        }
    }
    public string? AppImagePath
    {
        get => _appImagePath;
        set
        {
            _appImagePath = value;
            OnPropertyChanged(AppImagePath);
        }
    }
    public string? IconPath
    {
        get => _iconPath;
        set
        {
            _iconPath = value;
            OnPropertyChanged(IconPath);
        }
    }
    public string? Argument
    {
        get => _argument;
        set
        {
            _argument = value;
            OnPropertyChanged(Argument);
        }
    }

    public AppState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                OnPropertyChanged(nameof(State));
            }
        }
    }
    public TimeSpan RunTime
    {
        get => _runTime;
        set
        {
            _runTime = value;
            OnPropertyChanged(nameof(RunTimeFormatted));
        }
    }
    public string RunTimeFormatted => $"{(int)_runTime.Hours:00}:{_runTime.Minutes:00}";
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}